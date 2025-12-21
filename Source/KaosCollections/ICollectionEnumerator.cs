using System;
using System.Collections.Generic;

namespace System.Collections;

public interface ICollectionEnumerator<T>
{
    /// <summary>Gets the item at the current position of the enumerator.</summary>
    T Current { get; }

    /// <summary>Advances the enumerator to the next item in the set.</summary>
    /// <returns><b>true</b> if the enumerator was successfully advanced to the next item; <b>false</b> if the enumerator has passed the end of the set.</returns>
    /// <exception cref="InvalidOperationException">When the set was modified after the enumerator was created.</exception>
    bool MoveNext();

    /// <summary>Releases all resources used by the enumerator.</summary>
    void Dispose();

    /// <summary>Gets an iterator for this collection.</summary>
    /// <returns>An iterator for this collection.</returns>
    IEnumerator<T> GetEnumerator();

    /// <summary>Bypasses a supplied number of items and yields the remaining items.</summary>
    /// <param name="count">Number of items to skip.</param>
    /// <returns>The items after the supplied offset.</returns>
    /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
    /// <example>
    /// In the below snippet, both Skip operations perform an order of magnitude faster than their LINQ equivalent.
    /// <code source="..\Bench\RxExample01\RxExample01.cs" lang="cs" region="RsSkip" />
    /// </example>
    /// <exception cref="InvalidOperationException">When the set was modified after the enumerator was created.</exception>
    ICollectionEnumerator<T> Skip (int count);

    /// <summary>
    /// Bypasses items as long as a supplied condition is true and yields the remaining items.
    /// </summary>
    /// <param name="predicate">The condition to test for.</param>
    /// <returns>Remaining items after the first item that does not satisfy the supplied condition.</returns>
    /// <exception cref="InvalidOperationException">When the set was modified after the enumerator was created.</exception>
    ICollectionEnumerator<T> SkipWhile (Func<T,bool> predicate);

    /// <summary>
    /// Bypasses items as long as a supplied index-based condition is true and yields the remaining items.
    /// </summary>
    /// <param name="predicate">The condition to test for.</param>
    /// <returns>Remaining items after the first item that does not satisfy the supplied condition.</returns>
    /// <exception cref="InvalidOperationException">When the set was modified after the enumerator was created.</exception>
    ICollectionEnumerator<T> SkipWhile (Func<T,int,bool> predicate);
}