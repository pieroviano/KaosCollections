//
// Library: KaosCollections
// File:    TestRs.cs
//

using System;
using Xunit;
#if TEST_BCL
using System.Collections.Generic;
#else
using Kaos.Collections;
#pragma warning disable IDE0044
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#endif

namespace Kaos.Test.Collections;

public partial class TestRs : TestBtree, IClassFixture<BinaryFormatterEnableFixture>
{
    #region Test constructors
    [Fact]
    public void UnitRs_Inheritance()
    {
        Setup();
        setI.Add(42);
        setI.Add(21);
        setI.Add(63);
        var toISetI = setI as System.Collections.Generic.ISet<int>;
        var toIColI = setI as System.Collections.Generic.ICollection<int>;
        var toIEnuI = setI as System.Collections.Generic.IEnumerable<int>;
        var toIEnuO = setI as System.Collections.IEnumerable;
        var toIColO = setI as System.Collections.ICollection;
        var toIRocI = setI as System.Collections.Generic.IReadOnlyCollection<int>;
        Assert.NotNull(toISetI);
        Assert.NotNull(toIColI);
        Assert.NotNull(toIEnuI);
        Assert.NotNull(toIEnuO);
        Assert.NotNull(toIColO);
        Assert.NotNull(toIRocI);
        var ObjEnumCount = 0;
        for (var oe = toIEnuO.GetEnumerator(); oe.MoveNext();)
            ++ObjEnumCount;
        Assert.Equal(3, toISetI.Count);
        Assert.Equal(3, toIColI.Count);
        Assert.Equal(3, System.Linq.Enumerable.Count(toIEnuI));
        Assert.Equal(3, ObjEnumCount);
        Assert.Equal(3, toIColO.Count);
        Assert.Equal(3, toIRocI.Count);
    }

#if TEST_BCL
        public class DerivedS : SortedSet<int> { }
#else
    public class DerivedS : RankedSet<int>, IClassFixture<BinaryFormatterEnableFixture>
    {
    }

#endif
    [Fact]
    public void UnitRs_CtorSubclass()
    {
        var sub = new DerivedS();
        var isRO = ((System.Collections.Generic.ICollection<int>)sub).IsReadOnly;
        Assert.False(isRO);
    }

    [Fact]
    public void UnitRs_Ctor0Empty()
    {
        Setup();
        Assert.Equal(0, setTS1.Count);
    }

    [Fact]
    public void CrashRs_Ctor1ANullMissing_InvalidOperation()
    {
        try
        {
            var comp0 = (System.Collections.Generic.Comparer<Person>)null;
#if TEST_BCL
                var nullComparer = new SortedSet<Person> (comp0);
#else
            var nullComparer = new RankedSet<Person>(comp0);
#endif
            nullComparer.Add(new Person("Zed"));
            nullComparer.Add(new Person("Macron"));
            Assert.Fail("Expected exception not thrown");
        }
#if TEST_BCL
            catch (ArgumentException)
#else
        catch (InvalidOperationException)
#endif
        {
            Console.WriteLine("OK");
        }
    }

    // MS docs incorrectly state this will throw.
    [Fact]
    public void UnitRs_Ctor1ANullOk()
    {
        var comp0 = (System.Collections.Generic.Comparer<int>)null;
#if TEST_BCL
            var nullComparer = new SortedSet<int> (comp0);
#else
        var nullComparer = new RankedSet<int>(comp0);
#endif
        nullComparer.Add(4);
        nullComparer.Add(2);
    }

    [Fact]
    public void CrashRs_Ctor1A_InvalidOperation()
    {
        try
        {
#if TEST_BCL
                var sansComparer = new SortedSet<Person>();
#else
            var sansComparer = new RankedSet<Person>();
#endif
            foreach (var name in Person.names)
                sansComparer.Add(new Person(name));
            Assert.Fail("Expected exception not thrown.");
        }
#if TEST_BCL
            catch (ArgumentException)
#else
        catch (InvalidOperationException)
#endif
        {
            testOutputHelper.WriteLine("OK");
        }
    }

    [Fact]
    public void UnitRs_Ctor1A1()
    {
        Setup();
        foreach (var name in Person.names)
            personSet.Add(new Person(name));
        personSet.Add(null);
        personSet.Add(new Person("Zed"));
        Assert.Equal(Person.names.Length + 2, personSet.Count);
    }

