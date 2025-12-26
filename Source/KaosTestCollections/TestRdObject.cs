#pragma warning disable CS8602 // Dereference of a possibly null reference.

using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

namespace Kaos.Test.Collections;

public partial class TestRd
{
    #region Test object properties
    [Fact]
    public void UnitRd_odIsFixedSize()
    {
        Setup();
        var od = (IDictionary)dary1;
        Assert.False(od.IsFixedSize);
    }

    [Fact]
    public void UnitRd_odIsReadonly()
    {
        Setup();
        var od = (IDictionary)dary1;
        Assert.False(od.IsReadOnly);
    }

    [Fact]
    public void UnitRd_odIsSynchronized()
    {
        Setup();
        var od = (IDictionary)dary1;
        Assert.False(od.IsSynchronized);
    }

    [Fact]
    public void CrashRd_odItemGet_ArgumentNull()
    {
        Setup();
        var od = (IDictionary)dary2;
        od.Add("foo", 10);
        Assert.Throws<ArgumentNullException>(() =>
        {
            var zz = od[null];
        });
    }

    [Fact]
    public void UnitRd_odItemGetBadKey()
    {
        Setup();
        var od = (IDictionary)dary2;
        od.Add("foo", 10);
        var zz = od[45];
        Assert.Null(zz);
    }

    [Fact]
    public void CrashRd_odItemSetKey_ArgumentNull()
    {
        Setup();
        var od = (IDictionary)dary2;
        od.Add("foo", 10);
        Assert.Throws<ArgumentNullException>(() =>
        {
            od[null] = "bar";
        });
    }

    [Fact]
    public void CrashRd_odItemSetValue_ArgumentNull()
    {
        Setup();
        var od = (IDictionary)dary2;
        od.Add("foo", 10);
        Assert.Throws<ArgumentNullException>(() =>
        {
            od["foo"] = null;
        });
    }

    [Fact]
    public void CrashRd_odItemSetBadKey_Argument()
    {
        Setup();
        var od = (IDictionary)dary2;
        od.Add("foo", 10);
        Assert.Throws<ArgumentException>(() =>
        {
            od[23] = 45;
        });
    }

    [Fact]
    public void CrashRd_odItemSetBadValue_Argument()
    {
        Setup();
        var od = (IDictionary)dary2;
        od.Add("foo", 10);
        Assert.Throws<ArgumentException>(() =>
        {
            od["red"] = "blue";
        });
    }

    [Fact]
    public void UnitRd_odItem()
    {
        Setup();
        var od2 = (IDictionary)dary2;
        var od4 = (IDictionary)dary4;
        var j1 = od2["foo"];
        Assert.Null(j1);
        od2.Add("foo", 10);
        od2.Add("bar", 20);
        od2["raz"] = 30;
        Assert.Equal(3, od2.Count);
        od2["bar"] = 40;
        Assert.Equal(3, od2.Count);
        var j2 = od2["bar"];
        Assert.Equal(40, (int)(j2 ?? 0));
        od4[12] = "twelve";
        od4[13] = null;
        Assert.Equal(2, od4.Count);
    }

    [Fact]
    public void UnitRd_odSyncRoot()
    {
        Setup();
        var od = (IDictionary)dary2;
        Assert.False(od.SyncRoot.GetType().IsValueType);
    }

    #endregion
    #region Test object methods
    [Fact]
    public void CrashRd_odAddNullKey_Argument()
    {
        Setup();
        var od = (IDictionary)dary2;
        Assert.Throws<ArgumentNullException>(() =>
        {
            od.Add(null, 1);
        });
    }

    [Fact]
    public void CrashRd_odAddBadKey_Argument()
    {
        Setup();
        var od = (IDictionary)dary2;
        Assert.Throws<ArgumentException>(() =>
        {
            od.Add(23, 45);
        });
    }

    [Fact]
    public void CrashRd_odAddBadValue_Argument()
    {
        Setup();
        var od = (IDictionary)dary2;
        Assert.Throws<ArgumentException>(() =>
        {
            od.Add("razz", "matazz");
        });
    }

