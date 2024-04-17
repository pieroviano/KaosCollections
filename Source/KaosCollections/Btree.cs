//
// Library: KaosCollections
// File:    Btree.cs
// Purpose: Define base functionality for Ranked classes.
//
// Copyright © 2009-2021 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

[assembly: CLSCompliant (true)]
namespace Kaos.Collections
{
    /// <summary>Provides base functionality for other classes in this library.</summary>
    /// <typeparam name="T">The type of the keys in the derived class.</typeparam>
    /// <remarks>This class cannot be directly instantiated.</remarks>
    public abstract partial class Btree<T>
    {
        private const int MinimumOrder = 4;
        private const int DefaultOrder = 128;
        private const int MaximumOrder = 256;

        private protected Node root;
        private protected Leaf rightmostLeaf;
        private protected Leaf leftmostLeaf;
        private protected IComparer<T> keyComparer;
        private protected int maxKeyCount;
        private protected int stage = 0;

        private protected Btree (Leaf startLeaf)
        {
            this.maxKeyCount = DefaultOrder - 1;
            this.root = this.rightmostLeaf = this.leftmostLeaf = startLeaf;
        }

        private protected Btree (IComparer<T> comparer, Leaf startLeaf) : this (startLeaf)
         => this.keyComparer = comparer ?? Comparer<T>.Default;

        #region Nonpublic common methods

        private protected bool IsUnderflow (int count)
         => count < ((maxKeyCount + 1) >> 1);

        private protected void CopyKeysTo1 (T[] array, int index, int count)
        {
            if (array == null)
                throw new ArgumentNullException (nameof (array));

            if (index < 0)
                throw new ArgumentOutOfRangeException (nameof (index), index, "Argument was out of the range of valid values.");

            if (count < 0)
                throw new ArgumentOutOfRangeException (nameof (count), count, "Argument was out of the range of valid values.");

            if (count > array.Length - index)
                throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.");

            if (count > root.Weight)
                count = root.Weight;
            int stopIx = index + count;

            for (Leaf leaf = leftmostLeaf; ; leaf = leaf.rightLeaf)
            {
                if (leaf.KeyCount >= stopIx - index)
                {
                    leaf.CopyKeysTo (array, index, stopIx - index);
                    return;
                }

                leaf.CopyKeysTo (array, index, leaf.KeyCount);
                index += leaf.KeyCount;
            }
        }

        private protected void CopyKeysTo2 (Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException (nameof (array));

            if (array.Rank != 1)
                throw new ArgumentException ("Multidimension array is not supported on this operation.", nameof (array));

            if (array.GetLowerBound (0) != 0)
                throw new ArgumentException ("Target array has non-zero lower bound.", nameof (array));

            if (array is T[] genericArray)
            {
                CopyKeysTo1 (genericArray, index, root.Weight);
                return;
            }

            if (index < 0)
                throw new ArgumentOutOfRangeException (nameof (index), "Non-negative number required.");

            if (root.Weight > array.Length - index)
                throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.");

            if (array is object[] obArray)
                try
                {
                    for (Leaf leaf = leftmostLeaf; leaf != null; leaf = leaf.rightLeaf)
                        for (int ix = 0; ix < leaf.KeyCount; ++ix)
                            obArray[index++] = leaf.GetKey (ix);
                }
                catch (ArrayTypeMismatchException)
                { throw new ArgumentException ("Mismatched array type.", nameof (array)); }
            else
                throw new ArgumentException ("Invalid array type.", nameof (array));
        }

        /// <summary>Perform lite search for key.</summary>
        /// <param name="key">Target of search.</param>
        /// <param name="index">When found, holds index of returned Leaf; else ~index of nearest greater key.</param>
        /// <returns>Leaf holding target (found or not).</returns>
        private protected Leaf Find (T key, out int index)
        {
            //  Unfold on default comparer for 5% speed improvement.
            if (keyComparer == Comparer<T>.Default)
                for (Node node = root;;)
                {
                    index = node.Search (key);

                    if (node is Branch branch)
                        node = branch.GetChild (index < 0 ? ~index : index + 1);
                    else
                        return (Leaf) node;
                }
            else
                for (Node node = root;;)
                {
                    index = node.Search (key, keyComparer);

                    if (node is Branch branch)
                        node = branch.GetChild (index < 0 ? ~index : index + 1);
                    else
                        return (Leaf) node;
                }
        }

