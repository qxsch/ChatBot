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


        public void SetRuleHistorySize(int size)
        {
            _RuleHistory = new LinkedList<string>(_RuleHistory, size, false);
        }

        protected LinkedList<string> _RuleHistory = new LinkedList<string>(10, false);
        public void AddRuleToHistory(string rule)
        {
            _RuleHistory.Push(rule);
        }

        public Stack<string> GetRuleHistory()
        {
            return new Stack<string>(_RuleHistory.GetAsReverseArray());
        }
    }
}
