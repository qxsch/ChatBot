using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QXS.ChatBot
{
    public abstract class BotRuleContainer : BotRule
    {
        protected BotRuleContainer(string Name, int Weight)
            : base(Name, Weight)
        {
        }

        protected BotRuleContainer(string Name, int Weight, Regex MessagePattern)
            : base(Name, Weight, MessagePattern)
        {
        }

        protected BotRuleContainer(string Name, int Weight, Regex MessagePattern, Func<Match, IChatSessionInterface, string> Process)
            : base(Name, Weight, MessagePattern, Process)
        {

        }

        protected SortedList<int, List<BotRule>> _NestedBotRules = new SortedList<int, List<BotRule>>(new DescComparer<int>());
        public SortedList<int, List<BotRule>> NestedBotRules { get { return _NestedBotRules; } }
    }
}