    [Fact]
    public void CrashRs_Ctor1B_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var nullArg = (System.Collections.Generic.ICollection<int>)null;
#if TEST_BCL
            var set = new SortedSet<int>(nullArg);
#else
            var set = new RankedSet<int>(nullArg);
#endif
        });
    }

    [Fact]
    public void UnitRs_Ctor1B()
    {
#if TEST_BCL
            var set1 = new SortedSet<int> (iVals1);
            var set3 = new SortedSet<int> (iVals3);
#else
        var set1 = new RankedSet<int>(iVals1);
        var set3 = new RankedSet<int>(iVals3);
#endif
        Assert.Equal(iVals1.Length, set1.Count);
        Assert.Equal(4, set3.Count);
    }

    [Fact]
    public void UnitRs_Ctor2A()
    {
        var pa = new System.Collections.Generic.List<Person>();
        foreach (var name in Person.names)
            pa.Add(new Person(name));
#if TEST_BCL
            var people = new SortedSet<Person> (pa, new PersonComparer());
#else
        var people = new RankedSet<Person>(pa, new PersonComparer());
#endif
        Assert.Equal(Person.names.Length, people.Count);
    }

    #endregion
    #region Test class comparison
    [Fact]
    public void UnitRs_CreateSetComparer0()
    {
        Setup();
        setI.Add(3);
        setI.Add(5);
        setI.Add(7);
#if TEST_BCL
            var setJ = new SortedSet<int>();
            var classComparer = SortedSet<int>.CreateSetComparer();
#else
        var setJ = new RankedSet<int>();
        var classComparer = RankedSet<int>.CreateSetComparer();
#endif
        setJ.Add(3);
        setJ.Add(7);
        var b1 = classComparer.Equals(setI, setJ);
        Assert.False(b1);
        setJ.Add(5);
        var b2 = classComparer.Equals(setI, setJ);
        Assert.True(b2);
    }

    [Fact]
    public void UnitRs_CreateSetComparer1()
    {
        Setup();
#if TEST_BCL
            var setJ = new SortedSet<int>();
            var classComparer = SortedSet<int>.CreateSetComparer();
#else
        var setJ = new RankedSet<int>();
        var classComparer = RankedSet<int>.CreateSetComparer(System.Collections.Generic.EqualityComparer<int>.Default);
#endif
        setJ.Add(3);
        setJ.Add(7);
        var b1 = classComparer.Equals(setI, setI);
        Assert.True(b1);
    }

    #endregion
    #region Test properties
    [Fact]
    public void UnitRs_ocIsSynchronized()
    {
        Setup();
        var oc = (System.Collections.ICollection)setI;
        var isSync = oc.IsSynchronized;
        Assert.False(isSync);
    }

    [Fact]
    public void UnitRs_Max()
    {
        Setup(5);
        Assert.Equal(default(int), setI.Max);
        setI.Add(2);
        setI.Add(1);
        Assert.Equal(2, setI.Max);
        for (var ii = 996; ii >= 3; --ii)
            setI.Add(ii);
        Assert.Equal(996, setI.Max);
    }

    [Fact]
    public void UnitRs_Min()
    {
        Setup(4);
        Assert.Equal(default(int), setI.Min);
        setI.Add(97);
        setI.Add(98);
        Assert.Equal(97, setI.Min);
        for (var ii = 96; ii >= 1; --ii)
            setI.Add(ii);
        Assert.Equal(1, setI.Min);
    }

    [Fact]
    public void UnitRs_ocSyncRoot()
    {
        Setup();
        var oc = (System.Collections.ICollection)setI;
        Assert.False(oc.SyncRoot.GetType().IsValueType);
        Assert.False(oc.SyncRoot.GetType().IsValueType);
    }

    #endregion
    #region Test methods
    [Fact]
    public void UnitRs_Add()
    {
        Setup();
        setS.Add("aa");
        setS.Add("cc");
        var isOk1 = setS.Add("bb");
        Assert.True(isOk1);
        var isOk2 = setS.Add(null);
        Assert.True(isOk2);
        var isOk3 = setS.Add("cc");
        Assert.False(isOk3);
        Assert.Equal(4, setS.Count);
    }

    [Fact]
    public void UnitRs_ocAdd()
    {
        Setup();
        var oc = (System.Collections.Generic.ICollection<int>)setI;
        oc.Add(3);
        oc.Add(5);
        Assert.Equal(2, setI.Count);
        oc.Add(3);
        Assert.Equal(2, setI.Count);
    }

    [Fact]
    public void UnitRs_Clear()
    {
        Setup(4);
        for (var ix = 0; ix < 50; ++ix)
            setI.Add(ix);
        Assert.Equal(50, setI.Count);
        var k1 = 0;
        foreach (var i1 in setI.Reverse())
            ++k1;
        Assert.Equal(50, k1);
        setI.Clear();
        var k2 = 0;
        foreach (var i1 in setI.Reverse())
            ++k2;
        Assert.Equal(0, k2);
    }

    [Fact]
    public void CrashRs_CopyTo1_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            Setup();
            setI.CopyTo(null);
        });
    }

    [Fact]
    public void CrashRs_CopyTo1_Argument()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            Setup();
            var d1 = new int[1];
            setI.Add(1);
            setI.Add(11);
            setI.CopyTo(d1);
        });
    }

    [Fact]
    public void UnitRs_CopyTo1()
    {
        Setup();
        var e3 = new[]
        {
            3,
            5,
            7
        };
        var e4 = new[]
        {
            3,
            5,
            7,
            0
        };
        var d3 = new int[3];
        var d4 = new int[4];
        setI.Add(3);
        setI.Add(5);
        setI.Add(7);
        setI.CopyTo(d3);
        Assert.True(System.Linq.Enumerable.SequenceEqual(e3, d3));
        setI.CopyTo(d4);
        Assert.True(System.Linq.Enumerable.SequenceEqual(e4, d4));
    }

    [Fact]
    public void CrashRs_CopyTo2_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            Setup();
            setI.CopyTo(null, 0);
        });
    }

    [Fact]
    public void CrashRs_CopyTo2_ArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var d2 = new int[2];
            Setup();
            setI.Add(2);
            setI.CopyTo(d2, -1);
        });
    }

    [Fact]
    public void UnitRs_CopyTo2A()
    {
        Setup();
        var e2 = new[]
        {
            3,
            5
        };
        var e4 = new[]
        {
            0,
            3,
            5,
            0
        };
        var d2 = new int[2];
        var d4 = new int[4];
        setI.Add(3);
        setI.Add(5);
        setI.CopyTo(d2, 0);
        Assert.True(System.Linq.Enumerable.SequenceEqual(e2, d2));
        setI.CopyTo(d4, 1);
        Assert.True(System.Linq.Enumerable.SequenceEqual(e4, d4));
    }

    [Fact]
    public void UnitRs_CopyTo2B()
    {
        Setup();
        var i3 = new TS1[3];
        setTS1.Add(new TS1(4));
        setTS1.Add(new TS1(2));
        setTS1.CopyTo(i3, 1, 2);
    }

    [Fact]
    public void CrashRs_CopyTo3_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            Setup();
            setI.CopyTo(null, 0, 0);
        });
    }

    [Fact]
    public void CrashRs_CopyTo3A_ArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            Setup();
            var d2 = new int[2];
            setI.Add(2);
            setI.CopyTo(d2, -1, 0);
        });
    }

    [Fact]
    public void CrashRs_CopyTo3B_ArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            Setup();
            var d2 = new int[2];
            setI.Add(2);
            setI.CopyTo(d2, 0, -1);
        });
    }

    [Fact]
    public void CrashRs_CopyTo3A_Argument()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            Setup();
            var d2 = new int[2];
            setI.Add(3);
            setI.Add(5);
            setI.CopyTo(d2, 1, 2);
        });
    }

    [Fact]
    public void CrashRs_CopyTo3B_Argument()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            Setup();
            var d3 = new int[3];
            setI.Add(2);
            setI.Add(22);
            setI.CopyTo(d3, 1, 3);
        });
    }

    [Fact]
    public void UnitRs_CopyTo3()
    {
        Setup();
        var e2 = new[]
        {
            0,
            0
        };
        var e3 = new[]
        {
            3,
            5,
            7
        };
        var e4 = new[]
        {
            0,
            3,
            5,
            0
        };
        var e5 = new[]
        {
            0,
            3,
            5,
            7,
            0
        };
        var d2 = new int[2];
        var d3 = new int[3];
        var d4 = new int[4];
        var d5 = new int[5];
        setI.Add(3);
        setI.Add(5);
        setI.Add(7);
        setI.CopyTo(d2, 1, 0);
        Assert.True(System.Linq.Enumerable.SequenceEqual(e2, d2));
        setI.CopyTo(d3, 0, 3);
        Assert.True(System.Linq.Enumerable.SequenceEqual(e3, d3));
        setI.CopyTo(d4, 1, 2);
        Assert.True(System.Linq.Enumerable.SequenceEqual(e4, d4));
        setI.CopyTo(d5, 1, 4);
        Assert.True(System.Linq.Enumerable.SequenceEqual(e5, d5));
    }

    [Fact]
    public void CrashRs_ocCopyTo2_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            Setup();
            var oc = (System.Collections.ICollection)setI;
            oc.CopyTo(null, 0);
        });
    }

    [Fact]
    public void CrashRs_ocCopyTo2_ArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            Setup();
            var oc = (System.Collections.ICollection)setI;
            var d1 = new object[1];
            oc.CopyTo(d1, -1);
        });
    }

    [Fact]
    public void CrashRs_ocCopyTo2A_Argument()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            Setup();
            var oc = (System.Collections.ICollection)setI;
            var d2 = new object[2];
            setI.Add(3);
            setI.Add(5);
            oc.CopyTo(d2, 1);
        });
    }

    [Fact]
    public void CrashRs_ocCopyTo2B_Argument()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            Setup();
            var oc = (System.Collections.ICollection)setS;
            var s2 = new string[1, 2];
            oc.CopyTo(s2, 0);
        });
    }

    [Fact]
    public void CrashRs_ocCopyTo2C_Argument()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            Setup();
            var oc = (System.Collections.ICollection)setS;
            var a11 = Array.CreateInstance(typeof(int), new[] { 1 }, new[] { 1 });
            oc.CopyTo(a11, 1);
        });
    }

    [Fact]
    public void CrashRs_ocCopyTo2D_Argument()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            Setup();
            var oc = (System.Collections.ICollection)setI;
            var sa = new string[2];
            // ReSharper disable once CoVariantArrayConversion
            var oa = (object[])sa;
            setI.Add(3);
            setI.Add(5);
            oc.CopyTo(oa, 0);
        });
    }

    [Fact]
    public void CrashRs_ocCopyTo2E_Argument()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            Setup();
            setI.Add(3);
            setI.Add(5);
            var oc = (System.Collections.ICollection)setI;
            var wrongType = new long[2];
            oc.CopyTo(wrongType, 0);
        });
    }

    [Fact]
    public void UnitRs_ocCopyTo2()
    {
        Setup(4);
        var e2 = new object[]
        {
            3,
            5
        };
        var e4 = new object[]
        {
            null,
            3,
            5,
            null
        };
        var e6 = new object[]
        {
            null,
            3,
            5,
            7,
            9,
            11
        };
        var d2 = new object[2];
        var d4 = new object[4];
        var d6 = new object[6];
        var oc = (System.Collections.ICollection)setI;
        setI.Add(3);
        setI.Add(5);
        oc.CopyTo(d2, 0);
        Assert.True(System.Linq.Enumerable.SequenceEqual(e2, d2));
        oc.CopyTo(d4, 1);
        Assert.True(System.Linq.Enumerable.SequenceEqual(e4, d4));
        setI.Add(7);
        setI.Add(9);
        setI.Add(11);
        oc.CopyTo(d6, 1);
        Assert.True(System.Linq.Enumerable.SequenceEqual(e6, d6));
    }

    [Fact]
    public void UnitRs_Remove()
    {
        Setup(4);
#if STRESS
            int n=3200;
#else
        var n = 320;
#endif
        for (var ii = n; ii >= 0; --ii)
            setI.Add(ii);
        var expected = n + 1;
        for (var i2 = n; i2 >= 0; i2 -= 10)
        {
            var isRemoved = setI.Remove(i2);
            --expected;
            Assert.True(isRemoved);
        }

        Assert.Equal(expected, setI.Count);
        var isRemoved2 = setI.Remove(10);
        Assert.False(isRemoved2);
    }

    static int rwCounter = 9;
    static bool IsGe1000(int arg)
    {
        return arg >= 1000;
    }

    static bool IsHotAlways(int arg)
    {
        staticSetI.Add(++rwCounter);
        return true;
    }

