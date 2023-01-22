﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace QXS.ChatBot
{
    public class SpeechConversation : IChatSessionInterface, IDisposable
    {
        protected SpeechSynthesizer _speechSynthesizer;
        protected SpeechRecognitionEngine _speechRecognition;

        /// <summary>
        /// The session received a messsage
        /// </summary>
        public event Action<IChatSessionInterface, string> OnMessageReceived;

        /// <summary>
        /// The session replied to a message
        /// </summary>
        public event Action<IChatSessionInterface, string> OnMessageSent;

        public SpeechConversation(SpeechSynthesizer speechSynthesizer = null, SpeechRecognitionEngine speechRecognition = null)
        {
            SessionStorage = new SessionStorage();
            if(speechSynthesizer==null)
            {
                speechSynthesizer = new SpeechSynthesizer();
                speechSynthesizer.SetOutputToDefaultAudioDevice();
            }
            _speechSynthesizer = speechSynthesizer;
            if(speechRecognition==null)
            {
                speechRecognition = new SpeechRecognitionEngine(
                    new System.Globalization.CultureInfo("en-US")
                );
                // Create a default dictation grammar.
                DictationGrammar defaultDictationGrammar = new DictationGrammar();
                defaultDictationGrammar.Name = "default dictation";
                defaultDictationGrammar.Enabled = true;
                speechRecognition.LoadGrammar(defaultDictationGrammar);
                // Create the spelling dictation grammar.
                DictationGrammar spellingDictationGrammar = new DictationGrammar("grammar:dictation#spelling");
                spellingDictationGrammar.Name = "spelling dictation";
                spellingDictationGrammar.Enabled = true;
                speechRecognition.LoadGrammar(spellingDictationGrammar);

                // Configure input to the speech recognizer.
                speechRecognition.SetInputToDefaultAudioDevice();
            }
            _speechRecognition = speechRecognition;
        }

        public void Dispose()
        {
            _speechRecognition.Dispose();
        }

        public string ReadMessage()
        {
            RecognitionResult result = null;
            while( result == null)
                result = _speechRecognition.Recognize(new TimeSpan(0, 0, 30));
            Console.WriteLine("YOU> " + result.Text);

            if (result.Text != null && OnMessageReceived != null)
            {
                OnMessageReceived(this, result.Text);
            }

            return result.Text;
        }

        public void SendMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("BOT> ");
            Console.WriteLine(message.Replace("\n", "\n     "));
            Console.ResetColor();
            _speechSynthesizer.Speak(message);
            if (message != null && OnMessageSent != null)
            {
                OnMessageSent(this, message);
            }
        }

        public string AskQuestion(string message)
        {
            SendMessage(message);
            Console.Write("YOU> ");
            return ReadMessage();
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
