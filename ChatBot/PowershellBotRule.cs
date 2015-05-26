using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Collections.ObjectModel;

namespace QXS.ChatBot
{
    public class PowershellBotRule : BotRule
    {
        protected string _Script;

        public PowershellBotRule(string Name, int Weight, Regex MessagePattern, string Script)
            : base(Name, Weight, MessagePattern)
        {
            this._Script = Script;
            this._Process = this.ProcessScript;
        }

        
        public string ProcessScript(Match match, ChatSessionInterface session)
        {
            string output = "";

            using(PowerShell ps = PowerShell.Create())
            {
                ps.AddScript(this._Script);
                ps.AddParameter("match", match);
                ps.AddParameter("session", session);
                //Collection<PSObject> PSOutput = ps.Invoke();
                foreach (PSObject outputItem in ps.Invoke())
                {
                    output += outputItem.BaseObject.ToString() + "\n";
                }
            }

            return output.Trim();
        }
    }
}
