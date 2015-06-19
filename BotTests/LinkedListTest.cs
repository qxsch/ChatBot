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

        [TestMethod]
        public void TestAdd()
        {
            LinkedList<int> liste = new LinkedList<int>();
            liste.Add(1);
            liste.Add(2);
            liste.Add(3);

            int ii = 1;
            foreach (int i in liste)
            {
                Assert.AreEqual(ii++, i, "Add Method inserted a wrong result.");
            }
            Assert.AreEqual(1, liste.First, "First Value is incorrect.");
            Assert.AreEqual(3, liste.Last, "Last Value is incorrect.");
        }

        [TestMethod]
        public void TestEnqueue()
        {
            LinkedList<int> liste = new LinkedList<int>();
            liste.Enqueue(1);
            liste.Enqueue(2);
            liste.Enqueue(3);

            int ii = 3;
            foreach (int i in liste)
            {
                Assert.AreEqual(ii--, i, "Enqueue Method inserted a wrong result.");
            }
            Assert.AreEqual(3, liste.First, "First Value is incorrect.");
            Assert.AreEqual(1, liste.Last, "Last Value is incorrect.");
        }

        [TestMethod]
        public void TestPush()
        {
            LinkedList<int> liste = new LinkedList<int>();
            liste.Push(1);
            liste.Push(2);
            liste.Push(3);

            int ii = 3;
            foreach (int i in liste)
            {
                Assert.AreEqual(ii--, i, "Push Method inserted a wrong result.");
            }
            Assert.AreEqual(3, liste.First, "First Value is incorrect.");
            Assert.AreEqual(1, liste.Last, "Last Value is incorrect.");
        }

        [TestMethod]
        public void TestPop()
        {
            LinkedList<int> liste = new LinkedList<int>();
            liste.Push(1);
            liste.Push(2);
            liste.Push(3);

            for (int i = 3; i >= 1; i--)
            {
                Assert.AreEqual(i, liste.Pop(), "Pop Method returns a wrong result.");
            }
            Assert.AreEqual(0, liste.Count, "Count returns a wrong result.");
            Assert.AreEqual(0, liste.First, "First is not 0.");
            Assert.AreEqual(0, liste.Last, "Last is not 0.");

            LinkedList<string> liste2 = new LinkedList<string>();
            liste2.Push("1");
            liste2.Push("2");
            liste2.Push("3");

            for (int i = 3; i >= 1; i--)
            {
                Assert.AreEqual(i.ToString(), liste2.Pop(), "Pop Method returns a wrong result.");
            }
            Assert.AreEqual(0, liste2.Count, "Count returns a wrong result.");
            Assert.AreEqual(null, liste2.First, "First is not null.");
            Assert.AreEqual(null, liste2.Last, "Last is not null.");
        }

        [TestMethod]
        public void TestPopWithLimit()
        {
            LinkedList<int> liste = new LinkedList<int>(3, false);
            liste.Push(1);
            liste.Push(2);
            liste.Push(3);
            liste.Push(4);

            for (int i = 4; i >= 2; i--)
            {
                Assert.AreEqual(i, liste.Pop(), "Pop Method returns a wrong result.");
            }
            Assert.AreEqual(0, liste.Count, "Count returns a wrong result.");
            Assert.AreEqual(0, liste.First, "First is not 0.");
            Assert.AreEqual(0, liste.Last, "Last is not 0.");

            LinkedList<string> liste2 = new LinkedList<string>(3,false);
            liste2.Push("1");
            liste2.Push("2");
            liste2.Push("3");
            liste2.Push("4");
            for (int i = 4; i >= 2; i--)
            {
                Assert.AreEqual(i.ToString(), liste2.Pop(), "Pop Method returns a wrong result.");
            }
            Assert.AreEqual(0, liste2.Count, "Count returns a wrong result.");
            Assert.AreEqual(null, liste2.First, "First is not null.");
            Assert.AreEqual(null, liste2.Last, "Last is not null.");
        }

        [TestMethod]
        public void TestDequeue()
        {
            LinkedList<int> liste = new LinkedList<int>();
            liste.Enqueue(1);
            liste.Enqueue(2);
            liste.Enqueue(3);

            for (int i = 1; i <= 3; i++)
            {
                Assert.AreEqual(i, liste.Dequeue(), "Dequeue Method returns a wrong result.");
            }
            Assert.AreEqual(0, liste.Count, "Count returns a wrong result.");
            Assert.AreEqual(0, liste.First, "First is not 0.");
            Assert.AreEqual(0, liste.Last, "Last is not 0.");
        }

        [TestMethod]
        public void TestDequeueWithLimit()
        {
            LinkedList<int> liste = new LinkedList<int>(3, false);
            liste.Enqueue(1);
            liste.Enqueue(2);
            liste.Enqueue(3);
            liste.Enqueue(4);

            for (int i = 2; i <= 4; i++)
            {
                Assert.AreEqual(i, liste.Dequeue(), "Dequeue Method returns a wrong result.");
            }
            Assert.AreEqual(0, liste.Count, "Count returns a wrong result.");
            Assert.AreEqual(0, liste.First, "First is not 0.");
            Assert.AreEqual(0, liste.Last, "Last is not 0.");
        }

        [TestMethod, ExpectedException(typeof(LinkedListException))]
        public void TestDequeueThrowsException()
        {
            LinkedList<int> liste = new LinkedList<int>();
            liste.Enqueue(1);

            liste.Dequeue();
            liste.Dequeue();
        }

        [TestMethod, ExpectedException(typeof(LinkedListException))]
        public void TestPopThrowsException()
        {
            LinkedList<int> liste = new LinkedList<int>();
            liste.Push(1);

            liste.Pop();
            liste.Pop();
        }

        [TestMethod, ExpectedException(typeof(LinkedListException))]
        public void TestPushWithLimitThrowsException()
        {
            LinkedList<int> liste = new LinkedList<int>(3);
            liste.Push(1);
            liste.Push(2);
            liste.Push(3);
            liste.Push(4);

        }

        [TestMethod, ExpectedException(typeof(LinkedListException))]
        public void TestEnqueueWithLimitThrowsException()
        {
            LinkedList<int> liste = new LinkedList<int>(3);
            liste.Enqueue(1);
            liste.Enqueue(2);
            liste.Enqueue(3);
            liste.Enqueue(4);

        }

    }
}
