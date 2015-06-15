using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QXS.ChatBot
{
    public interface BotRuleVisualizerInterface
    {
        void Visualize(IEnumerable<BotRule> Rules);
    }
}
