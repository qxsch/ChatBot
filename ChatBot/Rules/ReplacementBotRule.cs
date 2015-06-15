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
    public class ReplacementBotRule : BotRule
    {
        protected Random rnd = new Random();
        protected string[] _Replacements;
        protected Regex _Regex = new Regex("\\$(r|s)\\$([a-z0-9]+)\\$", RegexOptions.IgnoreCase);
        protected Dictionary<string, string> _setters = new Dictionary<string, string>();

        public ReplacementBotRule(string Name, int Weight, Regex MessagePattern, string Replacement, Dictionary<string, string> setters)
            : this(Name, Weight, MessagePattern, Replacement)
        {
            this._setters = setters;
        }
        public ReplacementBotRule(string Name, int Weight, Regex MessagePattern, string[] Replacements, Dictionary<string, string> setters)
            : this(Name, Weight, MessagePattern, Replacements)
        {
            this._setters = setters;
        }

        public ReplacementBotRule(string Name, int Weight, Regex MessagePattern, string Replacement)
            : base(Name, Weight, MessagePattern)
        {
            this._Replacements = new string[] { Replacement };
            this._Process = this.ReplaceMessage;
        }

        public ReplacementBotRule(string Name, int Weight, Regex MessagePattern, string[] Replacements)
            : base(Name, Weight, MessagePattern)
        {
            this._Replacements = Replacements;
            this._Process = this.ReplaceMessage;
        }

        public string ReplaceMessage(Match match, ChatSessionInterface session)
        {
            // set the setters
            foreach (string key in this._setters.Keys)
            {
                session.SessionStorage.Values[key] = this._Regex.Replace(
                    this._setters[key],
                    (Match m) =>
                    {
                        switch (m.Groups[1].Value.ToLower())
                        {
                            case "s":
                                if (session.SessionStorage.Values.ContainsKey(m.Groups[2].Value))
                                {
                                    return session.SessionStorage.Values[m.Groups[2].Value];
                                }
                                break;
                            case "r":
                                return match.Groups[m.Groups[2].Value].Value;
                        }
                        return "";
                    }
                );

            }

            // send a anwer

            if (this._Replacements.Length == 0)
            {
                return "";
            }
            string msg;
            if (this._Replacements.Length > 1)
            {
                msg = this._Replacements[rnd.Next(this._Replacements.Length)];
            }
            else
            {
                msg = this._Replacements[0];
            }

            return this._Regex.Replace(
                msg,
                (Match m) =>
                {
                    switch (m.Groups[1].Value.ToLower())
                    {
                        case "s":
                            if (session.SessionStorage.Values.ContainsKey(m.Groups[2].Value))
                            {
                                return session.SessionStorage.Values[m.Groups[2].Value];
                            }
                            break;
                        case "r":
                            return match.Groups[m.Groups[2].Value].Value;
                    }
                    return "";
                }
            );
        }

        new public static BotRule CreateRuleFromXml(ChatBotRuleGenerator generator, XmlNode node)
        {
            // get unique setters
            Dictionary<string, string> setters = new Dictionary<string, string>();
            foreach (XmlNode subnode in node.SelectChatBotNodes("cb:Setters/cb:Set").Cast<XmlNode>().Where(n => n.Attributes["Key"] != null))
            {
                setters[subnode.Attributes["Key"].Value] = subnode.InnerText;
            }

            return new ReplacementBotRule(
                generator.GetRuleName(node),
                generator.GetRuleWeight(node),
                new Regex(generator.GetRulePattern(node)),
                node.SelectChatBotNodes("cb:Messages/cb:Message").Cast<XmlNode>().Select(n => n.InnerText).ToArray(),
                setters
            );
        }
    }
}