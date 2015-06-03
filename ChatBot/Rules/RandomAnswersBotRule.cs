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
    public class RandomAnswersBotRule : BotRule
    {
        protected Random rnd = new Random();

        protected string[] _messages;

        public RandomAnswersBotRule(string Name, int Weight, Regex MessagePattern, string[] Messages)
            : base(Name, Weight, MessagePattern)
        {
            this._messages = Messages;
            this._Process = this.SendRandomMessage;
        }

        public string SendRandomMessage(Match match, ChatSessionInterface session)
        {
            return this._messages[rnd.Next(this._messages.Length)];
        }

        new public static BotRule CreateRuleFromXml(ChatBotRuleGenerator generator, XmlNode node)
        {
            return new RandomAnswersBotRule(
                generator.GetRuleName(node),
                generator.GetRuleWeight(node),
                new Regex(generator.GetRulePattern(node)),
                node.SelectNodes("Messages/Message").Cast<XmlNode>().Select(n => n.InnerText).ToArray()
            );
        }
    }
}
