#if WITHLYNC
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;

namespace QXS.ChatBot
{
    public class LyncConversation : ChatSessionInterface
    {

        protected Conversation _conversation;

        /// <summary>
        /// The session received a messsage
        /// </summary>
        public event Action<ChatSessionInterface, string> OnMessageReceived;

        /// <summary>
        /// The session replied to a message
        /// </summary>
        public event Action<ChatSessionInterface, string> OnMessageSent;

        public LyncConversation(Conversation Conversation)
        {
            _conversation = Conversation;
            SessionStorage = new SessionStorage();
        }

        protected Queue<string> _IncomingMessages = new Queue<string>();

        protected bool _MessageSent = false;

        public void InstantMessageReceived(object sender, MessageSentEventArgs e)
        {
            _IncomingMessages.Enqueue(e.Text);
        }

        public void sendMessage(string message)
        {
            Console.WriteLine("WRITE #" + message + "#");
            _MessageSent = false;
            try
            {
                IDictionary<InstantMessageContentType, string> textMessage = new Dictionary<InstantMessageContentType, string>();
                textMessage.Add(InstantMessageContentType.PlainText, message);
                if (((InstantMessageModality)_conversation.Modalities[ModalityTypes.InstantMessage]).CanInvoke(ModalityAction.SendInstantMessage))
                {
                    ((InstantMessageModality)_conversation.Modalities[ModalityTypes.InstantMessage]).BeginSendMessage(
                        textMessage
                        , SendMessageCallback
                        , textMessage);
                }

                if (message != null && OnMessageSent != null)
                {
                    OnMessageSent(this, message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Client Platform Exception: " + e.Message, "Send Message");
            }
        }

        public void SendMessageCallback(IAsyncResult ar)
        {
            //((InstantMessageModality)_conversation.Modalities[ModalityTypes.InstantMessage]).BeginSetComposing(false, ComposingCallback, null);
            if (ar.IsCompleted == true)
            {
                _MessageSent = true;
                try
                {
                    ((InstantMessageModality)_conversation.Modalities[ModalityTypes.InstantMessage]).EndSendMessage(ar);
                }
                catch (LyncClientException lce)
                {
                    Console.WriteLine("Lync Client Exception on EndSendMessage " + lce.Message);
                }

            }
        }


        public string readMessage()
        {
            while (_IncomingMessages.Count <= 0)
            {
                Thread.Sleep(500);
            }
            string s = _IncomingMessages.Dequeue().Trim();
            Console.WriteLine("READ #" + s + "#");

            if (s != null && OnMessageReceived != null)
            {
                OnMessageReceived(this, s);
            }

            return s;
        }

        public string askQuestion(string message)
        {
            _IncomingMessages.Clear();
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
#endif