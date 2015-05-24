using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QXS.ChatBot;
using System.Text.RegularExpressions;

namespace QXS.ChatBot.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            /*string xml = @"<ChatBot>
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
                        </ChatBot>";*/


            ChatBot cb = new ChatBot(
                new List<BotRule>()
                {
                    // debug rule
                    new BotRule(
                        Name: "var_dump",
                        Weight: 200, 
                        MessagePattern: new Regex("^var_?dump$", RegexOptions.IgnoreCase),
                        Process: delegate(Match match, ChatSessionInterface session) {
                            string answer = "Variables: \n";
                            foreach (string key in session.SessionStorage.Values.Keys)
                            {
                                answer += "  " + key + " = " + session.SessionStorage.Values[key] + "\n";
                            }
                            answer += "---\n";
                            answer += "History: \n";
                            int i=0;
                            foreach (string ruleName in session.GetRuleHistory())
                            {
                                answer += "  " + (++i) + ". " + ruleName + "\n";
                            }
                            return answer;
                        }
                    ),

                    // chatbot specific behaviour
                    new ConditionBotRule(
                        "conditionBot", 
                        50, 
                        new Tuple<string, ConditionBotRule.Operator, string>[] { 
                            new Tuple<string, ConditionBotRule.Operator, string>("BotName", ConditionBotRule.Operator.EqualIgnoreCase, "chatbot") 
                        }, 
                        new BotRule[] {
                            // chatbot just knows positive feelings...
                            new RandomAnswersBotRule("getfeeling2", 40, new Regex("how do you feel", RegexOptions.IgnoreCase), new string[] {"i feel super", "i feel perfect", "i feel happy"}),
                        }
                    ),
                    // repet the last known sentence
                    new ReplacementBotRule("repeatLast", 41, new Regex("(please )?repeat the last sentence", RegexOptions.IgnoreCase), new string[] { "i repeat your last sentence:$s$sentence$", "$s$BotName$ repeats your last sentence:$s$sentence$"}),
                    // repet a sentence
                    new ReplacementBotRule("repeat", 40, new Regex("(please )?repeat(?<sentence> .*)", RegexOptions.IgnoreCase), new string[] { "i repeat your sentence:$r$sentence$", "$s$BotName$ repeats your sentence:$r$sentence$"}, new Dictionary<string,string>() { {"sentence", "$r$sentence$"} }),
                    // reports your feelings
                    new RandomAnswersBotRule("getfeeling", 40, new Regex("how do you feel", RegexOptions.IgnoreCase), new string[] {"i feel great", "i feel tired", "i feel bored"}),

                    // set the name of the bot
                    new BotRule(
                        Name: "setbotname",
                        Weight: 10, 
                        MessagePattern: new Regex("(your name is|you are) (now )?(.*)", RegexOptions.IgnoreCase),
                        Process: delegate(Match match, ChatSessionInterface session) {
                            session.SessionStorage.Values["BotName"] = match.Groups[3].Value;
                            return "My name is now " + session.SessionStorage.Values["BotName"];
                        }
                    ),
                    // show the bot's name
                    new BotRule(
                        Name: "getbotname",
                        Weight: 10, 
                        MessagePattern: new Regex("(who are you|(what is|say) your name)", RegexOptions.IgnoreCase),
                        Process: delegate(Match match, ChatSessionInterface session) {
                            if (!session.SessionStorage.Values.ContainsKey("BotName"))
                            {
                                return "I do not have a name";
                            }
                            if (match.Value.ToLower() == "who are you") 
                            {
                                return "i am " + session.SessionStorage.Values["BotName"];
                            }
                            return "My name is " + session.SessionStorage.Values["BotName"];
                        }
                    ),

                    // set the name of the user
                    new BotRule(
                        Name: "setusername",
                        Weight: 10, 
                        MessagePattern: new Regex("my name is (now )?(.*)", RegexOptions.IgnoreCase),
                        Process: delegate(Match match, ChatSessionInterface session) {
                            session.SessionStorage.Values["UserName"] = match.Groups[2].Value;
                            return "Hi " + session.SessionStorage.Values["UserName"];
                        }
                    ),
                    // show the user's name
                    new BotRule(
                        Name: "getusername",
                        Weight: 20, 
                        MessagePattern: new Regex("(what is|say) my name", RegexOptions.IgnoreCase),
                        Process: delegate(Match match, ChatSessionInterface session) {
                            if (!session.SessionStorage.Values.ContainsKey("UserName"))
                            {
                                return "Sorry, but you have not told my your name";
                            }
                            return "Your name is " + session.SessionStorage.Values["UserName"];
                        }
                    ),

                    // greet
                    new BotRule(
                        Name: "greet",
                        Weight: 1, 
                        MessagePattern: new Regex("(hi|hello)", RegexOptions.IgnoreCase),
                        Process: delegate(Match match, ChatSessionInterface session) {
                            string answer = "Hi";

                            if (session.SessionStorage.Values.ContainsKey("UserName"))
                            {
                                answer += " " + session.SessionStorage.Values["UserName"];
                            }
                            answer += "!";
                            if (session.SessionStorage.Values.ContainsKey("BotName"))
                            {
                                answer += " I'm " + session.SessionStorage.Values["BotName"];
                            }
                            return answer;
                        }

                    )
                }
            );

            cb.talkWith(new ConsoleChatSession());
        }
    }
}
