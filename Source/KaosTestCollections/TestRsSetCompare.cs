//
// Library: KaosCollections
// File:    TestRsSetCompare.cs
//

using Xunit;
#if TEST_BCL
using System.Collections.Generic;
#else
using Kaos.Collections;
#endif

namespace Kaos.Test.Collections
{
    public class UserComparer : System.Collections.Generic.Comparer<User>, IClassFixture<BinaryFormatterEnableFixture>
    {
        public override int Compare(User x, User y) => string.Compare(x.Name, y.Name);
    }

    public class User : System.IComparable<User>, IClassFixture<BinaryFormatterEnableFixture>
    {
        public string Name { get; private set; }

        public User(string name)
        {
            this.Name = name;
        }

        public int CompareTo(User other) => string.Compare(this.Name, other.Name);
    }

    public partial class TestRs : IClassFixture<BinaryFormatterEnableFixture>
    {
#if TEST_BCL
        SortedSet<string> setS1 = new SortedSet<string>(), setS2 = new SortedSet<string>();
        IEqualityComparer<SortedSet<string>> setComparer
            = SortedSet<string>.CreateSetComparer();
#else
        private RankedSet<string> setS1 = new RankedSet<string>();
        private RankedSet<string> setS2 = new RankedSet<string>();
        System.Collections.Generic.IEqualityComparer<RankedSet<string>> setComparer = RankedSet<string>.CreateSetComparer();
#endif
        [Fact]
        public void UnitRsc_Equals()
        {
#if TEST_BCL
            var setComparer2 = SortedSet<string>.CreateSetComparer();
            var setComparer3 = SortedSet<int>.CreateSetComparer();
#else
            var setComparer2 = RankedSet<string>.CreateSetComparer();
            var setComparer3 = RankedSet<int>.CreateSetComparer();
#endif
            bool eq0 = setComparer.Equals(null);
            Assert.False(eq0);
            bool eq1 = setComparer.Equals(setComparer);
            Assert.True(eq1);
            bool eq2 = setComparer.Equals(setComparer2);
            Assert.True(eq2);
            bool eq3 = setComparer.Equals(setComparer3);
            Assert.False(eq3);
        }

        [Fact]
        public void UnitRsc_GetHashCode()
        {
            int comparerHashCode0 = setComparer.GetHashCode(null);
            Assert.Equal(0, comparerHashCode0);
            int comparerHashCode1 = setComparer.GetHashCode();
            Assert.NotEqual(0, comparerHashCode1);
        }

        [Fact]
        public void UnitRsc_SetGetHashCode()
        {
            if (setS1 == null)
            {
                setS1 = new RankedSet<string>();
            }

            int hc0 = setComparer.GetHashCode(setS1);
            setS1.Add("ABC");
            int hc1 = setComparer.GetHashCode(setS1);
            Assert.NotEqual(hc0, hc1);
        }

        [Fact]
        public void UnitRsc_SetEquals1()
        {
            setS1.Add("ABC");
            setS2.Add("DEF");
            bool eq2 = setComparer.Equals(setS1, setS2);
            Assert.False(eq2);
            setS2.Add("ABC");
            bool eq3 = setComparer.Equals(setS1, setS2);
            Assert.False(eq3);
            setS1.Add("DEF");
            bool eq4 = setComparer.Equals(setS1, setS2);
            Assert.True(eq4);
            setS2 = null;
            bool eq0 = setComparer.Equals(setS1, setS2);
            Assert.False(eq0);
            setS1 = null;
            bool eq1 = setComparer.Equals(setS1, setS2);
            Assert.True(eq1);
        }

        [Fact]
        public void UnitRsc_SetEquals2()
        {
#if TEST_BCL
            var cp = SortedSet<User>.CreateSetComparer();
            var user1 = new SortedSet<User>(new UserComparer());
            var user2 = new SortedSet<User>(new UserComparer());
#else
            var cp = RankedSet<User>.CreateSetComparer();
            var user1 = new RankedSet<User>(new UserComparer());
            var user2 = new RankedSet<User>(new UserComparer());
#endif
            bool eq0 = cp.Equals(user1, user2);
            Assert.True(eq0);
            user1.Add(new User("admin"));
            user2.Add(new User("tester"));
            bool eq1 = cp.Equals(user1, user2);
            Assert.False(eq1);
            user1.Add(new User("tester"));
            user2.Add(new User("admin"));
            bool eq2 = cp.Equals(user1, user2);
            Assert.True(eq2);
        }
    }
}