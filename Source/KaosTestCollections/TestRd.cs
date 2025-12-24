//
// Library: KaosCollections
// File:    TestRd.cs
//

using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
#if !TEST_BCL
using Kaos.Collections;

#endif

namespace Kaos.Test.Collections
{
    public partial class TestRd : TestBtree, IClassFixture<BinaryFormatterEnableFixture>
    {
        private readonly ITestOutputHelper _output;

        public TestRd(ITestOutputHelper output)
        {
            _output = output;
        }

#region Test constructors
        [Fact]
        public void UnitRd_Inheritance()
        {
            Setup();
            Assert.True(dary2 is System.Collections.Generic.IDictionary<string, int>);
            Assert.True(dary2 is System.Collections.Generic.ICollection<KeyValuePair<string, int>>);
            Assert.True(dary2 is System.Collections.Generic.IEnumerable<KeyValuePair<string, int>>);
            Assert.True(dary2 is System.Collections.IEnumerable);
            Assert.True(dary2 is System.Collections.IDictionary);
            Assert.True(dary2 is System.Collections.ICollection);
            Assert.True(dary2 is System.Collections.Generic.IReadOnlyDictionary<string, int>);
            Assert.True(dary2 is System.Collections.Generic.IReadOnlyCollection<KeyValuePair<string, int>>);
            Assert.True(dary2.Keys is System.Collections.Generic.ICollection<string>);
            Assert.True(dary2.Keys is System.Collections.Generic.IEnumerable<string>);
            Assert.True(dary2.Keys is System.Collections.IEnumerable);
            Assert.True(dary2.Keys is System.Collections.ICollection);
            Assert.True(dary2.Keys is System.Collections.Generic.IReadOnlyCollection<string>);
            Assert.True(dary2.Values is System.Collections.Generic.ICollection<int>);
            Assert.True(dary2.Values is System.Collections.Generic.IEnumerable<int>);
            Assert.True(dary2.Values is System.Collections.IEnumerable);
            Assert.True(dary2.Values is System.Collections.ICollection);
            Assert.True(dary2.Values is System.Collections.Generic.IReadOnlyCollection<int>);
            Setup();
            System.Collections.IEnumerable roKeys = ((System.Collections.Generic.IReadOnlyDictionary<string, int>)dary2).Keys;
            System.Collections.IEnumerable roVals = ((System.Collections.Generic.IReadOnlyDictionary<string, int>)dary2).Values;
        }

#if TEST_BCL
        public class DerivedD : SortedDictionary<int,int> { }
#else
        public class DerivedD : RankedDictionary<int, int>, IClassFixture<BinaryFormatterEnableFixture>
        {
        }

#endif
        [Fact]
        public void UnitRd_CtorSubclass()
        {
            var sub = new DerivedD();
            bool isRO = ((System.Collections.Generic.IDictionary<int, int>)sub).IsReadOnly;
            Assert.False(isRO);
        }

        [Fact]
        public void UnitRd_Ctor0Empty()
        {
            Setup();
            Assert.Equal(0, dary1.Count);
        }

        [Fact]
        public void CrashRd_Ctor1NoComparer_InvalidOperation()
        {
#if TEST_BCL
            Assert.Throws<ArgumentException>(() =>
            {
                var comp0 = (System.Collections.Generic.Comparer<Person>) null;
                var d1 = new SortedDictionary<Person,int> (comp0);
                d1.Add (new Person ("Zed"), 1);
                d1.Add (new Person ("Macron"), 2);
            });
#else
            Assert.Throws<InvalidOperationException>(() =>
            {
                var comp0 = (System.Collections.Generic.Comparer<Person>)null;
                var d1 = new RankedDictionary<Person, int>(comp0);
                d1.Add(new Person("Zed"), 1);
                d1.Add(new Person("Macron"), 2);
            });
#endif
        }

        [Fact]
        public void UnitRd_Ctor1A1()
        {
#if TEST_BCL
            var tree = new SortedDictionary<string,int> (StringComparer.OrdinalIgnoreCase);
#else
            var tree = new RankedDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
#endif
            tree.Add("AAA", 0);
            tree.Add("bbb", 1);
            tree.Add("CCC", 2);
            tree.Add("ddd", 3);
            int actualPosition = 0;
            foreach (KeyValuePair<string, int> pair in tree)
            {
                Assert.Equal(actualPosition, pair.Value);
                ++actualPosition;
            }

            Assert.Equal(4, actualPosition);
        }

        [Fact]
        public void UnitRd_Ctor1A2()
        {
#if TEST_BCL
            var tree = new SortedDictionary<string,int> (StringComparer.Ordinal);
#else
            var tree = new RankedDictionary<string, int>(StringComparer.Ordinal);
#endif
            tree.Add("AAA", 0);
            tree.Add("bbb", 2);
            tree.Add("CCC", 1);
            tree.Add("ddd", 3);
            int actualPosition = 0;
            foreach (KeyValuePair<string, int> pair in tree)
            {
                Assert.Equal(actualPosition, pair.Value);
                ++actualPosition;
            }

            Assert.Equal(4, actualPosition);
        }

