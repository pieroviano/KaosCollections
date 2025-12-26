using System;
using Xunit;
#if TEST_BCL
using System.Collections.Generic;
using System.Linq;
#else
using Kaos.Collections;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#endif

namespace Kaos.Test.Collections;

public partial class TestRd
{
    #region Test Keys constructor
    [Fact]
    public void CrashRdk_Ctor_ArgumentNull()
    {
        Setup();
#if TEST_BCL
            Assert.Throws<ArgumentNullException>(() =>
            {
                var zz = new SortedDictionary<int,int>.KeyCollection (null);
            });
#else
        Assert.Throws<ArgumentNullException>(() =>
        {
            var zz = new RankedDictionary<int, int>.KeyCollection(null);
        });
#endif
    }

    [Fact]
    public void UnitRdk_Ctor()
    {
        Setup();
        dary1.Add(1, -1);
#if TEST_BCL
            var keys = new SortedDictionary<int,int>.KeyCollection (dary1);
#else
        var keys = new RankedDictionary<int, int>.KeyCollection(dary1);
#endif
        Assert.Equal(1, keys.Count);
    }

    #endregion
    #region Test Keys properties
    [Fact]
    public void UnitRdk_Count()
    {
        Setup();
        foreach (var key in iVals1)
            dary1.Add(key, key + 1000);
        Assert.Equal(iVals1.Length, dary1.Keys.Count);
    }

    [Fact]
    public void UnitRdk_gcIsReadonly()
    {
        Setup();
        var gc = (System.Collections.Generic.ICollection<int>)dary1.Keys;
        Assert.True(gc.IsReadOnly);
    }

    [Fact]
    public void UnitRdk_ocSyncRoot()
    {
        Setup();
        var oc = (System.Collections.ICollection)dary1.Keys;
        Assert.False(oc.SyncRoot.GetType().IsValueType);
    }

    #endregion
    #region Test Keys methods
    [Fact]
    public void CrashRdk_gcAdd_NotSupported()
    {
        Setup();
        var gc = (System.Collections.Generic.ICollection<string>)dary2.Keys;
        Assert.Throws<NotSupportedException>(() =>
        {
            gc.Add("omega");
        });
    }

    [Fact]
    public void CrashRdk_gcClear_NotSupported()
    {
        Setup();
        var gc = (System.Collections.Generic.ICollection<string>)dary2.Keys;
        Assert.Throws<NotSupportedException>(() =>
        {
            gc.Clear();
        });
    }

    [Fact]
    public void CrashRdk_gcContains_ArgumentNull()
    {
        Setup();
        var gc = (System.Collections.Generic.ICollection<string>)dary2.Keys;
        dary2.Add("alpha", 10);
        Assert.Throws<ArgumentNullException>(() =>
        {
            var zz = gc.Contains(null);
        });
    }

    [Fact]
    public void UnitRdk_gcContains()
    {
        Setup();
        var gc = (System.Collections.Generic.ICollection<string>)dary2.Keys;
        dary2.Add("alpha", 10);
        dary2.Add("beta", 20);
        Assert.True(gc.Contains("beta"));
        Assert.False(gc.Contains("zed"));
    }

    [Fact]
    public void CrashRdk_CopyTo_ArgumentNull()
    {
        Setup();
        var target = new int[10];
        Assert.Throws<ArgumentNullException>(() =>
        {
            dary1.Keys.CopyTo(null, -1);
        });
    }

