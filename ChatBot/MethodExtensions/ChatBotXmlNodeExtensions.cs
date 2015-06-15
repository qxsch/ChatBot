using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace QXS.ChatBot
{
    public static class ChatBotXmlNodeExtensions
    {
        public const string ChatBotNamespace = "http://www.qxs.ch/ChatBotSchema.xsd";

        public static XmlDocument GetXmlDocument(this XmlNode node)
        {
            XmlNode parentNode = node;
            while (!(parentNode is XmlDocument) && parentNode != null)
            {
                if (parentNode.OwnerDocument != null)
                {
                    return parentNode.OwnerDocument;
                }
                parentNode = node.ParentNode;
            }
            if (parentNode == null)
            {
                return null;
            }
            return (XmlDocument)parentNode;
        }

        public static XmlNodeList SelectChatBotNodes(this XmlNode node, string xpath)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(node.GetXmlDocument().NameTable);
            ns.AddNamespace("cb", ChatBotNamespace);
            return node.SelectNodes(xpath, ns);
        }
        public static XmlNode SelectSingleChatBotNode(this XmlNode node, string xpath)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(node.GetXmlDocument().NameTable);
            ns.AddNamespace("cb", ChatBotNamespace);
            return node.SelectSingleNode(xpath, ns);
        }
    }
}
