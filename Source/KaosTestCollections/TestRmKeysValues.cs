//
// Library: KaosCollections
// File:    TestRmKeysValues.cs
//

#if !TEST_BCL
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using Kaos.Collections;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace Kaos.Test.Collections;

public partial class TestRm
{
    #region Test Keys constructor
    [Fact]
    public void CrashRmk_Ctor_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var kc = new RankedMap<int, int>.KeyCollection(null);
        });
    }

    [Fact]
    public void UnitRmk_Ctor()
    {
        var rm = new RankedMap<int, int>
        {
            {
                1,
                -1
            }
        };
        var kc = new RankedMap<int, int>.KeyCollection(rm);
        Assert.Equal(1, kc.Count);
    }

    #endregion
    #region Test Keys properties
    [Fact]
    public void UnitRmk_Item()
    {
        var rm = new RankedMap<string, int>
        {
            {
                "0zero",
                0
            },
            {
                "1one",
                -1
            }
        };
        Assert.Equal("0zero", rm.Keys[0]);
        Assert.Equal("1one", rm.Keys[1]);
    }

    [Fact]
    public void UnitRmk_gcIsReadonly()
    {
        var rm = new RankedMap<int, int>();
        var gc = (ICollection<int>)rm.Keys;
        Assert.True(gc.IsReadOnly);
    }

    [Fact]
    public void UnitRmk_gcIsSynchronized()
    {
        var rm = new RankedMap<int, int>();
        var oc = (ICollection)rm.Keys;
        Assert.False(oc.IsSynchronized);
    }

    [Fact]
    public void UnitRmk_ocSyncRoot()
    {
        var rm = new RankedMap<int, int>();
        var oc = (ICollection)rm.Keys;
        Assert.False(oc.SyncRoot.GetType().IsValueType);
    }

    #endregion
    #region Test Keys methods
    [Fact]
    public void CrashRmk_gcAdd_NotSupported()
    {
        Assert.Throws<NotSupportedException>(() =>
        {
            var rm = new RankedMap<string, int>();
            var gc = (ICollection<string>)rm.Keys;
            gc.Add("omega");
        });
    }

    [Fact]
    public void CrashRmk_gcClear_NotSupported()
    {
        Assert.Throws<NotSupportedException>(() =>
        {
            var rm = new RankedMap<int, int>();
            var gc = (ICollection<int>)rm.Keys;
            gc.Clear();
        });
    }

    [Fact]
    public void CrashRmk_gcContains_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var rm = new RankedMap<string, int>
            {
                {
                    "zed",
                    26
                }
            };
            var gc = (ICollection<string>)rm.Keys;
            var zz = gc.Contains(null);
        });
    }

    [Fact]
    public void UnitRmk_gcContains()
    {
        var rm = new RankedMap<string, int>();
        var gc = (ICollection<string>)rm.Keys;
        rm.Add("alpha", 10);
        rm.Add("delta", 40);
        Assert.True(gc.Contains("delta"));
        Assert.False(gc.Contains("zed"));
    }

    [Fact]
    public void CrashRmk_CopyTo_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var rm = new RankedMap<int, int>();
            rm.Keys.CopyTo(null, -1);
        });
    }

    [Fact]
    public void CrashRmk_CopyTo_ArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var rm = new RankedMap<int, int>
            {
                {
                    1,
                    11
                }
            };
            var target = new int[1];
            rm.Keys.CopyTo(target, -1);
        });
    }

    [Fact]
    public void CrashRmk_CopyTo_Argument()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var rm = new RankedMap<int, int>();
            var target = new int[4];
            for (var key = 1; key < 10; ++key)
                rm.Add(key, key + 1000);
            rm.Keys.CopyTo(target, 2);
        });
    }

    [Fact]
    public void UnitRmk_CopyTo()
    {
        var rm = new RankedMap<int, int>();
        int n = 10, offset = 5;
        for (var k = 0; k < n; ++k)
            rm.Add(k, k + 1000);
        var target = new int[n + offset];
        rm.Keys.CopyTo(target, offset);
        for (var k = 0; k < n; ++k)
            Assert.Equal(k, target[k + offset]);
    }

    [Fact]
    public void UnitRmk_gcCopyTo()
    {
        var rm = new RankedMap<string, int>();
        var gc = (ICollection<string>)rm.Keys;
        rm.Add("alpha", 1);
        rm.Add("beta", 2);
        rm.Add("gamma", 3);
        var target = new string[rm.Count];
        gc.CopyTo(target, 0);
        Assert.Equal("alpha", target[0]);
        Assert.Equal("beta", target[1]);
        Assert.Equal("gamma", target[2]);
    }

    [Fact]
    public void UnitRmk_ocCopyTo()
    {
        var rm = new RankedMap<char, int>
        {
            {
                'a',
                1
            },
            {
                'b',
                2
            },
            {
                'z',
                26
            }
        };
        var oc = (ICollection)rm.Keys;
        var target = new char[4];
        oc.CopyTo(target, 1);
        Assert.Equal('a', target[1]);
        Assert.Equal('b', target[2]);
        Assert.Equal('z', target[3]);
    }

    [Fact]
    public void UnitRmk_GetCount()
    {
        var rm = new RankedMap<int, int>
        {
            Capacity = 4
        };
        foreach (var ii in new[]
                 {
                     1,
                     3,
                     5,
                     5,
                     5,
                     7
                 }

                )
            rm.Add(ii, -ii);
        Assert.Equal(0, rm.Keys.GetCount(0));
        Assert.Equal(1, rm.Keys.GetCount(3));
        Assert.Equal(3, rm.Keys.GetCount(5));
        Assert.Equal(1, rm.Keys.GetCount(7));
        Assert.Equal(0, rm.Keys.GetCount(9));
    }

    [Fact]
    public void UnitRmk_GetDistinctCount()
    {
        var rm0 = new RankedMap<int, int>();
        var rm = new RankedMap<int, int>
        {
            Capacity = 4
        };
        foreach (var ii in new[]
                 {
                     3,
                     5,
                     5,
                     5,
                     7
                 }

                )
            rm.Add(ii, -ii);
        Assert.Equal(0, rm0.Keys.GetDistinctCount());
        Assert.Equal(3, rm.Keys.GetDistinctCount());
    }

    [Fact]
    public void UnitRmk_IndexOf()
    {
        var rm = new RankedMap<string, int>
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
                "1one",
                -2
            }
        };
        var pc = (ICollection<KeyValuePair<string, int>>)rm;
        pc.Add(new KeyValuePair<string, int>(null, -1));
        Assert.Equal(0, rm.Keys.IndexOf(null));
        Assert.Equal(1, rm.Keys.IndexOf("0zero"));
        Assert.Equal(2, rm.Keys.IndexOf("1one"));
        Assert.Equal(~1, rm.Keys.IndexOf("00"));
        Assert.Equal(~2, rm.Keys.IndexOf("11"));
    }

    [Fact]
    public void CrashRmk_gcRemove_NotSupported()
    {
        Assert.Throws<NotSupportedException>(() =>
        {
            var rm = new RankedMap<string, int>();
            var gc = (ICollection<string>)rm.Keys;
            gc.Remove("omega");
        });
    }

    [Fact]
    public void UnitRmk_TryGetGEGT()
    {
        var rm = new RankedMap<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            Capacity = 4
        };
        for (var ci = 'b'; ci <= 'y'; ++ci)
        {
            rm.Add(ci.ToString().ToUpper(), 'a' - ci);
            rm.Add(ci.ToString(), ci - 'a');
        }

        var r0a = rm.Keys.TryGetGreaterThan("a", out var k0a);
        Assert.True(r0a);
        Assert.Equal("B", k0a);
        var r0b = rm.Keys.TryGetGreaterThanOrEqual("a", out var k0b);
        Assert.True(r0b);
        Assert.Equal("B", k0b);
        var r1 = rm.Keys.TryGetGreaterThan("B", out var k1);
        Assert.True(r1);
        Assert.Equal("C", k1);
        var r2 = rm.Keys.TryGetGreaterThanOrEqual("b", out var k2);
        Assert.True(r2);
        Assert.Equal("B", k2);
        var r3 = rm.Keys.TryGetGreaterThanOrEqual("a", out var k3);
        Assert.True(r3);
        Assert.Equal("B", k3);
        var r9a = rm.Keys.TryGetGreaterThan("y", out var k9a);
        Assert.False(r9a);
        Assert.Equal(default(string), k9a);
        var r9b = rm.Keys.TryGetGreaterThan("z", out var k9b);
        Assert.False(r9a);
        Assert.Equal(default(string), k9b);
        var r9c = rm.Keys.TryGetGreaterThanOrEqual("z", out var k9c);
        Assert.False(r9c);
        Assert.Equal(default(string), k9c);
    }

    [Fact]
    public void UnitRmkx_TryGetLELT()
    {
        var rm = new RankedMap<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            Capacity = 4
        };
        for (var ci = 'b'; ci <= 'y'; ++ci)
        {
            rm.Add(ci.ToString().ToUpper(), 'a' - ci);
            rm.Add(ci.ToString(), ci - 'a');
        }

        var r0a = rm.Keys.TryGetLessThan("B", out var k0a);
        Assert.False(r0a);
        Assert.Equal(default(string), k0a);
        var r0b = rm.Keys.TryGetLessThanOrEqual("A", out var k0b);
        Assert.False(r0b);
        Assert.Equal(default(string), k0b);
        var r1 = rm.Keys.TryGetLessThan("C", out var k1);
        Assert.True(r1);
        Assert.Equal("b", k1);
        var r2 = rm.Keys.TryGetLessThanOrEqual("C", out var k2);
        Assert.True(r2);
        Assert.Equal("C", k2);
        var r3 = rm.Keys.TryGetLessThanOrEqual("d", out var k3);
        Assert.True(r3);
        Assert.Equal("D", k3);
    }

    #endregion
    #region Test Keys enumeration
    [Fact]
    public void UnitRmk_gcEtor()
    {
        var rm = new RankedMap<int, int>
        {
            Capacity = 4
        };
        var n = 100;
        for (var k = 0; k < n; ++k)
            rm.Add(k, k + 1000);
        var actualCount = 0;
        foreach (var key in rm.Keys)
        {
            Assert.Equal(actualCount, key);
            ++actualCount;
        }

        Assert.Equal(n, actualCount);
    }

    [Fact]
    public void UnitRmk_gcGetEnumerator()
    {
        var rm = new RankedMap<string, int>
        {
            Capacity = 4
        };
        var gc = (ICollection<string>)rm.Keys;
        var n = 10;
        for (var k = 0; k < n; ++k)
            rm.Add(k.ToString(), k);
        var expected = 0;
        var etor = gc.GetEnumerator();
        var rewoundKey = etor.Current;
        Assert.Equal(rewoundKey, default(string));
        while (etor.MoveNext())
        {
            var key = etor.Current;
            Assert.Equal(expected.ToString(), key);
            Assert.Equal(expected.ToString(), (string?)((IEnumerator)etor).Current);
            ++expected;
        }

        Assert.Equal(n, expected);
    }

    [Fact]
    public void CrashRmk_EtorHotUpdate()
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
            foreach (var kv in rm.Keys)
                if (++n == 2)
                    rm.Remove("vv");
        });
    }

    [Fact]
    public void UnitRmk_EtorCurrentHotUpdate()
    {
        var rm1 = new RankedMap<int, int>
        {
            {
                3,
                -3
            }
        };
        var etor1 = rm1.Keys.GetEnumerator();
        Assert.Equal(default(int), etor1.Current);
        var ok1 = etor1.MoveNext();
        Assert.Equal(3, etor1.Current);
        rm1.Remove(3);
        Assert.Equal(3, etor1.Current);
        var rm2 = new RankedMap<string, int>
        {
            {
                "CC",
                3
            }
        };
        var etor2 = rm2.Keys.GetEnumerator();
        Assert.Equal(default(string), etor2.Current);
        var ok2 = etor2.MoveNext();
        Assert.Equal("CC", etor2.Current);
        rm2.Clear();
        Assert.Equal("CC", etor2.Current);
    }

    #endregion
    #region Test Keys object enumeration
    [Fact]
    public void CrashRmk_ocCurrent_InvalidOperation()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            var rm = new RankedMap<char, int>
            {
                {
                    'C',
                    3
                }
            };
            var oc = (ICollection)rm.Keys;
            var etor = oc.GetEnumerator();
            var cur = etor.Current;
        });
    }

    [Fact]
    public void UnitRmk_ocEtor()
    {
        var rm = new RankedMap<char, int>
        {
            {
                'a',
                1
            },
            {
                'b',
                2
            },
            {
                'c',
                3
            }
        };
        var oc = (ICollection)rm.Keys;
        var actual = 0;
        foreach (var oItem in oc)
        {
            Assert.Equal(rm.ElementAt(actual).Key, (char)oItem);
            ++actual;
        }

        Assert.Equal(rm.Count, actual);
    }

    [Fact]
    public void UnitRmk_ocCurrent_HotUpdate()
    {
        var rm = new RankedMap<char, int>
        {
            {
                'c',
                3
            }
        };
        System.Collections.ICollection oc = rm.Keys;
        var etor = oc.GetEnumerator();
        var ok = etor.MoveNext();
        Assert.Equal('c', etor.Current);
        rm.Clear();
        Assert.Equal('c', etor.Current);
    }

    [Fact]
    public void UnitRmk_oReset()
    {
        var rm = new RankedMap<int, int>
        {
            Capacity = 4
        };
        var n = 7;
        for (var ix = 0; ix < n; ++ix)
            rm.Add(ix * 10, -ix);
        var etor = rm.Keys.GetEnumerator();
        var ix1 = 0;
        while (etor.MoveNext())
        {
            Assert.Equal(ix1 * 10, etor.Current);
            ++ix1;
        }

        Assert.Equal(n, ix1);
        ((System.Collections.IEnumerator)etor).Reset();
        var ix2 = 0;
        while (etor.MoveNext())
        {
            Assert.Equal(ix2 * 10, etor.Current);
            ++ix2;
        }

        Assert.Equal(n, ix2);
    }

    #endregion
    #region Test Values constructor
    [Fact]
    public void CrashRmv_Ctor_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var vc = new RankedMap<int, int>.ValueCollection(null);
        });
    }

    [Fact]
    public void UnitRmv_Ctor()
    {
        var rm = new RankedMap<int, int>
        {
            {
                1,
                -1
            }
        };
        var vc = new RankedMap<int, int>.ValueCollection(rm);
        Assert.Equal(1, vc.Count);
    }

    #endregion
    #region Test Values properties
    [Fact]
    public void UnitRmv_Item()
    {
        var rm = new RankedMap<string, int>
        {
            {
                "0zero",
                0
            },
            {
                "1one",
                -1
            }
        };
        Assert.Equal(0, rm.Values[0]);
        Assert.Equal(-1, rm.Values[1]);
    }

    [Fact]
    public void UnitRmv_gcIsReadonly()
    {
        var rm = new RankedMap<int, int>();
        var gc = (ICollection<int>)rm.Values;
        Assert.True(gc.IsReadOnly);
    }

    [Fact]
    public void UnitRmv_gcIsSynchronized()
    {
        var rm = new RankedMap<int, int>();
        var oc = (ICollection)rm.Values;
        Assert.False(oc.IsSynchronized);
    }

    [Fact]
    public void UnitRmv_ocSyncRoot()
    {
        var rm = new RankedMap<int, int>();
        var oc = (ICollection)rm.Values;
        Assert.False(oc.SyncRoot.GetType().IsValueType);
    }

    #endregion
    #region Test Values methods
    [Fact]
    public void CrashRmv_gcAdd_NotSupported()
    {
        Assert.Throws<NotSupportedException>(() =>
        {
            var rm = new RankedMap<string, int>();
            var gc = (ICollection<int>)rm.Values;
            gc.Add(9);
        });
    }

    [Fact]
    public void CrashRmv_gcClear_NotSupported()
    {
        Assert.Throws<NotSupportedException>(() =>
        {
            var rm = new RankedMap<int, int>();
            var gc = (ICollection<int>)rm.Values;
            gc.Clear();
        });
    }

    [Fact]
    public void UnitRmv_gcContains()
    {
        var rm = new RankedMap<string, int>();
        var gc = (ICollection<int>)rm.Values;
        rm.Add("alpha", 10);
        rm.Add("beta", 20);
        Assert.True(gc.Contains(20));
        Assert.False(gc.Contains(-9));
    }

    [Fact]
    public void CrashRmv_CopyTo_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var rm = new RankedMap<int, int>();
            rm.Values.CopyTo(null, -1);
        });
    }

    [Fact]
    public void CrashRmv_CopyTo_ArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var rm = new RankedMap<int, int>()
            {
                {
                    1,
                    11
                }
            };
            var target = new int[1];
            rm.Values.CopyTo(target, -1);
        });
    }

    [Fact]
    public void CrashRmv_CopyTo_Argument()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var rm = new RankedMap<int, int>();
            var target = new int[4];
            for (var key = 1; key < 10; ++key)
                rm.Add(key, key + 1000);
            rm.Values.CopyTo(target, 2);
        });
    }

    [Fact]
    public void UnitRmv_CopyTo()
    {
        var rm = new RankedMap<int, int>();
        int n = 10, offset = 5;
        for (var k = 0; k < n; ++k)
            rm.Add(k, -k);
        var target = new int[n + offset];
        rm.Values.CopyTo(target, offset);
        for (var k = 0; k < n; ++k)
            Assert.Equal(k, -target[k + offset]);
    }

    [Fact]
    public void UnitRmv_gcCopyTo()
    {
        var rm = new RankedMap<string, int>();
        var gc = (ICollection<int>)rm.Values;
        rm.Add("alpha", 1);
        rm.Add("beta", 2);
        rm.Add("gamma", 3);
        var target = new int[rm.Count];
        gc.CopyTo(target, 0);
        Assert.Equal(1, target[0]);
        Assert.Equal(2, target[1]);
        Assert.Equal(3, target[2]);
    }

    [Fact]
    public void CrashRmv_ocCopyTo_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var rm = new RankedMap<int, int>();
            var oc = (ICollection)rm.Values;
            oc.CopyTo(null, 0);
        });
    }

    [Fact]
    public void CrashRmv_ocCopyToMultiDimensional_Argument()
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
            var oc = (ICollection)rm.Values;
            var target = new object[2, 3];
            oc.CopyTo(target, -1);
        });
    }

    [Fact]
    public void CrashRmv_ocCopyTo_ArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var rm = new RankedMap<int, int>
            {
                {
                    42,
                    420
                }
            };
            var oc = (ICollection)rm.Values;
            var target = new object[1];
            oc.CopyTo(target, -1);
        });
    }

    [Fact]
    public void CrashRmv_ocCopyToNotLongEnough_Argument()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var rm = new RankedMap<int, int>
            {
                Capacity = 4
            };
            var oc = (ICollection)rm.Values;
            for (var i = 0; i < 10; ++i)
                rm.Add(i + 100, i + 1000);
            var target = new object[10];
            oc.CopyTo(target, 5);
        });
    }

    [Fact]
    public void UnitRmv_ocCopyToA()
    {
        var rm = new RankedMap<int, int>
        {
            Capacity = 4
        };
        var oc = (ICollection)rm.Values;
        var n = 10;
        for (var i = 0; i < n; ++i)
            rm.Add(i + 100, i + 1000);
        var target = new object[n];
        oc.CopyTo(target, 0);
        for (var i = 0; i < n; ++i)
            Assert.Equal(i + 1000, (int)target[i]);
    }

    [Fact]
    public void UnitRmv_ocCopyToB()
    {
        var rm = new RankedMap<char, int>()
        {
            {
                'a',
                1
            },
            {
                'b',
                2
            },
            {
                'z',
                26
            }
        };
        var oc = (ICollection)rm.Values;
        var target = new int[4];
        oc.CopyTo(target, 1);
        Assert.Equal(1, target[1]);
        Assert.Equal(2, target[2]);
        Assert.Equal(26, target[3]);
    }

    [Fact]
    public void UnitRmv_IndexOf()
    {
        var rm = new RankedMap<string, int?>
        {
            {
                "1one",
                1
            },
            {
                "2two",
                2
            },
            {
                "2two",
                2
            },
            {
                "3tree",
                3
            },
            {
                "9nine",
                null
            }
        };
        Assert.Equal(0, rm.Values.IndexOf(1));
        Assert.Equal(1, rm.Values.IndexOf(2));
        Assert.Equal(3, rm.Values.IndexOf(3));
        Assert.Equal(4, rm.Values.IndexOf(null));
        Assert.Equal(-1, rm.Values.IndexOf(0));
        Assert.Equal(-1, rm.Values.IndexOf(4));
    }

    [Fact]
    public void CrashRmv_gcRemove_NotSupported()
    {
        Assert.Throws<NotSupportedException>(() =>
        {
            var rm = new RankedMap<int, int>();
            var gc = (ICollection<int>)rm.Values;
            gc.Remove(9);
        });
    }

    #endregion
    #region Test Values generic enumeration
    [Fact]
    public void UnitRmv_GetEnumerator()
    {
        var rm = new RankedMap<int, int>
        {
            Capacity = 4
        };
        var n = 100;
        for (var k = 0; k < n; ++k)
            rm.Add(k, k + 1000);
        var actualCount = 0;
        foreach (var val in rm.Values)
        {
            Assert.Equal(actualCount + 1000, val);
            ++actualCount;
        }

        Assert.Equal(n, actualCount);
    }

    [Fact]
    public void UnitRmv_gcGetEnumerator()
    {
        var rm = new RankedMap<string, int?>
        {
            Capacity = 4
        };
        var gc = (ICollection<int?>)rm.Values;
        var n = 10;
        for (var k = 0; k < n; ++k)
            rm.Add(k.ToString(), k + 1000);
        var expected = 0;
        var etor = gc.GetEnumerator();
        var rewoundKey = etor.Current;
        Assert.Equal(rewoundKey, default(int? ));
        while (etor.MoveNext())
        {
            var val = etor.Current;
            Assert.Equal(expected + 1000, val);
            ++expected;
        }

        Assert.Equal(n, expected);
    }

    [Fact]
    public void CrashRmv_EtorHotUpdate()
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
            foreach (var kv in rm.Values)
                if (++n == 2)
                    rm.Remove("vv");
        });
    }

    [Fact]
    public void UnitRmv_EtorCurrentHotUpdate()
    {
        var rm1 = new RankedMap<int, int>
        {
            {
                3,
                -3
            }
        };
        var etor1 = rm1.Values.GetEnumerator();
        Assert.Equal(default(int), etor1.Current);
        var ok1 = etor1.MoveNext();
        Assert.Equal(-3, etor1.Current);
        rm1.Remove(3);
        Assert.Equal(-3, etor1.Current);
        var rm2 = new RankedMap<string, int>
        {
            {
                "CC",
                3
            }
        };
        var etor2 = rm2.Values.GetEnumerator();
        Assert.Equal(default(int), etor2.Current);
        var ok2 = etor2.MoveNext();
        Assert.Equal(3, etor2.Current);
        rm2.Clear();
        Assert.Equal(3, etor2.Current);
    }

    [Fact]
    public void CrashRmv_ocCurrent_InvalidOperation()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            var rm = new RankedMap<char, int>
            {
                {
                    'C',
                    3
                }
            };
            var oc = (ICollection)rm.Values;
            var etor = oc.GetEnumerator();
            var cur = etor.Current;
        });
    }

    #endregion
    #region Test Values object enumeration
    [Fact]
    public void UnitRmv_ocEtor()
    {
        var rm = new RankedMap<char, int>
        {
            {
                'a',
                1
            },
            {
                'b',
                2
            },
            {
                'c',
                3
            }
        };
        var oc = (ICollection)rm.Values;
        var ix = 0;
        foreach (var oItem in oc)
        {
            Assert.Equal(ix + 1, (int)oItem);
            ++ix;
        }

        Assert.Equal(rm.Count, ix);
    }

    [Fact]
    public void UnitRmv_ocCurrent_HotUpdate()
    {
        var rm = new RankedMap<char, int>
        {
            {
                'c',
                3
            }
        };
        System.Collections.ICollection oc = rm.Values;
        var etor = oc.GetEnumerator();
        var ok = etor.MoveNext();
        Assert.Equal(3, etor.Current);
        rm.Clear();
        Assert.Equal(3, etor.Current);
    }

    [Fact]
    public void UnitRmv_oReset()
    {
        var rm = new RankedMap<int, int>
        {
            Capacity = 4
        };
        var n = 7;
        for (var ix = 0; ix < n; ++ix)
            rm.Add(ix, -ix);
        var etor = rm.Values.GetEnumerator();
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
#endif
