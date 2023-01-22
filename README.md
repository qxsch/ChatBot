# ChatBot

Just a Proof of Concept ;-)

 * Multiple BotRules are supported
   * BotRule
     `Runs a delegate / Extend this rule to create custom ones`
   * ConditionBotRule
     `Runs nested Rules, in case conditions are satisfied`
   * PowershellBotRule
     `Uses the powershell`
   * RandomAnswersBotRule
     `Generates a random answer`
   * ReplacementBotRule
     `Returns a answer, that contains words from the previous conversations, and can also be used to learn something`
 * Supported Conversation types:
   * Console Conversations
   * Lync Conversations
   * Text-To-Speech & Speech-Recognition Conversations
   * Any other Conversation, that implements the ChatSessionInterface
 * Supports Bot-Definition from:
   * XML
   * C#/.NET/Powershell
   * or any mixture of it
 * Visualizers, that visualize your current ruleset


Checkout the example

![Image of a chatbot conversation](https://raw.githubusercontent.com/qxsch/ChatBot/master/ChatBot.jpg)


### C# Example
```cs
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

            ChatBot cb = new ChatBot(
                new List<BotRule>()
                {
                    new ReplacementBotRule("repeat", 40, new Regex("(please )?repeat(?<sentence> .*)", RegexOptions.IgnoreCase), new string[] { "i repeat your sentence:$r$sentence$", "$s$BotName$ repeats your sentence:$r$sentence$"}),
                    new RandomAnswersBotRule("getfeeling", 40, new Regex("how do you feel", RegexOptions.IgnoreCase), new string[] {"i feel great", "i feel tired", "i feel awful", "i feel happy"}),
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
                    new BotRule(
                        Name: "setbotname",
                        Weight: 10, 
                        MessagePattern: new Regex("(your name is|you are) (now )?(.*)", RegexOptions.IgnoreCase),
                        Process: delegate(Match match, ChatSessionInterface session) {
                            session.SessionStorage.Values["BotName"] = match.Groups[3].Value;
                            return "My name is now " + session.SessionStorage.Values["BotName"];
                        }
                    ),
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
                    new BotRule(
                        Name: "setusername",
                        Weight: 10, 
                        MessagePattern: new Regex("my name is (now )?(.*)", RegexOptions.IgnoreCase),
                        Process: delegate(Match match, ChatSessionInterface session) {
                            session.SessionStorage.Values["UserName"] = match.Groups[2].Value;
                            return "Hi " + session.SessionStorage.Values["UserName"];
                        }
                    ),
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

            cb.TalkWith(new ConsoleChatSession());
        }
    }
}
```

### XML Example

```xml
<?xml version="1.0" encoding="utf-8"?>
<ChatBot xmlns="http://www.qxs.ch/ChatBotSchema.xsd">
  <Rules>
     <Rule Type="ReplacementBotRule" Name="repeat-last-sentence">
      <Pattern><![CDATA[(please )?repeat the last sentence]]></Pattern>
      <Weight>45</Weight>
      <Messages>
        <Message>i repeat your sentence: $s$sentence$</Message>
        <Message>$s$BotName$ repeats your sentence: $s$sentence$</Message>
      </Messages>
    </Rule>
      
    <Rule Type="ReplacementBotRule" Name="repeat-sentence">
      <Pattern><![CDATA[(please )?repeat(?<sentence> .*)]]></Pattern>
      <Weight>40</Weight>
      <Messages>
        <Message>i repeat your sentence: $r$sentence$</Message>
        <Message>$s$BotName$ repeats your sentence: $r$sentence$</Message>
      </Messages>
      <Setters>
        <Set Key="sentence">$r$sentence$</Set>
      </Setters>
    </Rule>
      
    <Rule Type="BotRule" Name="b1">
      <Pattern><![CDATA[please say (?<word>\S+)]]></Pattern>
      <Weight>40</Weight>
      <Process><![CDATA[
      // Enter C# Code
      // AVAILABLE VARIABLES:
      //   System.Text.RegularExpressions.Match  match;
      //   QXS.ChatBot.ChatSessionInterface      session;
      return "ok, i say " + match.Groups["word"].Value;
      ]]></Process>
    </Rule>

    <Rule Type="PowershellBotRule" Name="pstest">
      <Pattern><![CDATA[powershell]]></Pattern>
      <Weight>30</Weight>
      <Script><![CDATA[
        # Enter Powershell Code
        # AVAILABLE VARIABLES:
        #   System.Text.RegularExpressions.Match $match
        #   QXS.ChatBot.ChatSessionInterface     $session
        $name = "John Doe"
        if ($session.SessionStorage.Values.ContainsKey("username")) {
          $name = $session.SessionStorage.Values["username"]
        }
      ( "Hi $name from PowerShell " + $PSVersionTable.PSVersion )
      ]]></Script>
    </Rule>
    
    <Rule Type="RandomAnswersBotRule" Name="feeling">
      <Pattern><![CDATA[how (are you|do you feel)]]></Pattern>
      <Weight>20</Weight>
      <Messages>
        <Message>i feel ok</Message>
        <Message>i am a bit bored</Message>
      </Messages>
    </Rule>

    <Rule Type="ReplacementBotRule" Name="set-username">
      <Pattern><![CDATA[(my name is|i am|i'm) (now )?(?<username>.*)]]></Pattern>
      <Weight>20</Weight>
      <Messages>
        <Message>your name is now $r$username$</Message>
        <Message>you are now $r$username$</Message>
        <Message>pleased to meet you $r$username$</Message>
      </Messages>
      <Setters>
        <Set Key="username">$r$username$</Set>
      </Setters>
    </Rule>
    
    <Rule Type="RandomAnswersBotRule" Name="get-username">
      <Pattern><![CDATA[(what is|say) my name]]></Pattern>
      <Weight>20</Weight>
      <Messages>
        <Message>i don't know your name</Message>
        <Message>who are you?</Message>
      </Messages>
    </Rule>
    
    <Rule Type="ReplacementBotRule" Name="set-botname">
      <Pattern><![CDATA[(your name is|you are) (now )?(?<botname>.*)]]></Pattern>
      <Weight>20</Weight>
      <Messages>
        <Message>my name is now $r$botname$</Message>
        <Message>i am now $r$botname$</Message>
      </Messages>
      <Setters>
        <Set Key="botname">$r$botname$</Set>
      </Setters>
    </Rule>
    
    <Rule Type="RandomAnswersBotRule" Name="get-botname">
      <Pattern><![CDATA[(who are you|(what is|say) your name)]]></Pattern>
      <Weight>20</Weight>
      <Messages>
        <Message>i do not have a name</Message>
      </Messages>
    </Rule>

    <!-- This condition fires in case a botname was set -->
    <Rule Type="ConditionBotRule" Name="botname-condition">
      <Weight>60</Weight>
      <Conditions>
        <Condition Key="botname" Operator="ContainsKey"></Condition>
      </Conditions>
      <Rules>
        <Rule Type="ReplacementBotRule" Name="get-botname">
          <Pattern><![CDATA[(who are you|(what is|say) your name)]]></Pattern>
          <Weight>20</Weight>
          <Messages>
            <Message>My name is $s$botname$</Message>
            <Message>I am $s$botname$</Message>
          </Messages>
        </Rule>
      </Rules>
    </Rule>
  
    <!-- This condition fires in case a username was set -->
    <Rule Type="ConditionBotRule" Name="username-condition">
      <Weight>60</Weight>
      <Conditions>
        <Condition Key="username" Operator="ContainsKey"></Condition>
      </Conditions>
      <Rules>
        <Rule Type="ReplacementBotRule" Name="get-username">
          <Pattern><![CDATA[(what is|say) my name]]></Pattern>
          <Weight>20</Weight>
          <Messages>
            <Message>Your name is $s$username$</Message>
            <Message>You are $s$username$</Message>
          </Messages>
        </Rule>
      </Rules>
    </Rule>

  </Rules>
</ChatBot>
```
