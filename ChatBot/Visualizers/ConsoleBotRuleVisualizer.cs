using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QXS.ChatBot
{
    public class ConsoleBotRuleVisualizer : BotRuleVisualizerInterface
    {

        public void Visualize(IEnumerable<BotRule> Rules)
        {
            WriteRules(Rules);
        }

        protected void WriteRules(IEnumerable<BotRule> Rules, int indent=0)
        {
            string prefix = new String(' ', indent * 3);
            foreach (BotRule rule in Rules)
            {
                Console.WriteLine(prefix + "Type:   " + rule.GetType().FullName);
                Console.WriteLine(prefix + "Name:   " + rule.Name);
                Console.WriteLine(prefix + "Weight: " + rule.Weight);
                Console.WriteLine(prefix + "Regex:  " + rule.MessagePattern);
                Console.WriteLine();
                if (rule is BotRuleContainer)
                {
                    List<BotRule> NestedRules = new List<BotRule>();
                    foreach(KeyValuePair<int,List<BotRule>> kv in ((BotRuleContainer)rule).NestedBotRules)
                    {
                        foreach (BotRule b in kv.Value)
                        {
                            NestedRules.Add(b);
                        }
                    }
                    WriteRules(NestedRules, ++indent);
                }               
            }
        }
    }
}