        [Fact]
        public void CrashRd_Ctor1B_ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                IDictionary<int, int> listArg = null;
#if TEST_BCL
                IDictionary<int,int> gcp = new SortedDictionary<int,int> (listArg);
#else
                IDictionary<int, int> gcp = new RankedDictionary<int, int>(listArg);
#endif
            });
        }

        [Fact]
        public void UnitRd_Ctor1B()
        {
            Setup();
            IDictionary<string, int> sl = new SortedList<string, int>();
            sl.Add("Gremlin", 1);
            sl.Add("Pacer", 2);
#if TEST_BCL
            var dary = new SortedDictionary<string,int> (sl);
#else
            var dary = new RankedDictionary<string, int>(sl);
#endif
            Assert.Equal(1, dary["Gremlin"]);
            Assert.Equal(2, dary["Pacer"]);
        }

        [Fact]
        public void UnitRd_Ctor2()
        {
            IDictionary<Person, int> empDary = new SortedDictionary<Person, int>(new PersonComparer());
            empDary.Add(new KeyValuePair<Person, int>(new Person("fay"), 1));
            empDary.Add(new KeyValuePair<Person, int>(new Person("ann"), 2));
            empDary.Add(new KeyValuePair<Person, int>(new Person("sam"), 3));
#if TEST_BCL
            var people = new SortedDictionary<Person,int> (empDary, new PersonComparer());
#else
            var people = new RankedDictionary<Person, int>(empDary, new PersonComparer());
#endif
            Assert.Equal(3, people.Count);
        }

#endregion
#region Test properties
        [Fact]
        public void UnitRd_Comparer()
        {
            Setup();
            IComparer<string> result = dary2.Comparer;
            Assert.Equal(Comparer<string>.Default, result);
        }

        [Fact]
        public void UnitRd_Count()
        {
            Setup();
            for (int i = 0; i < iVals1.Length; ++i)
            {
                Assert.Equal(i, dary1.Count);
                dary1.Add(iVals1[i], iVals1[i] * 10);
            }

            Assert.Equal(iVals1.Length, dary1.Count);
            for (int i = 0; i < iVals1.Length; ++i)
            {
                dary1.Remove(iVals1[i]);
                Assert.Equal(iVals1.Length - i - 1, dary1.Count);
            }
        }

        [Fact]
        public void CrashRd_Item_ArgumentNullA()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                Setup();
                dary2[null !] = 42;
            });
        }

        [Fact]
        public void CrashRd_Item_ArgumentNullB()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                Setup();
                int zz = dary2[null !];
            });
        }

        [Fact]
        public void CrashRd_Item_KeyNotFoundA()
        {
            Assert.Throws<KeyNotFoundException>(() =>
            {
                Setup();
                dary2.Add("pi", 9);
                int zz = dary2["omicron"];
            });
        }

        [Fact]
        public void CrashRd_Item_KeyNotFoundB()
        {
            Assert.Throws<KeyNotFoundException>(() =>
            {
                Setup();
                dary1.Add(23, 230);
                int zz = dary1[9];
            });
        }

        [Fact]
        public void UnitRd_Item()
        {
            Setup();
            dary2["seven"] = 7;
            dary2["eleven"] = 111;
            Assert.Equal(7, dary2["seven"]);
            Assert.Equal(111, dary2["eleven"]);
            dary2["eleven"] = 11;
            Assert.Equal(11, dary2["eleven"]);
        }

