using System;
using System.Collections.Generic;
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

        public List<BotRule> Parse(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return Parse(doc);
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

        public List<BotRule> Parse(XmlDocument document, XmlNode startNode=null)
        {
            Type type;
            List<BotRule> liste = new List<BotRule>();
            if (startNode == null)
            {
                foreach (XmlNode node in document.SelectNodes("/ChatBot/Rules/Rule"))
                {
                    if (node.Attributes["Type"] == null || node.Attributes["Name"] == null)
                    {
                        continue;
                    }


                    type = resolveBotRuleTypeByName(node.Attributes["Type"].Value);
                    if (type == null)
                    {
                        continue;
                    }

                    // node.OwnerDocument

                    BotRule rule = CreateRuleFromXml(type, node);
                    //BotRule rule = (BotRule)type.GetMethod("CreateRuleFromXml", new Type[] { typeof(XmlNode) }).Invoke(null, new object[] { node });
                    //BotRule rule = (BotRule)type.GetMethod("CreateRuleFromXml", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { node });
                    /*if (rule != null)
                    {
                        liste.Add(rule);
                    }*/

                    Console.WriteLine("CLASS: " + type);
                    Console.WriteLine("INHER: " + type.BaseType);
                    Console.WriteLine("RULE " + rule);
                    Console.WriteLine();
                }
            }
            else
            {

            }

            return liste;
        }
        /*<ChatBot>
	                        <Rules>
		                        <Rule Type=""BotRule"" Name="""">
			                        <Pattern><![CDATA[]]></Pattern>
			                        <Weight></Weight>
			                        <Process><![CDATA[
			                        ]]></Process>
		                        </Rule>
		
		                        <Rule Type=""RandomAnswersBotRule"" Name="""">
			                        <Pattern><![CDATA[]]></Pattern>
			                        <Weight></Weight>
			                        <Messages>
				                        <Message></Message>
			                        </Messages>
		                        </Rule>
		
		                        <Rule Type=""ReplacementBotRule"" Name="""">
			                        <Pattern><![CDATA[]]></Pattern>
			                        <Weight></Weight>
			                        <Messages>
				                        <Message></Message>
			                        </Messages>
			                        <Setters>
				                        <Set key=""KEY"">Value</Set>
			                        </Setters>
		                        </Rule>
		
		                        <Rule Type=""ConditionalBotRule"" Name="""">
			                        <Weight></Weight>
			                        <Conditions>
				                        <Condition Key=""KEY"" Operator=""Equal"">VALUE</Condition>
			                        </Conditions>
			                        <Rules>
				                        <!-- ... -->
			                        </Rules>
		                        </Rule>
	                        </Rules>
                        </ChatBot> */
    }
}
