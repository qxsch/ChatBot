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


        public void SetResponseHistorySize(int Size)
        {
            _ResponseHistory = new LinkedList<BotResponse>(_ResponseHistory, Size, false);
        }
        protected LinkedList<BotResponse> _ResponseHistory = new LinkedList<BotResponse>(10, false);
        public void AddResponseToHistory(BotResponse Response)
        {
            _ResponseHistory.Push(Response);
        }

        public Stack<BotResponse> GetResponseHistory()
        {
            return new Stack<BotResponse>(_ResponseHistory.GetAsReverseArray());
        }

    }
}