#endregion
#region Test methods
        [Fact]
        public void CrashRd_Add_ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                Setup();
                dary2.Add(null !, 0);
            });
        }

        [Fact]
        public void CrashRd_Add_Argument()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                Setup();
                dary2.Add("foo", 1);
                dary2.Add("foo", 2);
            });
        }

        [Fact]
        public void UnitRd_AddCount()
        {
            Setup();
            dary1.Add(12, 120);
            dary1.Add(18, 180);
            Assert.Equal(2, dary1.Count);
        }

        [Fact]
        public void UnitRd_Clear()
        {
            Setup();
            dary1.Add(41, 410);
            Assert.Equal(1, dary1.Count);
            dary1.Clear();
            Assert.Equal(0, dary1.Count);
            int actualCount = 0;
            foreach (KeyValuePair<int, int> pair in dary1)
                ++actualCount;
            Assert.Equal(0, actualCount);
        }

        [Fact]
        public void CrashRd_ContainsKey_ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                Setup();
                dary2.Add("gamma", 3);
                // The nongeneric interface allows insert null key, but this is BCL behavior so...
                bool zz = dary2.ContainsKey(null !);
            });
        }

        [Fact]
        public void UnitRd_ContainsKey()
        {
            Setup();
            int key1 = 26;
            int key2 = 36;
            dary1.Add(key1, key1 * 10);
            Assert.True(dary1.ContainsKey(key1));
            Assert.False(dary1.ContainsKey(key2));
        }

        [Fact]
        public void UnitRd_ContainsValue()
        {
            Setup(5);
            int n = 200;
            for (int ii = 0; ii < n; ii += 2)
                dary1.Add(ii, -ii);
            for (int ii = 0; ii < n; ii += 2)
            {
                Assert.True(dary1.ContainsValue(-ii));
                Assert.False(dary1.ContainsValue(-ii - 1));
            }
        }

        [Fact]
        public void UnitRd_ContainsValueNullA()
        {
            Setup(4);
            for (int ii = 0; ii < 500; ++ii)
                dary3.Add(ii.ToString(), -ii);
            Assert.True(dary3.ContainsValue(-9));
            Assert.False(dary3.ContainsValue(null));
            dary3.Add("NaN", null);
            Assert.True(dary3.ContainsValue(null));
        }

        [Fact]
        public void UnitRd_ContainsValueNullB()
        {
            Setup();
            dary4.Add(5, "ee");
            dary4.Add(3, "cc");
            dary4.Add(7, "gg");
            Assert.False(dary4.ContainsValue(null));
            dary4.Add(-1, null);
            Assert.False(dary4.ContainsValue("dd"));
            Assert.True(dary4.ContainsValue("cc"));
            Assert.True(dary4.ContainsValue(null));
        }

        [Fact]
        public void CrashRd_CopyTo_ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                Setup();
                dary1.CopyTo(null !, -1);
            });
        }

        [Fact]
        public void CrashRd_CopyTo_ArgumentOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Setup();
                var target = new KeyValuePair<int, int>[iVals1.Length];
                dary1.CopyTo(target, -1);
            });
        }

        // MS docs incorrectly state ArgumentOutOfRangeException for this case.
        [Fact]
        public void CrashRd_CopyTo_ArgumentA()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                Setup();
                for (int key = 1; key < 10; ++key)
                    dary1.Add(key, key + 1000);
                var target = new KeyValuePair<int, int>[10];
                dary1.CopyTo(target, 25);
            });
        }

        [Fact]
        public void CrashRd_CopyTo_ArgumentB()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                Setup();
                for (int key = 1; key < 10; ++key)
                    dary1.Add(key, key + 1000);
                var target = new System.Collections.Generic.KeyValuePair<int, int>[4];
                dary1.CopyTo(target, 2);
            });
        }

        [Fact]
        public void UnitRd_CopyTo()
        {
            Setup();
            int offset = 1;
            int size = 20;
            for (int i = 0; i < size; ++i)
                dary1.Add(i + 1000, i + 10000);
            var pairs = new KeyValuePair<int, int>[size + offset];
            dary1.CopyTo(pairs, offset);
            for (int i = 0; i < offset; ++i)
            {
                Assert.Equal(0, pairs[i].Key);
                Assert.Equal(0, pairs[i].Value);
            }

            for (int i = 0; i < size; ++i)
            {
                Assert.Equal(i + 1000, pairs[i + offset].Key);
                Assert.Equal(i + 10000, pairs[i + offset].Value);
            }
        }

        [Fact]
        public void CrashRd_Remove_ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                Setup();
                dary2.Add("delta", 4);
                bool isRemoved = dary2.Remove(null !);
            });
        }

        [Fact]
        public void UnitRd_Remove()
        {
            Setup();
            foreach (int key in iVals1)
                dary1.Add(key, key + 1000);
            int c0 = dary1.Count;
            bool isRemoved1 = dary1.Remove(iVals1[3]);
            bool isRemoved2 = dary1.Remove(iVals1[3]);
            Assert.True(isRemoved1);
            Assert.False(isRemoved2);
            Assert.Equal(c0 - 1, dary1.Count);
        }

        [Fact]
        public void CrashRd_TryGetValue_ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                Setup();
                int resultValue;
                dary2.TryGetValue(null !, out resultValue);
            });
        }

        [Fact]
        public void UnitRd_TryGetValueOrUnfoundKeyInt()
        {
            Setup(5);
            for (int i = 2; i <= 50; i += 2)
                dary1.Add(i, i + 300);
            bool result1 = dary1.TryGetValue(5, out int val1);
            bool result2 = dary1.TryGetValue(18, out int val2);
            bool result3 = dary1.TryGetValue(26, out int val3);
            Assert.Equal(val2, 318);
            Assert.Equal(val3, 326);
            Assert.False(result1);
            Assert.True(result2);
            Assert.True(result3);
        }

        [Fact]
        public void UnitRd_TryGetValueForUnfoundKeyString()
        {
#if TEST_BCL
            var sd = new SortedDictionary<string,int> (StringComparer.Ordinal);
#else
            var sd = new RankedDictionary<string, int>(StringComparer.Ordinal)
            {
                Capacity = 4
            };
#endif
            for (char c = 'A'; c <= 'Z'; ++c)
                sd.Add(c.ToString(), c);
            bool result1 = sd.TryGetValue("M", out int val1);
            bool result2 = sd.TryGetValue("U", out int val2);
            bool result3 = sd.TryGetValue("$", out int val3);
            Assert.Equal(val1, 'M');
            Assert.Equal(val2, 'U');
            Assert.True(result1);
            Assert.True(result2);
            Assert.False(result3);
        }

        [Fact]
        public void UnitRd_TryGetValue()
        {
            Setup();
            foreach (int key in iVals1)
                dary1.Add(key, key + 1000);
            int resultValue;
            dary1.TryGetValue(iVals1[0], out resultValue);
            Assert.Equal(iVals1[0] + 1000, resultValue);
            dary1.TryGetValue(50, out resultValue);
            Assert.Equal(default(int), resultValue);
        }