    [Fact]
    public void CrashRdk_CopyTo_ArgumentOutOfRange()
    {
        Setup();
        var target = new int[iVals1.Length];
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            dary1.Keys.CopyTo(target, -1);
        });
    }

    [Fact]
    public void CrashRdk_CopyTo_Argument()
    {
        Setup();
        for (var key = 1; key < 10; ++key)
            dary1.Add(key, key + 1000);
        var target = new int[4];
        Assert.Throws<ArgumentException>(() =>
        {
            dary1.Keys.CopyTo(target, 2);
        });
    }

    [Fact]
    public void UnitRdk_CopyTo()
    {
        Setup();
        int n = 10, offset = 5;
        for (var k = 0; k < n; ++k)
            dary1.Add(k, k + 1000);
        var target = new int[n + offset];
        dary1.Keys.CopyTo(target, offset);
        for (var k = 0; k < n; ++k)
            Assert.Equal(k, target[k + offset]);
    }

    [Fact]
    public void UnitRdk_gcCopyTo()
    {
        Setup();
        var gc = (System.Collections.Generic.ICollection<string>)dary2.Keys;
        dary2.Add("alpha", 1);
        dary2.Add("beta", 2);
        dary2.Add("gamma", 3);
        var target = new string[dary2.Count];
        gc.CopyTo(target, 0);
        Assert.Equal("alpha", target[0]);
        Assert.Equal("beta", target[1]);
        Assert.Equal("gamma", target[2]);
    }

    [Fact]
    public void CrashRdk_gcRemove_NotSupported()
    {
        Setup();
        var gc = (System.Collections.Generic.ICollection<string>)dary2.Keys;
        Assert.Throws<NotSupportedException>(() =>
        {
            gc.Remove("omega");
        });
    }

    #endregion
    #region Test Keys bonus methods
#if !TEST_BCL
    [Fact]
    public void UnitRdkx_Indexer()
    {
        var rd = new RankedDictionary<string, int>
        {
            {
                "0zero",
                0
            },
            {
                "1one",
                -1
            },
            {
                "2two",
                -2
            }
        };
        Assert.Equal("0zero", rd.Keys[0]);
        Assert.Equal("1one", rd.Keys[1]);
        Assert.Equal("2two", rd.Keys[2]);
    }

    [Fact]
    public void CrashRdkx_ElementAt_ArgumentOutOfRange1()
    {
        var rd = new RankedDictionary<int, int>();
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var zz = rd.Keys.ElementAt(-1);
        });
    }

    [Fact]
    public void CrashRdkx_ElementAt_ArgumentOutOfRange2()
    {
        var rd = new RankedDictionary<int, int>();
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var zz = rd.Keys.ElementAt(0);
        });
    }

    [Fact]
    public void UnitRdkx_ElementAt()
    {
        var rd = new RankedDictionary<string, int>
        {
            {
                "one",
                1
            },
            {
                "two",
                2
            }
        };
        var k1 = rd.Keys.ElementAt(1);
        Assert.Equal("two", k1);
    }

    [Fact]
    public void UnitRdkx_ElementAtOrDefault()
    {
        var rd = new RankedDictionary<string, int>
        {
            {
                "one",
                1
            },
            {
                "two",
                2
            }
        };
        var kn = rd.Keys.ElementAtOrDefault(-1);
        var k1 = rd.Keys.ElementAtOrDefault(1);
        var k2 = rd.Keys.ElementAtOrDefault(2);
        Assert.Equal(default(string), kn);
        Assert.Equal("two", k1);
        Assert.Equal(default(string), k2);
    }

    [Fact]
    public void UnitRdkx_IndexOf()
    {
        var rd = new RankedDictionary<string, int>
        {
            {
                "one",
                1
            },
            {
                "two",
                2
            }
        };
        var pc = (System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<string, int>>)rd;
        pc.Add(new System.Collections.Generic.KeyValuePair<string, int>(null, -1));
        Assert.Equal(0, rd.Keys.IndexOf(null));
        Assert.Equal(2, rd.Keys.IndexOf("two"));
    }

    [Fact]
    public void UnitRdx_TryGet()
    {
        var rd = new RankedDictionary<string, int>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "AAA", 1 },
            { "bbb", 2 },
            { "ccc", 3 }
        };
        var got1 = rd.Keys.TryGet("aaa", out var actual1);
        Assert.True(got1);
        Assert.Equal("AAA", actual1);
        var got2 = rd.Keys.TryGet("bb", out var actual2);
        Assert.False(got2);
        var got3 = rd.Keys.TryGet("CCC", out var actual3);
        Assert.True(got3);
        Assert.Equal("ccc", actual3);
    }

    [Fact]
    public void UnitRdkx_TryGetGEGT()
    {
        var rd = new RankedDictionary<string, int>
        {
            {
                "BB",
                1
            },
            {
                "CC",
                2
            }
        };
        var r0a = rd.Keys.TryGetGreaterThan("CC", out var k0a);
        Assert.False(r0a);
        Assert.Equal(default(string), k0a);
        var r0b = rd.Keys.TryGetGreaterThanOrEqual("DD", out var k0b);
        Assert.False(r0b);
        Assert.Equal(default(string), k0b);
        var r1 = rd.Keys.TryGetGreaterThan("BB", out var k1);
        Assert.True(r1);
        Assert.Equal("CC", k1);
        var r2 = rd.Keys.TryGetGreaterThanOrEqual("BB", out var k2);
        Assert.True(r2);
        Assert.Equal("BB", k2);
        var r3 = rd.Keys.TryGetGreaterThanOrEqual("AA", out var k3);
        Assert.True(r3);
        Assert.Equal("BB", k3);
    }

    [Fact]
    public void UnitRdkx_TryGetLELT()
    {
        var rd = new RankedDictionary<string, int>
        {
            {
                "BB",
                1
            },
            {
                "CC",
                2
            }
        };
        var r0a = rd.Keys.TryGetLessThan("BB", out var k0a);
        Assert.False(r0a);
        Assert.Equal(default(string), k0a);
        var r0b = rd.Keys.TryGetLessThanOrEqual("AA", out var k0b);
        Assert.False(r0b);
        Assert.Equal(default(string), k0b);
        var r1 = rd.Keys.TryGetLessThan("CC", out var k1);
        Assert.True(r1);
        Assert.Equal("BB", k1);
        var r2 = rd.Keys.TryGetLessThanOrEqual("CC", out var k2);
        Assert.True(r2);
        Assert.Equal("CC", k2);
        var r3 = rd.Keys.TryGetLessThanOrEqual("DD", out var k3);
        Assert.True(r3);
        Assert.Equal("CC", k3);
    }

