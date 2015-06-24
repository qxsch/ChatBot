using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Xml;

namespace QXS.ChatBot
{
    public class PowershellBotRule : BotRule
    {
        protected string _Script;
        protected bool _showErrors = true;

        public PowershellBotRule(string Name, int Weight, Regex MessagePattern, string Script)
            : base(Name, Weight, MessagePattern)
        {
            this._Script = Script;
            this._Process = this.ProcessScript;
        }
        public PowershellBotRule(string Name, int Weight, Regex MessagePattern, string Script, bool showErrors)
            : this(Name, Weight, MessagePattern, Script)
        {
            this._showErrors = showErrors;
        }

        
        public string ProcessScript(Match match, ChatSessionInterface session)
        {
            string output = "";

            using(PowerShell ps = PowerShell.Create())
            {
                // we múst import the parameters $session, $match
                ps.AddScript("Param($match, $session)\n" + this._Script);
                ps.AddParameter("match", match);
                ps.AddParameter("session", session);
                foreach (PSObject outputItem in ps.Invoke())
                {
                    output += outputItem.BaseObject.ToString() + "\n";
                }
                if (this._showErrors)
                {
                    foreach (ErrorRecord e in ps.Streams.Error)
                    {
                        output += "ERROR: " + e.ToString() + "\n";
                    }
                }
            }

            output = output.Trim();
            if (output == "")
            {
                return null;
            }

            return output;
        }

        new public static BotRule CreateRuleFromXml(ChatBotRuleGenerator generator, XmlNode node)
        {
            return new PowershellBotRule(
                generator.GetRuleName(node),
                generator.GetRuleWeight(node),
                new Regex(generator.GetRulePattern(node)),
                node.SelectChatBotNodes("cb:Script").Cast<XmlNode>().First().InnerText
            );
        }
    }
}
