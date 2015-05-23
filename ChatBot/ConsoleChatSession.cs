using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace QXS.ChatBot
{
    public class ConsoleChatSession : ChatSessionInterface
    {
        public ConsoleChatSession()
        {
            SessionStorage = new SessionStorage();
        }

        public string readMessage()
        {
            Console.Write("YOU> ");
            return Console.ReadLine();
        }
        public void sendMessage(string message)
        {
            Console.Write("BOT> ");
            Console.WriteLine(message.Replace("\n", "\n     "));
        }

        public string askQuestion(string message)
        {
            sendMessage(message);
            return readMessage();
        }

        public bool IsInteractive { get { return true; } set { } }

        public SessionStorage SessionStorage { get; set; }



        /*protected int _RuleHistoryStackPos = 0;
        protected string[] _RuleHistory = new string[10];
        public void AddRule(string rule)
        {
            if (_RuleHistoryStackPos >= _RuleHistory.Length)
            {
                _RuleHistoryStackPos = 0;
            }
            _RuleHistory[_RuleHistoryStackPos] = rule;
            _RuleHistoryStackPos++;
        }

        public Stack<string> GetRuleHistory()
        {
            Stack<string> s = new Stack<string>();
            if (_RuleHistory[0] == null)
            {
                return s;
            }
            int i = 0;
            for (i = _RuleHistoryStackPos + 1; i != _RuleHistoryStackPos; i++)
            {
                if (i >= _RuleHistory.Length)
                {
                    i = 0;
                }
                if (_RuleHistory[i] == null)
                {
                    continue;
                }
                s.Push(_RuleHistory[i]);
            }
            if (i < _RuleHistory.Length && _RuleHistory[i] != null)
            {
                s.Push(_RuleHistory[i]);
            }

            return s;
        }*/

        protected LinkedList<string> _RuleHistory = new LinkedList<string>(10, false);
        public void AddRule(string rule)
        {
            _RuleHistory.Push(rule);
        }

        public Stack<string> GetRuleHistory()
        {
            return new Stack<string>(_RuleHistory.GetAsReverseArray());
        }
    }
}