#endregion
#region Test enumeration
        [Fact]
        public void UnitRd_GetEnumeratorOnEmpty()
        {
            int actual = 0;
            Setup();
            using (var e1 = dary2.GetEnumerator())
            {
                while (e1.MoveNext())
                    ++actual;
                var zz = e1.Current;
            }

            Assert.Equal(0, actual);
        }

        [Fact]
        public void UnitRd_GetEnumeratorPastEnd()
        {
            bool isMoved;
            int actual1 = 0, total1 = 0;
            Setup();
            dary2.Add("three", 3);
            dary2.Add("one", 1);
            dary2.Add("five", 5);
            using (var e1 = dary2.GetEnumerator())
            {
                while (e1.MoveNext())
                {
                    ++actual1;
                    total1 += e1.Current.Value;
                }

                isMoved = e1.MoveNext();
            }

            Assert.Equal(3, actual1);
            Assert.Equal(9, total1);
            Assert.False(isMoved);
        }

        [Fact]
        public void UnitRd_Etor()
        {
            Setup();
            for (int i = 0; i < iVals1.Length; ++i)
                dary1.Add(iVals1[i], iVals1[i] + 1000);
            int actualCount = 0;
            foreach (KeyValuePair<int, int> pair in dary1)
            {
                ++actualCount;
                Assert.Equal(pair.Key + 1000, pair.Value);
            }

            Assert.Equal(iVals1.Length, actualCount);
        }

        [Fact]
        public void CrashRd_oeCurrent_InvalidOperationA()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Setup();
                dary2.Add("cc", 3);
                IEnumerator<KeyValuePair<string, int>> kvEtor = dary2.GetEnumerator();
                object zz = ((System.Collections.IEnumerator)kvEtor).Current;
            });
        }

        [Fact]
        public void CrashRd_oeCurrent_InvalidOperationB()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Setup();
                dary2.Add("cc", 3);
                IEnumerator<KeyValuePair<string, int>> kvEtor = dary2.GetEnumerator();
                kvEtor.MoveNext();
                kvEtor.MoveNext();
                object zz = ((System.Collections.IEnumerator)kvEtor).Current;
            });
        }

        [Fact]
        public void UnitRd_EtorPair()
        {
            Setup();
            dary2.Add("nine", 9);
            IEnumerator<KeyValuePair<string, int>> kvEnum = dary2.GetEnumerator();
            KeyValuePair<string, int> pair0 = kvEnum.Current;
            Assert.Equal(default(int), pair0.Value);
            Assert.Equal(default(string), pair0.Key);
            kvEnum.MoveNext();
            KeyValuePair<string, int> pair = kvEnum.Current;
            Assert.Equal(9, pair.Value);
            Assert.Equal("nine", pair.Key);
            kvEnum.MoveNext();
            pair = kvEnum.Current;
            Assert.Equal(default(string), pair.Key);
            Assert.Equal(default(int), pair.Value);
        }

        [Fact]
        public void CrashRd_EtorHotUpdate()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Setup(4);
                dary2.Add("vv", 1);
                dary2.Add("mm", 2);
                dary2.Add("qq", 3);
                int n = 0;
                foreach (var kv in dary2)
                {
                    if (++n == 2)
                        dary2.Add("breaks enum", 4);
                }
            });
        }

        [Fact]
        public void UnitRd_ocCurrent_HotUpdate()
        {
            Setup();
            var kv = new KeyValuePair<string, int>("AA", 11);
            dary2.Add(kv.Key, kv.Value);
            System.Collections.ICollection oc = objCol2;
            System.Collections.IEnumerator etor = oc.GetEnumerator();
            bool ok = etor.MoveNext();
            Assert.Equal(kv, etor.Current);
            dary2.Clear();
            Assert.Equal(kv, etor.Current);
        }

        [Fact]
        public void UnitRd_EtorCurrentHotUpdate()
        {
            Setup();
            var kv1 = new KeyValuePair<int, int>(1, -1);
            var kvd1 = new KeyValuePair<int, int>(default(int), default(int));
            dary1.Add(kv1.Key, kv1.Value);
            var etor1 = dary1.GetEnumerator();
            Assert.Equal(kvd1, etor1.Current);
            bool ok1 = etor1.MoveNext();
            Assert.Equal(kv1, etor1.Current);
            dary1.Remove(1);
            Assert.Equal(kv1, etor1.Current);
            var kv2 = new KeyValuePair<string, int>("AA", 11);
            var kvd2 = new KeyValuePair<string, int>(default(string), default(int));
            dary2.Add(kv2.Key, kv2.Value);
            var etor2 = dary2.GetEnumerator();
            Assert.Equal(kvd2, etor2.Current);
            bool ok2 = etor2.MoveNext();
            Assert.Equal(kv2, etor2.Current);
            dary2.Clear();
            Assert.Equal(kv2, etor2.Current);
        }

        [Fact]
        public void UnitRd_oGetEnumerator()
        {
            Setup();
            var aa = (System.Collections.IEnumerable)dary1;
            var bb = aa.GetEnumerator();
        }

