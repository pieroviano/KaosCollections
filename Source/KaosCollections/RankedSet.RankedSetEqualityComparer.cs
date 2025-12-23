//
// Library: KaosCollections
// File:    RankedSet.RankedSetEqualityComparer.cs
//
// Copyright © 2009-2020 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System.Collections;
using System.Collections.Generic;

namespace Kaos.Collections;
#if PUBLIC
    public
#else
internal
#endif
    partial class RankedSet<T> : IRankedSet<T>
{
    private class RankedSetEqualityComparer : IEqualityComparer<RankedSet<T>>
    {
        private readonly IComparer<T> comparer;
        private readonly IEqualityComparer<T> equalityComparer;

        public RankedSetEqualityComparer (IEqualityComparer<T> equalityComparer)
        {
            this.comparer = Comparer<T>.Default;
            this.equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
        }

        public bool Equals (RankedSet<T> s1, RankedSet<T> s2)
            => RankedSet<T>.RankedSetEquals (s1, s2, comparer);

        public int GetHashCode (RankedSet<T> set)
        {
            var hashCode = 0;
            if (set != null)
                foreach (var item in set)
                    hashCode ^= (equalityComparer.GetHashCode (item) & 0x7FFFFFFF);

            return hashCode;
        }

        public override bool Equals (object obComparer)
            => obComparer is RankedSetEqualityComparer rsComparer && comparer == rsComparer.comparer;

        public override int GetHashCode()
            => comparer.GetHashCode() ^ equalityComparer.GetHashCode();
    }
}