        /// <summary>Perform traverse to leaf at index.</summary>
        /// <param name="treeIndex">Index of collection.</param>
        /// <param name="leafIndex">Leaf index of result.</param>
        /// <returns>Leaf holding item.</returns>
        private protected Node Find (int treeIndex, out int leafIndex)
        {
            System.Diagnostics.Debug.Assert (treeIndex < root.Weight);
            Node node = root;
            leafIndex = treeIndex;
            while (node is Branch branch)
                for (int ix = 0; ; ++ix)
                {
                    Node child = branch.GetChild (ix);
                    int cw = child.Weight;
                    if (cw > leafIndex)
                    {
                        node = child;
                        break;
                    }
                    leafIndex -= cw;
                }

            return node;
        }

        private protected int FindEdgeForIndex (T key, out Leaf leaf, out int leafIndex, bool leftEdge=false)
        {
            bool isFound = false;
            int treeIndex = 0;

            for (Node node = root;;)
            {
                int hi = node.KeyCount;
                if (leftEdge)
                    for (int lo = 0; lo != hi;)
                    {
                        int mid = (lo + hi) >> 1;
                        int diff = keyComparer.Compare (key, node.GetKey (mid));
                        if (diff <= 0)
                        {
                            if (diff == 0)
                                isFound = true;
                            hi = mid;
                        }
                        else
                            lo = mid + 1;
                    }
                else
                    for (int lo = 0; lo != hi;)
                    {
                        int mid = (lo + hi) >> 1;
                        int diff = keyComparer.Compare (key, node.GetKey (mid));
                        if (diff < 0)
                            hi = mid;
                        else
                        {
                            if (diff == 0)
                                isFound = true;
                            lo = mid + 1;
                        }
                    }

                if (node is Branch branch)
                {
                    if (hi <= branch.KeyCount >> 1)
                        for (int ix = 0; ix < hi; ++ix)
                            treeIndex += branch.GetChild (ix).Weight;
                    else
                    {
                        treeIndex += branch.Weight;
                        for (int ix = hi; ix <= branch.KeyCount; ++ix)
                            treeIndex -= branch.GetChild (ix).Weight;
                    }
                    node = branch.GetChild (hi);
                }
                else
                {
                    leafIndex = hi;
                    leaf = (Leaf) node;
                    return isFound ? treeIndex + hi : ~(treeIndex + hi);
                }
            }
        }

        private protected bool FindEdgeLeft (T key, out Leaf leaf, out int leafIndex)
        {
            bool isFound = false;

            for (Node node = root;;)
            {
                int hi = node.KeyCount;
                for (int lo = 0; lo != hi;)
                {
                    int mid = (lo + hi) >> 1;
                    int diff = keyComparer.Compare (key, node.GetKey (mid));
                    if (diff <= 0)
                    {
                        if (diff == 0)
                            isFound = true;
                        hi = mid;
                    }
                    else
                        lo = mid + 1;
                }

                if (node is Branch branch)
                    node = branch.GetChild (hi);
                else
                {
                    leafIndex = hi;
                    leaf = (Leaf) node;
                    return isFound;
                }
            }
        }

        private protected bool FindEdgeRight (T key, out Leaf leaf, out int leafIndex)
        {
            bool isFound = false;

            for (Node node = root;;)
            {
                int hi = node.KeyCount;
                for (int lo = 0; lo != hi; )
                {
                    int mid = (lo + hi) >> 1;
                    int diff = keyComparer.Compare (key, node.GetKey (mid));
                    if (diff < 0)
                        hi = mid;
                    else
                    {
                        if (diff == 0)
                            isFound = true;
                        lo = mid + 1;
                    }
                }

                if (node is Branch branch)
                    node = branch.GetChild (hi);
                else
                {
                    leafIndex = hi;
                    leaf = (Leaf) node;
                    return isFound;
                }
            }
        }

        private protected int GetCount2 (T key)
        {
            int treeIx1 = FindEdgeForIndex (key, out Leaf _, out int _, leftEdge:true);
            if (treeIx1 < 0)
                return 0;
            else
                return FindEdgeForIndex (key, out Leaf _, out int _, leftEdge:false) - treeIx1;
        }

        private protected int GetDistinctCount2()
        {
            int result = 0;
            if (root.Weight > 0)
            {
                Leaf leaf = leftmostLeaf;
                int leafIndex = 0;

                for (T currentKey = leaf.Key0;;)
                {
                    ++result;
                    if (leafIndex < leaf.KeyCount - 1)
                    {
                        ++leafIndex;
                        T nextKey = leaf.GetKey (leafIndex);
                        if (keyComparer.Compare (currentKey, nextKey) != 0)
                        { currentKey = nextKey; continue; }
                    }

                    FindEdgeRight (currentKey, out leaf, out leafIndex);
                    if (leafIndex >= leaf.KeyCount)
                    {
                        leaf = leaf.rightLeaf;
                        if (leaf == null)
                            break;
                        leafIndex = 0;
                    }
                    currentKey = leaf.GetKey (leafIndex);
                }
            }
            return result;
        }