#endregion
#region Test explicit implementation
        [Fact]
        public void CrashRd_pcAdd_Argument()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                Setup();
                var pc = (ICollection<KeyValuePair<string, int>>)dary2;
                var p1 = new KeyValuePair<string, int>("beta", 1);
                var p2 = new KeyValuePair<string, int>(null, 98);
                var p3 = new KeyValuePair<string, int>(null, 99);
                pc.Add(p1);
                // Adding a null key is allowed here!
                pc.Add(p2);
                int result2 = dary2.Count;
                Assert.Equal(2, result2);
                // Should bomb on the second null key.
                pc.Add(p3);
            });
        }

        [Fact]
        public void UnitRd_pcAdd()
        {
            Setup();
            var pc = (ICollection<KeyValuePair<int, int>>)dary1;
            var p1 = new KeyValuePair<int, int>(17, 170);
            pc.Add(p1);
            Assert.Equal(1, dary1.Count);
            Assert.True(dary1.ContainsKey(17));
        }

        [Fact]
        public void UnitRd_pcContains()
        {
            Setup(4);
            var pc = (ICollection<KeyValuePair<string, int>>)dary2;
            var nullKv = new KeyValuePair<string, int>(null, 0);
            var zedKv = new KeyValuePair<string, int>("z", 0);
            foreach (var kv in greek)
                pc.Add(kv);
            foreach (var kv in greek)
                Assert.True(pc.Contains(kv));
            Assert.False(pc.Contains(zedKv));
            Assert.False(pc.Contains(nullKv));
            // key of null allowed with generic collection explicit interface:
            pc.Add(nullKv);
            Assert.True(pc.Contains(nullKv));
        }

        [Fact]
        public void UnitRd_pcComparePairNullRef()
        {
            Setup();
            var pc = (ICollection<KeyValuePair<int, string>>)dary4;
            dary4.Add(3, "cc");
            dary4.Add(1, "aa");
            var pair0 = new KeyValuePair<int, string>(0, null);
            var pair1 = new KeyValuePair<int, string>(3, "cc");
            Assert.False(pc.Contains(pair0));
            Assert.True(pc.Contains(pair1));
            dary4.Add(0, null);
            Assert.True(pc.Contains(pair0));
        }

        [Fact]
        public void UnitRd_pcEtor()
        {
            Setup();
            var pc = (ICollection<KeyValuePair<int, int>>)dary1;
            foreach (int k in iVals1)
                dary1.Add(k, k + 100);
            int actualCount = 0;
            foreach (KeyValuePair<int, int> pair in pc)
            {
                Assert.Equal(pair.Key + 100, pair.Value);
                ++actualCount;
            }

            Assert.Equal(iVals1.Length, actualCount);
        }

        [Fact]
        public void UnitRd_peEtor()
        {
            Setup();
            var pe = (IEnumerable<KeyValuePair<int, int>>)dary1;
            foreach (int val in iVals1)
                dary1.Add(val, val + 100);
            int actualCount = 0;
            foreach (KeyValuePair<int, int> pair in pe)
            {
                Assert.Equal(pair.Key + 100, pair.Value);
                ++actualCount;
            }

            Assert.Equal(iVals1.Length, actualCount);
        }

        [Fact]
        public void UnitRd_oeGetEtor()
        {
            Setup();
            var oe = (System.Collections.IEnumerable)dary4;
            dary4.Add(3, "cc");
            int rowCount = 0;
            foreach (object row in oe)
            {
                var kv = (KeyValuePair<int, string>)row;
                Assert.Equal(3, kv.Key);
                Assert.Equal("cc", kv.Value);
                ++rowCount;
            }

            Assert.Equal(1, rowCount);
        }

        [Fact]
        public void UnitRd_pcIsReadonly()
        {
            Setup();
            var pc = (ICollection<KeyValuePair<int, int>>)dary1;
            Assert.False(pc.IsReadOnly);
        }

        [Fact]
        public void UnitRd_gdKeys()
        {
            Setup();
            dary2.Add("alpha", 1);
            dary2.Add("beta", 2);
            var gd = (IDictionary<string, int>)dary2;
            int count = gd.Keys.Count;
            Assert.Equal(2, count);
        }

        [Fact]
        public void UnitRd_pcRemovePair()
        {
            Setup();
            var pc = (ICollection<KeyValuePair<string, int>>)dary2;
            var pair0 = new KeyValuePair<string, int>(null, 0);
            var pair1 = new KeyValuePair<string, int>("ten", 10);
            var pair2 = new KeyValuePair<string, int>("ten", 100);
            var pair3 = new KeyValuePair<string, int>("twenty", 20);
            pc.Add(pair0);
            pc.Add(pair1);
            pc.Add(pair3);
            Assert.Equal(3, dary2.Count);
            bool isRemoved = pc.Remove(pair0);
            Assert.True(isRemoved);
            Assert.Equal(2, dary2.Count);
            isRemoved = pc.Remove(pair0);
            Assert.False(isRemoved);
            Assert.Equal(2, dary2.Count);
            isRemoved = pc.Remove(pair2);
            Assert.Equal(2, dary2.Count);
            isRemoved = pc.Remove(pair1);
            Assert.True(isRemoved);
            Assert.Equal(1, dary2.Count);
        }

        [Fact]
        public void UnitRd_pcRemovePairNull()
        {
            Setup();
            dary4.Add(3, "cc");
            dary4.Add(5, "ee");
            dary4.Add(4, null);
            var gpc = (ICollection<KeyValuePair<int, string>>)dary4;
            bool isOK = gpc.Remove(new KeyValuePair<int, string>(99, null));
            Assert.False(isOK);
            isOK = gpc.Remove(new KeyValuePair<int, string>(4, null));
            Assert.True(isOK);
            isOK = dary4.ContainsKey(4);
            Assert.False(isOK);
        }

        [Fact]
        public void UnitRd_gdValues()
        {
            Setup();
            dary2.Add("alpha", 1);
            dary2.Add("beta", 2);
            var gd = (IDictionary<string, int>)dary2;
            int count = gd.Values.Count;
            Assert.Equal(2, count);
        }

        [Fact]
        public void UnitRd_oReset()
        {
            Setup(4);
            var exKeys = new int[]
            {
                1,
                2,
                5,
                8,
                9
            };
            foreach (var x in exKeys)
                dary1.Add(x, -x);
            int ix;
            var etor = dary1.GetEnumerator();
            for (ix = 0; etor.MoveNext(); ++ix)
            {
                Assert.Equal(exKeys[ix], etor.Current.Key);
                Assert.Equal(-exKeys[ix], etor.Current.Value);
            }

            Assert.Equal(exKeys.Length, ix);
            ((System.Collections.IEnumerator)etor).Reset();
            for (ix = 0; etor.MoveNext(); ++ix)
            {
                Assert.Equal(exKeys[ix], etor.Current.Key);
                Assert.Equal(-exKeys[ix], etor.Current.Value);
            }

            Assert.Equal(exKeys.Length, ix);
        }