#if TEST_BCL
        static SortedSet<int> staticSetI = new SortedSet<int>();
#else
    static RankedSet<int> staticSetI = new RankedSet<int>();
#endif
    [Fact]
    public void CrashRs_RemoveWhereHotPredicate_InvalidOperation()
    {
#if !TEST_BCL
        try
        {
#endif
            Setup(4);
            staticSetI.Add(3);
            staticSetI.Add(4);
            // This does not throw for BCL, but it really should:
            staticSetI.RemoveWhere(IsHotAlways);
#if !TEST_BCL
            Assert.Fail("Expected exception not thrown");
        }
        catch (InvalidOperationException)
        {
            testOutputHelper.WriteLine("OK");
        }
#endif
    }

    [Fact]
    public void CrashRs_RemoveWhereHotEtor_InvalidOperation()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Setup();
            setI.Add(3);
            setI.Add(4);
            foreach (var key in setI)
                setI.RemoveWhere(IsEven);
        });
    }

    [Fact]
    public void UnitRs_RemoveWhereHotNonUpdate()
    {
        Setup();
        setI.Add(3);
        setI.Add(5);
        foreach (var key in setI)
            setI.RemoveWhere(IsEven);
    }

    [Fact]
    public void CrashRs_RemoveWhere_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            Setup();
            setI.RemoveWhere(null);
        });
    }

    [Fact]
    public void UnitRs_RemoveWhere()
    {
        Setup(5);
        for (var ix = 0; ix < 1200; ++ix)
            setI.Add(ix);
        var r1 = setI.RemoveWhere(IsGe1000);
        Assert.Equal(200, r1);
        Assert.Equal(1000, setI.Count);
        var c0 = setI.Count;
        var r2 = setI.RemoveWhere(IsEven);
        Assert.Equal(500, r2);
        foreach (var k2 in setI)
            Assert.True(k2 % 2 != 0);
        var r3 = setI.RemoveWhere(IsAlways);
        Assert.Equal(500, r3);
        Assert.Equal(0, setI.Count);
    }

    #endregion
    #region Test ISet methods
    [Fact]
    public void CrashRs_ExceptWith_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            Setup();
            setI.ExceptWith(null);
        });
    }

    [Fact]
    public void UnitRs_ExceptWith()
    {
        Setup();
        var a37 = new[]
        {
            3,
            7
        };
        var a5 = new[]
        {
            5
        };
        var a133799 = new[]
        {
            1,
            3,
            3,
            7,
            9,
            9
        };
        var empty = new int[]
        {
        };
        setI.ExceptWith(empty);
        Assert.Equal(0, setI.Count);
        setI.ExceptWith(a37);
        Assert.Equal(0, setI.Count);
        setI.Add(3);
        setI.Add(5);
        setI.Add(7);
        setI.ExceptWith(a133799);
        Assert.Equal(1, setI.Count);
        Assert.Equal(5, setI.Min);
        setI.ExceptWith(a5);
        Assert.Equal(0, setI.Count);
    }

    [Fact]
    public void UnitRs_ExceptWithSelf()
    {
        Setup();
        setI.Add(4);
        setI.Add(2);
        setI.ExceptWith(setI);
        Assert.Equal(0, setI.Count);
    }

    [Fact]
    public void CrashRs_IntersectWith_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            Setup();
            setI.IntersectWith(null);
        });
    }

    [Fact]
    public void UnitRs_IntersectWith()
    {
        Setup(4);
        var a1 = new[]
        {
            3,
            5,
            7,
            9,
            11,
            13
        };
        var empty = new int[]
        {
        };
        setI.IntersectWith(empty);
        Assert.Equal(0, setI.Count);
        foreach (var v1 in a1)
            setI.Add(v1);
        setI.IntersectWith(new[] { 1 });
        Assert.Equal(0, setI.Count);
        setI.Clear();
        foreach (var v1 in a1)
            setI.Add(v1);
        setI.IntersectWith(new[] { 15 });
        Assert.Equal(0, setI.Count);
        setI.Clear();
        foreach (var v1 in a1)
            setI.Add(v1);
        setI.IntersectWith(new[] { 1, 9, 15 });
        Assert.Equal(1, setI.Count);
        Assert.Equal(9, setI.Min);
        setI.IntersectWith(empty);
        Assert.Equal(0, setI.Count);
    }

    [Fact]
    public void CrashRs_SymmetricExceptWith_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            Setup();
            setI.SymmetricExceptWith(null);
        });
    }

    [Fact]
    public void UnitRs_SymmetricExceptWith()
    {
        Setup(4);
        setI.SymmetricExceptWith(new int[] { });
        Assert.Equal(0, setI.Count);
        setI.SymmetricExceptWith(new[] { 4, 5, 6 });
        Assert.True(System.Linq.Enumerable.SequenceEqual(new[] { 4, 5, 6 }, setI));
        setI.SymmetricExceptWith(new[] { 5, 7, 8 });
        Assert.True(System.Linq.Enumerable.SequenceEqual(new[] { 4, 6, 7, 8 }, setI));
        setI.SymmetricExceptWith(new[] { 1, 2, 7, 8 });
        Assert.True(System.Linq.Enumerable.SequenceEqual(new[] { 1, 2, 4, 6 }, setI));
        setI.SymmetricExceptWith(new[] { 2, 3 });
        Assert.True(System.Linq.Enumerable.SequenceEqual(new[] { 1, 3, 4, 6 }, setI));
    }

    [Fact]
    public void CrashRs_UnionWith_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            Setup();
            setI.UnionWith(null);
        });
    }

    [Fact]
    public void UnitRs_UnionWith()
    {
        Setup();
        var a357 = new[]
        {
            3,
            5,
            7
        };
        var a5599 = new[]
        {
            5,
            5,
            9,
            9
        };
        var empty = new int[]
        {
        };
        setI.UnionWith(empty);
        Assert.Equal(0, setI.Count);
        setI.UnionWith(a357);
        Assert.Equal(3, setI.Count);
        setI.UnionWith(a5599);
        Assert.Equal(4, setI.Count);
    }

    [Fact]
    public void CrashRs_IsSubsetOf_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            Setup();
            var result = setI.IsSubsetOf(null);
        });
    }

    [Fact]
    public void UnitRs_IsSubsetOf()
    {
        Setup();
        var a35779 = new[]
        {
            3,
            5,
            7,
            7,
            9
        };
        var a357 = new[]
        {
            3,
            5,
            7
        };
        var a35 = new[]
        {
            3,
            5
        };
        var empty = new int[]
        {
        };
        Assert.True(setI.IsSubsetOf(a35));
        Assert.True(setI.IsSubsetOf(empty));
        setI.Add(3);
        setI.Add(5);
        setI.Add(7);
        Assert.True(setI.IsSubsetOf(a35779));
        Assert.True(setI.IsSubsetOf(a357));
        Assert.False(setI.IsSubsetOf(a35));
        Assert.False(setI.IsSubsetOf(empty));
        Assert.False(setI.IsSubsetOf(new[] { 3, 5, 8 }));
    }

    [Fact]
    public void CrashRs_IsProperSubsetOf_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            Setup();
            var result = setI.IsProperSubsetOf(null);
        });
    }

    [Fact]
    public void UnitRs_IsProperSubsetOf()
    {
        Setup();
        var a35779 = new[]
        {
            3,
            5,
            7,
            7,
            9
        };
        var a357 = new[]
        {
            3,
            5,
            7
        };
        var a35 = new[]
        {
            3,
            5
        };
        var empty = new int[]
        {
        };
        Assert.True(setI.IsProperSubsetOf(a35));
        Assert.False(setI.IsProperSubsetOf(empty));
        setI.Add(3);
        setI.Add(5);
        setI.Add(7);
        Assert.True(setI.IsProperSubsetOf(a35779));
        Assert.False(setI.IsProperSubsetOf(a357));
        Assert.False(setI.IsProperSubsetOf(a35));
        Assert.False(setI.IsProperSubsetOf(new[] { 1, 2, 3, 4 }));
        Assert.False(setI.IsProperSubsetOf(empty));
    }

    [Fact]
    public void CrashRs_IsSupersetOf_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            Setup();
            var result = setI.IsSupersetOf(null);
        });
    }

    [Fact]
    public void UnitRs_IsSupersetOf()
    {
        Setup();
        var a3579 = new[]
        {
            3,
            5,
            7,
            9
        };
        var a357 = new[]
        {
            3,
            5,
            7
        };
        var a35 = new[]
        {
            3,
            5
        };
        var a355 = new[]
        {
            3,
            5,
            5
        };
        var empty = new int[]
        {
        };
        Assert.True(setI.IsSupersetOf(empty));
        Assert.False(setI.IsSupersetOf(a35));
        setI.Add(3);
        setI.Add(5);
        setI.Add(7);
        Assert.False(setI.IsSupersetOf(a3579));
        Assert.True(setI.IsSupersetOf(a357));
        Assert.True(setI.IsSupersetOf(a35));
        Assert.True(setI.IsSupersetOf(a355));
        Assert.True(setI.IsSupersetOf(empty));
    }

    [Fact]
    public void CrashRs_IsProperSupersetOf_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            Setup();
            var result = setI.IsProperSupersetOf(null);
        });
    }

    [Fact]
    public void UnitRs_IsProperSupersetOf()
    {
        Setup();
        var a3579 = new[]
        {
            3,
            5,
            7,
            9
        };
        var a357 = new[]
        {
            3,
            5,
            7
        };
        var a35 = new[]
        {
            3,
            5
        };
        var a355 = new[]
        {
            3,
            5,
            5
        };
        var empty = new int[]
        {
        };
        Assert.False(setI.IsProperSupersetOf(empty));
        Assert.False(setI.IsProperSupersetOf(a35));
        setI.Add(3);
        setI.Add(5);
        setI.Add(7);
        Assert.False(setI.IsProperSupersetOf(a3579));
        Assert.False(setI.IsProperSupersetOf(a357));
        Assert.True(setI.IsProperSupersetOf(a35));
        Assert.True(setI.IsProperSupersetOf(a355));
        Assert.True(setI.IsProperSupersetOf(empty));
        Assert.False(setI.IsProperSupersetOf(new[] { 2, 4 }));
    }

    [Fact]
    public void CrashRs_Overlaps_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            Setup();
            var result = setI.Overlaps(null);
        });
    }

    [Fact]
    public void UnitRs_OverlapsSet()
    {
        Setup();
        setI.Add(3);
        setI.Add(5);
        setI.Add(7);
        Assert.True(setI.Overlaps(setI));
#if TEST_BCL
            var set1 = new SortedSet<int> { 5, 6 };
            var set2 = new SortedSet<int> { 1, 8 };
            var set3 = new SortedSet<int> { 4 };
#else
        var set1 = new RankedSet<int>
        {
            5,
            6
        };
        var set2 = new RankedSet<int>
        {
            1,
            8
        };
        var set3 = new RankedSet<int>
        {
            4
        };
#endif
        Assert.True(setI.Overlaps(set1));
        Assert.False(setI.Overlaps(set2));
        Assert.False(setI.Overlaps(set3));
    }

    [Fact]
    public void UnitRs_OverlapsArray()
    {
        Setup();
        var a35779 = new[]
        {
            3,
            5,
            7,
            7,
            9
        };
        var a357 = new[]
        {
            3,
            5,
            7
        };
        var a35 = new[]
        {
            3,
            5
        };
        var a355 = new[]
        {
            3,
            5,
            5
        };
        var a19 = new[]
        {
            1,
            9
        };
        var empty = new int[]
        {
        };
        Assert.False(setI.Overlaps(empty));
        Assert.False(setI.Overlaps(a35));
        setI.Add(3);
        setI.Add(5);
        setI.Add(7);
        Assert.True(setI.Overlaps(a35779));
        Assert.True(setI.Overlaps(a357));
        Assert.True(setI.Overlaps(a35));
        Assert.False(setI.Overlaps(a19));
        Assert.False(setI.Overlaps(empty));
    }

    [Fact]
    public void CrashRs_SetEquals_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            Setup();
            var result = setI.SetEquals(null);
        });
    }

    [Fact]
    public void UnitRs_SetEquals()
    {
        Setup();
        var a359 = new[]
        {
            3,
            5,
            9
        };
        var a3557 = new[]
        {
            3,
            5,
            5,
            7
        };
        var a357 = new[]
        {
            3,
            5,
            7
        };
        var a35 = new[]
        {
            3,
            5
        };
        var a355 = new[]
        {
            3,
            5,
            5
        };
        var a19 = new[]
        {
            1,
            9
        };
        var empty = new int[]
        {
        };
        Assert.True(setI.SetEquals(empty));
        Assert.False(setI.SetEquals(a35));
        setI.Add(3);
        setI.Add(5);
        setI.Add(7);
        Assert.True(setI.SetEquals(a3557));
        Assert.True(setI.SetEquals(a357));
        Assert.False(setI.SetEquals(a359));
        Assert.False(setI.SetEquals(a35));
        Assert.False(setI.SetEquals(a355));
        Assert.False(setI.SetEquals(a19));
        Assert.False(setI.SetEquals(empty));
    }

    [Fact]
    public void UnitRs_SetEquals2()
    {
        Setup();
        personSet.Add(new Person("Fred"));
        personSet.Add(new Person("Wilma"));
        var pa = new[]
        {
            new Person("Wilma"),
            new Person("Fred")
        };
        Assert.True(personSet.SetEquals(pa));
        personSet.Add(new Person("Pebbles"));
        Assert.False(personSet.SetEquals(pa));
    }

    #endregion
    #region Test bonus methods
