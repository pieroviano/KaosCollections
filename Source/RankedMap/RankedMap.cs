﻿//
// Library: KaosCollections
// File:    RankedMap.cs
//
// Copyright © 2009-2017 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
#if NET35 || NET40 || NET45 || SERIALIZE
using System.Runtime.Serialization;
#endif

namespace Kaos.Collections
{
    /// <summary>
    /// Represents a collection of key/value pairs that can be accessed in sort order or by index.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the map.</typeparam>
    /// <typeparam name="TValue">The type of the values in the map.</typeparam>
    [DebuggerTypeProxy (typeof (ICollectionDebugView<,>))]
    [DebuggerDisplay("Count = {Count}")]
#if NET35 || NET40 || NET45 || SERIALIZE
    [Serializable]
#endif
    public partial class RankedMap<TKey,TValue> :
        Btree<TKey>
        , ICollection<KeyValuePair<TKey,TValue>>
        , ICollection
#if ! NET35 && ! NET40
        , IReadOnlyCollection<KeyValuePair<TKey,TValue>>
#endif
#if NET35 || NET40 || NET45 || SERIALIZE
        , ISerializable
        , IDeserializationCallback
#endif
    {
#if NET35 || NET40 || NET45 || SERIALIZE
        [NonSerialized]
#endif
        private KeyCollection keys;
#if NET35 || NET40 || NET45 || SERIALIZE
        [NonSerialized]
#endif
        private ValueCollection values;

        #region Constructors

        /// <summary>Initializes a new map instance using the default key comparer.</summary>
        public RankedMap() : base (Comparer<TKey>.Default, new PairLeaf<TValue>())
        { }

        /// <summary>Initializes a new map instance using the supplied key comparer.</summary>
        /// <param name="comparer">Comparison operator for keys.</param>
        /// <remarks>
        /// <para>
        /// This class requires an <see cref="IComparer"/> implementation to perform key comparisons.
        /// If <em>comparer</em> is <b>null</b>, the default comparer for the type will be used.
        /// If the key type implements the <see cref="IComparable"/>interface, the default comparer uses that implementation.
        /// If no comparison implementation is available, the Add method will fail on the second element.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">When <em>comparer</em> is <b>null</b> and no other comparer available.</exception>
        public RankedMap (IComparer<TKey> comparer) : base (comparer, new PairLeaf<TValue>())
        { }


        /// <summary>Initializes a new map instance that contains key/value pairs copied from the supplied map and sorted by the default comparer.</summary>
        /// <param name="map">The map to be copied.</param>
        /// <exception cref="ArgumentNullException">When <em>map</em> is <b>null</b>.</exception>
        public RankedMap (ICollection<KeyValuePair<TKey,TValue>> map) : this (map, Comparer<TKey>.Default)
        { }


        /// <summary>Initializes a new map instance that contains key/value pairs copied from the supplied map and sorted by the supplied comparer.</summary>
        /// <param name="map">The map to be copied.</param>
        /// <param name="comparer">Comparison operator for keys.</param>
        /// <exception cref="ArgumentNullException">When <em>map</em> is <b>null</b>.</exception>
        /// <exception cref="InvalidOperationException">When <em>comparer</em> is <b>null</b> and no other comparer available.</exception>
        public RankedMap (ICollection<KeyValuePair<TKey,TValue>> map, IComparer<TKey> comparer) : this (comparer)
        {
            if (map == null)
                throw new ArgumentNullException (nameof (map));

            foreach (KeyValuePair<TKey,TValue> pair in map)
                Add (pair.Key, pair.Value);
        }

        #endregion

        #region Properties

        /// <summary>Returns a wrapper of the method used to order elements in the map.</summary>
        /// <remarks>
        /// To override sorting based on the default comparer,
        /// supply an alternate comparer when constructing the map.
        /// </remarks>
        public IComparer<TKey> Comparer => keyComparer;


        /// <summary>Gets the number of elements in the map.</summary>
        /// <remarks>This is a O(1) operation.</remarks>
        public int Count => root.Weight;


        /// <summary>Indicates that the collection is not thread safe.</summary>
        bool ICollection.IsSynchronized => false;


        /// <summary>Gets a collection containing the keys of the map.</summary>
        /// <remarks>
        /// The keys given by this collection are sorted according to the <see cref="Comparer"/> property.
        /// </remarks>
        public KeyCollection Keys
        {
            get
            {
                if (keys == null)
                    keys = new KeyCollection (this);
                return keys;
            }
        }


        /// <summary>Gets an object that can be used to synchronize access to the collection.</summary>
        object ICollection.SyncRoot => GetSyncRoot();


        /// <summary>Gets a collection containing the values of the map.</summary>
        /// <remarks>
        /// The values given by this collection are sorted in the same order as their respective keys.
        /// </remarks>
        public ValueCollection Values
        {
            get
            {
                if (values == null)
                    values = new ValueCollection (this);
                return values;
            }
        }


        /// <summary>Gets the maximum key in the map per the comparer.</summary>
        /// <remarks>This is a O(1) operation.</remarks>
        public TKey MaxKey => Count == 0 ? default (TKey) : rightmostLeaf.GetKey (rightmostLeaf.KeyCount - 1);


        /// <summary>Gets the minimum key in the map per the comparer.</summary>
        /// <remarks>This is a O(1) operation.</remarks>
        public TKey MinKey => Count == 0 ? default (TKey) : leftmostLeaf.Key0;


        /// <summary>Indicates that this collection may be modified.</summary>
        bool ICollection<KeyValuePair<TKey,TValue>>.IsReadOnly => false;

        #endregion

        #region Methods

        /// <summary>Adds an element with the supplied key and value.</summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. May be null.</param>
        /// <returns><b>true</b> if this is the first occurrence of an element with this key; otherwise <b>false</b>.</returns>
        /// <remarks>
        /// <para>This is a O(log <em>n</em>) operation.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
        /// <exception cref="ArgumentException">When no comparer is available.</exception>
        public bool Add (TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException (nameof (key));

            var path = new NodeVector (this, key, leftEdge:false);
            Add2<TValue> (path, key, value);
            return ! path.IsFound;
        }


        /// <summary>Adds an element with the supplied key/value pair.</summary>
        /// <param name="keyValuePair">Contains the key and value of the element to add.</param>
        /// <exception cref="ArgumentException">When an element containing <em>key</em> has already been added.</exception>
        void ICollection<KeyValuePair<TKey,TValue>>.Add (KeyValuePair<TKey,TValue> keyValuePair)
        {
            var path = new NodeVector (this, keyValuePair.Key, leftEdge:false);
            Add2<TValue> (path, keyValuePair.Key, keyValuePair.Value);
        }


        /// <summary>Removes all elements from the map.</summary>
        /// <remarks>This is a O(1) operation.</remarks>
        public void Clear() => Initialize();


        /// <summary>Determines whether the map contains the supplied key.</summary>
        /// <param name="key">The key to locate.</param>
        /// <returns><b>true</b> if <em>key</em> is contained in the map; otherwise <b>false</b>.</returns>
        /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
        public bool ContainsKey (TKey key)
        {
            if (key == null)
                throw new ArgumentNullException (nameof (key));

            var leaf = (PairLeaf<TValue>) Find (key, out int index);
            return index >= 0;
        }


        /// <summary>Determines whether the map contains the supplied key/value pair.</summary>
        /// <param name="keyValuePair">The key/value pair to locate.</param>
        /// <returns><b>true</b> if <em>keyValuePair</em> is contained in the map; otherwise <b>false</b>.</returns>
        bool ICollection<KeyValuePair<TKey,TValue>>.Contains (KeyValuePair<TKey,TValue> keyValuePair)
        {
            if (FindEdgeLeft (keyValuePair.Key, out Leaf leaf, out int index))
            {
                PairLeaf<TValue> pairLeaf;
                if (index < leaf.KeyCount)
                    pairLeaf = (PairLeaf<TValue>) leaf;
                else
                {
                    pairLeaf = (PairLeaf<TValue>) leaf.rightLeaf;
                    index = 0;
                }
                do
                {
                    if (EqualityComparer<TValue>.Default.Equals (keyValuePair.Value, pairLeaf.GetValue (index)))
                        return true;

                    if (++index >= pairLeaf.KeyCount)
                    {
                        pairLeaf = (PairLeaf<TValue>) pairLeaf.rightLeaf;
                        if (pairLeaf == null)
                            break;
                        index = 0;
                    }
                }
                while (Comparer.Compare (keyValuePair.Key, pairLeaf.GetKey (index)) == 0);
            }
            return false;
        }


        /// <summary>Determines whether the map contains the supplied value.</summary>
        /// <param name="value">The value to locate.</param>
        /// <returns><b>true</b> if <em>value</em> is contained in the map; otherwise <b>false</b>.</returns>
        /// <remarks>This is a O(<em>n</em>) operation.</remarks>
        public bool ContainsValue (TValue value) => ContainsValue2<TValue> (value) >= 0;


        /// <summary>Copies the map to a compatible array, starting at the supplied position.</summary>
        /// <param name="array">A one-dimensional array that is the destination of the copy.</param>
        /// <param name="index">The zero-based starting position in <em>array</em>.</param>
        /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero.</exception>
        /// <exception cref="ArgumentException">When not enough space is given for the copy.</exception>
        public void CopyTo (KeyValuePair<TKey,TValue>[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException (nameof (array));

            if (index < 0)
                throw new ArgumentOutOfRangeException (nameof (index), "Index is less than zero.");

            if (Count > array.Length - index)
                throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.");

            for (var leaf = (PairLeaf<TValue>) leftmostLeaf; leaf != null; leaf = (PairLeaf<TValue>) leaf.rightLeaf)
                for (int leafIndex = 0; leafIndex < leaf.KeyCount; ++leafIndex)
                    array[index++] = new KeyValuePair<TKey,TValue> (leaf.GetKey (leafIndex), leaf.GetValue (leafIndex));
        }


        /// <summary>Copies the elements of the map to an array, starting at the supplied array index.</summary>
        /// <param name="array">The destination array of the copy.</param>
        /// <param name="index">The zero-based index in <em>array</em> at which copying begins.</param>
        /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero.</exception>
        /// <exception cref="ArgumentException">
        /// When array is multidimensional,
        /// the number of elements in the source is greater than the available space,
        /// or the type of the source cannot be cast for the destination.
        /// </exception>
        void ICollection.CopyTo (Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException (nameof (array));

            if (array.Rank > 1)
                throw new ArgumentException ("Multidimension array is not supported on this operation.", nameof (array));

            if (index < 0)
                throw new ArgumentOutOfRangeException (nameof (index), "Index is less than zero.");

            if (Count > array.Length - index)
                throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.", nameof (array));

            if (! (array is KeyValuePair<TKey,TValue>[]) && array.GetType() != typeof (Object[]))
                throw new ArgumentException ("Target array type is not compatible with the type of items in the collection.", nameof (array));

            for (var leaf = (PairLeaf<TValue>) leftmostLeaf; leaf != null; leaf = (PairLeaf<TValue>) leaf.rightLeaf)
                for (int leafIndex = 0; leafIndex < leaf.KeyCount; ++leafIndex)
                {
                    array.SetValue (new KeyValuePair<TKey,TValue>(leaf.GetKey (leafIndex), leaf.GetValue (leafIndex)), index);
                    ++index;
                }
        }


        /// <summary>Gets the key/value pair at the supplied index.</summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at <em>index</em>.</returns>
        /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero or greater than or equal to the number of keys.</exception>
        public KeyValuePair<TKey,TValue> ElementAt (int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException (nameof (index), "Argument is out of the range of valid values.");

            var leaf = (PairLeaf<TValue>) Find (index, out int leafIndex);
            return new KeyValuePair<TKey,TValue> (leaf.GetKey (leafIndex), leaf.GetValue (leafIndex));
        }


        /// <summary>Gets the key/value pair at the supplied index or the default if the index is out of range.</summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at <em>index</em>.</returns>
        /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
        public KeyValuePair<TKey,TValue> ElementAtOrDefault (int index)
        {
            if (index < 0 || index >= Count)
                return new KeyValuePair<TKey,TValue> (default (TKey), default (TValue));

            var leaf = (PairLeaf<TValue>) Find (index, out int leafIndex);
            return new KeyValuePair<TKey,TValue> (leaf.GetKey (leafIndex), leaf.GetValue (index));
        }


        /// <summary>Returns the number of occurrences of the supplied key in the map.</summary>
        /// <param name="key">The key to return the number of occurrences for.</param>
        /// <returns>The number of occurrences of <em>key</em>.</returns>
        /// <remarks>
        /// <para>
        /// This is a O(log <em>n</em>) operation
        /// where <em>n</em> is <see cref="Count"/>.
        /// </para>
        /// </remarks>
        public int GetCount (TKey key) => GetCount2 (key);


        /// <summary>Returns the number of distinct keys in the map.</summary>
        /// <returns>The number of distinct items in the map.</returns>
        /// <remarks>
        /// This is a O(<em>m</em> log <em>n</em>) operation
        /// where <em>m</em> is the distinct key count
        /// and <em>n</em> is <see cref="Count"/>.
        /// </remarks>
        public int GetDistinctCount() => GetDistinctCount2();


        /// <summary>Gets the index of the first element with the supplied key.</summary>
        /// <param name="key">The key of the element to find.</param>
        /// <returns>The index of the first element containing <em>key</em> if found; otherwise a negative value holding the bitwise complement of the insert point.</returns>
        /// <remarks>
        /// <para>
        /// Elements with multiple occurrences of a key will return the occurrence with the lowest index.
        /// If an element containing <em>key</em> is not found, apply the bitwise complement operator
        /// (<see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/bitwise-complement-operator">~</see>)
        /// to the result to get the index of the next higher item.
        /// </para>
        /// <para>
        /// This is a O(log <em>n</em>) operation.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
        public int IndexOfKey (TKey key)
        {
            if (key == null)
                throw new ArgumentNullException (nameof (key));

            return FindEdgeForIndex (key, out Leaf leaf, out int leafIndex, leftEdge:true);
        }


        /// <summary>Gets the index of the first element with the supplied value.</summary>
        /// <param name="value">The value of the element to seek.</param>
        /// <returns>The index of the element if found; otherwise -1.</returns>
        /// <remarks>This is a O(<em>n</em>) operation.</remarks>
        public int IndexOfValue (TValue value)
        {
            int result = 0;
            for (var leaf = (PairLeaf<TValue>) leftmostLeaf; leaf != null; leaf = (PairLeaf<TValue>) leaf.rightLeaf)
            {
                var ix = leaf.IndexOfValue (value);
                if (ix >= 0)
                    return result + ix;
                result += leaf.KeyCount;
            }

            return -1;
        }


        /// <summary>Removes all elements with the supplied key from the map.</summary>
        /// <param name="key">The key of the elements to remove.</param>
        /// <returns><b>true</b> if any elements were removed; otherwise <b>false</b>.</returns>
        /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
        /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
        public bool Remove (TKey key)
        {
            if (key == null)
                throw new ArgumentNullException (nameof (key));

            var path1 = new NodeVector (this, key, leftEdge:true);
            if (! path1.IsFound)
                return false;

            var path2 = new NodeVector (this, key, leftEdge:false);

            StageBump();
            Delete (path1, path2);
            return true;
        }

        /// <summary>Removes a supplied number of elements with the supplied key from the map.</summary>
        /// <param name="key">The key of the elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        /// <returns>The number of elements actually removed.</returns>
        /// <exception cref="ArgumentException">When <em>count</em> is less than zero.</exception>
        public int Remove (TKey key, int count)
        {
            if (count < 0)
                throw new ArgumentException ("Must be non-negative.", nameof (count));

            return Remove2 (key, count);
        }


        /// <summary>Deletes all occurrences of the supplied key and its associated value from the collection.</summary>
        /// <param name="keyValuePair">Contains key and value of elements to find and remove.</param>
        /// <returns><b>true</b> if any elements were removed; otherwise <b>false</b>.</returns>
        /// <remarks>No operation is taken unless both key and value match.</remarks>
        bool ICollection<KeyValuePair<TKey,TValue>>.Remove (KeyValuePair<TKey,TValue> keyValuePair)
        {
            var path = new NodeVector (this, keyValuePair.Key, leftEdge:true);
            if (! path.IsFound)
                return false;

            int leafLoss = 0, treeLoss = 0;
            var leaf = (PairLeaf<TValue>) path.TopNode;
            int ix = path.TopIndex;
            if (ix >= leaf.KeyCount)
            { ix = 0; leaf = (PairLeaf<TValue>) path.TraverseRight(); }

            for (;;)
            {
                if (EqualityComparer<TValue>.Default.Equals (keyValuePair.Value, leaf.GetValue (ix)))
                    ++leafLoss;
                else if (leafLoss != 0)
                {
                    leaf.CopyPairLeft (ix, leafLoss);
                    if (ix == leafLoss)
                        path.SetPivot (leaf.Key0);
                }

                if (++ix >= leaf.KeyCount)
                {
                    EndOfNode();
                    if (leaf == null)
                        break;
                }

                if (Comparer.Compare (keyValuePair.Key, leaf.GetKey (ix)) != 0)
                {
                    if (leafLoss != 0)
                    {
                        leaf.RemoveRange (ix-leafLoss, leafLoss);
                        if (ix == leafLoss)
                            path.SetPivot (leaf.Key0);
                        treeLoss -= leafLoss;
                        path.ChangePathWeight (-leafLoss);
                        if (IsUnderflow (leaf.KeyCount))
                            path.Balance();
                    }
                    break;
                }
            };

            StageBump();
            if (treeLoss != 0)
                TrimRoot();
            return treeLoss != 0;

            void EndOfNode()
            {
                if (leafLoss == 0)
                { ix = 0; leaf = (PairLeaf<TValue>) path.TraverseRight(); }
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
                    { ix = 0; leaf = (PairLeaf<TValue>) path.TraverseRight(); }
                    else
                    {
                        var path2 = new NodeVector (path, path.Height);
                        if (leaf.KeyCount > 0)
                        { ix = leaf.KeyCount; path2.Balance(); }
                        else
                        {
                            ix = 0; leaf = (PairLeaf<TValue>) path.TraverseLeft();
                            path2.Balance();
                            if (leaf == null)
                            {
                                path = NodeVector.CreateFromIndex (this, 0);
                                leaf = (PairLeaf<TValue>) path.TopNode;
                            }
                            else
                                leaf = (PairLeaf<TValue>) path.TraverseRight();
                        }
                    }
                }
            }
        }



        /// <summary>Removes an element at the supplied index from the map.</summary>
        /// <param name="index">The zero-based position of the element to remove.</param>
        /// <para>
        /// After this operation, the position of all following items is reduced by one.
        /// </para>
        /// <para>
        /// This is a O(log <em>n</em>) operation.
        /// </para>
        /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero or greater than or equal to <see cref="Count"/>.</exception>
        public void RemoveAt (int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException (nameof (index), "Argument is out of the range of valid values.");

            RemoveAt2 (index);
        }


        /// <summary>Removes an index range of elements from the map.</summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        /// <remarks>This is a O(log <em>n</em>) operation where <em>n</em> is <see cref="Count"/>.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> or <em>count</em> is less than zero.</exception>
        /// <exception cref="ArgumentException">When <em>index</em> and <em>count</em> do not denote a valid range of elements in the set.</exception>
        public void RemoveRange (int index, int count)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException ("Argument was out of the range of valid values.", nameof (index));

            if (count < 0)
                throw new ArgumentOutOfRangeException ("Argument was out of the range of valid values.", nameof (count));

            if (count > Size - index)
                throw new ArgumentException ("Argument was out of the range of valid values.");

            RemoveRange2 (index, count);
        }


