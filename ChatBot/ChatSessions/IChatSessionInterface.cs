using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace QXS.ChatBot
{
    public interface IChatSessionInterface
    {
        /// <summary>
        /// The session received a messsage
        /// </summary>
        event Action<IChatSessionInterface, string> OnMessageReceived;

        /// <summary>
        /// The session replied to a message
        /// </summary>
        event Action<IChatSessionInterface, string> OnMessageSent;

        string ReadMessage();
        void SendMessage(string message);

        string AskQuestion(string message);

        bool IsInteractive { get; set; }

        SessionStorage SessionStorage { get; set; }


        void SetResponseHistorySize(int Size);
        void AddResponseToHistory(BotResponse Response);
        Stack<BotResponse> GetResponseHistory();
    }
}