#if !TEST_BCL
    [Fact]
    public void CrashRsx_ElementsBetweenHotUpdate()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Setup(4);
            for (var ix = 0; ix < 10; ++ix)
                setI.Add(ix);
            var n = 0;
            foreach (var key in setI.ElementsBetween(3, 8))
            {
                if (++n == 2)
                    setI.Add(49);
            }
        });
    }

    [Fact]
    public void UnitRsx_ElementsBetween()
    {
        Setup(4);
        for (var ix = 0; ix < 20; ++ix)
            setI.Add(ix);
        var expected1 = 5;
        foreach (var key in setI.ElementsBetween(5, 15))
        {
            Assert.Equal(expected1, key);
            ++expected1;
        }

        Assert.Equal(expected1, 16);
        var expected2 = 15;
        foreach (var key in setI.ElementsBetween(15, 25))
        {
            Assert.Equal(expected2, key);
            ++expected2;
        }

        Assert.Equal(expected2, 20);
    }

    [Fact]
    public void UnitRsx_ElementsFromNull()
    {
        Setup(4);
        foreach (var px in personSet.ElementsFrom(null))
        {
        }
    }

    [Fact]
    public void UnitRsx_ElementsFrom()
    {
        Setup(4);
        for (var ii = 0; ii < 30; ++ii)
            setI.Add(ii);
        var ix = 20;
        foreach (var kx in setI.ElementsFrom(ix))
        {
            Assert.Equal(ix, kx);
            ++ix;
        }

        Assert.Equal(30, ix);
    }

    [Fact]
    public void CrashRsx_ElementsBetweenIndexesA_ArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var set = new RankedSet<int>
            {
                0,
                1,
                2
            };
            foreach (var val in set.ElementsBetweenIndexes(-1, 0))
            {
            }
        });
    }

    [Fact]
    public void CrashRsx_ElementsBetweenIndexesB_ArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var set = new RankedSet<int>
            {
                0,
                1,
                2
            };
            foreach (var val in set.ElementsBetweenIndexes(3, 0))
            {
            }
        });
    }

    [Fact]
    public void CrashRsx_ElementsBetweenIndexesC_ArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var set = new RankedSet<int>
            {
                0,
                1,
                2
            };
            foreach (var val in set.ElementsBetweenIndexes(0, -1))
            {
            }
        });
    }

    [Fact]
    public void CrashRsx_ElementsBetweenIndexesD_ArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var set = new RankedSet<int>
            {
                0,
                1,
                2
            };
            foreach (var val in set.ElementsBetweenIndexes(0, 3))
            {
            }
        });
    }

    [Fact]
    public void CrashRsx_ElementsBetweenIndexes_Argument()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var set = new RankedSet<int>
            {
                0,
                1,
                2
            };
            foreach (var val in set.ElementsBetweenIndexes(2, 1))
            {
            }
        });
    }

    [Fact]
    public void UnitRsx_ElementsBetweenIndexes()
    {
        var n = 33;
        var set = new RankedSet<int>
        {
            Capacity = 4
        };
        for (var ii = 0; ii < n; ++ii)
            set.Add(ii);
        for (var p1 = 0; p1 < n; ++p1)
        for (var p2 = p1; p2 < n; ++p2)
        {
            var actual = 0;
            foreach (var val in set.ElementsBetweenIndexes(p1, p2))
                actual += val;
            var expected = (p2 - p1 + 1) * (p1 + p2) / 2;
            Assert.Equal(expected, actual);
        }
    }

    [Fact]
    public void UnitRsx_IndexOf()
    {
        Setup(4);
        for (var ii = 0; ii <= 98; ii += 2)
            setI.Add(ii);
        var iz = setI.IndexOf(-1);
        var i0 = setI.IndexOf(0);
        var i8 = setI.IndexOf(8);
        var i98 = setI.IndexOf(98);
        var i100 = setI.IndexOf(100);
        Assert.Equal(~0, iz);
        Assert.Equal(0, i0);
        Assert.Equal(4, i8);
        Assert.Equal(49, i98);
        Assert.Equal(~50, i100);
    }

    [Fact]
    public void CrashRsx_RemoveAtA_ArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var rs = new RankedSet<int>
            {
                42
            };
            rs.RemoveAt(-1);
        });
    }

    [Fact]
    public void CrashRsx_RemoveAtB_ArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var rs = new RankedSet<int>();
            rs.RemoveAt(0);
        });
    }

    [Fact]
    public void UnitRsx_Replace1()
    {
        var rs = new RankedSet<string>(StringComparer.InvariantCultureIgnoreCase)
        {
            "aa",
            "BB",
            "cc"
        };
        var r1 = rs.Replace("bb");
        Assert.True(r1);
        var r2 = rs.Replace("dd");
        Assert.False(r2);
        var r3 = rs.Replace("AA");
        Assert.True(r3);
        Assert.True(System.Linq.Enumerable.SequenceEqual(rs, new[] { "AA", "bb", "cc" }));
    }

    [Fact]
    public void UnitRsx_Replace2()
    {
        var rs = new RankedSet<string>(StringComparer.InvariantCultureIgnoreCase)
        {
            "aa",
            "BB",
            "cc"
        };
        var r1 = rs.Replace("AA", true);
        Assert.True(r1);
        var r2 = rs.Replace("bb", false);
        Assert.True(r2);
        var r3 = rs.Replace("dd", true);
        Assert.False(r3);
        var r4 = rs.Replace("ee", false);
        Assert.False(r4);
        Assert.True(System.Linq.Enumerable.SequenceEqual(rs, new[] { "AA", "bb", "cc", "dd" }));
    }

    [Fact]
    public void UnitRsx_TryGet()
    {
        var rs = new RankedSet<string>(StringComparer.InvariantCultureIgnoreCase)
        {
            "AAA",
            "bbb",
            "ccc"
        };
        var got1 = rs.TryGet("aaa", out var actual1);
        Assert.True(got1);
        Assert.Equal("AAA", actual1);
        var got2 = rs.TryGet("bb", out var actual2);
        Assert.False(got2);
        var got3 = rs.TryGet("CCC", out var actual3);
        Assert.True(got3);
        Assert.Equal("ccc", actual3);
    }

    [Fact]
    public void UnitRsx_TryGetLELT()
    {
        Setup(4);
        var n = 25;
        for (var ii = 1; ii <= n; ii += 2)
            setI.Add(ii);
        var r0a = setS.TryGetLessThanOrEqual("AA", out var k0a);
        var r0b = setS.TryGetLessThan("BB", out var k0b);
        Assert.False(r0a);
        Assert.Equal(default(string), k0a);
        Assert.False(r0b);
        Assert.Equal(default(string), k0b);
        for (var i1 = 3; i1 <= n; i1 += 2)
        {
            var r1a = setI.TryGetLessThanOrEqual(i1, out var k1a);
            var r1b = setI.TryGetLessThan(i1, out var k1b);
            Assert.True(r1a);
            Assert.Equal(i1, k1a);
            Assert.True(r1b);
            Assert.Equal(i1 - 2, k1b);
        }

        for (var i2 = 2; i2 <= n + 1; i2 += 2)
        {
            var r2a = setI.TryGetLessThanOrEqual(i2, out var k2a);
            var r2b = setI.TryGetLessThan(i2, out var k2b);
            Assert.True(r2a);
            Assert.True(r2b);
            Assert.Equal(i2 - 1, k2a);
            Assert.Equal(i2 - 1, k2b);
        }

        var r3a = setI.TryGetLessThanOrEqual(1, out var k3a);
        Assert.True(r3a);
        Assert.Equal(1, k3a);
        var r3b = setI.TryGetLessThan(1, out var k3b);
        Assert.False(r3b);
        Assert.Equal(default(int), k3b);
        var r4a = setI.TryGetLessThanOrEqual(0, out var k4a);
        Assert.False(r4a);
        Assert.Equal(default(int), k4a);
        var r4b = setI.TryGetLessThan(0, out var k4b);
        Assert.False(r4b);
        Assert.Equal(default(int), k4b);
    }

    [Fact]
    public void UnitRsx_TryGetGEGT()
    {
        Setup(4);
        var n = 99;
        for (var ii = 1; ii <= n; ii += 2)
            setI.Add(ii);
        var r0a = setS.TryGetGreaterThanOrEqual("AA", out var k0a);
        var r0b = setS.TryGetGreaterThan("BB", out var k0b);
        Assert.False(r0a);
        Assert.Equal(default(string), k0a);
        Assert.False(r0b);
        Assert.Equal(default(string), k0b);
        for (var i1 = 1; i1 < n; i1 += 2)
        {
            var r1a = setI.TryGetGreaterThanOrEqual(i1, out var k1a);
            var r1b = setI.TryGetGreaterThan(i1, out var k1b);
            Assert.True(r1a);
            Assert.Equal(i1, k1a);
            Assert.True(r1b);
            Assert.Equal(i1 + 2, k1b);
        }

        for (var i2 = 0; i2 < n; i2 += 2)
        {
            var r2a = setI.TryGetGreaterThanOrEqual(i2, out var k2a);
            var r2b = setI.TryGetGreaterThan(i2, out var k2b);
            Assert.True(r2a);
            Assert.True(r2b);
            Assert.Equal(i2 + 1, k2a);
            Assert.Equal(i2 + 1, k2b);
        }

        var r3a = setI.TryGetGreaterThanOrEqual(n, out var k3a);
        Assert.True(r3a);
        Assert.Equal(n, k3a);
        var r3b = setI.TryGetGreaterThan(n, out var k3b);
        Assert.False(r3b);
        Assert.Equal(default(int), k3b);
        var r4a = setI.TryGetGreaterThanOrEqual(n + 1, out var k4a);
        Assert.False(r4a);
        Assert.Equal(default(int), k4a);
        var r4b = setI.TryGetGreaterThan(n + 1, out var k4b);
        Assert.False(r4b);
        Assert.Equal(default(int), k4b);
    }

    [Fact]
    public void UnitRsx_RemoveAt()
    {
        var rs = new RankedSet<int>
        {
            Capacity = 5
        };
#if STRESS
            int n = 5000, m = 10;
#else
        int n = 40, m = 5;
#endif
        for (var ii = 0; ii < n; ++ii)
            rs.Add(ii);
        for (var i2 = n - m; i2 >= 0; i2 -= m)
            rs.RemoveAt(i2);
        for (var i2 = 0; i2 < n; ++i2)
            if (i2 % m == 0)
                Assert.False(rs.Contains(i2));
            else
                Assert.True(rs.Contains(i2));
    }

    [Fact]
    public void CrashRsx_RemoveRangeA_ArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var rs = new RankedSet<int>();
            rs.RemoveRange(-1, 0);
        });
    }

    [Fact]
    public void CrashRsx_RemoveRangeB_ArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var rs = new RankedSet<int>();
            rs.RemoveRange(0, -1);
        });
    }

    [Fact]
    public void CrashRsx_RemoveRange_Argument()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var rs = new RankedSet<int>
            {
                3,
                5
            };
            rs.RemoveRange(1, 2);
        });
    }

    [Fact]
    public void UnitRsx_RemoveRange()
    {
        var set0 = new RankedSet<int>
        {
            Capacity = 7
        };
        for (var ii = 0; ii < 9; ++ii)
            set0.Add(ii);
        var set1 = new RankedSet<int>
        {
            Capacity = 4
        };
        for (var ii = 0; ii < 13; ++ii)
            set1.Add(ii);
        var set2 = new RankedSet<int>
        {
            Capacity = 4
        };
        for (var ii = 0; ii < 19; ++ii)
            set2.Add(ii);
        var set3 = new RankedSet<int>
        {
            Capacity = 4
        };
        for (var ii = 0; ii < 22; ++ii)
            set3.Add(ii);
        var set4 = new RankedSet<int>
        {
            Capacity = 7
        };
        for (var ii = 0; ii < 7; ++ii)
            set4.Add(ii);
        var set5 = new RankedSet<int>
        {
            Capacity = 7
        };
        for (var ii = 0; ii < 8; ++ii)
            set5.Add(ii);
        var set6 = new RankedSet<int>
        {
            Capacity = 7
        };
        for (var ii = 0; ii < 13; ++ii)
            set6.Add(ii);
        var set7 = new RankedSet<int>
        {
            Capacity = 7
        };
        for (var ii = 0; ii < 13; ++ii)
            set7.Add(ii);
        var set8 = new RankedSet<int>
        {
            Capacity = 5
        };
        for (var ii = 0; ii < 21; ++ii)
            set8.Add(ii);
        var set9 = new RankedSet<int>
        {
            Capacity = 7
        };
        for (var ii = 0; ii < 7; ++ii)
            set9.Add(ii);
        var setA = new RankedSet<int>
        {
            Capacity = 6
        };
        for (var ii = 0; ii < 31; ++ii)
            setA.Add(ii);
        var setB = new RankedSet<int>
        {
            Capacity = 6
        };
        for (var ii = 0; ii < 56; ++ii)
            setB.Add(ii);
        var setU = new RankedSet<int>
        {
            Capacity = 5
        };
        for (var ii = 0; ii < 21; ++ii)
            setU.Add(ii);
        var setV = new RankedSet<int>
        {
            Capacity = 5
        };
        for (var ii = 0; ii < 21; ++ii)
            setV.Add(ii);
        var setW = new RankedSet<int>
        {
            Capacity = 5
        };
        for (var ii = 0; ii < 21; ++ii)
            setW.Add(ii);
        var setX = new RankedSet<int>
        {
            Capacity = 6
        };
        for (var ii = 0; ii < 14; ++ii)
            setX.Add(ii);
        var setY = new RankedSet<int>
        {
            Capacity = 7
        };
        for (var ii = 0; ii < 500; ++ii)
            setY.Add(ii);
        var setZ = new RankedSet<int>();
        set0.RemoveRange(0, 2);
        Assert.Equal(7, set0.Count);
        set1.RemoveRange(10, 2);
        Assert.Equal(11, set1.Count);
        set2.RemoveRange(0, 6);
        Assert.Equal(13, set2.Count);
        set3.RemoveRange(9, 6);
        Assert.Equal(16, set3.Count);
        set4.RemoveRange(6, 1);
        Assert.Equal(6, set4.Count);
        set5.RemoveRange(5, 2);
        Assert.Equal(6, set5.Count);
        set6.RemoveRange(6, 4);
        Assert.Equal(9, set6.Count);
        set7.RemoveRange(1, 6);
        Assert.Equal(7, set7.Count);
        set8.RemoveRange(12, 4);
        Assert.Equal(17, set8.Count);
        set9.RemoveRange(1, 5);
        Assert.Equal(2, set9.Count);
        setA.RemoveRange(5, 23);
        Assert.Equal(8, setA.Count);
        setB.RemoveRange(5, 40);
        Assert.Equal(16, setB.Count);
        setU.RemoveRange(16, 3);
        Assert.Equal(18, setU.Count);
        setV.RemoveRange(16, 5);
        Assert.Equal(16, setV.Count);
        setW.RemoveRange(0, 16);
        Assert.Equal(5, setW.Count);
        setX.RemoveRange(1, 8);
        Assert.Equal(6, setX.Count);
        setY.RemoveRange(0, 500);
        Assert.Equal(0, setY.Count);
        setZ.RemoveRange(0, 0);
        Assert.Equal(0, setZ.Count);
#if DEBUG
        set0.SanityCheck();
        set1.SanityCheck();
        set2.SanityCheck();
        set3.SanityCheck();
        set4.SanityCheck();
        set5.SanityCheck();
        set6.SanityCheck();
        set7.SanityCheck();
        set8.SanityCheck();
        set9.SanityCheck();
        setA.SanityCheck();
        setB.SanityCheck();
        setU.SanityCheck();
        setV.SanityCheck();
        setW.SanityCheck();
        setX.SanityCheck();
        setY.SanityCheck();
        setZ.SanityCheck();
#endif
    }

    [Fact]
    public void StressRsx_RemoveRange()
    {
#if STRESS
            int n = 19;
#else
        var n = 11;
#endif
        for (var width = 1; width <= n; ++width)
        {
            for (var count = 0; count <= width; ++count)
            for (var index = 0; index <= width - count; ++index)
            {
                var set = new RankedSet<int>
                {
                    Capacity = 6
                };
                for (var ii = 0; ii < width; ++ii)
                    set.Add(ii);
                set.RemoveRange(index, count);
                Assert.Equal(width - count, set.Count);
#if DEBUG
                set.SanityCheck();
#endif
            }
        }
    }