        [NonSerialized]
        private object syncRoot = null;

        private protected object GetSyncRoot()
        {
            if (syncRoot == null)
                System.Threading.Interlocked.CompareExchange (ref syncRoot, new object(), null);
            return syncRoot;
        }

        private protected void Initialize()
        {
            StageBump();
            leftmostLeaf.Truncate (0);
            leftmostLeaf.rightLeaf = null;
            root = rightmostLeaf = leftmostLeaf;
        }

        private protected void Remove2 (NodeVector path)
        {
            StageBump();
            ((Leaf) path.TopNode).RemoveRange (path.TopIndex, 1);
            path.DecrementPathWeight();
            path.Balance();
        }

        private protected int Remove2 (T item, int count)
        {
            if (count <= 0)
                return 0;

            var path1 = new NodeVector (this, item, leftEdge:true);
            if (! path1.IsFound)
                return 0;

            // Seek by offset is faster, so try that first.
            var path2 = NodeVector.CreateFromOffset (path1, count);
            if (path2 == null || keyComparer.Compare (path2.LeftKey, item) != 0)
            {
                path2 = new NodeVector (this, item, leftEdge:false);
                count = path2.GetTreeIndex() - path1.GetTreeIndex();
            }

            StageBump();
            Delete (path1, path2);
            return count;
        }

        private protected void RemoveAt2 (int index)
         => Remove2 (NodeVector.CreateFromIndex (this, index));

        private protected void RemoveRange2 (int index, int count)
        {
            if (count == 0)
                return;

            var path1 = NodeVector.CreateFromIndex (this, index);
            var path2 = NodeVector.CreateFromIndex (this, index + count);

            StageBump();
            Delete (path1, path2);
        }