        /// <summary>Removes all elements from the map that match the condition defined by the supplied key-parameterized predicate.</summary>
        /// <param name="match">The condition of the elements to remove.</param>
        /// <returns>The number of elements removed from the map.</returns>
        /// <remarks>
        /// This is a O(<em>n</em> log <em>m</em>) operation
        /// where <em>m</em> is the number of elements removed and <em>n</em> is <see cref="Count"/>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">When <em>match</em> is <b>null</b>.</exception>
        public int RemoveWhere (Predicate<TKey> match)
        {
            return RemoveWhere2 (match);
        }

        /// <summary>Removes all elements from the map that match the condition defined by the supplied key/value-parameterized predicate.</summary>
        /// <param name="match">The condition of the elements to remove.</param>
        /// <returns>The number of elements removed from the map.</returns>
        /// <remarks>
        /// This is a O(<em>n</em> log <em>m</em>) operation
        /// where <em>m</em> is the number of elements removed and <em>n</em> is <see cref="Count"/>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">When <em>match</em> is <b>null</b>.</exception>
        public int RemoveWherePair (Predicate<KeyValuePair<TKey,TValue>> match)
        {
            return RemoveWhere2<TValue> (match);
        }

        #endregion

        #region ISerializable implementation and support
#if NET35 || NET40 || NET45 || SERIALIZE