#endif
    #endregion
    #region Test enumeration
    [Fact]
    public void CrashRs_EtorOverflow_InvalidOperation()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Setup(4);
            for (var ix = 0; ix < 10; ++ix)
                setI.Add(ix);
            var etor = setI.GetEnumerator();
            while (etor.MoveNext())
            {
            }

            var val = ((System.Collections.IEnumerator)etor).Current;
        });
    }

    [Fact]
    public void UnitRs_ocGetEnumerator()
    {
        Setup();
        var oc = ((System.Collections.Generic.ICollection<int>)setI);
        setI.Add(5);
        var xEtor = oc.GetEnumerator();
        xEtor.MoveNext();
        Assert.Equal(5, xEtor.Current);
    }

    [Fact]
    public void UnitRs_EtorOverflowNoCrash()
    {
        Setup(4);
        for (var ix = 0; ix < 10; ++ix)
            setI.Add(ix);
        var etor = setI.GetEnumerator();
        while (etor.MoveNext())
        {
        }

        var val = etor.Current;
        Assert.Equal(default(int), val);
    }

    [Fact]
    public void UnitRs_GetEnumerator()
    {
        int e1 = 0, e2 = 0;
        Setup(4);
        for (var ix = 0; ix < 10; ++ix)
            setI.Add(ix);
        var etor = setI.GetEnumerator();
        while (etor.MoveNext())
        {
            var gActual = etor.Current;
            var oActual = ((System.Collections.IEnumerator)etor).Current;
            Assert.Equal(e1, gActual);
            Assert.Equal(e1, oActual);
            ++e1;
        }

        Assert.Equal(10, e1);
        var gActualEnd = etor.Current;
        Assert.Equal(default(int), gActualEnd);
        var isValid = etor.MoveNext();
        Assert.False(isValid);
        ((System.Collections.IEnumerator)etor).Reset();
        while (etor.MoveNext())
        {
            var val = etor.Current;
            Assert.Equal(e2, val);
            ++e2;
        }

        Assert.Equal(10, e2);
    }

    [Fact]
    public void CrashRs_EtorHotUpdate()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Setup(4);
            for (var ix = 0; ix < 10; ++ix)
                setI.Add(ix);
            var n = 0;
            foreach (var kv in setI)
            {
                if (++n == 2)
                    setI.Add(49);
            }
        });
    }

    [Fact]
    public void UnitRs_ocCurrent_HotUpdate()
    {
        Setup();
        setI.Add(1);
        System.Collections.ICollection oc = setI;
        var etor = oc.GetEnumerator();
        var ok = etor.MoveNext();
        Assert.True(ok);
        Assert.Equal(1, etor.Current);
        setI.Clear();
        Assert.Equal(1, etor.Current);
    }

    [Fact]
    public void UnitRs_EtorCurrentHotUpdate()
    {
        Setup();
        setI.Add(1);
        var etor1 = setI.GetEnumerator();
        Assert.Equal(default(int), etor1.Current);
        var ok1 = etor1.MoveNext();
        Assert.True(ok1);
        Assert.Equal(1, etor1.Current);
        setI.Remove(1);
        Assert.Equal(1, etor1.Current);
        setS.Add("AA");
        var etor2 = setS.GetEnumerator();
        Assert.Equal(default(string), etor2.Current);
        var ok2 = etor2.MoveNext();
        Assert.Equal("AA", etor2.Current);
        setS.Clear();
        Assert.Equal("AA", etor2.Current);
    }

    [Fact]
    public void UnitRs_oReset()
    {
        Setup(4);
        var ia = new[]
        {
            1,
            2,
            5,
            8,
            9
        };
        foreach (var x in ia)
            setI.Add(x);
        var etor = setI.GetEnumerator();
        var ix1 = 0;
        while (etor.MoveNext())
        {
            Assert.Equal(ia[ix1], etor.Current);
            ++ix1;
        }

        Assert.Equal(ia.Length, ix1);
        ((System.Collections.IEnumerator)etor).Reset();
        var ix2 = 0;
        while (etor.MoveNext())
        {
            Assert.Equal(ia[ix2], etor.Current);
            ++ix2;
        }

        Assert.Equal(ia.Length, ix2);
    }
    #endregion
}