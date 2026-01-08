//
// Library: KaosCollections
// File:    ValueEnumerator.cs
//
// Copyright © 2009-2020 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

#nullable enable

using System;

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
    private protected class ValueEnumerator<V> : BaseEnumerator
    {
        public V? CurrentValue { get; private set; }

        public V CurrentValueOrDefault => (NotActive ? default : CurrentValue)!;

        public ValueEnumerator(Btree<T> owner, bool isReverse = false) : base(owner, isReverse)
        { }

        public ValueEnumerator(Btree<T> owner, int count) : base(owner, count)
        { }

        public ValueEnumerator(Btree<T> owner, Func<V, bool> condition) : base(owner)
            => Bypass2(condition, (leaf, ix) => ((PairLeaf<V>)leaf).GetValue(ix));

        public ValueEnumerator(Btree<T> owner, Func<V, int, bool> condition) : base(owner)
            => Bypass3(condition, (leaf, ix) => ((PairLeaf<V>)leaf).GetValue(ix));

        public void Initialize()
        {
            Init();
            CurrentValue = default;
        }

        public bool Advance()
        {
            if (AdvanceBase())
            {
                var pairLeaf = ((PairLeaf<V>?)leaf);
                if (pairLeaf != null)
                {
                    CurrentValue = pairLeaf.GetValue(leafIndex);
                }

                return true; }
            else
            { CurrentValue = default; return false; }
        }

        public void BypassValue(Func<V, bool> condition)
            => Bypass2(condition, (leaf, ix) => ((PairLeaf<V>)leaf).GetValue(ix));

        public void BypassValue(Func<V, int, bool> condition)
            => Bypass3(condition, (leaf, ix) => ((PairLeaf<V>)leaf).GetValue(ix));
    }
}