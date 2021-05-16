using System;
using System.Collections.Generic;

namespace HuffmanCompression
{
    class SortedLinkedList<T> where T : IComparable  // : LinkedList<T> // : struct, : Class puts a constraint on the type that can be used
    {
        private LinkedList<T> llist;

        public SortedLinkedList() // Initializes a linked list when a new SortedLinkedList is created.
        { llist = new LinkedList<T>(); }

        public int Count
        { get { return llist.Count; } }

        public LinkedListNode<T> First
        { get { return llist.First; } }

        public bool Remove(T Value)
        { return llist.Remove(Value); }

        public void Remove(LinkedListNode<T> node)
        { llist.Remove(node); }

        public LinkedList<T>.Enumerator GetEnumerator() // Allows us to use a foreach on our sorted LinkedList.
        { return llist.GetEnumerator(); }

        public override bool Equals(object obj)
        { return llist.Equals(obj); }

        public override int GetHashCode()
        { return llist.GetHashCode(); }

        public void Add(T toAdd) //Sorted smallest to largest, A - Z// O(n)
        {
            bool previousWasSmaller = false;
            bool previousWasTheSame = false;
            LinkedListNode<T> node = llist.First;

            if (toAdd == null)
                throw new ArgumentNullException("toAdd", "toAdd Cannot be null.");

            if (llist.Count == 0)
            {
                llist.AddFirst(toAdd);
            }
            // check if passedIn should be at the end of the list before going through the whole thing.
            else if (llist.Last.Value.CompareTo(toAdd) < 0 || llist.Last.Value.CompareTo(toAdd) == 0) // Speeeeed
            {
                llist.AddLast(toAdd);
            }
            else
            {
                //if (node.Value.CompareTo(toAdd) > 0 && node.Equals(llist.First))
                if (node.Value.CompareTo(toAdd) > 0 && node.Value.CompareTo(llist.First.Value) == 0)
                {
                    llist.AddFirst(toAdd);
                    return;
                }
                while (node != null) // O(n)
                {
                    if (node.Value.CompareTo(toAdd) > 0 && (previousWasSmaller || previousWasTheSame)) // item > passedIn
                    {
                        llist.AddBefore(node, toAdd);
                        return;
                    }
                    else if (node.Value.CompareTo(toAdd) == 0) // item = passedIn
                    {
                        previousWasTheSame = true;
                    }
                    else if (node.Value.CompareTo(toAdd) < 0) // item < passedIn
                    {
                        previousWasSmaller = true;
                    }
                    else
                    {
                        throw new Exception($"The CompareTo method must return an int.");
                    }
                    node = node.Next;
                }
            }
        }

        public void AddReversed(T toAdd) // Sorted Largest to Smallest, 10 to -10, Z - A // O(n)
        {
            bool NextWasSmaller = false;
            bool NextWasTheSame = false;
            LinkedListNode<T> node = llist.Last;

            if (toAdd == null)
                throw new ArgumentNullException("toAdd", "toAdd Cannot be null.");

            if (llist.Count == 0)
            {
                llist.AddLast(toAdd);
            }
            // check if passedIn should be at the start of the list before going through the whole thing.
            else if (llist.First.Value.CompareTo(toAdd) < 0 || llist.First.Value.CompareTo(toAdd) == 0) // Because Speeeeed
            {
                llist.AddFirst(toAdd);
            }
            else
            {
                if (node.Value.CompareTo(toAdd) > 0 && node.Equals(llist.Last))
                {
                    llist.AddLast(toAdd);
                    return;
                }
                while (node != null) // O(n)
                {
                    if (node.Value.CompareTo(toAdd) > 0 && (NextWasSmaller || NextWasTheSame)) // Current > passedIn
                    {
                        llist.AddAfter(node, toAdd);
                        return;
                    }
                    else if (node.Value.CompareTo(toAdd) == 0) // Current = passedIn
                    {
                        NextWasTheSame = true;
                    }
                    else if (node.Value.CompareTo(toAdd) < 0) // Current < passedIn
                    {
                        NextWasSmaller = true;
                    }
                    else
                    {
                        throw new Exception($"The CompareTo method should return an int.");
                    }
                    node = node.Previous;
                }
            }
        } // public void AddLTS(T passedIn)

    } // class SortedLinkedList<T> where T : IComparable
}