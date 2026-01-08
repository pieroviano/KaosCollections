//
// Library: KaosCollections
// File:    Btree.Leaf.cs
//
// Copyright © 2009-2021 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely

#nullable enable

#if COLLECTIONS
namespace System.Collections.Generic;
#else
namespace Kaos.Collections;
#endif

#if PUBLIC
    public
#else
internal
#endif
    abstract partial class Btree<T>
{
    /// <exclude />
    private protected class Leaf : Node
    {
        public Leaf? leftLeaf;

        public Leaf? rightLeaf;

        /// <summary>Create a siblingless leaf.</summary>
        /// <param name="capacity">The initial number of elements the page can store.</param>
        public Leaf(int capacity = 0) : base(capacity)
        {
            leftLeaf = rightLeaf = null;
        }

        /// <summary>Splice new leaf to right of leftLeaf".</summary>
        /// <param name="leftLeaf">Provides linked list insert point.</param>
        /// <param name="capacity">The initial number of elements the page can store.</param>
        /// <remarks>Caller must fixup rightLeaf field.</remarks>
        public Leaf(Leaf leftLeaf, int capacity) : base(capacity)
        {
            // Doubly linked list insertion.
            rightLeaf = leftLeaf.rightLeaf;
            leftLeaf.rightLeaf = this;
            this.leftLeaf = leftLeaf;
        }

        /// <summary>Number of key/value pairs in the subtree.</summary>
        public override int Weight
            => keys.Count;

        public void Add(Leaf source, int sourceStart, int sourceStop)
        {
            for (var ix = sourceStart; ix < sourceStop; ++ix)
            {
                keys.Add(source.GetKey(ix));
            }
        }

        public virtual void Coalesce()
        {
            for (var ix = 0; ix < (rightLeaf?.KeyCount ?? 0); ++ix)
            {
                if (rightLeaf != null)
                {
                    keys.Add(rightLeaf.keys[ix]);
                }
            }
        }

        public void CopyLeafLeft(int index, int offset)
        {
            keys[index - offset] = keys[index];
        }

        public virtual void MoveLeft(int count)
        {
            for (var ix = 0; ix < count; ++ix)
            {
                if (rightLeaf != null)
                {
                    keys.Add(rightLeaf.keys[ix]);
                }
            }

            rightLeaf?.keys.RemoveRange(0, count);
        }

        public virtual void RemoveRange(int index, int count)
        {
            keys.RemoveRange(index, count);
        }

        public virtual void Truncate(int index)
        {
            keys.RemoveRange(index, keys.Count - index);
        }
    }
}