        private SerializationInfo serializationInfo;

        /// <summary>Initializes a new map instance that contains serialized data.</summary>
        /// <param name="info">The object that contains the information required to serialize the map.</param>
        /// <param name="context">The structure that contains the source and destination of the serialized stream.</param>
        protected RankedMap (SerializationInfo info, StreamingContext context) : base (new PairLeaf<TValue>())
        {
            this.serializationInfo = info;
        }

        /// <summary>Returns the data needed to serialize the map.</summary>
        /// <param name="info">An object that contains the information required to serialize the map.</param>
        /// <param name="context">A structure that contains the source and destination of the serialized stream.</param>
        /// <exception cref="ArgumentNullException">When <em>info</em> is <b>null</b>.</exception>
        protected virtual void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException (nameof (info));

            info.AddValue ("Count", Count);
            info.AddValue ("Comparer", Comparer, typeof (IComparer<TKey>));
            info.AddValue ("Stage", stage);

            var keys = new TKey[Count];
            Keys.CopyTo (keys, 0);
            info.AddValue ("Keys", keys, typeof (TKey[]));

            var values = new TValue[Count];
            Values.CopyTo (values, 0);
            info.AddValue ("Values", values, typeof (TValue[]));
        }


        /// <summary>Implements the deserialization callback and raises the deserialization event when completed.</summary>
        /// <param name="sender">The source of the deserialization event.</param>
        /// <exception cref="ArgumentNullException">When <em>sender</em> is <b>null</b>.</exception>
        /// <exception cref="SerializationException">When the associated <em>SerializationInfo</em> is invalid.</exception>
        protected virtual void OnDeserialization (object sender)
        {
            if (keyComparer != null)
                return;  // Owner did the fixups.

            if (serializationInfo == null)
                throw new SerializationException ("Missing information.");

            keyComparer = (IComparer<TKey>) serializationInfo.GetValue ("Comparer", typeof (IComparer<TKey>));
            int storedCount = serializationInfo.GetInt32 ("Count");
            stage = serializationInfo.GetInt32 ("Stage");

            if (storedCount != 0)
            {
                var keys = (TKey[]) serializationInfo.GetValue ("Keys", typeof (TKey[]));
                if (keys == null)
                    throw new SerializationException ("Missing Keys.");

                var values = (TValue[]) serializationInfo.GetValue ("Values", typeof (TValue[]));
                if (keys == null)
                    throw new SerializationException ("Missing Values.");

                if (keys.Length != values.Length)
                    throw new SerializationException ("Mismatched key/value count.");

                for (int ix = 0; ix < keys.Length; ++ix)
                    Add (keys[ix], values[ix]);

                if (storedCount != keys.Length)
                    throw new SerializationException ("Mismatched count.");
            }

            serializationInfo = null;
        }


        /// <summary>Returns the data needed to serialize the map.</summary>
        /// <param name="info">An object that contains the information required to serialize the map.</param>
        /// <param name="context">A structure that contains the source and destination of the serialized stream.</param>
        /// <exception cref="ArgumentNullException">When <em>info</em> is <b>null</b>.</exception>
        void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
        { GetObjectData (info, context); }


        /// <summary>Implements the deserialization callback and raises the deserialization event when completed.</summary>
        /// <param name="sender">The source of the deserialization event.</param>
        /// <exception cref="ArgumentNullException">When <em>sender</em> is <b>null</b>.</exception>
        /// <exception cref="SerializationException">When the associated <em>SerializationInfo</em> is invalid.</exception>
        void IDeserializationCallback.OnDeserialization (Object sender)
        { OnDeserialization (sender); }

