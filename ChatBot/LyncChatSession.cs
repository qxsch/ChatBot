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

    /*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;

namespace LyncTest
{
    class Program
    {
        static LyncClient client;
        static Conversation _conversation;

        static void Main(string[] args)
        {
            client = LyncClient.GetClient();

            client.ConversationManager.ConversationAdded += ConversationManager_ConversationAdded;

            client.ConversationManager.ConversationRemoved += ConversationManager_ConversationRemoved;

            while (_conversation == null)
            {
                Thread.Sleep(1000);
            }

            Console.WriteLine("Got it");
            

            Console.ReadKey();
        }


        static void SendMessage(string messageToSend)
        {
            try
            {
                IDictionary<InstantMessageContentType, string> textMessage = new Dictionary<InstantMessageContentType, string>();
                textMessage.Add(InstantMessageContentType.PlainText, messageToSend);
                if (((InstantMessageModality)_conversation.Modalities[ModalityTypes.InstantMessage]).CanInvoke(ModalityAction.SendInstantMessage))
                {
                    ((InstantMessageModality)_conversation.Modalities[ModalityTypes.InstantMessage]).BeginSendMessage(
                        textMessage
                        , SendMessageCallback
                        , textMessage);
                }                
            }
            catch (Exception e)
            {
               Console.WriteLine("Client Platform Exception: " + e.Message, "Send Message");
            }
        }

        static void SendMessageCallback(IAsyncResult ar)
        {
            //((InstantMessageModality)_conversation.Modalities[ModalityTypes.InstantMessage]).BeginSetComposing(false, ComposingCallback, null);
            if (ar.IsCompleted == true)
            {

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


        static void ConversationManager_ConversationRemoved(object sender, ConversationManagerEventArgs e)
        {
            Conversation conversation = e.Conversation;

            if (conversation != null)
            {
                Console.WriteLine("Closed conversation details: ");
                if (conversation.Participants != null)
                {
                    foreach (Participant p in conversation.Participants)
                    {
                        Console.WriteLine("  P:" + p.Contact.Uri);
                    }

                }

                if (_conversation == conversation)
                {
                    _conversation = null;
                }
            }
        }

        static void ConversationManager_ConversationAdded(object sender, ConversationManagerEventArgs e)
        {

            Conversation conversation = e.Conversation;

            var details = "Hello Lync 2013 SDK!" + Environment.NewLine;



            if (conversation != null)
            {

                if (conversation.Properties.ContainsKey(ConversationProperty.Inviter))
                {

                    var contact = (Contact)conversation.Properties[ConversationProperty.Inviter];

                    if (contact != null)
                    {

                        details += "  INVITE FROM: " + contact.Uri + Environment.NewLine;

                    }

                }

                foreach (Participant p in conversation.Participants)
                {
                    details += "  Participiant:" + p.Contact.Uri + Environment.NewLine;
                }

                if (_conversation == null)
                {
                    _conversation = conversation;
                    foreach (Participant p in conversation.Participants)
                    {
                        if (client.Self.Contact.Uri.ToLower() != p.Contact.Uri.ToLower())
                        {
                            ((InstantMessageModality)p.Modalities[ModalityTypes.InstantMessage]).InstantMessageReceived += InstantMessageReceived;
                        }
                    }

                    SendMessage("hello");
                }
            }

            Console.WriteLine("Incomming conversation details: " + details);


        }

        static void InstantMessageReceived(object sender, MessageSentEventArgs e)
        {
            Console.WriteLine(e.Text);
            SendMessage("OK, i take care of that");

        }

    }
}
*/
    public class LyncConversation : ChatSessionInterface
    {

        protected Conversation _conversation;

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