    [Fact]
    public void CrashRd_odAddDupl_Argument()
    {
        Setup();
        var od = (IDictionary)dary2;
        Assert.Throws<ArgumentException>(() =>
        {
            od.Add("nn", 1);
            od.Add("nn", 2);
        });
    }

    [Fact]
    public void UnitRd_odContainsKey()
    {
        Setup();
        var od = (IDictionary)dary1;
        foreach (var key in iVals1)
            od.Add(key, key + 1000);
        Assert.True(od.Contains(iVals1[0]));
        Assert.False(od.Contains(-1));
        Assert.False(od.Contains("foo"));
    }

    [Fact]
    public void CrashRd_odContainsKey_ArgumentNull()
    {
        Setup();
        var od = (IDictionary)dary2;
        Assert.Throws<ArgumentNullException>(() =>
        {
            var isOK = objCol2.Contains(null);
        });
    }

    [Fact]
    public void CrashRd_odCopyTo_ArgumentNull()
    {
        Setup();
        var od = (IDictionary)dary1;
        var target = new KeyValuePair<int, int>[iVals1.Length];
        Assert.Throws<ArgumentNullException>(() =>
        {
            od.CopyTo(null, -1);
        });
    }

    [Fact]
    public void CrashRd_odCopyTo_ArgumentOutOfRange()
    {
        Setup();
        var od = (IDictionary)dary1;
        var target = new KeyValuePair<int, int>[iVals1.Length];
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            od.CopyTo(target, -1);
        });
    }

    [Fact]
    public void CrashRd_odCopyTo1_Argument()
    {
        Setup();
        var od = (IDictionary)dary1;
        var target = new KeyValuePair<int, int>[iVals1.Length, 2];
        Assert.Throws<ArgumentException>(() =>
        {
            od.CopyTo(target, 0);
        });
    }

    [Fact]
    public void CrashRd_odCopyTo2_Argument()
    {
        Setup();
        var od = (IDictionary)dary1;
        for (var key = 1; key < 10; ++key)
            dary1.Add(key, key + 1000);
        var target = new KeyValuePair<int, int>[1];
        Assert.Throws<ArgumentException>(() =>
        {
            od.CopyTo(target, 0);
        });
    }

    [Fact]
    public void CrashRd_odCopyToBadType_Argument()
    {
        Setup();
        var od = (IDictionary)dary1;
        dary1.Add(42, 420);
        var target = new string[5];
        Assert.Throws<ArgumentException>(() =>
        {
            od.CopyTo(target, 0);
        });
    }

    [Fact]
    public void UnitRd_odCopyTo()
    {
        Setup();
        var od = (IDictionary)dary1;
        foreach (var key in iVals1)
            dary1.Add(key, key + 1000);
        var target = new KeyValuePair<int, int>[iVals1.Length];
        od.CopyTo(target, 0);
        for (var i = 0; i < iVals1.Length; ++i)
            Assert.Equal(target[i].Key + 1000, target[i].Value);
    }

    [Fact]
    public void UnitRd_odCopyToDowncast()
    {
        Setup();
        var od = (IDictionary)dary2;
        dary2.Add("aardvark", 1);
        dary2.Add("bonobo", 2);
        var obj = new object[4];
        od.CopyTo(obj, 2);
        var pair = new KeyValuePair<string, int>();
        pair = (KeyValuePair<string, int>)obj[2];
        Assert.Equal("aardvark", pair.Key);
    }

    [Fact]
    public void CrashRd_odRemove_ArgumentNull()
    {
        Setup();
        var od = (IDictionary)dary1;
        Assert.Throws<ArgumentNullException>(() =>
        {
            od.Remove(null);
        });
    }

    [Fact]
    public void UnitRd_odRemove()
    {
        Setup();
        var od = (IDictionary)dary1;
        Assert.Equal(0, od.Count);
        od.Add(17, 170);
        Assert.Equal(1, od.Count);
        od.Remove(18);
        Assert.Equal(1, od.Count);
        od.Remove(17);
        Assert.Equal(0, od.Count);
        objCol1.Remove("ignore wrong type");
    }

    #endregion
    #region Test object enumeration
    [Fact]
    public void CrashRd_odEtorKey_InvalidOperation()
    {
        Setup();
        var od = (IDictionary)dary2;
        dary2.Add("cc", 3);
        var oEtor = od.GetEnumerator();
        Assert.Throws<InvalidOperationException>(() =>
        {
            var key = oEtor.Key;
        });
    }

    [Fact]
    public void CrashRd_odEtorValue_InvalidOperation()
    {
        Setup();
        var od = (IDictionary)dary2;
        dary2.Add("cc", 3);
        var etor = od.GetEnumerator();
        Assert.Throws<InvalidOperationException>(() =>
        {
            var val = etor.Value;
        });
    }

    [Fact]
    public void CrashRd_odEtorEntry_InvalidOperation()
    {
        Setup();
        var od = (IDictionary)dary2;
        dary2.Add("cc", 3);
        var oEtor = od.GetEnumerator();
        Assert.Throws<InvalidOperationException>(() =>
        {
            var entry = oEtor.Entry;
        });
    }

    [Fact]
    public void CrashRd_odEtorCurrent_InvalidOperation()
    {
        Setup();
        var od = (IDictionary)dary2;
        dary2.Add("cc", 3);
        var oEtor = od.GetEnumerator();
        Assert.Throws<InvalidOperationException>(() =>
        {
            var val = oEtor.Current;
        });
    }

    [Fact]
    public void UnitRd_odEtor()
    {
        Setup();
        var od = (IDictionary)dary1;
        dary1.Add(3, 33);
        dary1.Add(5, 55);
        var etor = od.GetEnumerator();
        etor.MoveNext();
        var key = etor.Key;
        var val = etor.Value;
        var de = etor.Entry;
        Assert.Equal(3, key);
        Assert.Equal(33, val);
        Assert.Equal(3, de.Key);
        Assert.Equal(33, de.Value);
    }

    [Fact]
    public void UnitRd_odEtorEntry()
    {
        Setup();
        var od = (IDictionary)dary1;
        foreach (var k in iVals1)
            dary1.Add(k, k + 1000);
        var actualCount = 0;
        foreach (DictionaryEntry de in od)
        {
            Assert.Equal((int)de.Key + 1000, de.Value);
            ++actualCount;
        }

        Assert.Equal(iVals1.Length, actualCount);
    }

    #endregion
    #region Test object Keys
    [Fact]
    public void UnitRdk_ocCount()
    {
        Setup();
        var oc = (ICollection)dary1.Keys;
        var n = 10;
        Assert.Equal(0, oc.Count);
        for (var i = 0; i < n; ++i)
            dary1.Add(i + 100, i + 1000);
        Assert.Equal(n, oc.Count);
    }

    [Fact]
    public void UnitRdk_ocIsSynchronized()
    {
        Setup();
        var oc = (ICollection)dary1.Keys;
        Assert.False(oc.IsSynchronized);
    }

    [Fact]
    public void CrashRdk_ocCopyTo_ArgumentNull()
    {
        Setup();
        var oc = (ICollection)dary1.Keys;
        Assert.Throws<ArgumentNullException>(() =>
        {
            oc.CopyTo(null, -1);
        });
    }

    [Fact]
    public void CrashRdk_ocCopyToMultiDimensional_Argument()
    {
        Setup();
        var oc = (ICollection)dary1.Keys;
        dary1.Add(42, 420);
        var target = new object[2, 3];
        Assert.Throws<ArgumentException>(() =>
        {
            oc.CopyTo(target, -1);
        });
    }

    [Fact]
    public void CrashRdk_ocCopyTo_ArgumentOutOfRange()
    {
        Setup();
        var oc = (ICollection)dary1.Keys;
        dary1.Add(42, 420);
        var target = new object[1];
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            oc.CopyTo(target, -1);
        });
    }

    [Fact]
    public void CrashRdk_ocCopyToNotLongEnough_Argument()
    {
        Setup();
        var oc = (ICollection)dary1.Keys;
        for (var i = 0; i < 10; ++i)
            dary1.Add(i + 100, i + 1000);
        var target = new object[10];
        Assert.Throws<ArgumentException>(() =>
        {
            oc.CopyTo(target, 5);
        });
    }

    [Fact]
    public void UnitRdk_ocCopyTo()
    {
        Setup();
        var oc = (ICollection)dary1.Keys;
        var n = 10;
        for (var i = 0; i < n; ++i)
            dary1.Add(i + 100, i + 1000);
        var target = new object[n];
        oc.CopyTo(target, 0);
        for (var i = 0; i < n; ++i)
            Assert.Equal(i + 100, (int)target[i]);
    }

    [Fact]
    public void UnitRdk_odEtor()
    {
        Setup();
        var od = (IDictionary)dary1;
        var n = 10;
        for (var k = 0; k < n; ++k)
            dary1.Add(k, k + 1000);
        var expected = 0;
        foreach (var j in od.Keys)
        {
            Assert.Equal(expected, (int)j);
            ++expected;
        }
    }

    #endregion
    #region Test object Values
    [Fact]
    public void UnitRdv_ocValuesCount()
    {
        Setup();
        var oc = (ICollection)dary1.Values;
        var n = 10;
        Assert.Equal(0, oc.Count);
        for (var i = 0; i < n; ++i)
            dary1.Add(i + 100, i + 1000);
        Assert.Equal(n, oc.Count);
    }

    [Fact]
    public void UnitRdv_ocIsSynchronized()
    {
        Setup();
        var oc = (ICollection)dary1.Values;
        Assert.False(oc.IsSynchronized);
    }

    [Fact]
    public void CrashRdv_ocCopyTo_ArgumentNull()
    {
        Setup();
        var oc = (ICollection)dary1.Values;
        Assert.Throws<ArgumentNullException>(() =>
        {
            oc.CopyTo(null, -1);
        });
    }

    [Fact]
    public void CrashRdv_ocCopyToMultiDimensional_Argument()
    {
        Setup();
        var oc = (ICollection)dary1.Values;
        dary1.Add(42, 420);
        var target = new object[2, 3];
        Assert.Throws<ArgumentException>(() =>
        {
            oc.CopyTo(target, -1);
        });
    }

    [Fact]
    public void CrashRdv_ocCopyTo_ArgumentOutOfRange()
    {
        Setup();
        var oc = (ICollection)dary1.Values;
        dary1.Add(42, 420);
        var target = new object[1];
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            oc.CopyTo(target, -1);
        });
    }

    [Fact]
    public void CrashRdv_ocCopyToNotLongEnough_Argument()
    {
        Setup();
        var oc = (ICollection)dary1.Values;
        for (var i = 0; i < 10; ++i)
            dary1.Add(i + 100, i + 1000);
        var target = new object[10];
        Assert.Throws<ArgumentException>(() =>
        {
            oc.CopyTo(target, 5);
        });
    }

    [Fact]
    public void UnitRdv_ocCopyTo()
    {
        Setup();
        var oc = (ICollection)dary1.Values;
        var n = 10;
        for (var i = 0; i < n; ++i)
            dary1.Add(i + 100, i + 1000);
        var target = new object[n];
        oc.CopyTo(target, 0);
        for (var i = 0; i < n; ++i)
            Assert.Equal(i + 1000, (int)target[i]);
    }

    [Fact]
    public void UnitRdv_ocGetEnumerator()
    {
        Setup();
        var od = (IDictionary)dary1;
        var n = 10;
        for (var k = 0; k < n; ++k)
            dary1.Add(k, k + 1000);
        var expected = 1000;
        foreach (var j in od.Values)
        {
            Assert.Equal(expected, (int)j);
            ++expected;
        }
    }
    #endregion
}