using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QXS.ChatBot
{



    internal class LinkedValue<T>
    {
        public T current;
        public LinkedValue<T> previous;
        public LinkedValue<T> next;
    }


    public class LinkedListEnumerator<T> : IEnumerator<T>, IEnumerator
    {
        private LinkedList<T> list;
        private LinkedValue<T> now;
        public LinkedListEnumerator(LinkedList<T> list)
        {
            this.list = list;
            this.now = list.firstValue;
        }

        public bool MoveNext()
        {
            if (this.now.next == null)
            {
                return false;
            }
            this.now = this.now.next;
            return true;
        }

        public void Reset()
        {
            this.now = list.firstValue;
        }

        void IDisposable.Dispose()
        {
            list = null;
            now = null;
        }

        public T Current
        {
            get { return this.now.current; }
        }


        object IEnumerator.Current
        {
            get { return (object)Current; }
        }

    }


    public class LinkedList<T> : ICollection<T>, IEnumerable<T>, IEnumerable
    {

        internal LinkedValue<T> firstValue;
        internal LinkedValue<T> lastValue;
        public readonly int Size;
        private bool _throwWhenFull;
        private int _elements;

        public LinkedList(int size=0, bool throwWhenFull=true)
        {
            this.Size = size;
            this._throwWhenFull = throwWhenFull;
            this._elements = 0;
        }

        public int Count { get { return _elements; } }
        public bool IsReadOnly { get { return false; } }

        public T First { get { return firstValue.current; } }
        public T Last { get { return lastValue.current; } }

        public IEnumerator<T> GetEnumerator()
        {
            return new LinkedListEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)new LinkedListEnumerator<T>(this);
        }


        public void CopyTo(T[] array, int arrayIndex)
        {
            LinkedValue<T> val = firstValue;

            while (val != null) {
                array[arrayIndex++] = val.current;
                val = val.next;
            }
        }

        public bool Remove(T item)
        {
            LinkedValue<T> val = firstValue;
            while (val != null)
            {
                if (val.current.Equals(item))
                {
                    val.previous.next = val.next;
                    val.next.previous = val.previous;
                    val.next = null;
                    val.previous = null;
                    return true;
                }

                val = val.next;
            }
            return false;
        }

        public bool Contains(T item)
        {
            LinkedValue<T> val = firstValue;
            while (val != null)
            {
                if (val.current.Equals(item))
                {
                    return true;
                }

                val = val.next;
            }
            return false;
        }
        public void Add(T item)
        {
            Push(item);
        }

        public void Push(T item) {
            firstValue = new LinkedValue<T>() { current = item, next = firstValue };
            _elements++;
            if (lastValue == null)
            {
                lastValue = firstValue;
            }
            if (firstValue.next != null)
            {
                firstValue.next.previous = firstValue;
            }
            
            if (_elements > Size && Size > 0)
            {
                if (_throwWhenFull)
                {
                    throw new Exception("The linked list haas reached the limit of " + Size + " items.");
                }
                _elements = Size;
                LinkedValue<T> value = lastValue;
                lastValue = value.previous;
                lastValue.next = null;
                value.previous = null;
            }
        }

        public void Enqueue(T item) {
            Push(item);
        }

        public T[] GetAsArray()
        {
            T[] array = new T[_elements];

            int i = 0;
            LinkedValue<T> val = firstValue;
            while (val != null)
            {
                array[i++] = val.current;
                val = val.next;
            }
            return array;
        }

        public T[] GetAsReverseArray()
        {
            T[] array = new T[_elements];

            int i=0;
            LinkedValue<T> val = lastValue;
            while (val != null)
            {
                array[i++] = val.current;
                val = val.previous;
            }
            return array;
        }

        public void Clear()
        {
            firstValue = null;
            lastValue = null;
            _elements = 0;
        }

        
        public T Dequeue()
        {
            if(lastValue==null)
            {
                throw new Exception("The linked list is empty.");
            }
            _elements--;

            LinkedValue<T> value = lastValue;
            if (value.previous != null)
            {
                lastValue = value.previous;
                lastValue.next = null;
            }
            else
            {
                firstValue = null;
                lastValue = null;
            }

            return value.current;
        }


        public T Pop()
        {
            if(firstValue==null)
            {
                throw new Exception("Whooot?");
            }
            _elements--;

            LinkedValue<T> value = firstValue;
            if (value.next != null)
            {
                firstValue = value.next;
                firstValue.previous = null;
            }
            else
            {
                firstValue = null;
                lastValue = null;
            }

            return value.current;
        }
    }
}