#endif
        #endregion

        #region Enumeration

        /// <summary>Returns an enumerator that iterates over a range with the supplied bounds.</summary>
        /// <param name="lower">Minimum key of the range.</param>
        /// <param name="upper">Maximum key of the range.</param>
        /// <returns>An enumerator for the specified range.</returns>
        /// <remarks>
        /// <para>
        /// If either <em>lower</em> or <em>upper</em> are present in the map,
        /// they will be included in the results.
        /// </para>
        /// <para>
        /// Retrieving the first element is a O(log <em>n</em>) operation.
        /// Retrieving subsequent elements is a O(1) operation per element.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">When the map was modified after the enumerator was created.</exception>
        public IEnumerable<KeyValuePair<TKey,TValue>> ElementsBetween (TKey lower, TKey upper)
        {
            int stageFreeze = stage;
            FindEdgeLeft (lower, out Leaf leaf, out int index);
            var pairLeaf = (PairLeaf<TValue>) leaf;

            for (;;)
            {
                if (index < leaf.KeyCount)
                {
                    if (Comparer.Compare (pairLeaf.GetKey (index), upper) > 0)
                        yield break;

                    yield return pairLeaf.GetPair (index);
                    StageCheck (stageFreeze);
                    ++index;
                    continue;
                }

                pairLeaf = (PairLeaf<TValue>) pairLeaf.rightLeaf;
                if (pairLeaf == null)
                    yield break;

                index = 0;
            }
        }


        /// <summary>Returns an enumerator that iterates over a range with the supplied lower bound.</summary>
        /// <param name="lower">Minimum key of the range.</param>
        /// <returns>An enumerator for the specified range.</returns>
        /// <remarks>
        /// <para>
        /// If <em>lower</em> is present in the map, it will be included in the results.
        /// </para>
        /// <para>
        /// Retrieving the initial item is a O(log <em>n</em>) operation.
        /// Retrieving each subsequent item is a O(1) operation.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">When the map was modified after the enumerator was created.</exception>
        public IEnumerable<KeyValuePair<TKey,TValue>> ElementsFrom (TKey lower)
        {

            int stageFreeze = stage;
            FindEdgeLeft (lower, out Leaf leaf, out int index);
            var pairLeaf = (PairLeaf<TValue>) leaf;

            for (;;)
            {
                if (index < pairLeaf.KeyCount)
                {
                    yield return pairLeaf.GetPair (index);
                    StageCheck (stageFreeze);
                    ++index;
                    continue;
                }

                pairLeaf = (PairLeaf<TValue>) pairLeaf.rightLeaf;
                if (pairLeaf == null)
                    yield break;

                index = 0;
            }
        }


        /// <summary>Returns an enumerator that iterates over a range with the supplied index bounds.</summary>
        /// <param name="lowerIndex">Minimum index of the range.</param>
        /// <param name="upperIndex">Maximum index of the range.</param>
        /// <returns>An enumerator for the specified index range.</returns>
        /// <remarks>
        /// <para>
        /// Index bounds are inclusive.
        /// </para>
        /// <para>
        /// Retrieving the initial item is a O(log <em>n</em>) operation.
        /// Retrieving each subsequent item is a O(1) operation.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">When <em>lowerIndex</em> is less than zero or not less than the number of elements.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When <em>upperIndex</em> is less than zero or not less than the number of elements.</exception>
        /// <exception cref="ArgumentException">When <em>lowerIndex</em> and <em>upperIndex</em> do not denote a valid range of elements in the map.</exception>
        /// <exception cref="InvalidOperationException">When the map was modified after the enumerator was created.</exception>
        public IEnumerable<KeyValuePair<TKey,TValue>> ElementsBetweenIndexes (int lowerIndex, int upperIndex)
        {
            if (lowerIndex < 0 || lowerIndex >= Count)
                throw new ArgumentOutOfRangeException (nameof (lowerIndex), "Argument was out of the range of valid values.");

            if (upperIndex < 0 || upperIndex >= Count)
                throw new ArgumentOutOfRangeException (nameof (upperIndex), "Argument was out of the range of valid values.");

            int toGo = upperIndex - lowerIndex;
            if (toGo < 0)
                throw new ArgumentException ("Arguments were out of the range of valid values.");

            int stageFreeze = stage;
            var leaf = (PairLeaf<TValue>) Find (lowerIndex, out int index);
            do
            {
                if (index >= leaf.KeyCount)
                { index = 0; leaf = (PairLeaf<TValue>) leaf.rightLeaf; }

                yield return leaf.GetPair (index);
                StageCheck (stageFreeze);
                ++index;
            }
            while (--toGo >= 0);
        }


        /// <summary>Returns an enumerator that iterates thru the map in reverse order.</summary>
        /// <returns>An enumerator that reverse iterates thru the map.</returns>
        public IEnumerable<KeyValuePair<TKey,TValue>> Reverse()
        {
            Enumerator etor = new Enumerator (this, isReverse:true);
            while (etor.MoveNext())
                yield return etor.Current;
        }


        /// <summary>Gets an enumerator that iterates thru the map.</summary>
        /// <returns>An enumerator for the map.</returns>
        public Enumerator GetEnumerator() => new Enumerator (this);

        /// <summary>Gets an enumerator that iterates thru the map.</summary>
        /// <returns>An enumerator for the map.</returns>
        IEnumerator<KeyValuePair<TKey,TValue>> IEnumerable<KeyValuePair<TKey,TValue>>.GetEnumerator()
            => new Enumerator (this);

        /// <summary>Gets an enumerator that iterates thru the collection.</summary>
        /// <returns>An enumerator for the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator (this, isReverse:false, nonGeneric:true);


        /// <summary>Enumerates the sorted key/value pairs of a <see cref="RankedMap{TKey,TValue}"/>.</summary>
        public sealed class Enumerator : IEnumerator<KeyValuePair<TKey,TValue>>
        {
            private readonly RankedMap<TKey,TValue> tree;
            private readonly bool isReverse;
            private readonly bool nonGeneric;
            private PairLeaf<TValue> leaf;
            private int index;
            private int stageFreeze;
            private int state;  // -1=rewound; 0=active; 1=consumed

            /// <summary>Make an iterator that will loop thru the collection in order.</summary>
            /// <param name="map">Collection containing these key/value pairs.</param>
            /// <param name="isReverse">Supply <b>true</b> to iterate from last to first.</param>
            /// <param name="nonGeneric">Supply <b>true</b> to indicate object Current should return DictionaryEntry values.</param>
            internal Enumerator (RankedMap<TKey,TValue> map, bool isReverse=false, bool nonGeneric=false)
            {
                this.tree = map;
                this.isReverse = isReverse;
                this.nonGeneric = nonGeneric;
                ((IEnumerator) this).Reset();
            }

            /// <summary>Gets the element at the current position.</summary>
            object IEnumerator.Current
            {
                get
                {
                    tree.StageCheck (stageFreeze);
                    if (state != 0)
                        throw new InvalidOperationException ("Enumeration is not active.");

                    if (nonGeneric)
                        return new DictionaryEntry (leaf.GetKey (index), leaf.GetValue (index));
                    else
                        return leaf.GetPair (index);
                }
            }

            /// <summary>Gets the key/value pair at the current position.</summary>
            /// <exception cref="InvalidOperationException">When the map was modified after the enumerator was created.</exception>
            public KeyValuePair<TKey,TValue> Current
            {
                get
                {
                    tree.StageCheck (stageFreeze);
                    return state == 0 ? leaf.GetPair (index)
                                      : new KeyValuePair<TKey,TValue> (default (TKey), default (TValue));
                }
            }

            /// <summary>Advances the enumerator to the next element in the map.</summary>
            /// <returns><b>true</b> if the enumerator was successfully advanced to the next element; <b>false</b> if the enumerator has passed the end of the collection.</returns>
            /// <exception cref="InvalidOperationException">When the map was modified after the enumerator was created.</exception>
            public bool MoveNext()
            {
                tree.StageCheck (stageFreeze);

                if (state != 0)
                    if (state > 0)
                        return false;
                    else
                    {
                        leaf = (PairLeaf<TValue>) (isReverse ? tree.rightmostLeaf : tree.leftmostLeaf);
                        index = isReverse ? leaf.KeyCount : -1;
                        state = 0;
                    }

                if (isReverse)
                {
                    if (--index >= 0)
                        return true;

                    leaf = (PairLeaf<TValue>) leaf.leftLeaf;
                    if (leaf != null)
                    { index = leaf.KeyCount - 1; return true; }
                }
                else
                {
                    if (++index < leaf.KeyCount)
                        return true;

                    leaf = (PairLeaf<TValue>) leaf.rightLeaf;
                    if (leaf != null)
                    { index = 0; return true; }
                }

                state = 1;
                return false;
            }

            /// <summary>Rewinds the enumerator to its initial state.</summary>
            void IEnumerator.Reset()
            {
                stageFreeze = tree.stage;
                state = -1;
            }

            /// <summary>Releases all resources used by the enumerator.</summary>
            public void Dispose() { }
        }

        #endregion
    }
}