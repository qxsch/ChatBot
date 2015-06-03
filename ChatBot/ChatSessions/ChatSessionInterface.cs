using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace QXS.ChatBot
{
    public interface ChatSessionInterface
    {
        string readMessage();
        void sendMessage(string message);

        string askQuestion(string message);

        bool IsInteractive { get; set; }

        SessionStorage SessionStorage { get; set; }


        void SetResponseHistorySize(int Size);
        void AddResponseToHistory(BotResponse Response);
        Stack<BotResponse> GetResponseHistory();
    }
}
