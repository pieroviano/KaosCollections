using System;
using NUnit.Framework;
#if TEST_BCL
using System.Collections.Generic;
using System.Linq;
#else
using Kaos.Collections;
#endif

namespace Kaos.Test.Collections
{
    public partial class TestRd
    {
        #region Test Keys constructor

        [Test]
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

        [Test]
        public void UnitRdk_Ctor()
        {
            Setup();
            dary1.Add(1, -1);
#if TEST_BCL
            var keys = new SortedDictionary<int,int>.KeyCollection (dary1);
#else
            var keys = new RankedDictionary<int, int>.KeyCollection(dary1);
#endif
            Assert.AreEqual(1, keys.Count);
        }

        #endregion

        #region Test Keys properties

        [Test]
        public void UnitRdk_Count()
        {
            Setup();
            foreach (int key in iVals1)
                dary1.Add(key, key + 1000);

            Assert.AreEqual(iVals1.Length, dary1.Keys.Count);
        }


        [Test]
        public void UnitRdk_gcIsReadonly()
        {
            Setup();
            var gc = (System.Collections.Generic.ICollection<int>)dary1.Keys;
            Assert.IsTrue(gc.IsReadOnly);
        }


        [Test]
        public void UnitRdk_ocSyncRoot()
        {
            Setup();
            var oc = (System.Collections.ICollection)dary1.Keys;
            Assert.IsFalse(oc.SyncRoot.GetType().IsValueType);
        }

        #endregion

        #region Test Keys methods

        [Test]
        public void CrashRdk_gcAdd_NotSupported()
        {
            Setup();
            var gc = (System.Collections.Generic.ICollection<string>)dary2.Keys;
            Assert.Throws<NotSupportedException>(() => { gc.Add("omega"); });
        }


        [Test]
        public void CrashRdk_gcClear_NotSupported()
        {
            Setup();
            var gc = (System.Collections.Generic.ICollection<string>)dary2.Keys;
            Assert.Throws<NotSupportedException>(() => { gc.Clear(); });
        }


        [Test]
        public void CrashRdk_gcContains_ArgumentNull()
        {
            Setup();
            var gc = (System.Collections.Generic.ICollection<string>)dary2.Keys;

            dary2.Add("alpha", 10);

            Assert.Throws<ArgumentNullException>(() => { var zz = gc.Contains(null); });
        }

        [Test]
        public void UnitRdk_gcContains()
        {
            Setup();
            var gc = (System.Collections.Generic.ICollection<string>)dary2.Keys;

            dary2.Add("alpha", 10);
            dary2.Add("beta", 20);

            Assert.IsTrue(gc.Contains("beta"));
            Assert.IsFalse(gc.Contains("zed"));
        }


        [Test]
        public void CrashRdk_CopyTo_ArgumentNull()
        {
            Setup();
            var target = new int[10];
            Assert.Throws<ArgumentNullException>(() => { dary1.Keys.CopyTo(null!, -1); });
        }

        [Test]
        public void CrashRdk_CopyTo_ArgumentOutOfRange()
        {
            Setup();
            var target = new int[iVals1.Length];
            Assert.Throws<ArgumentOutOfRangeException>(() => { dary1.Keys.CopyTo(target, -1); });
        }

        [Test]
        public void CrashRdk_CopyTo_Argument()
        {
            Setup();
            for (int key = 1; key < 10; ++key)
                dary1.Add(key, key + 1000);

            var target = new int[4];
            Assert.Throws<ArgumentException>(() => { dary1.Keys.CopyTo(target, 2); });
        }

        [Test]
        public void UnitRdk_CopyTo()
        {
            Setup();
            int n = 10, offset = 5;

            for (int k = 0; k < n; ++k)
                dary1.Add(k, k + 1000);

            int[] target = new int[n + offset];
            dary1.Keys.CopyTo(target, offset);

            for (int k = 0; k < n; ++k)
                Assert.AreEqual(k, target[k + offset]);
        }


        [Test]
        public void UnitRdk_gcCopyTo()
        {
            Setup();
            var gc = (System.Collections.Generic.ICollection<string>)dary2.Keys;

            dary2.Add("alpha", 1);
            dary2.Add("beta", 2);
            dary2.Add("gamma", 3);

            var target = new string[dary2.Count];

            gc.CopyTo(target, 0);

            Assert.AreEqual("alpha", target[0]);
            Assert.AreEqual("beta", target[1]);
            Assert.AreEqual("gamma", target[2]);
        }


