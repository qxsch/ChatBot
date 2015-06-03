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
    public class ChatBotRuleGenerator
    {
        public readonly string[] Prefixes;

        public ChatBotRuleGenerator(string[] prefixes = null)
        {
            if (prefixes == null)
            {
                Prefixes=new string[] { "", "QXS.ChatBot." };
            }
        }



        public string GetRuleName(XmlNode node)
        {
            return node.Attributes["Name"].Value;
        }

        public string GetRulePattern(XmlNode node)
        {
            foreach (XmlNode subnode in node.SelectNodes("Pattern"))
            {
                return subnode.InnerText;
            }
            return "";
        }

        public int GetRuleWeight(XmlNode node)
        {
            int weight = -100;
            foreach (XmlNode subnode in node.SelectNodes("Weight"))
            {
                if (Int32.TryParse(subnode.InnerText.Trim(), out weight))
                {
                    return weight;
                }
            }
            return weight;
        }

        public Type resolveBotRuleTypeByName(string name)
        {
            // resolve type
            Type type = null;
            foreach (string prefix in Prefixes)
            {
                type = Type.GetType(prefix + name);
                if (type != null)
                {
                    break;
                }

            }
            // instance of BotRule?
            if (!typeof(BotRule).IsAssignableFrom(type))
            {
                return null;
            }
            return type;
        }

        protected BotRule CreateRuleFromXml(Type type, XmlNode node)
        {
            MethodInfo method;
            while (type != null)
            {
                method = type.GetMethod("CreateRuleFromXml", new Type[] { typeof(ChatBotRuleGenerator), typeof(XmlNode) });
                if (method != null)
                {
                    return (BotRule)method.Invoke(null, new object[] { this, node });
                }
                type = type.BaseType;
            }
            return null;
        }


        protected BotRule ProcessNode(XmlNode node)
        {
            if (node.Attributes["Type"] == null || node.Attributes["Name"] == null)
            {
                return null;
            }

            Type type = resolveBotRuleTypeByName(node.Attributes["Type"].Value);
            if (type == null)
            {
                return null;
            }


            return CreateRuleFromXml(type, node);

        }

        public List<BotRule> Parse(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return Parse(doc);
        }

        public List<BotRule> Parse(Stream inStream)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(inStream);
            return Parse(doc);
        }

        public List<BotRule> Parse(TextReader reader)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            return Parse(doc);
        }

        public List<BotRule> Parse(XmlReader reader)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            return Parse(doc);
        }


        public List<BotRule> Parse(XmlDocument document, XmlNode startNode=null)
        {
            
            List<BotRule> liste = new List<BotRule>();
            if (startNode == null)
            {
                foreach (XmlNode node in document.SelectNodes("/ChatBot/Rules/Rule"))
                {
                    BotRule rule = ProcessNode(node);
                    if (rule != null)
                    {
                        liste.Add(rule);
                    }
                }
            }
            else
            {
                foreach (XmlNode node in startNode.SelectNodes("Rules/Rule"))
                {
                    BotRule rule = ProcessNode(node);
                    if (rule != null)
                    {
                        liste.Add(rule);
                    }
                }
            }

            return liste;
        }





    }
}
