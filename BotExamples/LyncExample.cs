using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;


namespace QXS.ChatBot.Examples
{


    public class LyncExample
    {
        static LyncClient client;
        static Conversation _conversation;
        static LyncConversation _LyncConversation;

        public static void LyncChat(List<BotRule> rules)
        {
            client = LyncClient.GetClient();

            client.ConversationManager.ConversationAdded += ConversationManager_ConversationAdded;

            client.ConversationManager.ConversationRemoved += ConversationManager_ConversationRemoved;

            while (_conversation == null)
            {
                Thread.Sleep(1000);
            }

            ChatBot cb = new ChatBot(rules);
            cb.talkWith(_LyncConversation);
            Console.ReadKey();
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
                    _LyncConversation = null;
                }
            }
        }

        static void ConversationManager_ConversationAdded(object sender, ConversationManagerEventArgs e)
        {

            Conversation conversation = e.Conversation;

            var details = "Hello Lync 2013 SDK!" + Environment.NewLine;



            if (conversation != null)
            {
                if (e.Conversation.Modalities[ModalityTypes.InstantMessage].State == ModalityState.Notified)
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
                        _LyncConversation = new LyncConversation(conversation);
                        foreach (Participant p in conversation.Participants)
                        {
                            if (client.Self.Contact.Uri.ToLower() != p.Contact.Uri.ToLower())
                            {
                                ((InstantMessageModality)p.Modalities[ModalityTypes.InstantMessage]).InstantMessageReceived += _LyncConversation.InstantMessageReceived;
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Incomming conversation details: " + details);


        }



    }
}
