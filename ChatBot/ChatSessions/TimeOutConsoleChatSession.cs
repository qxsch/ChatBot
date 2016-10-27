using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QXS.ChatBot.ChatSessions
{
    class TimeOutConsoleChatSession : ChatSessionInterface
    {
        /// <summary>
        /// The session received a messsage
        /// </summary>
        public event Action<ChatSessionInterface, string> OnMessageReceived;

        /// <summary>
        /// The session replied to a message
        /// </summary>
        public event Action<ChatSessionInterface, string> OnMessageSent;

        public int readTimeoutMS = 6000;

        public TimeOutConsoleChatSession()
        {
            SessionStorage = new SessionStorage();
        }

        public static string ReadLineWithTimeout(int timeoutms = 5000)
        {
            ReadLineDelegate d = Console.ReadLine;
            IAsyncResult result = d.BeginInvoke(null, null);
            result.AsyncWaitHandle.WaitOne(timeoutms); //timeout e.g. 15000 for 15 secs
            if (result.IsCompleted)
            {
                string resultstr = d.EndInvoke(result);
                return resultstr;
            }
            else
            {
                throw new TimeoutException("Timed Out!");
            }
        }

        delegate string ReadLineDelegate();

        public string readMessage()
        {
            Console.Write("YOU> ");
            string s = ReadLineWithTimeout(readTimeoutMS);
            if (s != null && OnMessageReceived != null)
            {
                OnMessageReceived(this, s);
            }
            return s;
        }
        public void sendMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("BOT> ");
            Console.WriteLine(message.Replace("\n", "\n     "));
            Console.ResetColor();
            if (message != null && OnMessageSent != null)
            {
                OnMessageSent(this, message);
            }
        }

        public string askQuestion(string message)
        {
            sendMessage(message);
            Console.Write("YOU> ");
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

    class AsyncConsoleReader
    {



    }
}
