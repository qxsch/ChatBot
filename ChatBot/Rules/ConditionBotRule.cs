using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections;

namespace QXS.ChatBot
{
    public class ConditionBotRule : BotRule
    {
        public enum Operator
        {
            Equal,
            EqualIgnoreCase,
            NotEqual,
            NotEqualIgnoreCase,
        }

        protected IEnumerable<Tuple<string, Operator, string>> _Conditions;
        protected SortedList<int, List<BotRule>> _BotRules = new SortedList<int, List<BotRule>>(new DescComparer<int>());

        public ConditionBotRule(string Name, int Weight, IEnumerable<Tuple<string, Operator, string>> Conditions, IEnumerable<BotRule> Rules)
            : base(Name, Weight)
        {
            this._MessagePattern = new Regex("^.*$");
            this._Conditions = Conditions;
            Dictionary<string, bool> ruleNames = new Dictionary<string, bool>();
            foreach (BotRule rule in Rules)
            {
                if (rule.Process == null)
                {
                    throw new ArgumentException("Process is null.", "Rules");
                }
                if (rule.MessagePattern == null)
                {
                    throw new ArgumentException("MessagePattern is null.", "Rules");
                }
                if (ruleNames.ContainsKey(rule.Name))
                {
                    throw new ArgumentException("Names are not unique. Duplicate key found for rule name \"" + rule.Name + "\".", "Rules");
                }
                ruleNames[rule.Name] = true;
                if (!this._BotRules.ContainsKey(rule.Weight))
                {
                    this._BotRules[rule.Weight] = new List<BotRule>();
                }
                this._BotRules[rule.Weight].Add(rule);
            }

            this._Process = this.ProcessSubrules;
        }

        public string ProcessSubrules(Match match, ChatSessionInterface session)
        {
            foreach (Tuple<string, Operator, string> condition in this._Conditions)
            {
                if (!session.SessionStorage.Values.ContainsKey(condition.Item1))
                {
                    return null;
                }
                switch (condition.Item2)
                {
                    case Operator.Equal:
                        if (session.SessionStorage.Values[condition.Item1] != condition.Item3)
                        {
                            return null;
                        }
                        break;
                    case Operator.NotEqual:
                        if (session.SessionStorage.Values[condition.Item1] == condition.Item3)
                        {
                            return null;
                        }
                        break;
                    case Operator.EqualIgnoreCase:
                        if (session.SessionStorage.Values[condition.Item1].ToLower() != condition.Item3.ToLower())
                        {
                            return null;
                        }
                        break;
                    case Operator.NotEqualIgnoreCase:
                        if (session.SessionStorage.Values[condition.Item1].ToLower() == condition.Item3.ToLower())
                        {
                            return null;
                        }
                        break;
                }
            }

            foreach (List<BotRule> rules in this._BotRules.Values)
            {
                foreach (BotRule rule in rules)
                {
                    Match submatch = rule.MessagePattern.Match(match.Value);
                    if (submatch.Success)
                    {

                        string msg = rule.Process(submatch, session);
                        if (msg != null)
                        {
                            return msg;
                        }
                    }
                }
            }
            // no hit found
            return null;
        }

        new public static BotRule CreateRuleFromXml(ChatBotRuleGenerator generator, XmlNode node)
        {
            // get unique setters
            List<Tuple<string, Operator, string>> conditions = new List<Tuple<string, Operator, string>>();
            foreach (XmlNode subnode in node.SelectChatBotNodes("cb:Conditions/cb:Condition").Cast<XmlNode>().Where(n => n.Attributes["Key"] != null && n.Attributes["Operator"] != null))
            {
                switch (subnode.Attributes["Operator"].Value.Trim().ToLower())
                {
                    case "equal":
                    case "eq":
                        conditions.Add(new Tuple<string, Operator, string>(subnode.Attributes["Key"].Value, Operator.Equal, subnode.InnerText));
                        break;
                    case "equalignorecase":
                    case "ieq":
                        conditions.Add(new Tuple<string, Operator, string>(subnode.Attributes["Key"].Value, Operator.EqualIgnoreCase, subnode.InnerText));
                        break;
                    case "notequal":
                    case "ne":
                        conditions.Add(new Tuple<string, Operator, string>(subnode.Attributes["Key"].Value, Operator.NotEqual, subnode.InnerText));
                        break;
                    case "notequalignorecase":
                    case "ine":
                        conditions.Add(new Tuple<string, Operator, string>(subnode.Attributes["Key"].Value, Operator.NotEqualIgnoreCase, subnode.InnerText));
                        break;

                }

            }

            return new ConditionBotRule(
                generator.GetRuleName(node),
                generator.GetRuleWeight(node),
                conditions,
                generator.Parse(node.OwnerDocument, node)
            );
        }
    }
}