        // Bulk delete all elements between path1 & path2 inclusive.
        private protected void Delete (NodeVector path1, NodeVector path2)
        {
            var leaf1 = (Leaf) path1.TopNode; int ix1 = path1.TopIndex, deltaW1 = 0;
            var leaf2 = (Leaf) path2.TopNode; int ix2 = path2.TopIndex, deltaW2 = 0;
            int j1 = ix1 == 0 && leaf1.leftLeaf == null ? 0 : 1;
            int j2 = ix2 == leaf2.KeyCount && leaf2.rightLeaf == null ? 0 : 1;

            if (j1 == 0 && j2 == 0)
            { Initialize(); return; }

            // Left-edge normalize path1, right-edge normalize path2.
            if (j1 != 0 && ix1 == 0)
            { leaf1 = leaf1.leftLeaf; ix1 = leaf1.KeyCount; path1.TraverseLeft(); }
            if (j2 != 0 && ix2 == leaf2.KeyCount)
            { leaf2 = leaf2.rightLeaf; ix2 = 0; path2.TraverseRight(); }

            if (leaf1 == leaf2)
            {
                int count = ix2 - ix1;
                path2.ChangePathWeight (-count);
                leaf2.RemoveRange (ix1, count);
                path2.Balance();
                return;
            }

            if (j2 == 0)
            {
                leaf1.rightLeaf = null;
                rightmostLeaf = leaf1;
                deltaW1 = ix1 - leaf1.KeyCount;
                leaf1.RemoveRange (ix1, leaf1.KeyCount - ix1);
            }
            else
            {
                deltaW2 = -ix2;
                if (j1 != 0)
                    leaf1.rightLeaf = leaf2;
                leaf2.RemoveRange (0, ix2);
                if (j1 == 0)
                { leaf2.leftLeaf = null; leftmostLeaf = leaf2; }
                else
                {
                    if (ix2 != 0)
                        path2.SetPivot (leaf2.Key0);
                    deltaW1 = ix1-leaf1.KeyCount;
                    leaf2.leftLeaf = leaf1;
                    leaf1.RemoveRange (ix1, leaf1.KeyCount-ix1);
                }
            }

            for (int level = path2.Height-2;; --level)
            {
                var bh1 = (Branch) path1.GetNode (level); ix1 = path1.GetIndex (level);
                var bh2 = (Branch) path2.GetNode (level); ix2 = path2.GetIndex (level);
                if (bh1 == bh2)
                {
                    int ix9 = ix2+1-j2;
                    for (int ix = ix1+j1; ix < ix9; ++ix)
                        deltaW1 -= bh2.GetChild(ix).Weight;
                    if (j1 == 0)
                        bh2.RemoveChildRange2 (0, ix2-ix1);
                    else
                        bh2.RemoveChildRange1 (ix1, ix2-ix1-j2);
                    for (;;)
                    {
                        bh2.AdjustWeight (deltaW1 + deltaW2);
                        if (--level < 0)
                            break;
                        bh2 = (Branch) path2.GetNode (level);
                    }
                    break;
                }

                if (j1 != 0)
                {
                    for (int ix = ix1+1; ix <= bh1.KeyCount; ++ix)
                        deltaW1 -= bh1.GetChild(ix).Weight;
                    bh1.RemoveChildRange1 (ix1, bh1.KeyCount - ix1);
                    bh1.AdjustWeight (deltaW1);
                }
                if (j2 != 0)
                {
                    if (ix2 > 0)
                    {
                        for (int ix = 0; ix < ix2; ++ix)
                            deltaW2 -= bh2.GetChild(ix).Weight;
                        path2.SetPivot (bh2.GetKey (ix2-1), level);
                        bh2.RemoveChildRange2 (0, ix2);
                    }
                    bh2.AdjustWeight (deltaW2);
                }
            }

            // Deletions complete. Now fixup underflows, leaf first:

            if (j2 != 0)
            {
                if (j1 == 0)
                { path1 = NodeVector.CreateFromIndex (this, 0); leaf1 = leftmostLeaf; }

                if (leaf1.rightLeaf != null)
                {
                    if (IsUnderflow (leaf1.KeyCount))
                    {
                        path2.Copy (path1, path1.Height);
                        path2.BalanceLeaf();
                        if (j1 != 0 && leaf1.rightLeaf != null && IsUnderflow (leaf1.KeyCount))
                        {
                            path2.Copy (path1, path1.Height);
                            path2.BalanceLeaf();
                        }
                    }
                    if (j1 != 0)
                    {
                        leaf2 = leaf1.rightLeaf;
                        if (leaf2 != null && leaf2.rightLeaf != null && IsUnderflow (leaf2.KeyCount))
                        {
                            path2.Copy (path1, path1.Height);
                            path2.TraverseRight();
                            path2.BalanceLeaf();
                        }
                    }
                }

                for (int level = path1.Height-1; level > 1; --level)
                {
                    var branch1 = (Branch) path1.GetNode (level-1);
                    if (IsUnderflow (branch1.ChildCount))
                    {
                        path2.Copy (path1, level);
                        var branch2 = (Branch) path2.TraverseRight();
                        if (branch2 == null)
                            continue;
                        path2.BalanceBranch (branch1);
                        if (j1 != 0 && IsUnderflow (branch1.ChildCount))
                        {
                            path2.Copy (path1, level);
                            branch2 = (Branch) path2.TraverseRight();
                            if (branch2 == null)
                                continue;
                            path2.BalanceBranch (branch1);
                        }
                    }
                    if (j1 != 0)
                    {
                        path2.Copy (path1, level);
                        branch1 = (Branch) path2.TraverseRight();
                        if (branch1 != null && IsUnderflow (branch1.ChildCount))
                        {
                            var branch2 = (Branch) path2.TraverseRight();
                            if (branch2 != null)
                                path2.BalanceBranch (branch1);
                        }
                    }
                }
            }

            TrimRoot();
        }

        private protected int RemoveWhere2 (Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException (nameof (match));

            int stageFreeze = stage;
            int leafLoss = 0, treeLoss = 0;
            var path = NodeVector.CreateFromIndex (this, 0);
            var leaf = (Leaf) path.TopNode;
            int ix = 0;

            for (;; ++ix)
            {
                if (ix >= leaf.KeyCount)
                {
                    endOfLeaf();
                    if (leaf == null)
                        break;
                }

                bool isMatch = match (leaf.GetKey (ix));
                StageCheck (stageFreeze);
                if (isMatch)
                {
                    ++leafLoss;
                    stageFreeze = StageBump();
                }
                else if (leafLoss != 0)
                {
                    leaf.CopyLeafLeft (ix, leafLoss);
                    if (ix == leafLoss)
                        path.SetPivot (leaf.Key0);
                }
            }

            if (treeLoss != 0)
                TrimRoot();
            return treeLoss;

            void endOfLeaf()
            {
                if (leafLoss == 0)
                { ix = 0; leaf = (Leaf) path.TraverseRight(); }
                else
                {
                    leaf.Truncate (ix-leafLoss);
                    treeLoss += leafLoss; path.ChangePathWeight (-leafLoss); leafLoss = 0;

                    if (leaf.rightLeaf == null)
                    {
                        if (leaf.KeyCount == 0)
                            path.Balance();
                        leaf = null;
                    }
                    else if (! IsUnderflow (leaf.KeyCount))
                    { ix = 0; leaf = (Leaf) path.TraverseRight(); }
                    else
                    {
                        var path2 = new NodeVector (path, path.Height);
                        if (leaf.KeyCount > 0)
                        { ix = leaf.KeyCount; path2.Balance(); }
                        else
                        {
                            ix = 0; leaf = (Leaf) path.TraverseLeft();
                            path2.Balance();
                            if (leaf != null)
                                leaf = (Leaf) path.TraverseRight();
                            else
                            {
                                path = NodeVector.CreateFromIndex (this, 0);
                                leaf = (Leaf) path.TopNode;
                            }
                        }
                    }
                }
            }
        }