#endif
    #endregion
    #region Test Keys enumeration
    [Fact]
    public void CrashRdk_ocCurrent_InvalidOperation()
    {
        Setup();
        dary2.Add("CC", 3);
        var oc = objCol2.Keys;
        var etor = oc.GetEnumerator();
        Assert.Throws<InvalidOperationException>(() =>
        {
            var zz = etor.Current;
        });
    }

    [Fact]
    public void UnitRdk_GetEnumerator()
    {
        Setup(4);
        var n = 100;
        for (var k = 0; k < n; ++k)
            dary1.Add(k, k + 1000);
        var actualCount = 0;
        foreach (var key in dary1.Keys)
        {
            Assert.Equal(actualCount, key);
            ++actualCount;
        }

        Assert.Equal(n, actualCount);
    }

    [Fact]
    public void UnitRdk_gcGetEnumerator()
    {
        Setup();
        var n = 10;
        for (var k = 0; k < n; ++k)
            dary2.Add(k.ToString(), k);
        var expected = 0;
        var etor = genKeys2.GetEnumerator();
        var rewoundKey = etor.Current;
        Assert.Equal(rewoundKey, null);
        while (etor.MoveNext())
        {
            var key = etor.Current;
            Assert.Equal(expected.ToString(), key);
            ++expected;
        }

        Assert.Equal(n, expected);
    }

    [Fact]
    public void UnitRdk_Reverse()
    {
        Setup(4);
        var n = 50;
        for (var k = 0; k < n; ++k)
            dary1.Add(k, k + 1000);
        var expected = n;
        foreach (var ak in dary1.Keys.Reverse())
        {
            --expected;
            Assert.Equal(expected, ak);
        }

        Assert.Equal(0, expected);
    }

    [Fact]
    public void CrashRdk_EtorHotUpdate()
    {
        Setup(4);
        dary2.Add("vv", 1);
        dary2.Add("mm", 2);
        dary2.Add("qq", 3);
        Assert.Throws<InvalidOperationException>(() =>
        {
            var n = 0;
            foreach (var kv in dary2.Keys)
            {
                if (++n == 2)
                    dary2.Remove("vv");
            }
        });
    }

    [Fact]
    public void UnitRdk_ocCurrent_HotUpdate()
    {
        Setup();
        dary2.Add("AA", 11);
        var oc = objCol2.Keys;
        var etor = oc.GetEnumerator();
        var ok = etor.MoveNext();
        Assert.Equal("AA", etor.Current);
        dary2.Clear();
        Assert.Equal("AA", etor.Current);
    }

    [Fact]
    public void UnitRdk_EtorCurrentHotUpdate()
    {
        Setup();
        dary1.Add(1, -1);
        var etor1 = dary1.Keys.GetEnumerator();
        Assert.Equal(default(int), etor1.Current);
        var ok1 = etor1.MoveNext();
        Assert.Equal(1, etor1.Current);
        dary1.Remove(1);
        Assert.Equal(1, etor1.Current);
        dary2.Add("AA", 11);
        var etor2 = dary2.Keys.GetEnumerator();
        Assert.Equal(default(string), etor2.Current);
        var ok2 = etor2.MoveNext();
        Assert.Equal("AA", etor2.Current);
        dary2.Clear();
        Assert.Equal("AA", etor2.Current);
    }

    [Fact]
    public void UnitRdk_oReset()
    {
        Setup(5);
        var n = 9;
        for (var ix = 0; ix < n; ++ix)
            dary1.Add(ix, -ix);
#if TEST_BCL
            SortedDictionary<int,int>.KeyCollection.Enumerator etor;
#else
        RankedDictionary<int, int>.KeyCollection.Enumerator etor;
#endif
        etor = dary1.Keys.GetEnumerator();
        var ix1 = 0;
        while (etor.MoveNext())
        {
            Assert.Equal(ix1, etor.Current);
            ++ix1;
        }

        Assert.Equal(n, ix1);
        ((System.Collections.IEnumerator)etor).Reset();
        var ix2 = 0;
        while (etor.MoveNext())
        {
            Assert.Equal(ix2, etor.Current);
            ++ix2;
        }

        Assert.Equal(n, ix2);
    }

    #endregion
    #region Test Values constructor
    [Fact]
    public void CrashRdv_Ctor_ArgumentNull()
    {
        Setup();
#if TEST_BCL
            Assert.Throws<ArgumentNullException>(() =>
            {
                var vals = new SortedDictionary<int,int>.ValueCollection (null);
            });
#else
        Assert.Throws<ArgumentNullException>(() =>
        {
            var vals = new RankedDictionary<int, int>.ValueCollection(null);
        });
#endif
    }

    [Fact]
    public void UnitRdv_Ctor()
    {
        Setup();
        dary1.Add(1, -1);
#if TEST_BCL
            var vals = new SortedDictionary<int,int>.ValueCollection (dary1);
#else
        var vals = new RankedDictionary<int, int>.ValueCollection(dary1);
#endif
        Assert.Equal(1, vals.Count);
    }

    #endregion
    #region Test Values properties
    [Fact]
    public void UnitRdv_Count()
    {
        Setup();
        foreach (var key in iVals1)
            dary1.Add(key, key + 1000);
        Assert.Equal(iVals1.Length, dary1.Values.Count);
    }

    [Fact]
    public void UnitRdv_gcIsReadonly()
    {
        Setup();
        var gc = (System.Collections.Generic.ICollection<int>)dary1.Values;
        Assert.True(gc.IsReadOnly);
    }

    [Fact]
    public void UnitRdv_ocSyncRoot()
    {
        Setup();
        var oc = (System.Collections.ICollection)dary2.Values;
        Assert.False(oc.SyncRoot.GetType().IsValueType);
    }

    #endregion
    #region Test Values methods
    [Fact]
    public void CrashRdv_gcAdd_NotSupported()
    {
        Setup();
        var gc = (System.Collections.Generic.ICollection<int>)dary2.Values;
        Assert.Throws<NotSupportedException>(() =>
        {
            gc.Add(9);
        });
    }

    [Fact]
    public void CrashRdv_gcClear_NotSupported()
    {
        Setup();
        var gc = (System.Collections.Generic.ICollection<int>)dary2.Values;
        Assert.Throws<NotSupportedException>(() =>
        {
            gc.Clear();
        });
    }

    [Fact]
    public void UnitRdv_gcContains()
    {
        Setup();
        var gc = (System.Collections.Generic.ICollection<int>)dary2.Values;
        dary2.Add("alpha", 10);
        dary2.Add("beta", 20);
        Assert.True(gc.Contains(20));
        Assert.False(gc.Contains(15));
    }

    [Fact]
    public void CrashRdv_CopyTo_ArgumentNull()
    {
        Setup();
        var target = new int[iVals1.Length];
        Assert.Throws<ArgumentNullException>(() =>
        {
            dary1.Values.CopyTo(null, -1);
        });
    }

    [Fact]
    public void CrashRdv_CopyTo_ArgumentOutOfRange()
    {
        Setup();
        var target = new int[10];
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            dary1.Values.CopyTo(target, -1);
        });
    }

    [Fact]
    public void CrashRdv_CopyTo_Argument()
    {
        Setup();
        for (var key = 1; key < 10; ++key)
            dary1.Add(key, key + 1000);
        var target = new int[4];
        Assert.Throws<ArgumentException>(() =>
        {
            dary1.Values.CopyTo(target, 2);
        });
    }

    [Fact]
    public void UnitRdv_CopyTo()
    {
        Setup();
        int n = 10, offset = 5;
        for (var k = 0; k < n; ++k)
            dary1.Add(k, k + 1000);
        var target = new int[n + offset];
        dary1.Values.CopyTo(target, offset);
        for (var k = 0; k < n; ++k)
            Assert.Equal(k + 1000, target[k + offset]);
    }

    [Fact]
    public void UnitRdv_gcCopyTo()
    {
        Setup();
        var gc = (System.Collections.Generic.ICollection<int>)dary2.Values;
        dary2.Add("alpha", 1);
        dary2.Add("beta", 2);
        dary2.Add("gamma", 3);
        var target = new int[dary2.Count];
        gc.CopyTo(target, 0);
        Assert.Equal(1, target[0]);
        Assert.Equal(2, target[1]);
        Assert.Equal(3, target[2]);
    }

    [Fact]
    public void CrashRdv_gcRemove_NotSupported()
    {
        Setup();
        var gc = (System.Collections.Generic.ICollection<int>)dary2.Values;
        Assert.Throws<NotSupportedException>(() =>
        {
            gc.Remove(9);
        });
    }

    #endregion
    #region Test Values bonus methods
