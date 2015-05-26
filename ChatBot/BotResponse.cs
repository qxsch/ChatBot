using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QXS.ChatBot
{
    public class BotResponse
    {
        public BotResponse(string RuleName, string Question, string Answer)
        {
            this.RuleName = RuleName;
            this.Question = Question;
            this.Answer = (Answer.Length <= _MaxAnswerSize ? Answer : Answer.Substring(0, _MaxAnswerSize - 3));
        }
        public readonly string RuleName;
        public readonly string Question;
        public readonly string Answer;


        protected static int _MaxAnswerSize = 4096;
        public static int MaxAnswerSize
        {
            get { return _MaxAnswerSize;  }
            set
            {
                if (value < 10)
                {
                    throw new ArgumentOutOfRangeException("Size must be at least 10.");
                }
            }
        }
    }
}
