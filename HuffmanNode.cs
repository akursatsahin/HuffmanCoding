using System;

namespace HuffmanCoding
{
    public class HuffmanNode
    {
        public HuffmanNode parentNode;
        private HuffmanNode leftChild;
        private HuffmanNode rightChild;
        private bool isLeaf;
        private int freq;
        private char ch;
        private int depth;
        public HuffmanNode(int freq, char ch)
        {
            this.parentNode = this.leftChild = this.rightChild = null;
            this.isLeaf = true;
            this.freq = freq;
            this.ch = ch;
        }
        public HuffmanNode(int freq, char ch, int depth) : this(freq, ch)
        {
            this.depth = depth;

        }
        public HuffmanNode(HuffmanNode leftChild, HuffmanNode rightChild)
        {
            this.parentNode = null;
            this.leftChild = leftChild;
            this.leftChild.parentNode = this;
            this.rightChild = rightChild;
            this.rightChild.parentNode = this;
            this.isLeaf = false;
            this.freq = this.leftChild.Frequency + this.rightChild.Frequency;

        }
        public HuffmanNode(HuffmanNode leftChild, HuffmanNode rightChild, int depth)
        {
            if (leftChild.parentNode != null)
            {
                this.parentNode = leftChild.parentNode;
                this.parentNode.leftChild = this;
            }
            else
                this.parentNode = null;
            this.leftChild = leftChild;
            this.leftChild.parentNode = this;
            this.rightChild = rightChild;
            this.rightChild.parentNode = this;
            this.isLeaf = false;
            this.freq = this.leftChild.Frequency + this.rightChild.Frequency;
            this.depth = depth;
            this.leftChild.depth = this.depth - 2;
            this.rightChild.depth = this.depth - 1;
        }
        public HuffmanNode()
        {
            this.depth = 256;
            this.leftChild = null;
            this.rightChild = null;
            this.isLeaf = false;
            this.freq = 0;
        }

        public int Depth
        {
            get
            {
                return this.depth;
            }
            set
            {
                this.depth = value;
            }
        }
        public int Frequency
        {
            get
            {
                return this.freq;
            }
            set
            {
                this.freq = value;
            }
        }
        public char Char
        {
            get
            {
                return this.ch;
            }
        }
        public Boolean IsLeaf
        {
            get
            {
                return this.isLeaf;
            }
        }
        public HuffmanNode LeftChild
        {
            get
            {
                return this.leftChild;
            }
        }
        public HuffmanNode RightChild
        {
            get
            {
                return this.rightChild;
            }
        }
        public HuffmanNode Parent
        {
            get
            {
                return this.parentNode;
            }
        }
        private void unAssignParent()
        {
            this.parentNode = null;
        }

        public static void swap(HuffmanNode node1, HuffmanNode node2)
        {
            //Console.WriteLine("TriedToSwap");
            HuffmanNode leftParent = node1.parentNode;
            HuffmanNode rightParent = node2.parentNode;
            if (leftParent == node2 || rightParent == node1)
            {
                return;
            }
            if (leftParent == rightParent)
            {
                if (leftParent.leftChild == node1)
                {
                    leftParent.leftChild = node2;
                    leftParent.rightChild = node1;
                }
                else
                {
                    leftParent.leftChild = node1;
                    leftParent.rightChild = node2;
                }
                return;
            }

            int temp = node1.depth;
            node1.depth = node2.depth;
            node2.depth = temp;
            if (leftParent == null)
            {
                node2.unAssignParent();
            }
            else if (leftParent.leftChild == node1)
                leftParent.leftChild = node2;
            else if (leftParent.rightChild == node1)
                leftParent.rightChild = node2;

            if (leftParent != null)
                node2.parentNode = leftParent;

            if (rightParent == null)
            {
                node1.unAssignParent();
            }
            else if (rightParent.leftChild == node2)
                rightParent.leftChild = node1;
            else if (rightParent.rightChild == node2)
                rightParent.rightChild = node1;

            node1.parentNode = rightParent;

        }
        public string getBit()
        {
            return parentNode == null ? "" : (parentNode.leftChild == this ? parentNode.getBit() + "0" : parentNode.getBit() + "1");
        }
        public override string ToString()
        {
            return isLeaf ? ch.ToString() : freq.ToString();
        }
    }
    public class HuffmanListSorter : System.Collections.Generic.IComparer<HuffmanNode>
    {
        public int Compare(HuffmanNode x, HuffmanNode y)
        {
            if (x.Frequency > y.Frequency)
                return 1;
            else if (x.Frequency < y.Frequency)
                return -1;
            else
                return 0;
        }
    }
}