        [Test]
        public void CrashRdk_gcRemove_NotSupported()
        {
            Setup();
            var gc = (System.Collections.Generic.ICollection<string>)dary2.Keys;
            Assert.Throws<NotSupportedException>(() => { gc.Remove("omega"); });
        }

        #endregion

        #region Test Keys bonus methods
#if ! TEST_BCL

        [Test]
        public void UnitRdkx_Indexer()
        {
            var rd = new RankedDictionary<string, int> { { "0zero", 0 }, { "1one", -1 }, { "2two", -2 } };

            Assert.AreEqual("0zero", rd.Keys[0]);
            Assert.AreEqual("1one", rd.Keys[1]);
            Assert.AreEqual("2two", rd.Keys[2]);
        }


        [Test]
        public void CrashRdkx_ElementAt_ArgumentOutOfRange1()
        {
            var rd = new RankedDictionary<int, int>();
            Assert.Throws<ArgumentOutOfRangeException>(() => { var zz = rd.Keys.ElementAt(-1); });
        }

        [Test]
        public void CrashRdkx_ElementAt_ArgumentOutOfRange2()
        {
            var rd = new RankedDictionary<int, int>();
            Assert.Throws<ArgumentOutOfRangeException>(() => { var zz = rd.Keys.ElementAt(0); });
        }

        [Test]
        public void UnitRdkx_ElementAt()
        {
            var rd = new RankedDictionary<string, int> { { "one", 1 }, { "two", 2 } };
            string k1 = rd.Keys.ElementAt(1);

            Assert.AreEqual("two", k1);
        }


        [Test]
        public void UnitRdkx_ElementAtOrDefault()
        {
            var rd = new RankedDictionary<string, int> { { "one", 1 }, { "two", 2 } };

            string kn = rd.Keys.ElementAtOrDefault(-1);
            string k1 = rd.Keys.ElementAtOrDefault(1);
            string k2 = rd.Keys.ElementAtOrDefault(2);

            Assert.AreEqual(default(string), kn);
            Assert.AreEqual("two", k1);
            Assert.AreEqual(default(string), k2);
        }


        [Test]
        public void UnitRdkx_IndexOf()
        {
            var rd = new RankedDictionary<string, int> { { "one", 1 }, { "two", 2 } };
            var pc = (System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<string, int>>)rd;

            pc.Add(new System.Collections.Generic.KeyValuePair<string, int>(null, -1));

            Assert.AreEqual(0, rd.Keys.IndexOf(null));
            Assert.AreEqual(2, rd.Keys.IndexOf("two"));
        }


        [Test]
        public void UnitRdx_TryGet()
        {
            var rd = new RankedDictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
            rd.Add("AAA", 1);
            rd.Add("bbb", 2);
            rd.Add("ccc", 3);

            bool got1 = rd.Keys.TryGet("aaa", out string actual1);
            Assert.IsTrue(got1);
            Assert.AreEqual("AAA", actual1);

            bool got2 = rd.Keys.TryGet("bb", out string actual2);
            Assert.IsFalse(got2);

            bool got3 = rd.Keys.TryGet("CCC", out string actual3);
            Assert.IsTrue(got3);
            Assert.AreEqual("ccc", actual3);
        }


        [Test]
        public void UnitRdkx_TryGetGEGT()
        {
            var rd = new RankedDictionary<string, int> { { "BB", 1 }, { "CC", 2 } };

            bool r0a = rd.Keys.TryGetGreaterThan("CC", out string k0a);
            Assert.IsFalse(r0a);
            Assert.AreEqual(default(string), k0a);

            bool r0b = rd.Keys.TryGetGreaterThanOrEqual("DD", out string k0b);
            Assert.IsFalse(r0b);
            Assert.AreEqual(default(string), k0b);

            bool r1 = rd.Keys.TryGetGreaterThan("BB", out string k1);
            Assert.IsTrue(r1);
            Assert.AreEqual("CC", k1);

            bool r2 = rd.Keys.TryGetGreaterThanOrEqual("BB", out string k2);
            Assert.IsTrue(r2);
            Assert.AreEqual("BB", k2);

            bool r3 = rd.Keys.TryGetGreaterThanOrEqual("AA", out string k3);
            Assert.IsTrue(r3);
            Assert.AreEqual("BB", k3);
        }


