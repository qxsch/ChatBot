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

    /// <summary>
    /// The Chatbot
    /// </summary>
    public class ChatBot
    {
        /// <summary>
        /// A conversation started
        /// </summary>
        public event Action<ChatSessionInterface> OnConverationStarted;

        /// <summary>
        /// A conversation ended
        /// </summary>
        public event Action<ChatSessionInterface> OnConverationEnded;

        /// <summary>
        /// The chatbot received a messsage
        /// </summary>
        public event Action<ChatSessionInterface, string> OnMessageReceived;

        /// <summary>
        /// The chatbot replied to a message
        /// </summary>
        public event Action<ChatSessionInterface, string> OnMessageSent;

        /// <summary>
        /// Sets the Exit Condition for an ending conversation
        /// </summary>
        public Func<string, bool> ExitCondition;

        /// <summary>
        /// Sets the default resonse in case no appropriate Rule was found
        /// </summary>
        public Func<string, string> DefaultAnswer;

        protected Stack<string> _commandHistory = new Stack<string>();
        protected SortedList<int, List<BotRule>> _botRules = new SortedList<int, List<BotRule>>(new DescComparer<int>());

        /// <summary>
        /// Creaates the Chatbot
        /// </summary>
        /// <param name="Rules"></param>
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

        /// <summary>
        /// Find a matching rule/reponse to a question/message
        /// </summary>
        /// <param name="session">The session, that should be used</param>
        /// <param name="messageIn">The message that came in</param>
        /// <returns>the response string or null in case no answer was found</returns>
        protected string findAnswer(ChatSessionInterface session, string messageIn)
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
                            session.AddResponseToHistory(new BotResponse(rule.Name, messageIn, msg));
                            return msg;
                        }
                    }
                }
            }

            return null;
        }

        protected void sendResponse(ChatSessionInterface session, string messageOut)
        {
            session.sendMessage(messageOut);
            if (OnMessageSent != null)
            {
                OnMessageSent(session, messageOut);
            }
        }

        /// <summary>
        /// Starts a conversation over a session
        /// </summary>
        /// <param name="session"></param>
        public void talkWith(ChatSessionInterface session)
        {
            if (session == null)
            {
                return;
            }

            if (OnConverationStarted != null)
            {
                OnConverationStarted(session);
            }
            

            string messageIn="";
            string messageOut="";
            for (messageIn = session.readMessage(); !this.ExitCondition(messageIn); messageIn = session.readMessage())
            {
                if (OnMessageReceived != null)
                {
                    OnMessageReceived(session, messageIn);
                }

                messageOut = findAnswer(session, messageIn);
                if (messageOut == null)
                {
                    // do we have a default answer?
                    if (DefaultAnswer != null)
                    {
                        messageOut=DefaultAnswer(messageIn);
                    }
                    // still null?
                    if (messageOut == null)
                    {
                        sendResponse(session, "What did you say?");
                    }
                    else
                    {
                        sendResponse(session, messageOut);
                    }
                    
                }
                else
                {
                    sendResponse(session, messageOut);
                }
                // still interactive?
                if (!session.IsInteractive)
                {
                    break;
                }
            }

            if (OnConverationEnded != null)
            {
                OnConverationEnded(session);
            }
        }

        /// <summary>
        /// Sets the default Exit Condition for the conversation
        /// </summary>
        /// <seealso cref="ExitCondition"/>
        /// <param name="message">Message, that came in</param>
        /// <returns>Returns true, in case the conversation should be ended</returns>
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
