using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QXS.ChatBot
{
    internal class DescComparer<T> : IComparer<T>
    {
        public int Compare(T x, T y)
        {
            return Comparer<T>.Default.Compare(y, x);
        }
    }

    public class ChatBot
    {
        public Func<string, bool> ExitCondition;

        protected Stack<string> _commandHistory = new Stack<string>();
        protected SortedList<int, List<BotRule>> _botRules = new SortedList<int, List<BotRule>>(new DescComparer<int>());
        public ChatBot(IEnumerable<BotRule> Rules)
        {
           Dictionary<string, bool> ruleNames = new Dictionary<string, bool>();
           foreach(BotRule rule in Rules)
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
               if(!this._botRules.ContainsKey(rule.Weight))
               {
                   this._botRules[rule.Weight] = new List<BotRule>();
               }
               this._botRules[rule.Weight].Add(rule);
           }

           ExitCondition = this.isGoodBye;
        }


        protected string answer(ChatSessionInterface session, string messageIn)
        {
            foreach (List<BotRule> rules in this._botRules.Values)
            {
                foreach (BotRule rule in rules)
                {
                    Match match = rule.MessagePattern.Match(messageIn);
                    if (match.Success)
                    {
                        string msg = rule.Process(match, session);
                        if (msg != null)
                        {
                            session.AddRule(rule.Name);
                            return msg;
                        }
                    }
                }
            }

            return null;
        }

        public void talkWith(ChatSessionInterface session)
        {
            string messageIn="";
            string messageOut="";
            for (messageIn = session.readMessage(); !this.ExitCondition(messageIn); messageIn = session.readMessage())
            {
                messageOut = answer(session, messageIn);
                if (messageOut == null)
                {
                    session.sendMessage("What did you say?");
                }
                else
                {
                    session.sendMessage(messageOut);
                }
                // still interactive?
                if (!session.IsInteractive)
                {
                    break;
                }
            }
        }

        public bool isGoodBye(string message)
        {
            switch(message.ToLower())
            {
                case "quit": return true;
                case "exit": return true;
                case "goodbye": return true;
                case "good bye": return true;
                case "bye": return true;
            }
            return false;
        }
    }
}