        [Test]
        public void UnitRdkx_TryGetLELT()
        {
            var rd = new RankedDictionary<string, int> { { "BB", 1 }, { "CC", 2 } };

            bool r0a = rd.Keys.TryGetLessThan("BB", out string k0a);
            Assert.IsFalse(r0a);
            Assert.AreEqual(default(string), k0a);

            bool r0b = rd.Keys.TryGetLessThanOrEqual("AA", out string k0b);
            Assert.IsFalse(r0b);
            Assert.AreEqual(default(string), k0b);

            bool r1 = rd.Keys.TryGetLessThan("CC", out string k1);
            Assert.IsTrue(r1);
            Assert.AreEqual("BB", k1);

            bool r2 = rd.Keys.TryGetLessThanOrEqual("CC", out string k2);
            Assert.IsTrue(r2);
            Assert.AreEqual("CC", k2);

            bool r3 = rd.Keys.TryGetLessThanOrEqual("DD", out string k3);
            Assert.IsTrue(r3);
            Assert.AreEqual("CC", k3);
        }

#endif
        #endregion

        #region Test Keys enumeration

        [Test]
        public void CrashRdk_ocCurrent_InvalidOperation()
        {
            Setup();
            dary2.Add("CC", 3);

            System.Collections.ICollection oc = objCol2.Keys;
            System.Collections.IEnumerator etor = oc.GetEnumerator();

            Assert.Throws<InvalidOperationException>(() => { object zz = etor.Current; });
        }

        [Test]
        public void UnitRdk_GetEnumerator()
        {
            Setup(4);
            int n = 100;

            for (int k = 0; k < n; ++k)
                dary1.Add(k, k + 1000);

            int actualCount = 0;
            foreach (int key in dary1.Keys)
            {
                Assert.AreEqual(actualCount, key);
                ++actualCount;
            }

            Assert.AreEqual(n, actualCount);
        }

        [Test]
        public void UnitRdk_gcGetEnumerator()
        {
            Setup();
            int n = 10;

            for (int k = 0; k < n; ++k)
                dary2.Add(k.ToString(), k);

            int expected = 0;
            var etor = genKeys2.GetEnumerator();

            var rewoundKey = etor.Current;
            Assert.AreEqual(rewoundKey, null);

            while (etor.MoveNext())
            {
                var key = etor.Current;
                Assert.AreEqual(expected.ToString(), key);
                ++expected;
            }
            Assert.AreEqual(n, expected);
        }


        [Test]
        public void UnitRdk_Reverse()
        {
            Setup(4);
            int n = 50;
            for (int k = 0; k < n; ++k)
                dary1.Add(k, k + 1000);

            int expected = n;
            foreach (int ak in dary1.Keys.Reverse())
            {
                --expected;
                Assert.AreEqual(expected, ak);
            }

            Assert.AreEqual(0, expected);
        }


        [Test]
        public void CrashRdk_EtorHotUpdate()
        {
            Setup(4);
            dary2.Add("vv", 1);
            dary2.Add("mm", 2);
            dary2.Add("qq", 3);

            Assert.Throws<InvalidOperationException>(() =>
            {
                int n = 0;
                foreach (var kv in dary2.Keys)
                {
                    if (++n == 2)
                        dary2.Remove("vv");
                }
            });
        }

        [Test]
        public void UnitRdk_ocCurrent_HotUpdate()
        {
            Setup();
            dary2.Add("AA", 11);

            System.Collections.ICollection oc = objCol2.Keys;
            System.Collections.IEnumerator etor = oc.GetEnumerator();

            bool ok = etor.MoveNext();
            Assert.AreEqual("AA", etor.Current);

            dary2.Clear();
            Assert.AreEqual("AA", etor.Current);
        }

        [Test]
        public void UnitRdk_EtorCurrentHotUpdate()
        {
            Setup();
            dary1.Add(1, -1);
            var etor1 = dary1.Keys.GetEnumerator();
            Assert.AreEqual(default(int), etor1.Current);
            bool ok1 = etor1.MoveNext();
            Assert.AreEqual(1, etor1.Current);
            dary1.Remove(1);
            Assert.AreEqual(1, etor1.Current);

            dary2.Add("AA", 11);
            var etor2 = dary2.Keys.GetEnumerator();
            Assert.AreEqual(default(string), etor2.Current);
            bool ok2 = etor2.MoveNext();
            Assert.AreEqual("AA", etor2.Current);
            dary2.Clear();
            Assert.AreEqual("AA", etor2.Current);
        }


        [Test]
        public void UnitRdk_oReset()
        {
            Setup(5);
            int n = 9;

            for (int ix = 0; ix < n; ++ix)
                dary1.Add(ix, -ix);
#if TEST_BCL
            SortedDictionary<int,int>.KeyCollection.Enumerator etor;
#else
            RankedDictionary<int, int>.KeyCollection.Enumerator etor;
#endif
            etor = dary1.Keys.GetEnumerator();

            int ix1 = 0;
            while (etor.MoveNext())
            {
                Assert.AreEqual(ix1, etor.Current);
                ++ix1;
            }
            Assert.AreEqual(n, ix1);

            ((System.Collections.IEnumerator)etor).Reset();

            int ix2 = 0;
            while (etor.MoveNext())
            {
                Assert.AreEqual(ix2, etor.Current);
                ++ix2;
            }
            Assert.AreEqual(n, ix2);
        }