#if !TEST_BCL
    [Fact]
    public void UnitRdvx_Indexer()
    {
        var rd = new RankedDictionary<string, int>
        {
            Capacity = 4
        };
        foreach (var kv in greek)
            rd.Add(kv.Key, kv.Value);
        Assert.Equal(11, rd.Values[7]);
    }

    [Fact]
    public void UnitRdvx_IndexOf()
    {
        var rd = new RankedDictionary<int, int>
        {
            Capacity = 5
        };
        for (var ii = 0; ii < 900; ++ii)
            rd.Add(ii, ii + 1000);
        var ix1 = rd.Values.IndexOf(1500);
        Assert.Equal(500, ix1);
        var ix2 = rd.Values.IndexOf(77777);
        Assert.Equal(-1, ix2);
    }

#endif
    #endregion
    #region Test Values enumeration
    [Fact]
    public void CrashRdv_ocCurrent_InvalidOperation()
    {
        Setup();
        dary2.Add("CC", 3);
        var oc = objCol2.Values;
        var etor = oc.GetEnumerator();
        Assert.Throws<InvalidOperationException>(() =>
        {
            var zz = etor.Current;
        });
    }

    [Fact]
    public void UnitRdv_GetEtor()
    {
        Setup();
        var n = 100;
        for (var k = 0; k < n; ++k)
            dary1.Add(k, k + 1000);
        var actualCount = 0;
        foreach (var value in dary1.Values)
        {
            Assert.Equal(actualCount + 1000, value);
            ++actualCount;
        }

        Assert.Equal(n, actualCount);
    }

    [Fact]
    public void UnitRdv_gcGetEnumerator()
    {
        Setup();
        var n = 10;
        for (var k = 0; k < n; ++k)
            dary2.Add(k.ToString(), k);
        var expected = 0;
        var etor = genValues2.GetEnumerator();
        var rewoundVal = etor.Current;
        Assert.Equal(rewoundVal, default(int));
        while (etor.MoveNext())
        {
            var val = etor.Current;
            Assert.Equal(expected, val);
            ++expected;
        }

        Assert.Equal(n, expected);
    }

    [Fact]
    public void UnitRdv_ocCurrent_HotUpdate()
    {
        Setup();
        dary2.Add("AA", 11);
        var oc = objCol2.Values;
        var etor = oc.GetEnumerator();
        var ok = etor.MoveNext();
        Assert.Equal(11, etor.Current);
        dary2.Clear();
        Assert.Equal(11, etor.Current);
    }

    [Fact]
    public void UnitRdv_EtorCurrentHotUpdate()
    {
        Setup();
        dary1.Add(1, -1);
        var etor1 = dary1.Values.GetEnumerator();
        Assert.Equal(default(int), etor1.Current);
        var ok1 = etor1.MoveNext();
        Assert.Equal(-1, etor1.Current);
        dary1.Remove(1);
        Assert.Equal(-1, etor1.Current);
        dary2.Add("AA", 11);
        var etor2 = dary2.Values.GetEnumerator();
        Assert.Equal(default(int), etor2.Current);
        var ok2 = etor2.MoveNext();
        Assert.Equal(11, etor2.Current);
        dary2.Clear();
        Assert.Equal(11, etor2.Current);
    }

    [Fact]
    public void CrashRdv_EtorHotUpdate()
    {
        Setup(4);
        dary2.Add("vv", 1);
        dary2.Add("mm", 2);
        dary2.Add("qq", 3);
        Assert.Throws<InvalidOperationException>(() =>
        {
            var n = 0;
            foreach (var kv in dary2.Keys)
            {
                if (++n == 2)
                    dary2.Clear();
            }
        });
    }

    [Fact]
    public void UnitRdv_oReset()
    {
        Setup(5);
        var n = 9;
        for (var ix = 0; ix < n; ++ix)
            dary1.Add(ix, -ix);
#if TEST_BCL
            SortedDictionary<int,int>.ValueCollection.Enumerator etor;
#else
        RankedDictionary<int, int>.ValueCollection.Enumerator etor;
#endif
        etor = dary1.Values.GetEnumerator();
        var ix1 = 0;
        while (etor.MoveNext())
        {
            Assert.Equal(-ix1, etor.Current);
            ++ix1;
        }

        Assert.Equal(n, ix1);
        ((System.Collections.IEnumerator)etor).Reset();
        var ix2 = 0;
        while (etor.MoveNext())
        {
            Assert.Equal(-ix2, etor.Current);
            ++ix2;
        }

        Assert.Equal(n, ix2);
    }
    #endregion
}