#endregion
#region Test bonus methods
#if !TEST_BCL
        [Fact]
        public void UnitRdx_MinMax()
        {
            var rd = new RankedDictionary<int, int>
            {
                Capacity = 4
            };
            int min0 = rd.MinKey;
            int max0 = rd.MaxKey;
            Assert.Equal(default(int), min0);
            Assert.Equal(default(int), max0);
            for (int i1 = 1; i1 <= 99; ++i1)
                rd.Add(i1, i1 + 100);
            int min = rd.MinKey;
            int max = rd.MaxKey;
            Assert.Equal(1, min);
            Assert.Equal(99, max);
        }

        [Fact]
        public void CrashRdx_Capacity_ArgumentOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var rd = new RankedDictionary<int, int>();
                rd.Capacity = -1;
            });
        }

        [Fact]
        public void UnitRdx_Capacity()
        {
            var rd = new RankedDictionary<int, int>();
            var initial = rd.Capacity;
            rd.Capacity = 0;
            Assert.Equal(initial, rd.Capacity);
            rd.Capacity = 3;
            Assert.Equal(initial, rd.Capacity);
            rd.Capacity = 257;
            Assert.Equal(initial, rd.Capacity);
            rd.Capacity = 4;
            Assert.Equal(4, rd.Capacity);
            rd.Capacity = 256;
            Assert.Equal(256, rd.Capacity);
            rd.Add(1, 11);
            rd.Capacity = 128;
            Assert.Equal(256, rd.Capacity);
        }

        [Fact]
        public void UnitRdx_ElementsBetweenA()
        {
            var rd = new RankedDictionary<string, int>();
            var pc = (ICollection<KeyValuePair<string, int>>)rd;
            rd.Add("Alpha", 1);
            rd.Add("Beta", 2);
            rd.Add("Omega", 24);
            pc.Add(new KeyValuePair<string, int>(null, 0));
            int actual = 0;
            foreach (var kv in rd.ElementsBetween(null, "C"))
                ++actual;
            Assert.Equal(3, actual);
        }

        [Fact]
        public void UnitRdx_ElementsBetweenB()
        {
            var rd = new RankedDictionary<int, int>
            {
                Capacity = 4
            };
            for (int i = 90; i >= 0; i -= 10)
                rd.Add(i, -100 - i);
            int iterations = 0;
            int sumVals = 0;
            foreach (var kv in rd.ElementsBetween(35, 55))
            {
                ++iterations;
                sumVals += kv.Value;
            }

            Assert.Equal(2, iterations);
            Assert.Equal(-290, sumVals);
        }

        [Fact]
        public void UnitRdx_ElementsBetweenPassedEnd()
        {
            var rd = new RankedDictionary<int, int>();
            for (int i = 0; i < 1000; ++i)
                rd.Add(i, -i);
            int iterations = 0;
            int sumVals = 0;
            foreach (KeyValuePair<int, int> e in rd.ElementsBetween(500, 1500))
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
        public void UnitRdx_ElementsFromA()
        {
            var rd = new RankedDictionary<string, int>()
            {
                {
                    "A",
                    -1
                },
                {
                    "B",
                    -2
                }
            };
            var pc = (ICollection<KeyValuePair<string, int>>)rd;
            int actual1 = 0;
            foreach (var pair in rd.ElementsFrom(null))
                ++actual1;
            pc.Add(new KeyValuePair<string, int>(null, 0));
            int actual2 = 0;
            foreach (var pair in rd.ElementsFrom(null))
                ++actual2;
            Assert.Equal(2, actual1);
            Assert.Equal(3, actual2);
        }

        [Fact]
        public void UnitRdx_ElementsFromB()
        {
            var rd = new RankedDictionary<int, int>();
            for (int i = 1; i <= 1000; ++i)
                rd.Add(i, -i);
            int firstKey = -1;
            int iterations = 0;
            foreach (var e in rd.ElementsFrom(501))
            {
                if (iterations == 0)
                    firstKey = e.Key;
                ++iterations;
            }

            Assert.Equal(501, firstKey);
            Assert.Equal(500, iterations);
        }

        [Fact]
        public void UnitRdx_ElementsFromMissingVal()
        {
            var rd = new RankedDictionary<int, int>()
            {
                Capacity = 8
            };
#if STRESS
            int n = 1000;
#else
            int n = 10;
#endif
            for (int i = 0; i < n; i += 2)
                rd.Add(i, -i);
            for (int i = 1; i < n - 1; i += 2)
            {
                foreach (var x in rd.ElementsFrom(i))
                {
                    try
                    {
                        Assert.Equal(i + 1, x.Key);
                    }
                    catch
                    {
                        _output.WriteLine("Incorrect key value");
                        throw;
                    }

                    break;
                }
            }
        }

        [Fact]
        public void UnitRdx_ElementsFromPassedEnd()
        {
            var rd = new RankedDictionary<int, int>();
            for (int i = 0; i < 1000; ++i)
                rd.Add(i, -i);
            int iterations = 0;
            foreach (var x in rd.ElementsFrom(2000))
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
        public void CrashRdx_ElementsBetweenIndexes_ArgumentOutOfRangeA()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var rd = new RankedDictionary<int, int>
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
                foreach (var pair in rd.ElementsBetweenIndexes(-1, 0))
                {
                }
            });
        }

        [Fact]
        public void CrashRdx_ElementsBetweenIndexes_ArgumentOutOfRangeB()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var rd = new RankedDictionary<int, int>
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
                foreach (var pair in rd.ElementsBetweenIndexes(2, 0))
                {
                }
            });
        }

        [Fact]
        public void CrashRdx_ElementsBetweenIndexes_ArgumentOutOfRangeC()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var rd = new RankedDictionary<int, int>
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
                foreach (var pair in rd.ElementsBetweenIndexes(0, -1))
                {
                }
            });
        }

        [Fact]
        public void CrashRdx_ElementsBetweenIndexes_ArgumentOutOfRangeD()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var rd = new RankedDictionary<int, int>
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
                foreach (var pair in rd.ElementsBetweenIndexes(0, 2))
                {
                }
            });
        }

        [Fact]
        public void CrashRdx_ElementsBetweenIndexes_Argument()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var rd = new RankedDictionary<int, int>
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
                foreach (var pair in rd.ElementsBetweenIndexes(2, 1))
                {
                }
            });
        }

        [Fact]
        public void UnitRdx_ElementsBetweenIndexes()
        {
            int n = 30;
            var rd = new RankedDictionary<int, int>
            {
                Capacity = 4
            };
            for (int ii = 0; ii < n; ++ii)
                rd.Add(ii, -ii);
            for (int p1 = 0; p1 < n; ++p1)
                for (int p2 = p1; p2 < n; ++p2)
                {
                    int actual = 0;
                    foreach (var pair in rd.ElementsBetweenIndexes(p1, p2))
                        actual += pair.Key;
                    int expected = (p2 - p1 + 1) * (p1 + p2) / 2;
                    Assert.Equal(expected, actual);
                }
        }

        [Fact]
        public void CrashRdx_IndexOfKey_ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var rd = new RankedDictionary<string, int>();
                int ix = rd.IndexOfKey(null);
            });
        }

        [Fact]
        public void UnitRdx_IndexOfKey()
        {
            var rd = new RankedDictionary<int, int>
            {
                Capacity = 5
            };
            for (int ii = 0; ii < 500; ii += 2)
                rd.Add(ii, ii + 1000);
            for (int ii = 0; ii < 500; ii += 2)
            {
                int ix = rd.IndexOfKey(ii);
                Assert.Equal(ii / 2, ix);
            }

            int iw = rd.IndexOfKey(-1);
            Assert.Equal(~0, iw);
            int iy = rd.IndexOfKey(500);
            Assert.Equal(~250, iy);
        }

        [Fact]
        public void UnitRdx_IndexOfValue()
        {
            var rd = new RankedDictionary<int, int>();
            for (int ii = 0; ii < 500; ++ii)
                rd.Add(ii, ii + 1000);
            var ix1 = rd.IndexOfValue(1400);
            Assert.Equal(400, ix1);
            var ix2 = rd.IndexOfValue(88888);
            Assert.Equal(-1, ix2);
        }

        [Fact]
        public void CrashRd_RemoveAll_ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                Setup();
                dary1.RemoveAll(null);
            });
        }

        [Fact]
        public void UnitRd_RemoveAll()
        {
            Setup(4);
            int rem0 = dary1.RemoveAll(new int[] { 2 });
            Assert.Equal(0, rem0);
            foreach (var ii in new int[]
            {
                1,
                3,
                5,
                7,
                9
            }

            )
                dary1.Add(ii, -ii);
            int rem2 = dary1.RemoveAll(new int[] { 2 });
            Assert.Equal(0, rem0);
            int rem3 = dary1.RemoveAll(new int[] { 3, 4, 7, 7, 11 });
            Assert.Equal(2, rem3);
            Assert.True(System.Linq.Enumerable.SequenceEqual(new int[] { 1, 5, 9 }, dary1.Keys));
            int rem4 = dary1.RemoveAll(dary1.Keys);
            Assert.Equal(3, rem4);
            Assert.Equal(0, dary1.Count);
        }

        [Fact]
        public void CrashRdx_RemoveAtA_ArgumentOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var rd = new RankedDictionary<int, int>();
                rd.Add(42, 24);
                rd.RemoveAt(-1);
            });
        }

        [Fact]
        public void CrashRdx_RemoveAtB_ArgumentOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var rd = new RankedDictionary<int, int>();
                rd.RemoveAt(0);
            });
        }

        [Fact]
        public void UnitRdx_RemoveAt()
        {
            var rd = new RankedDictionary<int, int>();
            for (int ii = 0; ii < 5000; ++ii)
                rd.Add(ii, -ii);
            for (int i2 = 4900; i2 >= 0; i2 -= 100)
                rd.RemoveAt(i2);
            for (int i2 = 0; i2 < 5000; ++i2)
                if (i2 % 100 == 0)
                    Assert.False(rd.ContainsKey(i2));
                else
                    Assert.True(rd.ContainsKey(i2));
        }

        [Fact]
        public void CrashRdx_RemoveRange_ArgumentOutOfRangeA()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var rd = new RankedDictionary<int, int>();
                rd.RemoveRange(-1, 0);
            });
        }

        [Fact]
        public void CrashRdx_RemoveRange_ArgumentOutOfRangeB()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var rd = new RankedDictionary<int, int>();
                rd.RemoveRange(0, -1);
            });
        }

        [Fact]
        public void CrashRdx_RemoveRange_Argument()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var rd = new RankedDictionary<int, int>();
                rd.Add(3, 33);
                rd.Add(5, 55);
                rd.RemoveRange(1, 2);
            });
        }

        [Fact]
        public void UnitRdx_RemoveRange()
        {
            var rd = new RankedDictionary<int, int>
            {
                Capacity = 7
            };
            for (int ii = 0; ii < 20; ++ii)
                rd.Add(ii, -ii);
            rd.RemoveRange(20, 0);
            Assert.Equal(20, rd.Count);
            rd.RemoveRange(12, 4);
            Assert.Equal(16, rd.Count);
#if DEBUG
            rd.SanityCheck();
#endif
        }

        [Fact]
        public void UnitRdx_TryGetGEGT()
        {
            var rd = new RankedDictionary<string, int?>
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
            bool r0a = rd.TryGetGreaterThan("CC", out KeyValuePair<string, int?> p0a);
            Assert.False(r0a);
            Assert.Equal(default(string), p0a.Key);
            Assert.Equal(default(int? ), p0a.Value);
            bool r0b = rd.TryGetGreaterThanOrEqual("DD", out KeyValuePair<string, int?> p0b);
            Assert.False(r0b);
            Assert.Equal(default(string), p0b.Key);
            Assert.Equal(default(int? ), p0b.Value);
            bool r1 = rd.TryGetGreaterThan("BB", out KeyValuePair<string, int?> p1);
            Assert.True(r1);
            Assert.Equal("CC", p1.Key);
            Assert.Equal(2, p1.Value);
            bool r2 = rd.TryGetGreaterThanOrEqual("BB", out KeyValuePair<string, int?> p2);
            Assert.True(r2);
            Assert.Equal("BB", p2.Key);
            Assert.Equal(1, p2.Value);
            bool r3 = rd.TryGetGreaterThanOrEqual("AA", out KeyValuePair<string, int?> p3);
            Assert.True(r3);
            Assert.Equal("BB", p3.Key);
            Assert.Equal(1, p3.Value);
        }

        [Fact]
        public void UnitRdx_TryGetLELT()
        {
            var rd = new RankedDictionary<string, int?>
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
            bool r0a = rd.TryGetLessThan("BB", out KeyValuePair<string, int?> p0a);
            Assert.False(r0a);
            Assert.Equal(default(string), p0a.Key);
            Assert.Equal(default(int? ), p0a.Value);
            bool r0b = rd.TryGetLessThanOrEqual("AA", out KeyValuePair<string, int?> p0b);
            Assert.False(r0b);
            Assert.Equal(default(string), p0b.Key);
            Assert.Equal(default(int? ), p0b.Value);
            bool r1 = rd.TryGetLessThan("CC", out KeyValuePair<string, int?> p1);
            Assert.True(r1);
            Assert.Equal("BB", p1.Key);
            Assert.Equal(1, p1.Value);
            bool r2 = rd.TryGetLessThanOrEqual("CC", out KeyValuePair<string, int?> p2);
            Assert.True(r2);
            Assert.Equal("CC", p2.Key);
            Assert.Equal(2, p2.Value);
            bool r3 = rd.TryGetLessThanOrEqual("DD", out KeyValuePair<string, int?> p3);
            Assert.True(r3);
            Assert.Equal("CC", p3.Key);
            Assert.Equal(2, p3.Value);
        }

        [Fact]
        public void UnitRdx_TryGetValueIndex()
        {
            var rd = new RankedDictionary<int, int>();
            rd.Capacity = 5;
            for (int ii = 0; ii < 500; ii += 2)
                rd.Add(ii, ii + 1000);
            for (int ii = 0; ii < 500; ii += 2)
            {
                bool isOk = rd.TryGetValueAndIndex(ii, out int v1, out int i1);
                Assert.True(isOk);
                Assert.Equal(ii / 2, i1);
                Assert.Equal(ii + 1000, v1);
            }

            bool isOkNot = rd.TryGetValueAndIndex(111, out int v2, out int i2);
            Assert.False(isOkNot);
        }

        [Fact]
        public void CrashRd_RemoveWhere_ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var rd = new RankedDictionary<int, int>();
                rd.RemoveWhere(null);
            });
        }

        [Fact]
        public void UnitRdx_RemoveWhereA()
        {
            var rd = new RankedDictionary<int, int>();
            for (int ix = 0; ix < 1000; ++ix)
                rd.Add(ix, ix + 1000);
            int c0 = rd.Count;
            int removed = rd.RemoveWhere(IsEven);
            Assert.Equal(500, removed);
            foreach (int key in rd.Keys)
                Assert.True(key % 2 != 0);
        }

        [Fact]
        public void UnitRdx_RemoveWhereB()
        {
            int n = 2000;
            var rd = new RankedDictionary<int, int>
            {
                Capacity = 7
            };
            for (int ix = 0; ix < n; ++ix)
                rd.Add(ix, -ix);
            int removed = rd.RemoveWhere(IsAlways);
            Assert.Equal(n, removed);
            Assert.Equal(0, rd.Count);
        }

        [Fact]
        public void CrashRdx_RemoveWhereElement_ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var rd = new RankedDictionary<int, int>();
                rd.RemoveWhereElement(null);
            });
        }

        [Fact]
        public void UnitRdx_RemoveWhereElement()
        {
            var rd = new RankedDictionary<int, int>();
            for (int ix = 0; ix < 1000; ++ix)
                rd.Add(ix, -ix);
            int c0 = rd.Count;
            int removed = rd.RemoveWhereElement(IsPairEven);
            Assert.Equal(500, removed);
            foreach (int val in rd.Values)
                Assert.True(val % 2 != 0);
        }

        [Fact]
        public void UnitRdx_ReverseEmpty()
        {
            int total = 0;
            Setup(5);
            foreach (var countdown in dary1.Reverse())
                ++total;
            Assert.Equal(0, total);
        }

        [Fact]
        public void UnitRdx_Reverse()
        {
            var rd = new RankedDictionary<int, int>
            {
                Capacity = 5
            };
            int expected = 500;
            for (int ii = 1; ii <= expected; ++ii)
                rd.Add(ii, -ii);
            foreach (var actual in rd.Reverse())
            {
                Assert.Equal(expected, actual.Key);
                Assert.Equal(-expected, actual.Value);
                --expected;
            }

            Assert.Equal(0, expected);
        }
#endif
#endregion
    }
}