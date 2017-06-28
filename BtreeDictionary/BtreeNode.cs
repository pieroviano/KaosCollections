﻿//
// Library: KaosCollections
// File:    BtreeNode.cs
// Purpose: Define nonpublic tree structure and its basic operations.
//
// Copyright © 2009-2017 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Kaos.Collections
{
    public partial class BtreeDictionary<TKey,TValue>
    {
        /// <summary>A page of the B+ tree. May be internal (Branch) or terminal (Leaf).</summary>
        private class Node
        {
            protected readonly List<TKey> keys;

            protected Node (int keyCapacity)
            { this.keys = new List<TKey> (keyCapacity); }

            public int KeyCount { get { return keys.Count; } }

            public void AddKey (TKey key) { keys.Add (key); }
            public TKey GetKey (int index) { return keys[index]; }
            public int Search (TKey key) { return keys.BinarySearch (key); }
            public int Search (TKey key, IComparer<TKey> comp) { return keys.BinarySearch (key, comp); }
            public void SetKey (int index, TKey key) { keys[index] = key; }
            public void RemoveKey (int index) { keys.RemoveAt (index); }
            public void RemoveKeys (int index, int count) { keys.RemoveRange (index, count); }
            public void TruncateKeys (int index) { keys.RemoveRange (index, keys.Count - index); }
            public void InsertKey (int index, TKey key) { keys.Insert (index, key); }

#if DEBUG
            public StringBuilder Append (StringBuilder sb)
            {
                for (int ix = 0; ix < this.KeyCount; ix++)
                {
                    if (ix > 0)
                        sb.Append (',');
                    sb.Append (GetKey (ix));
                }
                return sb;
            }
#endif
        }


        /// <summary>An internal B+ tree page.</summary>
        /// <remarks>
        /// Contains copies of the first key of every leaf (except the leftmost).
        /// </remarks>
        private class Branch : Node
        {
            private readonly List<Node> childNodes;

            // Called on initialization only.
            public Branch (Leaf leaf, int keyCapacity=0) : base (keyCapacity)
            {
                this.childNodes = new List<Node> (keyCapacity) { leaf };
            }

            public Branch (Branch leftBranch, int keyCapacity) : base (keyCapacity)
            {
                this.childNodes = new List<Node> (keyCapacity + 1);
            }

            public Branch (Node child, int keyCapacity) : base (keyCapacity)
            {
                this.childNodes = new List<Node> (keyCapacity + 1) { child };
            }


            public int ChildCount
            { get { return childNodes.Count; } }

            public Node GetChild (int childIndex)
            { return childNodes[childIndex]; }

            public Node FirstChild
            { get { return childNodes[0]; } }

            public void RemoveChild (int index)
            { childNodes.RemoveAt (index); }

            public void Truncate (int index)
            {
                TruncateKeys (index);
                childNodes.RemoveRange (index + 1, childNodes.Count - (index + 1));
            }

            public void Add (Node node)
            { childNodes.Add (node); }

            public void Add (TKey key, Node node)
            {
                AddKey (key);
                childNodes.Add (node);
            }

            public void Insert (int index, Node node)
            {
                childNodes.Insert (index, node);
            }

            public void Remove (int index, int count)
            {
                RemoveKeys (index, count);
                childNodes.RemoveRange (index, count);
            }
        }


        /// <summary>An terminal B+ tree page.</summary>
        /// <remarks>
        /// All key/value pairs are contained in this class.
        /// Leaf is a sequential linked list also referenced by parent branches.
        /// </remarks>
        private class Leaf : Node
        {
            private readonly List<TValue> values;  // Payload.
            private Leaf rightLeaf;  // For the linked leaf list.

            public Leaf (int capacity=0) : base (capacity)
            {
                this.values = new List<TValue> (capacity);
                this.rightLeaf = null;
            }

            /// <summary>Splice a leaf to right of <paramref name="leftLeaf"/>.</summary>
            /// <param name="leftLeaf">Provides linked list insert point.</param>
            /// <param name="capacity">The initial number of elements the page can store.</param>
            public Leaf (Leaf leftLeaf, int capacity) : base (capacity)
            {
                this.values = new List<TValue> (capacity);

                // Linked list insertion.
                this.rightLeaf = leftLeaf.rightLeaf;
                leftLeaf.rightLeaf = this;
            }


            /// <summary>Next leaf in linked list.</summary>
            public Leaf RightLeaf
            {
                get { return rightLeaf; }
                set { rightLeaf = value; }
            }


            public int ValueCount
            { get { return values.Count; } }


            public KeyValuePair<TKey, TValue> GetPair (int pairIndex)
            { return new KeyValuePair<TKey, TValue> (keys[pairIndex], values[pairIndex]); }


            public TValue GetValue (int valueIndex)
            { return values[valueIndex]; }


            public void SetValue (int valueIndex, TValue value)
            { values[valueIndex] = value; }


            public void Add (TKey key, TValue value)
            {
                AddKey (key);
                values.Add (value);
            }

            public void Add (Leaf source, int sourceStart, int sourceStop)
            {
                for (int ix = sourceStart; ix < sourceStop; ++ix)
                    Add (source.GetKey (ix), source.GetValue (ix));
            }

            public void Insert (int index, TKey key, TValue value)
            {
                Debug.Assert (index >= 0 && index <= ValueCount);
                InsertKey (index, key);
                values.Insert (index, value);
            }

            public void Remove (int index)
            {
                Debug.Assert (index >= 0 && index <= ValueCount);
                RemoveKey (index);
                values.RemoveAt (index);
            }

            public void Remove (int index, int count)
            {
                Debug.Assert (index >= 0 && index + count <= ValueCount);
                RemoveKeys (index, count);
                values.RemoveRange (index, count);
            }

            public void Truncate (int index)
            {
                Debug.Assert (index >= 0 && index < ValueCount);
                RemoveKeys (index, KeyCount - index);
                values.RemoveRange (index, ValueCount - index);
            }
        }
    }
}
