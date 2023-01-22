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
    /// <summary>
    /// Uses PowerShell to execute the powershell script
    /// </summary>
    public class PowershellBotRule : BotRule
    {
        protected string _script;
        protected bool _showErrors = true;

        public PowershellBotRule(string name, int weight, Regex messagePattern, string script)
            : base(name, weight, messagePattern)
        {
            this._script = script;
            this._Process = this.ProcessScript;
        }

        public PowershellBotRule(string name, int weight, Regex messagePattern, string script, bool showErrors)
            : this(name, weight, messagePattern, script)
        {
            this._showErrors = showErrors;
        }


        /// <summary>
        /// Process the powershell script
        /// </summary>
        /// <param name="match"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public string ProcessScript(Match match, IChatSessionInterface session)
        {
            string output = "";

            using(PowerShell ps = PowerShell.Create())
            {
                // we must import the parameters $session, $match
                ps.AddScript("Param($match, $session)\n" + this._script);
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