        #endregion


        #region Test Values constructor

        [Test]
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

        [Test]
        public void UnitRdv_Ctor()
        {
            Setup();
            dary1.Add(1, -1);
#if TEST_BCL
            var vals = new SortedDictionary<int,int>.ValueCollection (dary1);
#else
            var vals = new RankedDictionary<int, int>.ValueCollection(dary1);
#endif
            Assert.AreEqual(1, vals.Count);
        }

        #endregion

        #region Test Values properties

        [Test]
        public void UnitRdv_Count()
        {
            Setup();
            foreach (int key in iVals1)
                dary1.Add(key, key + 1000);

            Assert.AreEqual(iVals1.Length, dary1.Values.Count);
        }


        [Test]
        public void UnitRdv_gcIsReadonly()
        {
            Setup();
            var gc = (System.Collections.Generic.ICollection<int>)dary1.Values;
            Assert.IsTrue(gc.IsReadOnly);
        }


        [Test]
        public void UnitRdv_ocSyncRoot()
        {
            Setup();
            var oc = (System.Collections.ICollection)dary2.Values;
            Assert.IsFalse(oc.SyncRoot.GetType().IsValueType);
        }

        #endregion

        #region Test Values methods

        [Test]
        public void CrashRdv_gcAdd_NotSupported()
        {
            Setup();
            var gc = (System.Collections.Generic.ICollection<int>)dary2.Values;
            Assert.Throws<NotSupportedException>(() => { gc.Add(9); });
        }


        [Test]
        public void CrashRdv_gcClear_NotSupported()
        {
            Setup();
            var gc = (System.Collections.Generic.ICollection<int>)dary2.Values;
            Assert.Throws<NotSupportedException>(() => { gc.Clear(); });
        }


        [Test]
        public void UnitRdv_gcContains()
        {
            Setup();
            var gc = (System.Collections.Generic.ICollection<int>)dary2.Values;

            dary2.Add("alpha", 10);
            dary2.Add("beta", 20);

            Assert.IsTrue(gc.Contains(20));
            Assert.IsFalse(gc.Contains(15));
        }


        [Test]
        public void CrashRdv_CopyTo_ArgumentNull()
        {
            Setup();
            var target = new int[iVals1.Length];
            Assert.Throws<ArgumentNullException>(() => { dary1.Values.CopyTo(null!, -1); });
        }

        [Test]
        public void CrashRdv_CopyTo_ArgumentOutOfRange()
        {
            Setup();
            var target = new int[10];
            Assert.Throws<ArgumentOutOfRangeException>(() => { dary1.Values.CopyTo(target, -1); });
        }

        [Test]
        public void CrashRdv_CopyTo_Argument()
        {
            Setup();

            for (int key = 1; key < 10; ++key)
                dary1.Add(key, key + 1000);

            var target = new int[4];
            Assert.Throws<ArgumentException>(() => { dary1.Values.CopyTo(target, 2); });
        }

        [Test]
        public void UnitRdv_CopyTo()
        {
            Setup();
            int n = 10, offset = 5;

            for (int k = 0; k < n; ++k)
                dary1.Add(k, k + 1000);

            int[] target = new int[n + offset];
            dary1.Values.CopyTo(target, offset);

            for (int k = 0; k < n; ++k)
                Assert.AreEqual(k + 1000, target[k + offset]);
        }


        [Test]
        public void UnitRdv_gcCopyTo()
        {
            Setup();
            var gc = (System.Collections.Generic.ICollection<int>)dary2.Values;

            dary2.Add("alpha", 1);
            dary2.Add("beta", 2);
            dary2.Add("gamma", 3);

            var target = new int[dary2.Count];

            gc.CopyTo(target, 0);

            Assert.AreEqual(1, target[0]);
            Assert.AreEqual(2, target[1]);
            Assert.AreEqual(3, target[2]);
        }


        [Test]
        public void CrashRdv_gcRemove_NotSupported()
        {
            Setup();
            var gc = (System.Collections.Generic.ICollection<int>)dary2.Values;
            Assert.Throws<NotSupportedException>(() => { gc.Remove(9); });
        }

        #endregion

        #region Test Values bonus methods
#if ! TEST_BCL