        private protected void TrimRoot()
        {
            while (root is Branch branch && root.KeyCount == 0)
                root = branch.Child0;
        }

        private protected void TryGetLT (T key, out Leaf leaf, out int index)
        {
            bool _ = FindEdgeLeft (key, out leaf, out index);
            if (--index < 0)
                leaf = null;
        }

        private protected void TryGetLE (T key, out Leaf leaf, out int index)
        {
            if (FindEdgeLeft (key, out leaf, out index))
            {
                if (index >= leaf.KeyCount)
                {
                    leaf = leaf.rightLeaf;
                    index = 0;
                }
            }
            else if (--index < 0)
                leaf = null;
        }

        private protected void TryGetGT (T key, out Leaf leaf, out int index)
        {
            bool _ = FindEdgeRight (key, out leaf, out index);
            if (index >= leaf.KeyCount)
            {
                leaf = leaf.rightLeaf;
                index = 0;
            }
        }

        private protected void TryGetGE (T key, out Leaf leaf, out int index)
        {
            bool _ = FindEdgeLeft (key, out leaf, out index);
            if (index >= leaf.KeyCount)
            {
                leaf = leaf.rightLeaf;
                index = 0;
            }
        }

        /// <exclude />
        protected int StageBump()
         => ++stage;

        /// <exclude />
        protected void StageCheck (int expected)
        {
            if (stage != expected)
                throw new InvalidOperationException ("Operation is not valid because collection was modified.");
        }

        #endregion

        #region Nonpublic item methods

        private protected bool AddKey (T item, NodeVector path)
        {
            StageBump();

            path.IncrementPathWeight();

            var leaf = (Leaf) path.TopNode;
            if (leaf.KeyCount < maxKeyCount)
            {
                leaf.InsertKey (path.TopIndex, item);
                return true;
            }

            // Leaf is full so right split a new leaf.
            var newLeaf = new Leaf (leaf, maxKeyCount);
            int pathIndex = path.TopIndex;

            if (newLeaf.rightLeaf != null)
                newLeaf.rightLeaf.leftLeaf = newLeaf;
            else
            {
                rightmostLeaf = newLeaf;

                if (pathIndex == leaf.KeyCount)
                {
                    newLeaf.AddKey (item);
                    path.Promote (item, (Node) newLeaf, true);
                    return true;
                }
            }

            int splitIndex = leaf.KeyCount / 2 + 1;
            if (pathIndex < splitIndex)
            {
                // Left-side insert: Copy right side to the split leaf.
                newLeaf.Add (leaf, splitIndex - 1, leaf.KeyCount);
                leaf.Truncate (splitIndex - 1);
                leaf.InsertKey (pathIndex, item);
            }
            else
            {
                // Right-side insert: Copy split leaf parts and new key.
                newLeaf.Add (leaf, splitIndex, pathIndex);
                newLeaf.AddKey (item);
                newLeaf.Add (leaf, pathIndex, leaf.KeyCount);
                leaf.Truncate (splitIndex);
            }

            // Promote anchor of split leaf.
            path.Promote (newLeaf.Key0, (Node) newLeaf, newLeaf.rightLeaf == null);
            return true;
        }

        private protected System.Collections.Generic.IEnumerable<T> Distinct2()
        {
            if (root.Weight == 0)
                yield break;

            int stageFreeze = stage;
            int ix = 0;
            Leaf leaf = leftmostLeaf;
            for (T key = leaf.Key0;;)
            {
                yield return key;
                StageCheck (stageFreeze);

                if (++ix < leaf.KeyCount)
                {
                    T nextKey = leaf.GetKey (ix);
                    if (keyComparer.Compare (key, nextKey) != 0)
                    { key = nextKey; continue; }
                }

                FindEdgeRight (key, out leaf, out ix);
                if (ix >= leaf.KeyCount)
                {
                    leaf = leaf.rightLeaf;
                    if (leaf == null)
                        yield break;
                    ix = 0;
                }
                key = leaf.GetKey (ix);
            }
        }

