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
    /// <summary>
    /// Bot rule which randomly returns one of the messages
    /// </summary>
    public class RandomAnswersBotRule : BotRule
    {
        protected Random _rnd = new Random();

        protected string[] _messages;

        public RandomAnswersBotRule(string name, int Weight, Regex messagePattern, string[] messages)
            : base(name, Weight, messagePattern)
        {
            this._messages = messages;
            this._Process = this.SendRandomMessage;
        }

        /// <summary>
        /// Sends randomly one of the messages
        /// </summary>
        /// <param name="match"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public string SendRandomMessage(Match match, IChatSessionInterface session)
        {
            return this._messages[_rnd.Next(this._messages.Length)];
        }

        new public static BotRule CreateRuleFromXml(ChatBotRuleGenerator generator, XmlNode node)
        {
            return new RandomAnswersBotRule(
                generator.GetRuleName(node),
                generator.GetRuleWeight(node),
                new Regex(generator.GetRulePattern(node)),
                node.SelectChatBotNodes("cb:Messages/cb:Message").Cast<XmlNode>().Select(n => n.InnerText).ToArray()
            );
        }
    }
}
