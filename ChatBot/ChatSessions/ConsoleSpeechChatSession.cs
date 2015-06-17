using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace QXS.ChatBot
{
    public class ConsoleSpeechChatSession : ChatSessionInterface
    {
        protected SpeechSynthesizer _speechSynthesizer;

        public ConsoleSpeechChatSession()
        {
            SessionStorage = new SessionStorage();

            SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
            speechSynthesizer.SetOutputToDefaultAudioDevice();
            _speechSynthesizer = speechSynthesizer;
        }

        public ConsoleSpeechChatSession(SpeechSynthesizer speechSynthesizer)
        {
            SessionStorage = new SessionStorage();
            if (speechSynthesizer == null)
            {
                speechSynthesizer = new SpeechSynthesizer();
                speechSynthesizer.SetOutputToDefaultAudioDevice();
            }
            _speechSynthesizer = speechSynthesizer;
        }

        public string readMessage()
        {
            Console.Write("YOU> ");
            return Console.ReadLine();
        }
        public void sendMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("BOT> ");
            Console.WriteLine(message.Replace("\n", "\n     "));
            Console.ResetColor();
            _speechSynthesizer.Speak(message);
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
}
