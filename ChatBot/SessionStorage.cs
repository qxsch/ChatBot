using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QXS.ChatBot
{
    public class SessionStorage
    {
        public Dictionary<string, string> Values = new Dictionary<string, string>();
        public Stack<string> Stack = new Stack<string>();
        public void TrimStack(int elementsToKeep=5)
        {
            string[] newStack = Stack.ToArray();
            Stack = new Stack<string>(newStack.Skip(Math.Max(0, newStack.Length - elementsToKeep)));
        }
    }
}
