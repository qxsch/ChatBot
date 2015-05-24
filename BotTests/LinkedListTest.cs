using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QXS.ChatBot;

namespace QXS.ChatBot.Tests
{
    [TestClass]
    public class LinkedListTest
    {

        [TestMethod]
        public void TestCount()
        {
            LinkedList<int> liste = new LinkedList<int>();
            Assert.AreEqual(0, liste.Count, "Count Method returns a wrong result.");

            for (int i = 1; i <= 3; i++)
            {
                liste.Push(i);
                Assert.AreEqual(i, liste.Count, "Count Method returns a wrong result.");
            }
             
        }
    }
}
