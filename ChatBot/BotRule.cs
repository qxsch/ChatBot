using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace QXS.ChatBot
{
    public class ReplacementBotRule : BotRule
    {
        protected Random rnd = new Random();
        protected string[] _Replacements;
        protected Regex _Regex = new Regex("\\$(r|s)\\$([a-z0-9]+)\\$", RegexOptions.IgnoreCase);

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
                    switch(m.Groups[1].Value.ToLower())
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
    }

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
    }

    public class BotRule
    {
        protected BotRule(string Name, int Weight)
        {
            if (Name == null)
            {
                throw new ArgumentException("Name is null.", "Name");
            }

            this._Name = Name;
            this._Weight = Weight;
        }

        protected BotRule(string Name, int Weight, Regex MessagePattern)
            : this(Name, Weight)
        {
            if (MessagePattern == null)
            {
                throw new ArgumentException("MessagePattern is null.", "MessagePattern");
            }
            this._MessagePattern = MessagePattern;
        }

        public BotRule(string Name, int Weight, Regex MessagePattern, Func<Match, ChatSessionInterface, string> Process)
            : this(Name, Weight, MessagePattern)
        {
            if (Process == null)
            {
                throw new ArgumentException("Process is null.", "Process");
            }
            this._Process = Process;
        }

        protected string _Name;
        public string Name { get { return _Name; } }

        protected int _Weight;
        public int Weight { get { return _Weight; } }

        protected Regex _MessagePattern;
        public Regex MessagePattern { get { return _MessagePattern; } }

        protected Func<Match, ChatSessionInterface, string> _Process;
        public Func<Match, ChatSessionInterface, string> Process { get { return _Process; } }
    }

}