        [Test]
        public void UnitRdvx_Indexer()
        {
            var rd = new RankedDictionary<string, int> { Capacity = 4 };
            foreach (var kv in greek) rd.Add(kv.Key, kv.Value);

            Assert.AreEqual(11, rd.Values[7]);
        }


        [Test]
        public void UnitRdvx_IndexOf()
        {
            var rd = new RankedDictionary<int, int> { Capacity = 5 };
            for (int ii = 0; ii < 900; ++ii)
                rd.Add(ii, ii + 1000);

            var ix1 = rd.Values.IndexOf(1500);
            Assert.AreEqual(500, ix1);

            var ix2 = rd.Values.IndexOf(77777);
            Assert.AreEqual(-1, ix2);
        }

#endif
        #endregion

        #region Test Values enumeration

        [Test]
        public void CrashRdv_ocCurrent_InvalidOperation()
        {
            Setup();
            dary2.Add("CC", 3);

            System.Collections.ICollection oc = objCol2.Values;
            System.Collections.IEnumerator etor = oc.GetEnumerator();

            Assert.Throws<InvalidOperationException>(() => { object zz = etor.Current; });
        }

        [Test]
        public void UnitRdv_GetEtor()
        {
            Setup();
            int n = 100;

            for (int k = 0; k < n; ++k)
                dary1.Add(k, k + 1000);

            int actualCount = 0;
            foreach (int value in dary1.Values)
            {
                Assert.AreEqual(actualCount + 1000, value);
                ++actualCount;
            }

            Assert.AreEqual(n, actualCount);
        }

        [Test]
        public void UnitRdv_gcGetEnumerator()
        {
            Setup();
            int n = 10;

            for (int k = 0; k < n; ++k)
                dary2.Add(k.ToString(), k);

            int expected = 0;
            var etor = genValues2.GetEnumerator();

            var rewoundVal = etor.Current;
            Assert.AreEqual(rewoundVal, default(int));

            while (etor.MoveNext())
            {
                var val = etor.Current;
                Assert.AreEqual(expected, val);
                ++expected;
            }
            Assert.AreEqual(n, expected);
        }


        [Test]
        public void UnitRdv_ocCurrent_HotUpdate()
        {
            Setup();
            dary2.Add("AA", 11);

            System.Collections.ICollection oc = objCol2.Values;
            System.Collections.IEnumerator etor = oc.GetEnumerator();

            bool ok = etor.MoveNext();
            Assert.AreEqual(11, etor.Current);

            dary2.Clear();
            Assert.AreEqual(11, etor.Current);
        }

        [Test]
        public void UnitRdv_EtorCurrentHotUpdate()
        {
            Setup();
            dary1.Add(1, -1);
            var etor1 = dary1.Values.GetEnumerator();
            Assert.AreEqual(default(int), etor1.Current);
            bool ok1 = etor1.MoveNext();
            Assert.AreEqual(-1, etor1.Current);
            dary1.Remove(1);
            Assert.AreEqual(-1, etor1.Current);

            dary2.Add("AA", 11);
            var etor2 = dary2.Values.GetEnumerator();
            Assert.AreEqual(default(int), etor2.Current);
            bool ok2 = etor2.MoveNext();
            Assert.AreEqual(11, etor2.Current);
            dary2.Clear();
            Assert.AreEqual(11, etor2.Current);
        }


        [Test]
        public void CrashRdv_EtorHotUpdate()
        {
            Setup(4);
            dary2.Add("vv", 1);
            dary2.Add("mm", 2);
            dary2.Add("qq", 3);

            Assert.Throws<InvalidOperationException>(() =>
            {
                int n = 0;
                foreach (var kv in dary2.Keys)
                {
                    if (++n == 2)
                        dary2.Clear();
                }
            });
        }

        [Test]
        public void UnitRdv_oReset()
        {
            Setup(5);
            int n = 9;

            for (int ix = 0; ix < n; ++ix)
                dary1.Add(ix, -ix);
#if TEST_BCL
            SortedDictionary<int,int>.ValueCollection.Enumerator etor;
#else
            RankedDictionary<int, int>.ValueCollection.Enumerator etor;
#endif
            etor = dary1.Values.GetEnumerator();

            int ix1 = 0;
            while (etor.MoveNext())
            {
                Assert.AreEqual(-ix1, etor.Current);
                ++ix1;
            }
            Assert.AreEqual(n, ix1);

            ((System.Collections.IEnumerator)etor).Reset();

            int ix2 = 0;
            while (etor.MoveNext())
            {
                Assert.AreEqual(-ix2, etor.Current);
                ++ix2;
            }
            Assert.AreEqual(n, ix2);
        }

        #endregion
    }
}
