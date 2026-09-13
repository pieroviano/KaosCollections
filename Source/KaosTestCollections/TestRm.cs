//
// Library: KaosCollections
// File:    TestRm.cs
//

#if !TEST_BCL
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using Kaos.Collections;
using Xunit.Abstractions;

namespace Kaos.Test.Collections;

public partial class TestRm : TestBtree, IClassFixture<BinaryFormatterEnableFixture>
{
    public TestRm(ITestOutputHelper output)
    {
        _output = output;
    }

    private readonly ITestOutputHelper _output;
    #region Test constructors
    [Fact]
    public void UnitRm_Inheritance()
    {
        var rm = new RankedMap<string, int>();
        Assert.True(rm is System.Collections.Generic.ICollection<KeyValuePair<string, int>>);
        Assert.True(rm is System.Collections.Generic.IEnumerable<KeyValuePair<string, int>>);
        Assert.True(rm is System.Collections.IEnumerable);
        Assert.True(rm is System.Collections.ICollection);
        Assert.True(rm is System.Collections.Generic.IReadOnlyCollection<KeyValuePair<string, int>>);
        Assert.True(rm.Keys is System.Collections.Generic.ICollection<string>);
        Assert.True(rm.Keys is System.Collections.Generic.IEnumerable<string>);
        Assert.True(rm.Keys is System.Collections.IEnumerable);
        Assert.True(rm.Keys is System.Collections.ICollection);
        Assert.True(rm.Keys is System.Collections.Generic.IReadOnlyCollection<string>);
        Assert.True(rm.Values is System.Collections.Generic.ICollection<int>);
        Assert.True(rm.Values is System.Collections.Generic.IEnumerable<int>);
        Assert.True(rm.Values is System.Collections.IEnumerable);
        Assert.True(rm.Values is System.Collections.ICollection);
        Assert.True(rm.Values is System.Collections.Generic.IReadOnlyCollection<int>);
    }

    [Fact]
    public void UnitRm_Ctor0Empty()
    {
        var rm = new RankedMap<string, int>();
        Assert.Equal(0, rm.Count);
    }

