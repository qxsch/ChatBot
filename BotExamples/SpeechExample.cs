using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Speech.Recognition;

namespace QXS.ChatBot.Examples
{
    class SpeechExample
    {
        static SpeechConversation _SpeechConversation;

        public static void SpeechChat(List<BotRule> rules)
        {

            using(SpeechRecognitionEngine speechRecognition = new SpeechRecognitionEngine(
                new System.Globalization.CultureInfo("en-US")
            ))
            { 
                // Create a default dictation grammar.
                DictationGrammar defaultDictationGrammar = new DictationGrammar()
                {
                    Name = "default dictation",
                    Enabled = true

                };
                speechRecognition.LoadGrammar(defaultDictationGrammar);
                // Create the spelling dictation grammar.
                DictationGrammar spellingDictationGrammar = new DictationGrammar("grammar:dictation#spelling")
                {
                    Name = "spelling dictation",
                    Enabled = true
                };
                speechRecognition.LoadGrammar(spellingDictationGrammar);

                // Add Grammar for the demo, to make it more reliable:
                //  https://msdn.microsoft.com/en-us/library/hh362944(v=office.14).aspx
                //  http://dailydotnettips.com/2014/01/18/using-wildcard-with-grammar-builder-kinect-speech-recognition/


                // Configure input to the speech recognizer.
                speechRecognition.SetInputToDefaultAudioDevice();


                using (_SpeechConversation = new SpeechConversation(speechRecognition: speechRecognition))
                {
                    ChatBot cb = new ChatBot(rules);
                    cb.talkWith(_SpeechConversation);
                    Console.ReadKey();
                }
            }
        }
    }
}