        private protected int RemoveAll2 (System.Collections.Generic.IEnumerable<T> other)
        {
            int removed = 0;
            if (root.Weight > 0)
            {
                StageBump();
                if (other == this)
                {
                    removed = root.Weight;
                    Initialize();
                }
                else
                {
                    var oBag = other as RankedBag<T> ?? new RankedBag<T> (other, keyComparer);
                    if (oBag.Count > 0)
                        foreach (var oKey in oBag.Distinct())
                        {
                            var oCount = oBag.GetCount (oKey);
                            int actual = Remove2 (oKey, oCount);
                            removed += actual;
                        }
                }
            }
            return removed;
        }

        private protected void ReplaceKey (NodeVector path, T item)
        {
            StageBump();
            ((Leaf) path.TopNode).SetKey (path.TopIndex, item);
        }

        # endregion

        #region Nonpublic pair methods

        private protected void Add2<TValue> (NodeVector path, T key, TValue value)
        {
            StageBump();

            var leaf = (PairLeaf<TValue>) path.TopNode;
            int pathIndex = path.TopIndex;

            path.IncrementPathWeight();
            if (leaf.KeyCount < maxKeyCount)
            {
                leaf.Insert (pathIndex, key, value);
                return;
            }

            // Leaf is full so right split a new leaf.
            var newLeaf = new PairLeaf<TValue> (leaf, maxKeyCount);

            if (newLeaf.rightLeaf != null)
                newLeaf.rightLeaf.leftLeaf = newLeaf;
            else
            {
                rightmostLeaf = newLeaf;

                if (pathIndex == leaf.KeyCount)
                {
                    newLeaf.Add (key, value);
                    path.Promote (key, (Node) newLeaf, true);
                    return;
                }
            }

            int splitIndex = leaf.KeyCount / 2 + 1;
            if (pathIndex < splitIndex)
            {
                // Left-side insert: Copy right side to the split leaf.
                newLeaf.Add (leaf, splitIndex - 1, leaf.KeyCount);
                leaf.Truncate (splitIndex - 1);
                leaf.Insert (pathIndex, key, value);
            }
            else
            {
                // Right-side insert: Copy split leaf parts and new key.
                newLeaf.Add (leaf, splitIndex, pathIndex);
                newLeaf.Add (key, value);
                newLeaf.Add (leaf, pathIndex, leaf.KeyCount);
                leaf.Truncate (splitIndex);
            }

            // Promote anchor of split leaf.
            path.Promote (newLeaf.Key0, (Node) newLeaf, newLeaf.rightLeaf==null);
        }

        private protected int ContainsValue2<TValue> (TValue value)
        {
            int result = 0;

            if (value != null)
            {
                var comparer = System.Collections.Generic.EqualityComparer<TValue>.Default;
                for (var leaf = (PairLeaf<TValue>) leftmostLeaf; leaf != null; leaf = (PairLeaf<TValue>) leaf.rightLeaf)
                {
                    for (int vix = 0; vix < leaf.ValueCount; ++vix)
                        if (comparer.Equals (leaf.GetValue (vix), value))
                            return result + vix;
                    result += leaf.KeyCount;
                }
            }
            else
                for (var leaf = (PairLeaf<TValue>) leftmostLeaf; leaf != null; leaf = (PairLeaf<TValue>) leaf.rightLeaf)
                {
                    for (int vix = 0; vix < leaf.ValueCount; ++vix)
                        if (leaf.GetValue (vix) == null)
                            return result + vix;
                    result += leaf.KeyCount;
                }

            return -1;
        }