    [Fact]
    public void CrashRm_Ctor1NoComparer_InvalidOperation()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var comp0 = (Comparer<Person>)null;
            var rm = new RankedMap<Person, int>(comp0)
            {
                { new Person("Carlos"), 1 },
                { new Person("Macron"), 2 }
            };
        });
    }

    [Fact]
    public void UnitRm_Ctor1A1()
    {
        var rm = new RankedMap<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "AAA", 0 },
            { "bbb", 1 },
            { "CCC", 2 },
            { "ddd", 3 }
        };
        var actualPosition = 0;
        foreach (var pair in rm)
        {
            Assert.Equal(actualPosition, pair.Value);
            ++actualPosition;
        }

        Assert.Equal(4, actualPosition);
    }

    [Fact]
    public void UnitRm_Ctor1A2()
    {
        var rm = new RankedMap<string, int>(StringComparer.Ordinal)
        {
            { "AAA", 0 },
            { "bbb", 2 },
            { "CCC", 1 },
            { "ddd", 3 }
        };
        var actualPosition = 0;
        foreach (var kv in rm)
        {
            Assert.Equal(actualPosition, kv.Value);
            ++actualPosition;
        }

        Assert.Equal(4, actualPosition);
    }

    [Fact]
    public void CrashRm_Ctor1B_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            ICollection<KeyValuePair<int, int>> gcp1 = null;
            ICollection<KeyValuePair<int, int>> gcp2 = new RankedMap<int, int>(gcp1);
        });
    }

    [Fact]
    public void UnitRm_Ctor1B()
    {
        var gsl = new SortedList<string, int>
        {
            {
                "Gremlin",
                1
            },
            {
                "Pacer",
                2
            }
        };
        var gcp = (ICollection<KeyValuePair<string, int>>)gsl;
        var rm = new RankedMap<string, int>(gcp);
        var actual = 0;
        foreach (var kv in rm)
        {
            Assert.Equal(gsl.Keys[actual], kv.Key);
            Assert.Equal(gsl.Values[actual], kv.Value);
            ++actual;
        }

        Assert.Equal(2, actual);
    }

    [Fact]
    public void UnitRm_Ctor2()
    {
        ICollection<KeyValuePair<Person, int>> cpl = new SortedList<Person, int>(new PersonComparer());
        cpl.Add(new KeyValuePair<Person, int>(new Person("fay"), 1));
        cpl.Add(new KeyValuePair<Person, int>(new Person("ann"), 2));
        cpl.Add(new KeyValuePair<Person, int>(new Person("sam"), 3));
        var people = new RankedMap<Person, int>(cpl, new PersonComparer());
        Assert.Equal(3, people.Count);
    }

    #endregion
    #region Test properties
    [Fact]
    public void UnitRm_Comparer()
    {
        var rm = new RankedMap<string, int>();
        var comp = rm.Comparer;
        Assert.Equal(Comparer<string>.Default, comp);
    }

    [Fact]
    public void UnitRm_gcIsReadonly()
    {
        var rm = new RankedMap<string, int>();
        var gcp = (ICollection<KeyValuePair<string, int>>)rm;
        Assert.False(gcp.IsReadOnly);
    }

    [Fact]
    public void UnitRm_ocIsSynchronized()
    {
        var rm = new RankedMap<string, int>();
        var oc = (ICollection)rm;
        Assert.False(oc.IsSynchronized);
    }

    [Fact]
    public void UnitRm_MinMax()
    {
        var rm = new RankedMap<int, int>
        {
            Capacity = 4
        };
        var min0 = rm.MinKey;
        var max0 = rm.MaxKey;
        Assert.Equal(default(int), min0);
        Assert.Equal(default(int), max0);
        for (var i1 = 1; i1 <= 99; ++i1)
            rm.Add(i1, i1 + 100);
        var min = rm.MinKey;
        var max = rm.MaxKey;
        Assert.Equal(1, min);
        Assert.Equal(99, max);
    }

    [Fact]
    public void UnitRm_ocSyncRoot()
    {
        var rm = new RankedMap<int, int>();
        var oc = (ICollection)rm;
        Assert.False(oc.SyncRoot.GetType().IsValueType);
    }

    #endregion
    #region Test methods
    [Fact]
    public void CrashRm_Add_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var rm = new RankedMap<string, int> { { null, 0 } };
        });
    }

    [Fact]
    public void UnitRm_Add()
    {
        var rm = new RankedMap<string, int>
        {
            {
                "foo",
                1
            },
            {
                "foo",
                2
            },
            {
                "zax",
                3
            }
        };
        Assert.Equal(3, rm.Count);
    }

    [Fact]
    public void UnitRm_pcAddPair()
    {
        var rm = new RankedMap<string, int>();
        var pc = (ICollection<KeyValuePair<string, int>>)rm;
        var p1 = new KeyValuePair<string, int>("beta", 1);
        var p2 = new KeyValuePair<string, int>(null, 98);
        var p3 = new KeyValuePair<string, int>(null, 99);
        pc.Add(p1);
        Assert.True(rm.ContainsKey("beta"));
        // Adding a null key is allowed here to be consistent with SortedDictionary.
        pc.Add(p2);
        Assert.Equal(2, rm.Count);
        // Using the ICollection interface allows multiple nulls.
        pc.Add(p3);
        Assert.Equal(3, rm.Count);
    }

    [Fact]
    public void UnitRm_Clear()
    {
        var rm = new RankedMap<string, int>
        {
            Capacity = 4
        };
        rm.Add("foo", 1);
        rm.Add("foo", 2);
        rm.Add("zax", 3);
        rm.Add("zax", 4);
        rm.Add("fod", 5);
        Assert.Equal(5, rm.Count);
        rm.Clear();
        Assert.Equal(0, rm.Count);
        var actualCount = 0;
        foreach (var pair in rm)
            ++actualCount;
        Assert.Equal(0, actualCount);
    }

    [Fact]
    public void UnitRm_pcContainsA()
    {
        var rm = new RankedMap<string, int>
        {
            Capacity = 4
        };
        var pc = (ICollection<KeyValuePair<string, int>>)rm;
        var nullKv = new KeyValuePair<string, int>(null, 0);
        var zedKv = new KeyValuePair<string, int>("z", 0);
        foreach (var kv in greek)
            pc.Add(kv);
        foreach (var kv in greek)
            Assert.True(pc.Contains(kv));
        Assert.False(pc.Contains(zedKv));
        Assert.False(pc.Contains(nullKv));
        pc.Add(nullKv);
        Assert.True(pc.Contains(nullKv));
        pc.Add(new KeyValuePair<string, int>("alpha", 0));
        Assert.True(pc.Contains(new KeyValuePair<string, int>("alpha", 0)));
        Assert.False(pc.Contains(new KeyValuePair<string, int>("alpha", 99)));
    }

    [Fact]
    public void UnitRm_pcContainsB()
    {
        var rm = new RankedMap<int, int>
        {
            Capacity = 4
        };
        var pc = (ICollection<KeyValuePair<int, int>>)rm;
        pc.Add(new KeyValuePair<int, int>(1, 11));
        pc.Add(new KeyValuePair<int, int>(1, 12));
        pc.Add(new KeyValuePair<int, int>(1, 13));
        pc.Add(new KeyValuePair<int, int>(2, 21));
        pc.Add(new KeyValuePair<int, int>(2, 22));
        pc.Add(new KeyValuePair<int, int>(2, 23));
        pc.Add(new KeyValuePair<int, int>(2, 24));
        Assert.True(pc.Contains(new KeyValuePair<int, int>(2, 23)));
        pc.Remove(new KeyValuePair<int, int>(2, 22));
        Assert.False(pc.Contains(new KeyValuePair<int, int>(2, 22)));
    }

    [Fact]
    public void CrashRm_ContainsKey_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var rm = new RankedMap<string, int>
            {
                {
                    "beta",
                    2
                }
            };
            var zz = rm.ContainsKey(null);
        });
    }

    [Fact]
    public void UnitRm_ContainsKey()
    {
        var rm = new RankedMap<int, int>();
        var key1 = 26;
        var key2 = 36;
        rm.Add(key1, key1 * 10);
        Assert.True(rm.ContainsKey(key1));
        Assert.False(rm.ContainsKey(key2));
    }

    [Fact]
    public void UnitRm_ContainsValue()
    {
        var rm = new RankedMap<int, int>();
        var key1 = 26;
        var key2 = 36;
        var key3 = 46;
        rm.Add(key1, key1 + 1000);
        rm.Add(key3, key3 + 1000);
        Assert.True(rm.ContainsValue(key1 + 1000));
        Assert.False(rm.ContainsValue(key2 + 1000));
    }

    [Fact]
    public void CrashRm_CopyTo_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var rm = new RankedMap<int, int>();
            rm.CopyTo(null !, -1);
        });
    }

    [Fact]
    public void CrashRm_CopyTo1_ArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var rm = new RankedMap<int, int>();
            var target = new KeyValuePair<int, int>[iVals1.Length];
            rm.CopyTo(target, -1);
        });
    }

    [Fact]
    public void CrashRm_CopyTo1_Argument()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var rm = new RankedMap<int, int>();
            for (var key = 1; key < 10; ++key)
                rm.Add(key, key + 1000);
            var target = new KeyValuePair<int, int>[10];
            rm.CopyTo(target, 25);
        });
    }

    [Fact]
    public void CrashRm_CopyTo2_Argument()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var rm = new RankedMap<int, int>();
            for (var key = 1; key < 10; ++key)
                rm.Add(key, key + 1000);
            var target = new KeyValuePair<int, int>[4];
            rm.CopyTo(target, 2);
        });
    }

    [Fact]
    public void UnitRm_CopyTo2()
    {
        var rm = new RankedMap<int, int>();
        int offset = 1, size = 20;
        for (var i = 0; i < size; ++i)
            rm.Add(i + 1000, i + 10000);
        var pairs = new KeyValuePair<int, int>[size + offset];
        rm.CopyTo(pairs, offset);
        for (var i = 0; i < offset; ++i)
        {
            Assert.Equal(0, pairs[i].Key);
            Assert.Equal(0, pairs[i].Value);
        }

        for (var i = 0; i < size; ++i)
        {
            Assert.Equal(i + 1000, pairs[i + offset].Key);
            Assert.Equal(i + 10000, pairs[i + offset].Value);
        }
    }

    [Fact]
    public void CrashRm_ocCopyTo_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var rm = new RankedMap<int, int>();
            var oc = (ICollection)rm;
            oc.CopyTo(null !, 0);
        });
    }

    [Fact]
    public void CrashRm_ocCopyTo_ArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var rm = new RankedMap<int, int>();
            var oc = (ICollection)rm;
            var target = new KeyValuePair<int, int>[1];
            oc.CopyTo(target, -1);
        });
    }

    [Fact]
    public void CrashRm_ocCopyTo1_Argument()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var rm = new RankedMap<string, int>
            {
                {
                    "foo",
                    1
                }
            };
            var oc = (ICollection)rm;
            var target = new KeyValuePair<string, int>[1, 2];
            oc.CopyTo(target, 0);
        });
    }

    [Fact]
    public void CrashRm_ocCopyTo2_Argument()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var rm = new RankedMap<int, int>();
            var oc = (ICollection)rm;
            for (var key = 1; key < 10; ++key)
                rm.Add(key, key + 1000);
            var target = new KeyValuePair<int, int>[1];
            oc.CopyTo(target, 0);
        });
    }

    [Fact]
    public void CrashRm_ocCopyToBadType_Argument()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var rm = new RankedMap<int, int>
            {
                {
                    42,
                    420
                }
            };
            var oc = (ICollection)rm;
            var target = new string[5];
            oc.CopyTo(target, 0);
        });
    }

    [Fact]
    public void UnitRm_oCopyTo()
    {
        var rm = new RankedMap<int, int>();
        var oc = (ICollection)rm;
        foreach (var key in iVals1)
            rm.Add(key, key + 1000);
        var target = new KeyValuePair<int, int>[iVals1.Length];
        oc.CopyTo(target, 0);
        for (var i = 0; i < iVals1.Length; ++i)
            Assert.Equal(target[i].Key + 1000, target[i].Value);
    }

    [Fact]
    public void CrashRm_IndexOfKey_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var rm = new RankedMap<string, int>();
            var zz = rm.IndexOfKey(null);
        });
    }

    [Fact]
    public void UnitRm_IndexOfKey1()
    {
        var rm = new RankedMap<int, int>
        {
            Capacity = 5
        };
        for (var ii = 0; ii < 500; ii += 2)
            rm.Add(ii, ii + 1000);
        for (var ii = 0; ii < 500; ii += 2)
        {
            var ix = rm.IndexOfKey(ii);
            Assert.Equal(ii / 2, ix);
        }

        var iw = rm.IndexOfKey(-1);
        Assert.Equal(~0, iw);
        var iy = rm.IndexOfKey(500);
        Assert.Equal(~250, iy);
    }

    [Fact]
    public void UnitRm_IndexOfKey2()
    {
        var rm = new RankedMap<int, int>
        {
            Capacity = 4
        };
        int n = 100, w = 5;
        for (var ii = 0; ii < n; ++ii)
        for (var i2 = 0; i2 < w; ++i2)
            rm.Add(ii, -ii);
        for (var ii = 0; ii < n; ++ii)
            Assert.Equal(ii * w, rm.IndexOfKey(ii));
    }

    [Fact]
    public void UnitRm_IndexOfValue()
    {
        var rm = new RankedMap<int, int>
        {
            Capacity = 4
        };
        for (var ii = 0; ii < 500; ++ii)
            rm.Add(ii, ii + 1000);
        var ix1 = rm.IndexOfValue(1400);
        Assert.Equal(400, ix1);
        var ix2 = rm.IndexOfValue(88888);
        Assert.Equal(-1, ix2);
    }

    [Fact]
    public void CrashRm_Remove_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var rm = new RankedMap<string, int>
            {
                {
                    "apple",
                    4
                }
            };
            var isRemoved = rm.Remove(null);
        });
    }

    [Fact]
    public void UnitRm_Remove1()
    {
        var rm = new RankedMap<int, int>();
        foreach (var key in iVals1)
            rm.Add(key, key + 1000);
        var c0 = rm.Count;
        var isRemoved1 = rm.Remove(iVals1[3]);
        var isRemoved2 = rm.Remove(iVals1[3]);
        Assert.True(isRemoved1);
        Assert.False(isRemoved2);
        Assert.Equal(c0 - 1, rm.Count);
    }

    [Fact]
    public void CrashRm_Remove2_Argument()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var rm = new RankedMap<int, int>();
            rm.Remove(1, -1);
        });
    }

    [Fact]
    public void UnitRm_Remove2()
    {
        var rm0 = new RankedMap<int, int>();
        var rm1 = new RankedMap<int, int>
        {
            Capacity = 5
        };
        var rm2 = new RankedMap<int, int>
        {
            Capacity = 5
        };
        foreach (var ii in new[]
                 {
                     3,
                     5,
                     5,
                     7,
                     7,
                     7,
                     9
                 }

                )
            rm1.Add(ii, -ii);
        foreach (var ii in new[]
                 {
                     3,
                     3,
                     3,
                     5,
                     5,
                     5,
                     7,
                     7,
                     7,
                     9
                 }

                )
            rm2.Add(ii, -ii);
        var rem0 = rm0.Remove(0, 1);
        Assert.Equal(0, rem0);
        var rem2 = rm1.Remove(2, 2);
        Assert.Equal(0, rem2);
        var rem70 = rm1.Remove(7, 0);
        Assert.Equal(0, rem70);
        var rem7 = rm1.Remove(7, 1);
        Assert.Equal(1, rem7);
        Assert.Equal(6, rm1.Count);
        var rem5 = rm1.Remove(5, 3);
        Assert.Equal(2, rem5);
        Assert.Equal(4, rm1.Count);
        var rem9 = rm1.Remove(10);
        Assert.False(rem9);
        var rem53 = rm2.Remove(5, 3);
        Assert.Equal(3, rem53);
        var rem33 = rm2.Remove(3, Int32.MaxValue);
        Assert.Equal(3, rem33);
        var rem99 = rm2.Remove(9, 9);
        Assert.Equal(1, rem99);
        Assert.Equal(3, rm2.Count);
    }

    [Fact]
    public void UnitRm_pcRemovePairNullValue()
    {
        var rm = new RankedMap<int, string>();
        var pc = (ICollection<KeyValuePair<int, string>>)rm;
        rm.Add(3, "cc");
        rm.Add(5, "ee");
        rm.Add(4, null);
        var isOK = pc.Remove(new KeyValuePair<int, string>(99, null));
        Assert.False(isOK);
        isOK = pc.Remove(new KeyValuePair<int, string>(4, null));
        Assert.True(isOK);
        isOK = rm.ContainsKey(4);
        Assert.False(isOK);
    }

    [Fact]
    public void UnitRm_pcRemovePair()
    {
        var rm = new RankedMap<string, int>();
        var pc = (ICollection<KeyValuePair<string, int>>)rm;
        var pair0 = new KeyValuePair<string, int>(null, 0);
        var pair1 = new KeyValuePair<string, int>("ten", 10);
        var pair2 = new KeyValuePair<string, int>("ten", 100);
        var pair3 = new KeyValuePair<string, int>("twenty", 20);
        pc.Add(pair0);
        pc.Add(pair1);
        pc.Add(pair3);
        Assert.Equal(3, rm.Count);
        var isRemoved = pc.Remove(pair0);
        Assert.True(isRemoved);
        Assert.Equal(2, rm.Count);
        isRemoved = pc.Remove(pair0);
        Assert.False(isRemoved);
        Assert.Equal(2, rm.Count);
        isRemoved = pc.Remove(pair2);
        Assert.Equal(2, rm.Count);
        isRemoved = pc.Remove(pair1);
        Assert.True(isRemoved);
        Assert.Equal(1, rm.Count);
    }

    [Fact]
    public void StressRm_pcRemovePair()
    {
        for (var cap = 4; cap < 40; ++cap)
        {
            var rm = new RankedMap<int, int>
            {
                Capacity = cap
            };
            var pc = (ICollection<KeyValuePair<int, int>>)rm;
            var ed = new int[13];
            int i1;
            for (i1 = 0; i1 < 6; ++i1)
            for (var i2 = 0; i2 < 3; ++i2)
            {
                rm.Add(i1, -i1);
                ++ed[i1];
            }

            rm.Add(4, -4);
            ++ed[4];
            for (; i1 < 11; ++i1)
            for (var i2 = 0; i2 < 2; ++i2)
            {
                rm.Add(i1, -i1);
                ++ed[i1];
            }

            for (; i1 < 13; ++i1)
            for (var i2 = 0; i2 < 3; ++i2)
            {
                rm.Add(i1, -i1);
                ++ed[i1];
            }

            rm.Add(11, 11);
            ++ed[11];
            rm.Add(11, -11);
            ++ed[11];
            pc.Remove(new KeyValuePair<int, int>(0, 0));
            ed[0] -= 3;
            pc.Remove(new KeyValuePair<int, int>(2, -2));
            ed[2] -= 3;
            pc.Remove(new KeyValuePair<int, int>(7, -7));
            ed[7] -= 2;
            pc.Remove(new KeyValuePair<int, int>(11, 11));
            ed[11] -= 1;
            pc.Remove(new KeyValuePair<int, int>(12, -12));
            ed[12] -= 3;
            for (var ix = 0; ix < ed.Length; ++ix)
                try
                {
                    Assert.Equal(ed[ix], rm.Keys.GetCount(ix));
                }
                catch
                {
                    _output.WriteLine("cap=" + cap + ",ix=" + ix);
                    throw;
                }
#if DEBUG
            rm.SanityCheck();
#endif
        }
    }

    [Fact]
    public void CrashRm_RemoveAll_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var rm = new RankedMap<int, int>();
            rm.RemoveAll(null);
        });
    }

    [Fact]
    public void UnitRm_RemoveAll()
    {
        var rm0 = new RankedMap<int, int>();
        var rm = new RankedMap<int, int>
        {
            Capacity = 4
        };
        foreach (var ii in new[]
                 {
                     3,
                     3,
                     5,
                     5,
                     7,
                     7
                 }

                )
            rm.Add(ii, -ii);
        var rem0 = rm0.RemoveAll(new[] { 2 });
        Assert.Equal(0, rem0);
        var rem2 = rm.RemoveAll(new[] { 2 });
        Assert.Equal(0, rem2);
        var rem57 = rm.RemoveAll(new[] { 3, 3, 3, 7 });
        Assert.Equal(3, rem57);
        Assert.True(System.Linq.Enumerable.SequenceEqual(new[] { 5, 5, 7 }, rm.Keys));
    }

    [Fact]
    public void CrashRm_RemoveAtA_ArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var rm = new RankedMap<int, int>()
            {
                {
                    42,
                    24
                }
            };
            rm.RemoveAt(-1);
        });
    }

    [Fact]
    public void CrashRm_RemoveAtB_ArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var rm = new RankedMap<int, int>();
            rm.RemoveAt(0);
        });
    }

    [Fact]
    public void UnitRm_RemoveAt()
    {
        var rm = new RankedMap<int, int>()
        {
            Capacity = 4
        };
#if STRESS
            int n = 500, m = 10;
#else
        int n = 50, m = 5;
#endif
        for (var ii = 0; ii < n; ++ii)
            rm.Add(ii, -ii);
        for (var i2 = n - m; i2 >= 0; i2 -= m)
            rm.RemoveAt(i2);
        for (var i2 = 0; i2 < n; ++i2)
            if (i2 % m == 0)
            {
                Assert.False(rm.ContainsKey(i2));
                Assert.False(rm.ContainsValue(-i2));
            }
            else
            {
                Assert.True(rm.ContainsKey(i2));
                Assert.True(rm.ContainsValue(-i2));
            }
    }

    [Fact]
    public void CrashRm_RemoveRangeA_ArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var rm = new RankedMap<int, int>();
            rm.RemoveRange(-1, 0);
        });
    }

    [Fact]
    public void CrashRm_RemoveRangeB_ArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var rm = new RankedMap<int, int>();
            rm.RemoveRange(0, -1);
        });
    }

    [Fact]
    public void CrashRm_RemoveRange_Argument()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var rm = new RankedMap<int, int>
            {
                {
                    3,
                    33
                },
                {
                    5,
                    55
                }
            };
            rm.RemoveRange(1, 2);
        });
    }

    [Fact]
    public void UnitRm_RemoveRange()
    {
        var rm = new RankedMap<int, int>()
        {
            Capacity = 7
        };
        for (var ii = 0; ii < 20; ++ii)
            rm.Add(ii, -ii);
        rm.RemoveRange(20, 0);
        Assert.Equal(20, rm.Count);
        rm.RemoveRange(12, 4);
        Assert.Equal(16, rm.Count);
#if DEBUG
        rm.SanityCheck();
#endif
    }

    [Fact]
    public void CrashRm_RemoveWhere_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var rm = new RankedMap<int, int>();
            rm.RemoveWhere(null);
        });
    }

    [Fact]
    public void UnitRm_RemoveWhereA()
    {
        var rm = new RankedMap<int, int>
        {
            Capacity = 5
        };
        var n = 2000;
        for (var ix = 0; ix < n; ++ix)
            rm.Add(ix / 2, -ix);
        var removed = rm.RemoveWhere(IsEven);
        Assert.Equal(n / 2, removed);
        foreach (var key in rm.Keys)
            Assert.True(key % 2 != 0);
    }

    [Fact]
    public void UnitRm_RemoveWhereB()
    {
        var rm = new RankedMap<int, int>
        {
            Capacity = 4
        };
        var n = 2000;
        for (var ix = 0; ix < n; ++ix)
            rm.Add(ix, -ix);
        var removed = rm.RemoveWhere(IsAlways);
        Assert.Equal(n, removed);
        Assert.Equal(0, rm.Count);
    }

    [Fact]
    public void CrashRm_RemoveWherePair_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var rm = new RankedMap<int, int>();
            rm.RemoveWhereElement(null);
        });
    }

    [Fact]
    public void UnitRm_RemoveWherePair()
    {
        var rm = new RankedMap<int, int>
        {
            Capacity = 6
        };
        for (var ix = 0; ix < 110; ++ix)
            rm.Add(ix, -ix);
        var r1 = rm.RemoveWhereElement(IsPairLeN100);
        Assert.Equal(10, r1);
        Assert.Equal(100, rm.Count);
        var c0 = rm.Count;
        var r2 = rm.RemoveWhereElement(IsPairEven);
        Assert.Equal(50, r2);
        foreach (var v2 in rm.Values)
            Assert.True(v2 % 2 != 0);
        var r2b = rm.RemoveWhereElement(IsPairEven);
        Assert.Equal(0, r2b);
        var r3 = rm.RemoveWhereElement(IsPairAlways);
        Assert.Equal(50, r3);
        Assert.Equal(0, rm.Count);
    }

    [Fact]
    public void UnitRm_TryGetGEGT()
    {
        var rm = new RankedMap<string, int?>(StringComparer.OrdinalIgnoreCase)
        {
            Capacity = 4
        };
        for (var ci = 'b'; ci <= 'y'; ++ci)
        {
            rm.Add(ci.ToString(), ci - 'a');
            rm.Add(ci.ToString().ToUpper(), 'a' - ci);
        }

        var r0a = rm.TryGetGreaterThan("y", out var p0a);
        Assert.False(r0a);
        Assert.Equal(default(string), p0a.Key);
        Assert.Equal(default(int? ), p0a.Value);
        var r0b = rm.TryGetGreaterThanOrEqual("z", out var p0b);
        Assert.False(r0b);
        Assert.Equal(default(string), p0b.Key);
        Assert.Equal(default(int? ), p0b.Value);
        var r1 = rm.TryGetGreaterThan("B", out var p1);
        Assert.True(r1);
        Assert.Equal("c", p1.Key);
        Assert.Equal(2, p1.Value);
        var r2 = rm.TryGetGreaterThanOrEqual("B", out var p2);
        Assert.True(r2);
        Assert.Equal("b", p2.Key);
        Assert.Equal(1, p2.Value);
        var r3 = rm.TryGetGreaterThanOrEqual("A", out var p3);
        Assert.True(r3);
        Assert.Equal("b", p3.Key);
        Assert.Equal(1, p3.Value);
    }

    [Fact]
    public void UnitRm_TryGetLELT()
    {
        var rm = new RankedMap<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            Capacity = 4
        };
        for (var ci = 'b'; ci <= 'y'; ++ci)
        {
            rm.Add(ci.ToString(), ci - 'a');
            rm.Add(ci.ToString().ToUpper(), 'A' - ci);
        }

        var r0a = rm.TryGetLessThan("b", out var p0a);
        Assert.False(r0a);
        Assert.Equal(default(KeyValuePair<string, int>), p0a);
        var r0b = rm.TryGetLessThanOrEqual("A", out var p0b);
        Assert.False(r0b);
        Assert.Equal(default(KeyValuePair<string, int>), p0b);
        var r1 = rm.TryGetLessThan("c", out var p1);
        Assert.True(r1);
        Assert.Equal("B", p1.Key);
        var r2 = rm.TryGetLessThanOrEqual("c", out var p2);
        Assert.True(r2);
        Assert.Equal("c", p2.Key);
        var r3 = rm.TryGetLessThanOrEqual("D", out var p3);
        Assert.True(r3);
        Assert.Equal("d", p3.Key);
    }

    #endregion
    #region Test generic enumeration
    [Fact]
    public void UnitRm_ElementsBetweenA()
    {
        var rm = new RankedMap<string, int>();
        var pc = (ICollection<KeyValuePair<string, int>>)rm;
        rm.Add("Alpha", 1);
        rm.Add("Beta", 2);
        rm.Add("Omega", 24);
        var actual1 = 0;
        foreach (var kv in rm.ElementsBetween(null, "C"))
            ++actual1;
        pc.Add(new KeyValuePair<string, int>(null, 0));
        var actual2 = 0;
        foreach (var kv in rm.ElementsBetween(null, "C"))
            ++actual2;
        Assert.Equal(2, actual1);
        Assert.Equal(3, actual2);
    }

    [Fact]
    public void UnitRm_ElementsBetweenB()
    {
        var rm = new RankedMap<int, int>
        {
            Capacity = 4
        };
        for (var i = 90; i >= 0; i -= 10)
            rm.Add(i, -100 - i);
        var iterations = 0;
        var sumVals = 0;
        foreach (var kv in rm.ElementsBetween(35, 55))
        {
            ++iterations;
            sumVals += kv.Value;
        }

        Assert.Equal(2, iterations);
        Assert.Equal(-290, sumVals);
    }

    [Fact]
    public void UnitRm_ElementsBetweenMissingVal()
    {
        var rm = new RankedMap<int, int>
        {
            Capacity = 8
        };
        for (var i = 0; i < 1000; i += 2)
            rm.Add(i, -i);
        for (var i = 1; i < 990; i += 2)
        {
            var isFirst = true;
            foreach (var item in rm.ElementsBetween(i, i + 8))
                if (isFirst)
                {
                    try
                    {
                        Assert.Equal(i + 1, item.Key);
                    }
                    catch
                    {
                        _output.WriteLine("Incorrect key");
                        throw;
                    }

                    isFirst = false;
                }
        }
    }

    [Fact]
    public void UnitRm_ElementsBetweenPassedEnd()
    {
        var rm = new RankedMap<int, int>
        {
            Capacity = 5
        };
        for (var i = 0; i < 1000; ++i)
            rm.Add(i, -i);
        var iterations = 0;
        var sumVals = 0;
        foreach (var e in rm.ElementsBetween(500, 1500))
        {
            ++iterations;
            sumVals += e.Value;
        }

        Assert.Equal(500, iterations);
        try
        {
            Assert.Equal(-374750, sumVals);
        }
        catch
        {
            _output.WriteLine("Sum of values not correct");
            throw;
        }
    }

    [Fact]
    public void UnitRm_ElementsFrom()
    {
        var rm = new RankedMap<int, int>
        {
            Capacity = 4
        };
        for (var i = 1; i <= 1000; ++i)
            rm.Add(i, -i);
        var firstKey = -1;
        var actual1 = 0;
        foreach (var e in rm.ElementsFrom(501))
        {
            if (actual1 == 0)
                firstKey = e.Key;
            ++actual1;
        }

        var actual2 = 0;
        foreach (var e in rm.ElementsFrom(-1))
            ++actual2;
        Assert.Equal(501, firstKey);
        Assert.Equal(500, actual1);
        Assert.Equal(1000, actual2);
    }

    [Fact]
    public void UnitRm_ElementsFromMissingVal()
    {
        var rm = new RankedMap<int, int>()
        {
            Capacity = 6
        };
#if STRESS
            int n = 1000;
#else
        var n = 10;
#endif
        for (var i = 0; i < n; i += 2)
            rm.Add(i, -i);
        for (var i = 1; i < n - 1; i += 2)
        {
            foreach (var item in rm.ElementsFrom(i))
            {
                try
                {
                    Assert.Equal(i + 1, item.Key);
                }
                catch
                {
                    _output.WriteLine("Incorrect key");
                    throw;
                }

                break;
            }
        }
    }

    [Fact]
    public void UnitRm_ElementsFromPassedEnd()
    {
        var rm = new RankedMap<int, int>();
        for (var i = 0; i < 1000; ++i)
            rm.Add(i, -i);
        var iterations = 0;
        foreach (var item in rm.ElementsFrom(2000))
            ++iterations;
        try
        {
            Assert.Equal(0, iterations);
        }
        catch
        {
            _output.WriteLine("SkipUntilKey shouldn't find anything");
            throw;
        }
    }

    [Fact]
    public void CrashRm_ElementsBetweenIndexes_ArgumentOutOfRangeA()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var rm = new RankedMap<int, int>
            {
                {
                    0,
                    0
                },
                {
                    1,
                    -1
                }
            };
            foreach (var pair in rm.ElementsBetweenIndexes(-1, 0))
            {
            }
        });
    }

    [Fact]
    public void CrashRm_ElementsBetweenIndexes_ArgumentOutOfRangeB()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var rm = new RankedMap<int, int>
            {
                {
                    0,
                    0
                },
                {
                    1,
                    -1
                }
            };
            foreach (var pair in rm.ElementsBetweenIndexes(2, 0))
            {
            }
        });
    }

    [Fact]
    public void CrashRm_ElementsBetweenIndexes_ArgumentOutOfRangeC()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var rm = new RankedMap<int, int>
            {
                {
                    0,
                    0
                },
                {
                    1,
                    -1
                }
            };
            foreach (var pair in rm.ElementsBetweenIndexes(0, -1))
            {
            }
        });
    }

    [Fact]
    public void CrashRm_ElementsBetweenIndexes_ArgumentOutOfRangeD()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var rm = new RankedMap<int, int>
            {
                {
                    0,
                    0
                },
                {
                    1,
                    -1
                }
            };
            foreach (var pair in rm.ElementsBetweenIndexes(0, 2))
            {
            }
        });
    }

    [Fact]
    public void CrashRm_ElementsBetweenIndexes_Argument()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var rm = new RankedMap<int, int>
            {
                {
                    0,
                    0
                },
                {
                    1,
                    -1
                },
                {
                    2,
                    -2
                }
            };
            foreach (var pair in rm.ElementsBetweenIndexes(2, 1))
            {
            }
        });
    }

    [Fact]
    public void UnitRm_ElementsBetweenIndexes()
    {
        var rm = new RankedMap<int, int>
        {
            Capacity = 4
        };
        var n = 30;
        for (var ii = 0; ii < n; ++ii)
            rm.Add(ii, -ii);
        for (var p1 = 0; p1 < n; ++p1)
        for (var p2 = p1; p2 < n; ++p2)
        {
            var actual = 0;
            foreach (var pair in rm.ElementsBetweenIndexes(p1, p2))
                actual += pair.Key;
            var expected = (p2 - p1 + 1) * (p1 + p2) / 2;
            Assert.Equal(expected, actual);
        }
    }

    [Fact]
    public void UnitRm_gcEnumeration()
    {
        var rm = new RankedMap<int, int>
        {
            Capacity = 4
        };
        var pc = (ICollection<KeyValuePair<int, int>>)rm;
        foreach (var k in iVals1)
            rm.Add(k, k + 100);
        var actualCount = 0;
        foreach (var pair in pc)
        {
            Assert.Equal(pair.Key + 100, pair.Value);
            ++actualCount;
        }

        Assert.Equal(iVals1.Length, actualCount);
    }

    [Fact]
    public void UnitRm_peEtor()
    {
        var rm = new RankedMap<int, int>
        {
            Capacity = 4
        };
        var pe = (IEnumerable<KeyValuePair<int, int>>)rm;
        foreach (var k in iVals1)
            rm.Add(k, k + 100);
        var actualCount = 0;
        foreach (var pair in pe)
        {
            Assert.Equal(pair.Key + 100, pair.Value);
            ++actualCount;
        }

        Assert.Equal(iVals1.Length, actualCount);
    }

    [Fact]
    public void UnitRm_GetEnumeratorOnEmpty()
    {
        var rm = new RankedMap<string, int>();
        var actual = 0;
        using (var etor = rm.GetEnumerator())
        {
            while (etor.MoveNext())
                ++actual;
            var zz = etor.Current;
        }

        Assert.Equal(0, actual);
    }

    [Fact]
    public void UnitRm_GetEnumeratorPastEnd()
    {
        var rm2 = new RankedMap<string, int>();
        bool isMoved;
        int actualCount = 0, total = 0;
        rm2.Add("three", 3);
        rm2.Add("one", 1);
        rm2.Add("five", 5);
        rm2.Add("three", 3);
        using (var etor = rm2.GetEnumerator())
        {
            while (etor.MoveNext())
            {
                ++actualCount;
                total += etor.Current.Value;
            }

            isMoved = etor.MoveNext();
        }

        Assert.Equal(4, actualCount);
        Assert.Equal(12, total);
        Assert.False(isMoved);
    }

    [Fact]
    public void CrashRm_oeEtorRewound_InvalidOperation()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            var rm = new RankedMap<string, int>
            {
                {
                    "cc",
                    3
                }
            };
            IEnumerator<KeyValuePair<string, int>> kvEtor = rm.GetEnumerator();
            var zz = ((IEnumerator)kvEtor).Current;
        });
    }

    [Fact]
    public void CrashRm_peEtorPastUnwound_InvalidOperation()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            var rm = new RankedMap<string, int>
            {
                {
                    "cc",
                    3
                }
            };
            IEnumerator<KeyValuePair<string, int>> peEtor = rm.GetEnumerator();
            peEtor.MoveNext();
            peEtor.MoveNext();
            var zz = ((IEnumerator)peEtor).Current;
        });
    }

    [Fact]
    public void UnitRm_pcEtorPairPastEnd()
    {
        var rm = new RankedMap<string, int>
        {
            {
                "nine",
                9
            }
        };
        IEnumerator<KeyValuePair<string, int>> pcEtor = rm.GetEnumerator();
        var pair0 = pcEtor.Current;
        Assert.Equal(default(int), pair0.Value);
        Assert.Equal(default(string), pair0.Key);
        pcEtor.MoveNext();
        var pair1 = pcEtor.Current;
        Assert.Equal("nine", pair1.Key);
        Assert.Equal(9, pair1.Value);
        var oPair1 = ((IEnumerator)pcEtor).Current;
        Assert.Equal("nine", ((KeyValuePair<string, int>)oPair1).Key);
        Assert.Equal(9, ((KeyValuePair<string, int>)oPair1).Value);
        pcEtor.MoveNext();
        var pair2 = pcEtor.Current;
        Assert.Equal(default(string), pair2.Key);
        Assert.Equal(default(int), pair2.Value);
    }

    [Fact]
    public void CrashRm_EtorHotUpdate()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            var rm = new RankedMap<string, int>
            {
                {
                    "vv",
                    1
                },
                {
                    "mm",
                    2
                },
                {
                    "qq",
                    3
                }
            };
            var n = 0;
            foreach (var kv in rm)
                if (++n == 2)
                    rm.Add("kaboom", 4);
        });
    }

    [Fact]
    public void UnitRm_EtorCurrentHotUpdate()
    {
        var rm1 = new RankedMap<int, int>();
        var kv1 = new KeyValuePair<int, int>(1, -1);
        var kvd1 = new KeyValuePair<int, int>(default(int), default(int));
        rm1.Add(kv1.Key, kv1.Value);
        var etor1 = rm1.GetEnumerator();
        Assert.Equal(kvd1, etor1.Current);
        var ok1 = etor1.MoveNext();
        Assert.Equal(kv1, etor1.Current);
        rm1.Remove(kv1.Key);
        Assert.Equal(kv1, etor1.Current);
        var rm2 = new RankedMap<string, int>();
        var kv2 = new KeyValuePair<string, int>("MM", 13);
        var kvd2 = new KeyValuePair<string, int>(default(string), default(int));
        rm2.Add(kv2.Key, kv2.Value);
        var etor2 = rm2.GetEnumerator();
        Assert.Equal(kvd2, etor2.Current);
        var ok2 = etor2.MoveNext();
        Assert.Equal(kv2, etor2.Current);
        rm2.Clear();
        Assert.Equal(kv2, etor2.Current);
    }

    #endregion
    #region Test object enumeration
    [Fact]
    public void UnitRm_ocGetEnumerator()
    {
        var rm = new RankedMap<int, int>();
        var oc = (ICollection)rm;
        var zz = oc.GetEnumerator();
    }

    [Fact]
    public void UnitRm_ocEtor()
    {
        var rm = new RankedMap<int, string>
        {
            {
                3,
                "cc"
            }
        };
        var oc = (ICollection)rm;
        var rowCount = 0;
        foreach (DictionaryEntry row in oc)
        {
            Assert.Equal(3, (int)row.Key);
            Assert.Equal("cc", (string)row.Value);
            ++rowCount;
        }

        Assert.Equal(1, rowCount);
    }

    [Fact]
    public void UnitRm_oEtorEntry()
    {
        var rm = new RankedMap<int, int>();
        var oc = (ICollection)rm;
        foreach (var k in iVals1)
            rm.Add(k, k + 1000);
        var actualCount = 0;
        foreach (var oItem in oc)
        {
            var de = (DictionaryEntry)oItem;
            Assert.Equal((int)de.Key + 1000, de.Value);
            ++actualCount;
        }

        Assert.Equal(iVals1.Length, actualCount);
    }

    [Fact]
    public void UnitRm_ocCurrent_HotUpdate()
    {
        var rm = new RankedMap<int, int>();
        var kv = new KeyValuePair<int, int>(1, -1);
        rm.Add(kv.Key, kv.Value);
        System.Collections.ICollection oc = rm;
        var etor = oc.GetEnumerator();
        var ok = etor.MoveNext();
        Assert.Equal(new DictionaryEntry(kv.Key, kv.Value), etor.Current);
        rm.Clear();
        Assert.Equal(new DictionaryEntry(kv.Key, kv.Value), etor.Current);
    }

    [Fact]
    public void UnitRm_oReset()
    {
        var rm = new RankedMap<int, int>
        {
            Capacity = 5
        };
        var n = 11;
        for (var ix = 0; ix < n; ++ix)
            rm.Add(ix, -ix);
        var etor = rm.GetEnumerator();
        var ix1 = 0;
        while (etor.MoveNext())
        {
            Assert.Equal(ix1, etor.Current.Key);
            Assert.Equal(-ix1, etor.Current.Value);
            ++ix1;
        }

        Assert.Equal(n, ix1);
        ((System.Collections.IEnumerator)etor).Reset();
        var ix2 = 0;
        while (etor.MoveNext())
        {
            Assert.Equal(ix2, etor.Current.Key);
            Assert.Equal(-ix2, etor.Current.Value);
            ++ix2;
        }

        Assert.Equal(n, ix2);
    }
    #endregion
}
#endif
