using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HuffmanCompression
{
    class BinaryTree<T> where T : IComparable
    {

        private BinaryTreeNode<T> root;

        public BinaryTreeNode<T> Root
        {
            get { return root; }
        }

        public BinaryTree(BinaryTreeNode<T> RootFromNode = null)
        {
            if (RootFromNode == null)
                root = new BinaryTreeNode<T>();
            else
                root = RootFromNode;
        }

        public void AddLeft(T element, BinaryTreeNode<T> binaryTreeNode)
        {
            binaryTreeNode.Left = new BinaryTreeNode<T>(element, binaryTreeNode);
        }

        public void AddRight(T element, BinaryTreeNode<T> binaryTreeNode)
        {
            binaryTreeNode.Right = new BinaryTreeNode<T>(element, binaryTreeNode);
        }

        private void RemoveSingleNode(BinaryTreeNode<T> NodeToRemove)
        {
            if (NodeToRemove.Parrent.Left != null && NodeToRemove.Parrent.Left.Equals(NodeToRemove))
            {
                NodeToRemove.Parrent.Left = null;
                NodeToRemove.Dispose();
                NodeToRemove = null;
            }
            else
            if (NodeToRemove.Parrent.Right != null && NodeToRemove.Parrent.Right.Equals(NodeToRemove))
            {
                NodeToRemove.Parrent.Right = null;
                NodeToRemove.Dispose();
                NodeToRemove = null;
            }
        }

        public int Remove(BinaryTreeNode<T> NodeToRemove)
        {
            BinaryTreeNode<T> Parrent;
            BinaryTreeNode<T> StartingPoint = NodeToRemove;
            int NodesRemoved = 0;
            while (!StartingPoint.Leaf)
            {
                while (!NodeToRemove.Leaf)
                {
                    Console.Write("Node isn't a leaf: ");
                    if (NodeToRemove.Left != null)
                    {
                        NodeToRemove = NodeToRemove.Left;
                        Console.WriteLine("Moving Left");
                    }
                    else if (NodeToRemove.Right != null)
                    {
                        NodeToRemove = NodeToRemove.Right;
                        Console.WriteLine("Moving Right");
                    }
                }
                Parrent = NodeToRemove.Parrent;
                Console.Write("Found Leaf: {0} ", NodeToRemove);
                RemoveSingleNode(NodeToRemove);
                Console.WriteLine("<- was deleted");
                NodeToRemove = Parrent;
                NodesRemoved++;
            }
            Console.WriteLine($"Nodes Removed: {NodesRemoved}");
            return NodesRemoved;
        }

        // More elegant solution, but you'll get a StackOverFlow Exception if it needs to delete more than 9000 items.
        /*public void Remove(BinaryTreeNode<T> NodeToRemove) 
        {
            if (!NodeToRemove.Leaf)
            {
                if (NodeToRemove.Left != null)
                {
                    Remove(NodeToRemove.Left);
                }
                if (NodeToRemove.Right != null)
                {
                    Remove(NodeToRemove.Right);
                }
            }
            RemoveSingleNode(NodeToRemove);
        }
        */

        public void Combine(BinaryTreeNode<T> LeftNode, BinaryTreeNode<T> RightNode, BinaryTreeNode<T> NodeForRoot = null)
        {
            if (NodeForRoot == null)
                NodeForRoot = new BinaryTreeNode<T>();
            NodeForRoot.Left = LeftNode;
            NodeForRoot.Right = RightNode;
            LeftNode.Parrent = NodeForRoot;
            RightNode.Parrent = NodeForRoot;
            root = NodeForRoot;
        }

        public static void Combine(BinaryTree<T> LeftNode, BinaryTree<T> RightNode, BinaryTree<T> NodeForRoot)
        {
            NodeForRoot.Root.Left = LeftNode.Root;
            NodeForRoot.Root.Right = RightNode.Root;
            LeftNode.Root.Parrent = NodeForRoot.Root;
            RightNode.Root.Parrent = NodeForRoot.Root;
        }

        public BinaryTreeNode<T> Goto(BitArray path)
        {
            BinaryTreeNode<T> ToFind = Root;
            foreach (bool bit in path)
            {
                if (bit)
                    ToFind = ToFind.Left;

                else
                    ToFind = ToFind.Right;

            }
            return ToFind;
        }

        public void inOrder(BinaryTreeNode<T> p)
        {
            if (p != null)
            {
                inOrder(p.Left);
                Console.Write(p.Value.ToString());
                Console.Write(p.BitPath);
                Console.WriteLine();
                inOrder(p.Right);
            }
        }

        public void preOrder(BinaryTreeNode<T> p)
        {
            if (p != null)
            {
                if (p.Value != null)
                    Console.WriteLine(p.Value.ToString());
                /*
                Console.Write(" ");
                if (p.Leaf)
                {
                    Console.Write(p.BitPathToSting);
                }
                Console.WriteLine();
                */
                preOrder(p.Left);
                preOrder(p.Right);
            }
        }

        public static BinaryTree<Encodings> encodingsToBinaryTree(Encodings[] encodings)
        {
            BinaryTree<Encodings> bt = new BinaryTree<Encodings>();
            BinaryTreeNode<Encodings> currentNode = bt.Root;
            foreach (Encodings encoding in encodings)
            {
                if (encoding != null)
                {
                    foreach (char bit in encoding.Bits)
                    {
                        if (bit == '1')
                        {
                            if (currentNode.Left == null)
                            {
                                currentNode.Left = new BinaryTreeNode<Encodings>();
                            }
                            currentNode = currentNode.Left;
                        }
                        if (bit == '0')
                        {
                            if (currentNode.Right == null)
                            {
                                currentNode.Right = new BinaryTreeNode<Encodings>();
                            }
                            currentNode = currentNode.Right;
                        }
                    }
                    currentNode.Value = encoding;
                }
                currentNode = bt.Root;
            }
            return bt;
        }
    }

    class BinaryTreeNode<T> : IComparable where T : IComparable
    {
        public T Value;
        private BinaryTreeNode<T> parrent;
        private BinaryTreeNode<T> left;
        private BinaryTreeNode<T> right;

        public BinaryTreeNode(T element = default(T), BinaryTreeNode<T> Nodeparrent = null)
        {
            Value = element;
            parrent = Nodeparrent;
            left = null;
            right = null;
        }

        public BinaryTreeNode<T> Left
        {
            get { return left; }
            set
            {
                if (value != null)
                {
                    left = value;
                    left.parrent = this;
                }
                else { left = null; }
            }
        }

        public BinaryTreeNode<T> Right
        {
            get { return right; }
            set
            {
                if (value != null)
                {
                    right = value;
                    right.parrent = this;
                }
                else { right = null; }
            }
        }

        public BinaryTreeNode<T> Parrent
        {
            get { return parrent; }
            set { parrent = value; }
        }

        public bool Leaf => left == null && right == null;

        public BitArray BitPath
        {
            get
            {
                BinaryTreeNode<T> PathFinder = parrent;
                BinaryTreeNode<T> Child = this;
                LinkedList<bool> Bits = new LinkedList<bool>();
                LinkedListNode<bool> CurrentNode = Bits.Last;
                bool Bit;

                while (PathFinder != null) // O(n) where n is the same as this.Depth
                {
                    if (PathFinder.Left != null && PathFinder.Left.Equals(Child))
                        Bit = true;

                    else
                    if (PathFinder.Right != null && PathFinder.Right.Equals(Child))
                        Bit = false;

                    else throw/*stuff*/ new Exception($"{Child} is an orphan to {PathFinder}");

                    try
                    { Bits.AddBefore(CurrentNode, Bit); }

                    catch (ArgumentNullException)
                    { Bits.AddLast(Bit); }

                    try
                    { CurrentNode = CurrentNode.Previous; }

                    catch (NullReferenceException)
                    { CurrentNode = Bits.Last; }

                    PathFinder = PathFinder.Parrent;
                    Child = Child.Parrent;
                }

                BitArray bitArray = new BitArray(Bits.Count);
                CurrentNode = Bits.First;
                int i = 0;
                while (CurrentNode != null)
                {
                    bitArray[i] = CurrentNode.Value;
                    i++;
                    CurrentNode = CurrentNode.Next;
                }
                return bitArray;
            }
        }

        public string BitPathToSting
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (bool bit in BitPath)
                {
                    if (bit)
                        sb.Append("1");
                    else
                        sb.Append("0");
                }
                return sb.ToString();
            }
        }

        public int Depth
        {
            get
            {
                BinaryTreeNode<T> DepthFinder = this;
                int depth = 0;
                while (DepthFinder.Parrent != null) // O(n) where n is how many parrents are above this node.
                {
                    depth++;
                    DepthFinder = DepthFinder.Parrent;
                }
                return depth;
            }
        }

        public override string ToString()
        {
            if (Value == null)
                throw new ArgumentNullException("Value", "The value of [current node] is null");
            return Value.ToString();
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !GetType().Equals(obj.GetType()))
                return false;

            else
            {
                BinaryTreeNode<T> bt = (BinaryTreeNode<T>)obj;
                return Value.GetHashCode() == bt.Value.GetHashCode();
            }
        }

        public override int GetHashCode()
        {
            if (Left != null)
            {
                if (Right != null)
                {
                    if (Parrent != null)
                        return Value.GetHashCode() ^ Left.Value.GetHashCode() ^ Right.Value.GetHashCode() ^ Parrent.Value.GetHashCode();
                    else
                        return Value.GetHashCode() ^ Left.Value.GetHashCode() ^ Right.Value.GetHashCode();
                }
                else
                    return Value.GetHashCode() ^ Left.Value.GetHashCode();
            }
            else
                return Value.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            int result = 0;
            if (obj == null)
                result = int.MaxValue;

            if (!(obj is BinaryTreeNode<T>))
                throw new ArgumentException($"{obj} is not a Character Frequency object");

            BinaryTreeNode<T> bt = obj as BinaryTreeNode<T>;

            if (Value.CompareTo(bt.Value) < 0)
                result = int.MinValue;

            else if (Value.Equals(bt.Value))
                result = 0;

            else
                result = int.MaxValue;

            return result;
        }

        public void Dispose()
        {
            Value = default(T);
            parrent = null;
            left = null;
            right = null;
        }

        public static BinaryTreeNode<CharacterFrequency> BuildEncodingTree(CharacterFrequency[] charFreq)
        {
            SortedLinkedList<BinaryTreeNode<CharacterFrequency>> TreeBuilder = new SortedLinkedList<BinaryTreeNode<CharacterFrequency>>();

            foreach (CharacterFrequency CharFreqObj in charFreq)
            {
                if (CharFreqObj.Frequency > 0)
                {
                    BinaryTreeNode<CharacterFrequency> Node = new BinaryTreeNode<CharacterFrequency>(CharFreqObj);
                    TreeBuilder.Add(Node);
                }
            }

            while (TreeBuilder.Count > 1)
            {
                //Console.WriteLine(TreeBuilder.Count);
                BinaryTreeNode<CharacterFrequency> CombinedNode = new BinaryTreeNode<CharacterFrequency>();
                CombinedNode.Value = new CharacterFrequency();

                CombinedNode.Left = TreeBuilder.First.Value;
                CombinedNode.Right = TreeBuilder.First.Next.Value;

                CombinedNode.Left.Parrent = CombinedNode;
                CombinedNode.Right.Parrent = CombinedNode;

                CombinedNode.Value.Frequency = CombinedNode.Left.Value.Frequency + CombinedNode.Right.Value.Frequency;

                TreeBuilder.Remove(TreeBuilder.First); // First entry.
                TreeBuilder.Remove(TreeBuilder.First); // Second entry.

                TreeBuilder.Add(CombinedNode);
            }
            return TreeBuilder.First.Value;
        }

    }
}