        private protected int RemoveWhere2<V> (Predicate<KeyValuePair<T,V>> match)
        {
            if (match == null)
                throw new ArgumentNullException (nameof (match));

            int stageFreeze = stage;
            int leafLoss = 0, treeLoss = 0;
            var path = NodeVector.CreateFromIndex (this, 0);
            var leaf = (PairLeaf<V>) path.TopNode;
            int ix = 0;

            for (;; ++ix)
            {
                if (ix >= leaf.KeyCount)
                {
                    endOfLeaf();
                    if (leaf == null)
                        break;
                }

                bool isMatch = match (new KeyValuePair<T,V> (leaf.GetKey (ix), leaf.GetValue (ix)));
                StageCheck (stageFreeze);
                if (isMatch)
                {
                    ++leafLoss;
                    stageFreeze = StageBump();
                }
                else if (leafLoss != 0)
                {
                    leaf.CopyPairLeft (ix, leafLoss);
                    if (ix == leafLoss)
                        path.SetPivot (leaf.Key0);
                }
            }

            if (treeLoss != 0)
                TrimRoot();
            return treeLoss;

            void endOfLeaf()
            {
                if (leafLoss == 0)
                { ix = 0; leaf = (PairLeaf<V>) path.TraverseRight(); }
                else
                {
                    leaf.Truncate (ix-leafLoss);
                    treeLoss += leafLoss; path.ChangePathWeight (-leafLoss); leafLoss = 0;

                    if (leaf.rightLeaf == null)
                    {
                        if (leaf.KeyCount == 0)
                            path.Balance();
                        leaf = null;
                    }
                    else if (! IsUnderflow (leaf.KeyCount))
                    { ix = 0; leaf = (PairLeaf<V>) path.TraverseRight(); }
                    else
                    {
                        var path2 = new NodeVector (path, path.Height);
                        if (leaf.KeyCount > 0)
                        { ix = leaf.KeyCount; path2.Balance(); }
                        else
                        {
                            ix = 0; leaf = (PairLeaf<V>) path.TraverseLeft();
                            path2.Balance();
                            if (leaf != null)
                                leaf = (PairLeaf<V>) path.TraverseRight();
                            else
                            {
                                path = NodeVector.CreateFromIndex (this, 0);
                                leaf = (PairLeaf<V>) path.TopNode;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Public properties and methods

        /// <summary>Gets or sets the <em>order</em> of the underlying B+ tree structure.</summary>
        /// <remarks>
        /// <para>
        /// The <em>order</em> of a tree (also known as branching factor) is the maximum number of child nodes that a branch may reference.
        /// The minimum number of child node references for a non-rightmost branch is <em>order</em>/2.
        /// The maximum number of elements in a leaf is <em>order</em>-1.
        /// The minimum number of elements in a non-rightmost leaf is <em>order</em>/2.
        /// </para>
        /// <para>
        /// Changing this property may degrade performance and is provided for experimental purposes only.
        /// The default value of 128 should always be adequate.
        /// </para>
        /// <para>
        /// Attempts to set this value when <em>Count</em> is non-zero are ignored.
        /// Non-negative values below 4 or above 256 are ignored.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">When supplied value is less than zero.</exception>
        public int Capacity
        {
            get => maxKeyCount + 1;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException ("Must be between " + MinimumOrder + " and " + MaximumOrder + ".");

                if (root.Weight == 0 && value >= MinimumOrder && value <= MaximumOrder)
                    maxKeyCount = value - 1;
            }
        }

        #endregion

        #region Debug methods
#if DEBUG

        // Telemetry counters:
        /// <summary>Maximum number of keys that the existing branches can hold.</summary>
        public int BranchSlotCount { get; private set; }
        /// <summary>Number of keys contained in the branches.</summary>
        public int BranchSlotsUsed { get; private set; }
        /// <summary>Maximum number of keys that the existing leaves can hold.</summary>
        public int LeafSlotCount { get; private set; }
        /// <summary>Number of keys contained in the leaves.</summary>
        public int LeafSlotsUsed { get; private set; }

        /// <summary>Perform diagnostics check for data structure sanity errors.</summary>
        /// <remarks>
        /// Since this is an in-memory managed structure, any errors would indicate a bug.
        /// Also performs space complexity diagnostics to ensure that all non-rightmost nodes maintain 50% fill.
        /// </remarks>
        public void SanityCheck()
        {
            BranchSlotCount = 0;
            BranchSlotsUsed = 0;
            LeafSlotCount = 0;
            LeafSlotsUsed = 0;

            Leaf lastLeaf;
            if (root is Branch branch)
                lastLeaf = CheckBranch (branch, 1, GetHeight(), true, default, null);
            else
                lastLeaf = CheckLeaf ((Leaf) root, default, null);

            root.SanityCheck();

            if (lastLeaf.rightLeaf != null)
                throw new InvalidOperationException ("Last leaf has invalid RightLeaf");

            if (root.Weight != LeafSlotsUsed)
                throw new InvalidOperationException ("Mismatched Count=" + root.Weight + ", expected=" + LeafSlotsUsed);

            if (leftmostLeaf.leftLeaf != null)
                throw new InvalidOperationException ("leftmostLeaf has a left leaf");

            if (rightmostLeaf.rightLeaf != null)
                throw new InvalidOperationException ("rightmostLeaf has a right leaf");
        }

        /// <summary>Gets maximum number of children of a branch.</summary>
        /// <returns>Maximum number of children of a branch.</returns>
        public int GetOrder()
         => maxKeyCount + 1;

        /// <summary>Gets the number of levels in the tree.</summary>
        /// <returns>Number of levels in the tree.</returns>
        public int GetHeight()
        {
            Node node = root;
            for (int level = 1; ; ++level)
                if (node is Branch branch)
                    node = branch.Child0;
                else
                    return level;
        }

        private Leaf CheckBranch
        (
            Branch branch,
            int level, int height,
            bool isRightmost,
            T anchor,  // ignored when isRightmost true
            Leaf visited
        )
        {
            BranchSlotCount += maxKeyCount;
            BranchSlotsUsed += branch.KeyCount;

            if (! isRightmost && IsUnderflow (branch.ChildCount))
                throw new InvalidOperationException ("Branch underflow");

            if (branch.ChildCount != branch.KeyCount + 1)
                throw new InvalidOperationException ("Branch mismatched ChildCount, KeyCount");

            int actualWeight = 0;
            for (int i = 0; i < branch.ChildCount; ++i)
            {
                T anchor0 = i == 0 ? anchor : branch.GetKey (i - 1);
                bool isRightmost0 = isRightmost && i == branch.KeyCount;
                if (i < branch.KeyCount - 1)
                    if (keyComparer.Compare (branch.GetKey (i), branch.GetKey (i + 1)) > 0)
                        throw new InvalidOperationException ("Branch keys descending");

                if (level + 1 < height)
                    visited = CheckBranch ((Branch) branch.GetChild (i), level+1, height, isRightmost0, anchor0, visited);
                else
                    visited = CheckLeaf ((Leaf) branch.GetChild (i), anchor0, visited);

                branch.GetChild (i).SanityCheck();
                actualWeight += branch.GetChild (i).Weight;
            }
            if (branch.Weight != actualWeight)
                throw new InvalidOperationException ("Branch mismatched weight");

            return visited;
        }

        private Leaf CheckLeaf (Leaf leaf, T anchor, Leaf visited)
        {
            LeafSlotCount += maxKeyCount;
            LeafSlotsUsed += leaf.KeyCount;

            if (leaf.rightLeaf != null && IsUnderflow (leaf.KeyCount))
                throw new InvalidOperationException ("Leaf underflow");

            if (! anchor.Equals (default (T)) && ! anchor.Equals (leaf.Key0))
                throw new InvalidOperationException ("Leaf has wrong anchor");

            for (int i = 0; i < leaf.KeyCount; ++i)
                if (i < leaf.KeyCount - 1 && keyComparer.Compare (leaf.GetKey (i), leaf.GetKey (i + 1)) > 0)
                    throw new InvalidOperationException ("Leaf keys descending");

            if (visited == null)
            {
                if (! anchor.Equals (default (T)))
                    throw new InvalidOperationException ("Inconsistent visited, anchor");
            }
            else
                if (visited.rightLeaf != leaf)
                    throw new InvalidOperationException ("Leaf has bad RightLeaf");

            return leaf;
        }

        /// <summary>Gets telemetry summary.</summary>
        /// <returns>Telemetry summary.</returns>
        public string GetTreeStatsText()
        {
            SanityCheck();
            string result = "--- height = " + GetHeight();

            if (BranchSlotCount != 0)
                result += ", branch fill = " + (int) (BranchSlotsUsed * 100.0 / BranchSlotCount + 0.5) + "%";

            return result + ", leaf fill = " + (int) (LeafSlotsUsed * 100.0 / LeafSlotCount + 0.5) + "%";
        }

        /// <summary>Generates content of tree by level (breadth first).</summary>
        /// <returns>Text lines where each line is a level of the tree.</returns>
        public IEnumerable<string> GenerateTreeText (bool showWeight=false)
        {
            int level = 0;
            Node leftmost;
            var sb = new StringBuilder();

            for (;;)
            {
                var branchPath = new NodeVector (this, level);
                leftmost = branchPath.TopNode;
                if (leftmost is Leaf)
                    break;

                var branch = (Branch) leftmost;

                sb.Append ('B');
                sb.Append (level);
                sb.Append (": ");
                for (;;)
                {
                    branch.Append (sb);
                    if (showWeight)
                    {
                        sb.Append (" (");
                        sb.Append (branch.Weight);
                        sb.Append (") ");
                    }

                    branch = (Branch) branchPath.TraverseRight();
                    if (branch == null)
                        break;

                    sb.Append (" | ");
                }
                ++level;
                yield return sb.ToString();
                sb.Length = 0;
            }

            var leafPath = new NodeVector (this, level);
            sb.Append ('L');
            sb.Append (level);
            sb.Append (": ");
            for (var leaf = (Leaf) leftmost;;)
            {
                leaf.Append (sb);
                leaf = (Leaf) leafPath.TraverseRight();
                if (leaf == null)
                    break;

                if (leafPath.IsLeftmostNode)
                    sb.Append (" | ");
                else
                    sb.Append ('|');
            }
            yield return sb.ToString();
        }

#endif
        #endregion
    }
}
