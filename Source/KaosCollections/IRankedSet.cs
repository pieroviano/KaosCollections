using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace System.Collections;

public interface IRankedSet<T> :
    ISet<T>,
    ICollection<T>,
    ICollection,
    IReadOnlyCollection<T>,
    ISerializable,
    IDeserializationCallback
{
    /// <summary>Returns a wrapper of the method used to order items in the set.</summary>
    /// <remarks>
    /// To override sorting based on the default comparer,
    /// supply an alternate comparer when constructing the set.
    /// </remarks>
    IComparer<T> Comparer { get; }

    /// <summary>Gets the number of items in the set.</summary>
    /// <remarks>This is a O(1) operation.</remarks>
    int Count { get; }

    /// <summary>Gets the maximum item in the set per the comparer.</summary>
    /// <remarks>This is a O(1) operation.</remarks>
    T Max { get; }

    /// <summary>Gets the minimum item in the set per the comparer.</summary>
    /// <remarks>This is a O(1) operation.</remarks>
    T Min { get; }

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
    int Capacity { get; set; }

    /// <summary>Adds an item to the set and returns a success indicator.</summary>
    /// <param name="item">The item to add.</param>
    /// <returns><b>true</b> if <em>item</em> was added to the set; otherwise <b>false</b>.</returns>
    /// <remarks>
    /// <para>
    /// If <em>item</em> is already in the set, this method returns <b>false</b> and does not throw an exception.
    /// </para>
    /// <para>This is a O(log <em>n</em>) operation.</para>
    /// </remarks>
    /// <exception cref="ArgumentException">When no comparer is available.</exception>
    bool Add(T item);

    /// <summary>Removes all items from the set.</summary>
    /// <remarks>This is a O(1) operation.</remarks>
    void Clear();

    /// <summary>Determines whether the set contains the supplied item.</summary>
    /// <param name="item">The item to locate.</param>
    /// <returns><b>true</b> if <em>item</em> is contained in the set; otherwise <b>false</b>.</returns>
    /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
    bool Contains(T item);

    /// <summary>Copies the set to a compatible array, starting at the beginning of the array.</summary>
    /// <param name="array">A one-dimensional array that is the destination of the copy.</param>
    /// <remarks>This is a O(<em>n</em>) operation.</remarks>
    /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
    /// <exception cref="ArgumentException">When not enough space is available for the copy.</exception>
    void CopyTo(T[] array);

    /// <summary>Copies the set to a compatible array, starting at the supplied position.</summary>
    /// <param name="array">A one-dimensional array that is the destination of the copy.</param>
    /// <param name="index">The zero-based starting position in <em>array</em>.</param>
    /// <remarks>This is a O(<em>n</em>) operation.</remarks>
    /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero.</exception>
    /// <exception cref="ArgumentException">When not enough space is available for the copy.</exception>
    void CopyTo(T[] array, int index);

    /// <summary>Copies a supplied number of items to a compatible array, starting at the supplied position.</summary>
    /// <param name="array">A one-dimensional array that is the destination of the copy.</param>
    /// <param name="index">The zero-based starting position in <em>array</em>.</param>
    /// <param name="count">The number of items to copy.</param>
    /// <remarks>This is a O(<em>n</em>) operation.</remarks>
    /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> or <em>count</em> is less than zero.</exception>
    /// <exception cref="ArgumentException">When not enough space is available for the copy.</exception>
    void CopyTo(T[] array, int index, int count);

    /// <summary>Removes the supplied item from the set.</summary>
    /// <param name="item">The item to remove.</param>
    /// <returns><b>true</b> if <em>item</em> was found and removed; otherwise <b>false</b>.</returns>
    /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
    bool Remove(T item);

    /// <summary>Removes an index range of items from the set.</summary>
    /// <param name="index">The zero-based starting index of the range of items to remove.</param>
    /// <param name="count">The number of items to remove.</param>
    /// <remarks>This is a O(log <em>n</em>) operation where <em>n</em> is <see cref="Count"/>.</remarks>
    /// <example>
    /// <para>Here, this method is is used to truncate a set.</para>
    /// <code source="..\Bench\RsExample01\RsExample01.cs" lang="cs"/>
    /// </example>
    /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> or <em>count</em> is less than zero.</exception>
    /// <exception cref="ArgumentException">When <em>index</em> and <em>count</em> do not denote a valid range of items in the set.</exception>
    void RemoveRange(int index, int count);

    /// <summary>Removes all items that match the condition defined by the supplied predicate from the set.</summary>
    /// <param name="match">The condition of the items to remove.</param>
    /// <returns>The number of items removed from the set.</returns>
    /// <remarks>
    /// This is a O(<em>n</em> log <em>m</em>) operation
    /// where <em>m</em> is the number of items removed and <em>n</em> is <see cref="Count"/>.
    /// </remarks>
    /// <example>
    /// <para>Here, this method is is used to remove strings containing a space.</para>
    /// <code source="..\Bench\RsExample01\RsExample01.cs" lang="cs"/>
    /// </example>
    /// <exception cref="ArgumentNullException">When <em>match</em> is <b>null</b>.</exception>
    int RemoveWhere(Predicate<T> match);

    /// <summary>Removes all items in the supplied collection from the set.</summary>
    /// <param name="other">The collection of items to remove.</param>
    /// <remarks>
    /// Duplicate values in <em>other</em> are ignored.
    /// Values in <em>other</em> that are not in the set are ignored.
    /// </remarks>
    /// <example>
    /// <code source="..\Bench\RsExample04\RsExample04.cs" lang="cs" />
    /// </example>
    /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
    void ExceptWith(IEnumerable<T> other);

    /// <summary>Removes all items that are not in a supplied collection.</summary>
    /// <param name="other">The collection of items to intersect.</param>
    /// <example>
    /// <code source="..\Bench\RsExample04\RsExample04.cs" lang="cs" />
    /// </example>
    /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
    void IntersectWith(IEnumerable<T> other);

    /// <summary>Determines whether the set is a proper subset of the supplied collection.</summary>
    /// <param name="other">The collection to compare to this set.</param>
    /// <returns><b>true</b> if the set is a proper subset of <em>other</em>; otherwise <b>false</b>.</returns>
    /// <example>
    /// <code source="..\Bench\RsExample03\RsExample03.cs" lang="cs" />
    /// </example>
    /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
    bool IsProperSubsetOf(IEnumerable<T> other);

    /// <summary>Determines whether the set is a proper superset of the supplied collection.</summary>
    /// <param name="other">The collection to compare to this set.</param>
    /// <returns><b>true</b> if the set is a proper superset of <em>other</em>; otherwise <b>false</b>.</returns>
    /// <example>
    /// <code source="..\Bench\RsExample03\RsExample03.cs" lang="cs" />
    /// </example>
    /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
    bool IsProperSupersetOf(IEnumerable<T> other);

    /// <summary>Determines whether the set is a subset of the supplied collection.</summary>
    /// <param name="other">The collection to compare to this set.</param>
    /// <returns><b>true</b> if the set is a subset of <em>other</em>; otherwise <b>false</b>.</returns>
    /// <example>
    /// <code source="..\Bench\RsExample03\RsExample03.cs" lang="cs" />
    /// </example>
    /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
    bool IsSubsetOf(IEnumerable<T> other);

    /// <summary>Determines whether a set is a superset of the supplied collection.</summary>
    /// <param name="other">The items to compare to the current set.</param>
    /// <returns><b>true</b> if the set is a superset of <em>other</em>; otherwise <b>false</b>.</returns>
    /// <example>
    /// <code source="..\Bench\RsExample03\RsExample03.cs" lang="cs" />
    /// </example>
    /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
    bool IsSupersetOf(IEnumerable<T> other);

    /// <summary>Determines whether the set and a supplied collection share common items.</summary>
    /// <param name="other">The collection to compare to this set.</param>
    /// <returns><b>true</b> if the set and <em>other</em> share at least one common item; otherwise <b>false</b>.</returns>
    /// <example>
    /// <code source="..\Bench\RsExample03\RsExample03.cs" lang="cs" />
    /// </example>
    /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
    bool Overlaps(IEnumerable<T> other);

    /// <summary>Determines whether the set and the supplied collection contain the same items.</summary>
    /// <param name="other">The collection to compare to this set.</param>
    /// <returns><b>true</b> if the set is equal to <em>other</em>; otherwise <b>false</b>.</returns>
    /// <remarks>Duplicate values in <em>other</em> are ignored.</remarks>
    /// <example>
    /// <code source="..\Bench\RsExample03\RsExample03.cs" lang="cs" />
    /// </example>
    /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
    bool SetEquals(IEnumerable<T> other);

    /// <summary>Modifies the set so that it contains only items that are present either in itself or in the supplied collection, but not both.</summary>
    /// <param name="other">The collection to compare to this set.</param>
    /// <example>
    /// <code source="..\Bench\RsExample04\RsExample04.cs" lang="cs" />
    /// </example>
    /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
    void SymmetricExceptWith(IEnumerable<T> other);

    /// <summary>Add all items in <em>other</em> to this set that are not already in this set.</summary>
    /// <param name="other">The collection to add to this set.</param>
    /// <remarks>Duplicate values in <em>other</em> are ignored.</remarks>
    /// <example>
    /// <code source="..\Bench\RsExample04\RsExample04.cs" lang="cs" />
    /// </example>
    /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
    void UnionWith(IEnumerable<T> other);

    /// <summary>Gets the item at the supplied index.</summary>
    /// <param name="index">The zero-based index of the item to get.</param>
    /// <returns>The item at <em>index</em>.</returns>
    /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
    /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero or not less than the number of items.</exception>
    T ElementAt(int index);

    /// <summary>Gets the item at the supplied index or the default if the index is out of range.</summary>
    /// <param name="index">The zero-based index of the item to get.</param>
    /// <returns>The item at <em>index</em>.</returns>
    /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
    T ElementAtOrDefault(int index);

    /// <summary>Gets the minimum item in the set per the comparer.</summary>
    /// <returns>The minimum item in the set.</returns>
    /// <remarks>This is a O(1) operation.</remarks>
    /// <exception cref="InvalidOperationException">When <see cref="Count"/> is zero.</exception>
    T First();

    /// <summary>Gets the maximum item in the set per the comparer.</summary>
    /// <returns>The maximum item in the set.</returns>
    /// <remarks>This is a O(1) operation.</remarks>
    /// <exception cref="InvalidOperationException">When <see cref="Count"/> is zero.</exception>
    T Last();

    /// <summary>Returns an IEnumerable that iterates thru the set in reverse order.</summary>
    /// <returns>An enumerator that reverse iterates thru the set.</returns>
    /// <exception cref="InvalidOperationException">When the set was modified after the enumerator was created.</exception>
    ICollectionEnumerator<T> Reverse();

    /// <summary>Returns an enumerator that iterates over a range with the supplied bounds.</summary>
    /// <param name="lower">Minimum item value of the range.</param>
    /// <param name="upper">Maximum item value of the range.</param>
    /// <returns>An enumerator for the specified range.</returns>
    /// <remarks>
    /// <para>
    /// If either <em>lower</em> or <em>upper</em> are present in the set,
    /// they will be included in the results.
    /// </para>
    /// <para>
    /// Retrieving the initial item is a O(log <em>n</em>) operation.
    /// Retrieving each subsequent item is a O(1) operation.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">When the set was modified after the enumerator was created.</exception>
    IEnumerable<T> ElementsBetween(T lower, T upper);

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
    /// <exception cref="ArgumentOutOfRangeException">When <em>lowerIndex</em> is less than zero or not less than <see cref="Count"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">When <em>upperIndex</em> is less than zero or not less than <see cref="Count"/>.</exception>
    /// <exception cref="ArgumentException">When <em>lowerIndex</em> and <em>upperIndex</em> do not denote a valid range of indexes.</exception>
    /// <exception cref="InvalidOperationException">When the set was modified after the enumerator was created.</exception>
    IEnumerable<T> ElementsBetweenIndexes(int lowerIndex, int upperIndex);

    /// <summary>Returns an enumerator that iterates over a range with the supplied lower bound.</summary>
    /// <param name="lower">Minimum of the range.</param>
    /// <returns>An enumerator for the specified range.</returns>
    /// <remarks>
    /// <para>
    /// If <em>lower</em> is present in the set, it will be included in the results.
    /// </para>
    /// <para>
    /// Retrieving the initial item is a O(log <em>n</em>) operation.
    /// Retrieving each subsequent item is a O(1) operation.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">When the set was modified after the enumerator was created.</exception>
    IEnumerable<T> ElementsFrom(T lower);

    /// <summary>Gets the index of the supplied item.</summary>
    /// <param name="item">The item to find.</param>
    /// <returns>The index of <em>item</em> if found; otherwise a negative value holding the bitwise complement of the insert point.</returns>
    /// <remarks>
    /// <para>
    /// If <em>item</em> is not found, apply the bitwise complement operator
    /// (<see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/bitwise-complement-operator">~</see>)
    /// to the result to get the index of the next higher item.
    /// </para>
    /// <para>
    /// This is a O(log <em>n</em>) operation.
    /// </para>
    /// </remarks>
    int IndexOf(T item);

    /// <summary>Removes the item at the supplied index from the set.</summary>
    /// <param name="index">The zero-based position of the item to remove.</param>
    /// <para>
    /// After this operation, the position of all following items is reduced by one.
    /// </para>
    /// <para>
    /// This is a O(log <em>n</em>) operation.
    /// </para>
    /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero or greater than or equal to <see cref="Count"/>.</exception>
    void RemoveAt(int index);

    /// <summary>Replace an item if present.</summary>
    /// <param name="item">The replacement item.</param>
    /// <returns><b>true</b> if an item is replaced; otherwise <b>false</b>.</returns>
    /// <remarks>
    /// This single operation is equivalent to performing a
    /// <see cref="Remove"/> operation followed by an
    /// <see cref="Add"/> operation.
    /// </remarks>
    bool Replace(T item);

    /// <summary>Replace an item or optionally add it if missing.</summary>
    /// <param name="item">The replacement or new item.</param>
    /// <param name="addIfMissing"><b>true</b> to add <em>item</em> if not already present.</param>
    /// <returns><b>true</b> if item is replaced; otherwise <b>false</b>.</returns>
    /// <remarks>
    /// This operation is an optimized alternative to performing the implicit operations separately.
    /// </remarks>
    bool Replace(T item, bool addIfMissing);

    /// <summary>Bypasses a supplied number of items and yields the remaining items.</summary>
    /// <param name="count">Number of items to skip.</param>
    /// <returns>The items after the supplied offset.</returns>
    /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
    /// <example>
    /// In the below snippet, both Skip operations perform an order of magnitude faster than their LINQ equivalent.
    /// <code source="..\Bench\RxExample01\RxExample01.cs" lang="cs" region="RsSkip" />
    /// </example>
    /// <exception cref="InvalidOperationException">When the set was modified after the enumerator was created.</exception>
    ICollectionEnumerator<T> Skip(int count);

    /// <summary>
    /// Bypasses elements as long as a supplied condition is true and yields the remaining items.
    /// </summary>
    /// <param name="predicate">The condition to test for.</param>
    /// <returns>Remaining items after the first item that does not satisfy the supplied condition.</returns>
    /// <exception cref="InvalidOperationException">When the set was modified after the enumerator was created.</exception>
    ICollectionEnumerator<T> SkipWhile(Func<T, bool> predicate);

    /// <summary>
    /// Bypasses elements as long as a supplied index-based condition is true and yields the remaining items.
    /// </summary>
    /// <param name="predicate">The condition to test for.</param>
    /// <returns>Remaining items after the first item that does not satisfy the supplied condition.</returns>
    /// <exception cref="InvalidOperationException">When the set was modified after the enumerator was created.</exception>
    ICollectionEnumerator<T> SkipWhile(Func<T, int, bool> predicate);

    /// <summary>Gets the actual item for the supplied search item.</summary>
    /// <param name="getItem">The item to find.</param>
    /// <param name="item">
    /// If <em>item</em> is found, its value is placed here;
    /// otherwise it will be loaded with the default for its type.
    /// </param>
    /// <returns><b>true</b> if <em>getItem</em> is found; otherwise <b>false</b>.</returns>
    bool TryGet(T getItem, out T item);

    /// <summary>Gets the least item greater than the supplied item.</summary>
    /// <param name="getItem">The item to use for comparison.</param>
    /// <param name="item">The actual item if found; otherwise the default.</param>
    /// <returns><b>true</b> if item greater than <em>getItem</em> is found; otherwise <b>false</b>.</returns>
    bool TryGetGreaterThan(T getItem, out T item);

    /// <summary>Gets the least item greater than or equal to the supplied item.</summary>
    /// <param name="getItem">The item to use for comparison.</param>
    /// <param name="item">The actual item if found; otherwise the default.</param>
    /// <returns><b>true</b> if item greater than or equal to <em>getItem</em> found; otherwise <b>false</b>.</returns>
    bool TryGetGreaterThanOrEqual(T getItem, out T item);

    /// <summary>Gets the greatest item that is less than the supplied item.</summary>
    /// <param name="getItem">The item to use for comparison.</param>
    /// <param name="item">The actual item if found; otherwise the default.</param>
    /// <returns><b>true</b> if item less than <em>item</em> found; otherwise <b>false</b>.</returns>
    bool TryGetLessThan(T getItem, out T item);

    /// <summary>Gets the greatest item that is less than or equal to the supplied item.</summary>
    /// <param name="getItem">The item to use for comparison.</param>
    /// <param name="item">The actual item if found; otherwise the default.</param>
    /// <returns><b>true</b> if item less than or equal to <em>item</em> found; otherwise <b>false</b>.</returns>
    bool TryGetLessThanOrEqual(T getItem, out T item);

    /// <summary>Returns an enumerator that iterates thru the set.</summary>
    /// <returns>An enumerator that iterates thru the set in sorted order.</returns>
    ICollectionEnumerator<T> GetEnumerator();
}