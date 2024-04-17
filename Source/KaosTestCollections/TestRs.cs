//
// Library: KaosCollections
// File:    TestRs.cs
//

using System;
using NUnit.Framework;
#if TEST_BCL
using System.Collections.Generic;
#else
using Kaos.Collections;
#endif

namespace Kaos.Test.Collections
{
    [TestFixture]
    public partial class TestRs : TestBtree
    {
        #region Test constructors

        [Test]
        public void UnitRs_Inheritance()
        {
            Setup();
            setI.Add (42); setI.Add (21); setI.Add (63);

            var toISetI = setI as System.Collections.Generic.ISet<int>;
            var toIColI = setI as System.Collections.Generic.ICollection<int>;
            var toIEnuI = setI as System.Collections.Generic.IEnumerable<int>;
            var toIEnuO = setI as System.Collections.IEnumerable;
            var toIColO = setI as System.Collections.ICollection;
            var toIRocI = setI as System.Collections.Generic.IReadOnlyCollection<int>;

            Assert.IsNotNull (toISetI);
            Assert.IsNotNull (toIColI);
            Assert.IsNotNull (toIEnuI);
            Assert.IsNotNull (toIEnuO);
            Assert.IsNotNull (toIColO);
            Assert.IsNotNull (toIRocI);

            int ObjEnumCount = 0;
            for (var oe = toIEnuO.GetEnumerator(); oe.MoveNext(); )
                ++ObjEnumCount;

            Assert.AreEqual (3, toISetI.Count);
            Assert.AreEqual (3, toIColI.Count);
            Assert.AreEqual (3, System.Linq.Enumerable.Count (toIEnuI));
            Assert.AreEqual (3, ObjEnumCount);
            Assert.AreEqual (3, toIColO.Count);
            Assert.AreEqual (3, toIRocI.Count);
        }


#if TEST_BCL
        public class DerivedS : SortedSet<int> { }
#else
        public class DerivedS : RankedSet<int> { }
#endif

        [Test]
        public void UnitRs_CtorSubclass()
        {
            var sub = new DerivedS();
            bool isRO = ((System.Collections.Generic.ICollection<int>) sub).IsReadOnly;
            Assert.IsFalse (isRO);
        }


        [Test]
        public void UnitRs_Ctor0Empty()
        {
            Setup();
            Assert.AreEqual (0, setTS1.Count);
        }


        [Test]
#if TEST_BCL
        [ExpectedException (typeof (ArgumentException))]
#else
        [ExpectedException (typeof (InvalidOperationException))]
#endif
        public void CrashRs_Ctor1ANullMissing_InvalidOperation()
        {
            var comp0 = (System.Collections.Generic.Comparer<Person>) null;
#if TEST_BCL
            var nullComparer = new SortedSet<Person> (comp0);
#else
            var nullComparer = new RankedSet<Person> (comp0);
#endif
            nullComparer.Add (new Person ("Zed"));
            nullComparer.Add (new Person ("Macron"));
        }

        // MS docs incorrectly state this will throw.
        [Test]
        public void UnitRs_Ctor1ANullOk()
        {
            var comp0 = (System.Collections.Generic.Comparer<int>) null;
#if TEST_BCL
            var nullComparer = new SortedSet<int> (comp0);
#else
            var nullComparer = new RankedSet<int> (comp0);
#endif
            nullComparer.Add (4);
            nullComparer.Add (2);
        }

        [Test]
#if TEST_BCL
        [ExpectedException (typeof (ArgumentException))]
#else
        [ExpectedException (typeof (InvalidOperationException))]
#endif
        public void CrashRs_Ctor1A_InvalidOperation()
        {
#if TEST_BCL
            var sansComparer = new SortedSet<Person>();
#else
            var sansComparer = new RankedSet<Person>();
#endif
            foreach (var name in Person.names)
                sansComparer.Add (new Person (name));
        }

        [Test]
        public void UnitRs_Ctor1A1()
        {
            Setup();

            foreach (var name in Person.names) personSet.Add (new Person (name));
            personSet.Add (null);
            personSet.Add (new Person ("Zed"));

            Assert.AreEqual (Person.names.Length+2, personSet.Count);
        }

        [Test]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_Ctor1B_ArgumentNull()
        {
            var nullArg = (System.Collections.Generic.ICollection<int>) null;
#if TEST_BCL
            var set = new SortedSet<int>(nullArg);
#else
            var set = new RankedSet<int>(nullArg);
#endif
        }

        [Test]
        public void UnitRs_Ctor1B()
        {
#if TEST_BCL
            var set1 = new SortedSet<int> (iVals1);
            var set3 = new SortedSet<int> (iVals3);
#else
            var set1 = new RankedSet<int> (iVals1);
            var set3 = new RankedSet<int> (iVals3);
#endif
            Assert.AreEqual (iVals1.Length, set1.Count);
            Assert.AreEqual (4, set3.Count);
        }


        [Test]
        public void UnitRs_Ctor2A()
        {
            var pa = new System.Collections.Generic.List<Person>();
            foreach (var name in Person.names) pa.Add (new Person (name));

#if TEST_BCL
            var people = new SortedSet<Person> (pa, new PersonComparer());
#else
            var people = new RankedSet<Person> (pa, new PersonComparer());
#endif
            Assert.AreEqual (Person.names.Length, people.Count);
        }

        #endregion

        #region Test class comparison

        [Test]
        public void UnitRs_CreateSetComparer0()
        {
            Setup();
            setI.Add (3); setI.Add (5); setI.Add (7);

#if TEST_BCL
            var setJ = new SortedSet<int>();
            var classComparer = SortedSet<int>.CreateSetComparer();
#else
            var setJ = new RankedSet<int>();
            System.Collections.Generic.IEqualityComparer<RankedSet<int>> classComparer
                = RankedSet<int>.CreateSetComparer();
#endif
            setJ.Add (3); setJ.Add (7);

            bool b1 = classComparer.Equals (setI, setJ);
            Assert.IsFalse (b1);

            setJ.Add (5);
            bool b2 = classComparer.Equals (setI, setJ);
            Assert.IsTrue (b2);
        }

        [Test]
        public void UnitRs_CreateSetComparer1()
        {
            Setup();
#if TEST_BCL
            var setJ = new SortedSet<int>();
            var classComparer = SortedSet<int>.CreateSetComparer();
#else
            var setJ = new RankedSet<int>();
            System.Collections.Generic.IEqualityComparer<RankedSet<int>> classComparer
                = RankedSet<int>.CreateSetComparer (System.Collections.Generic.EqualityComparer<int>.Default);
#endif
            setJ.Add (3); setJ.Add (7);

            bool b1 = classComparer.Equals (setI, setI);
            Assert.IsTrue (b1);
        }

        #endregion

        #region Test properties

        [Test]
        public void UnitRs_ocIsSynchronized()
        {
            Setup();
            var oc = (System.Collections.ICollection) setI;
            bool isSync = oc.IsSynchronized;
            Assert.IsFalse (isSync);
        }


        [Test]
        public void UnitRs_Max()
        {
            Setup (5);
            Assert.AreEqual (default (int), setI.Max);

            setI.Add (2);
            setI.Add (1);
            Assert.AreEqual (2, setI.Max);

            for (int ii = 996; ii >= 3; --ii)
                setI.Add (ii);
            Assert.AreEqual (996, setI.Max);
        }


        [Test]
        public void UnitRs_Min()
        {
            Setup (4);
            Assert.AreEqual (default (int), setI.Min);

            setI.Add (97);
            setI.Add (98);
            Assert.AreEqual (97, setI.Min);

            for (int ii = 96; ii >= 1; --ii)
                setI.Add (ii);
            Assert.AreEqual (1, setI.Min);
        }


        [Test]
        public void UnitRs_ocSyncRoot()
        {
            Setup();
            var oc = (System.Collections.ICollection) setI;
            Assert.IsFalse (oc.SyncRoot.GetType().IsValueType);
            Assert.IsFalse (oc.SyncRoot.GetType().IsValueType);
        }

        #endregion

        #region Test methods

        [Test]
        public void UnitRs_Add()
        {
            Setup();

            setS.Add ("aa");
            setS.Add ("cc");
            bool isOk1 = setS.Add ("bb");
            Assert.IsTrue (isOk1);
            bool isOk2 = setS.Add (null);
            Assert.IsTrue (isOk2);
            bool isOk3 = setS.Add ("cc");
            Assert.IsFalse (isOk3);

            Assert.AreEqual (4, setS.Count);
        }


        [Test]
        public void UnitRs_ocAdd()
        {
            Setup();
            var oc = (System.Collections.Generic.ICollection<int>) setI;

            oc.Add (3); oc.Add (5);
            Assert.AreEqual (2, setI.Count);

            oc.Add (3);
            Assert.AreEqual (2, setI.Count);
        }


        [Test]
        public void UnitRs_Clear()
        {
            Setup (4);
            for (int ix = 0; ix < 50; ++ix)
                setI.Add (ix);

            Assert.AreEqual (50, setI.Count);

            int k1 = 0;
            foreach (var i1 in setI.Reverse())
                ++k1;
            Assert.AreEqual (50, k1);

            setI.Clear();

            int k2 = 0;
            foreach (var i1 in setI.Reverse())
                ++k2;
            Assert.AreEqual (0, k2);
        }


        [Test]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_CopyTo1_ArgumentNull()
        {
            Setup();
            setI.CopyTo (null);
        }

        [Test]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRs_CopyTo1_Argument()
        {
            Setup();
            var d1 = new int[1];

            setI.Add (1); setI.Add (11);
            setI.CopyTo (d1);
        }

        [Test]
        public void UnitRs_CopyTo1()
        {
            Setup();
            var e3 = new int[] { 3, 5, 7 };
            var e4 = new int[] { 3, 5, 7, 0 };
            var d3 = new int[3];
            var d4 = new int[4];

            setI.Add (3); setI.Add (5); setI.Add (7);

            setI.CopyTo (d3);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e3, d3));

            setI.CopyTo (d4);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e4, d4));
        }

        [Test]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_CopyTo2_ArgumentNull()
        {
            Setup();
            setI.CopyTo (null, 0);
        }

        [Test]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRs_CopyTo2_ArgumentOutOfRange()
        {
            var d2 = new int[2];
            Setup();
            setI.Add (2);
            setI.CopyTo (d2, -1);
        }

        [Test]
        public void UnitRs_CopyTo2A()
        {
            Setup();
            var e2 = new int[] { 3, 5 };
            var e4 = new int[] { 0, 3, 5, 0 };
            var d2 = new int[2];
            var d4 = new int[4];

            setI.Add (3); setI.Add (5);

            setI.CopyTo (d2, 0);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e2, d2));

            setI.CopyTo (d4, 1);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e4, d4));
        }

        [Test]
        public void UnitRs_CopyTo2B()
        {
            Setup();
            var i3 = new TS1[3];

            setTS1.Add (new TS1 (4));
            setTS1.Add (new TS1 (2));

            setTS1.CopyTo (i3, 1, 2);
        }

        [Test]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_CopyTo3_ArgumentNull()
        {
            Setup();
            setI.CopyTo (null, 0, 0);
        }

        [Test]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRs_CopyTo3A_ArgumentOutOfRange()
        {
            Setup();
            var d2 = new int[2];

            setI.Add (2);
            setI.CopyTo (d2, -1, 0);
        }

        [Test]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRs_CopyTo3B_ArgumentOutOfRange()
        {
            Setup();
            var d2 = new int[2];

            setI.Add (2);
            setI.CopyTo (d2, 0, -1);
        }

        [Test]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRs_CopyTo3A_Argument()
        {
            Setup();
            var d2 = new int[2];

            setI.Add (3); setI.Add (5);

            setI.CopyTo (d2, 1, 2);
        }


        [Test]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRs_CopyTo3B_Argument()
        {
            Setup();
            var d3 = new int[3];

            setI.Add (2); setI.Add (22);

            setI.CopyTo (d3, 1, 3);
        }

        [Test]
        public void UnitRs_CopyTo3()
        {
            Setup();
            var e2 = new int[] { 0, 0 };
            var e3 = new int[] { 3, 5, 7 };
            var e4 = new int[] { 0, 3, 5, 0 };
            var e5 = new int[] { 0, 3, 5, 7, 0 };
            var d2 = new int[2];
            var d3 = new int[3];
            var d4 = new int[4];
            var d5 = new int[5];

            setI.Add (3); setI.Add (5); setI.Add (7);

            setI.CopyTo (d2, 1, 0);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e2, d2));

            setI.CopyTo (d3, 0, 3);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e3, d3));

            setI.CopyTo (d4, 1, 2);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e4, d4));

            setI.CopyTo (d5, 1, 4);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e5, d5));
        }


        [Test]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_ocCopyTo2_ArgumentNull()
        {
            Setup();
            var oc = (System.Collections.ICollection) setI;

            oc.CopyTo (null, 0);
        }

        [Test]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRs_ocCopyTo2_ArgumentOutOfRange()
        {
            Setup();
            var oc = (System.Collections.ICollection) setI;
            var d1 = new object[1];

            oc.CopyTo (d1, -1);
        }

        [Test]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRs_ocCopyTo2A_Argument()
        {
            Setup();
            var oc = (System.Collections.ICollection) setI;
            var d2 = new object[2];

            setI.Add (3); setI.Add (5);

            oc.CopyTo (d2, 1);
        }

        [Test]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRs_ocCopyTo2B_Argument()
        {
            Setup();
            var oc = (System.Collections.ICollection) setS;
            var s2 = new string[1,2];
            oc.CopyTo (s2, 0);
        }

        [Test]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRs_ocCopyTo2C_Argument()
        {
            Setup();
            var oc = (System.Collections.ICollection) setS;
            var a11 = Array.CreateInstance (typeof (int), new int[]{1}, new int[]{1});

            oc.CopyTo (a11, 1);
        }

        [Test]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRs_ocCopyTo2D_Argument()
        {
            Setup();
            var oc = (System.Collections.ICollection) setI;
            var sa = new string[2];
            var oa = (object[]) sa;

            setI.Add (3); setI.Add (5);
            oc.CopyTo (oa, 0);
        }

        [Test]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRs_ocCopyTo2E_Argument()
        {
            Setup();
            setI.Add (3); setI.Add (5);

            var oc = (System.Collections.ICollection) setI;

            var wrongType = new long[2];
            oc.CopyTo (wrongType, 0);
        }

        [Test]
        public void UnitRs_ocCopyTo2()
        {
            Setup (4);
            var e2 = new object[] { 3, 5 };
            var e4 = new object[] { null, 3, 5, null };
            var e6 = new object[] { null, 3, 5, 7, 9, 11 };
            var d2 = new object[2];
            var d4 = new object[4];
            var d6 = new object[6];
            var oc = (System.Collections.ICollection) setI;
            setI.Add (3); setI.Add (5);

            oc.CopyTo (d2, 0);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e2, d2));

            oc.CopyTo (d4, 1);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e4, d4));

            setI.Add (7); setI.Add (9); setI.Add (11);
            oc.CopyTo (d6, 1);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e6, d6));
        }

        [Test]
        public void UnitRs_Remove()
        {
            Setup (4);
#if STRESS
            int n=3200;
#else
            int n=320;
#endif

            for (int ii = n; ii >= 0; --ii)
                setI.Add (ii);

            int expected = n+1;
            for (int i2 = n; i2 >= 0; i2 -= 10)
            {
                bool isRemoved = setI.Remove (i2);
                --expected;
                Assert.IsTrue (isRemoved);
            }

            Assert.AreEqual (expected, setI.Count);
            bool isRemoved2 = setI.Remove (10);
            Assert.IsFalse (isRemoved2);
        }


        static int rwCounter = 9;
        static bool IsGe1000 (int arg) { return arg >= 1000; }
        static bool IsHotAlways (int arg) { staticSetI.Add (++rwCounter); return true; }
#if TEST_BCL
        static SortedSet<int> staticSetI = new SortedSet<int>();
#else
        static RankedSet<int> staticSetI = new RankedSet<int>();
#endif

        [Test]
#if ! TEST_BCL
        [ExpectedException (typeof (InvalidOperationException))]
#endif
        public void CrashRs_RemoveWhereHotPredicate_InvalidOperation()
        {
            Setup (4);
            staticSetI.Add (3); staticSetI.Add (4);

            // This does not throw for BCL, but it really should:
            staticSetI.RemoveWhere (IsHotAlways);
        }

        [Test]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRs_RemoveWhereHotEtor_InvalidOperation()
        {
            Setup();
            setI.Add (3); setI.Add (4);

            foreach (var key in setI)
                setI.RemoveWhere (IsEven);
        }

        [Test]
        public void UnitRs_RemoveWhereHotNonUpdate()
        {
            Setup();
            setI.Add (3); setI.Add (5);

            foreach (var key in setI)
                setI.RemoveWhere (IsEven);
        }

        [Test]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_RemoveWhere_ArgumentNull()
        {
            Setup();
            setI.RemoveWhere (null);
        }

        [Test]
        public void UnitRs_RemoveWhere()
        {
            Setup (5);

            for (int ix = 0; ix < 1200; ++ix)
                setI.Add (ix);

            int r1 = setI.RemoveWhere (IsGe1000);
            Assert.AreEqual (200, r1);
            Assert.AreEqual (1000, setI.Count);

            int c0 = setI.Count;
            int r2 = setI.RemoveWhere (IsEven);

            Assert.AreEqual (500, r2);
            foreach (int k2 in setI)
                Assert.IsTrue (k2 % 2 != 0);

            int r3 = setI.RemoveWhere (IsAlways);
            Assert.AreEqual (500, r3);
            Assert.AreEqual (0, setI.Count);
        }

        #endregion

        #region Test ISet methods

        [Test]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_ExceptWith_ArgumentNull()
        {
            Setup();
            setI.ExceptWith (null);
        }

        [Test]
        public void UnitRs_ExceptWith()
        {
            Setup();
            var a37 = new int[] { 3, 7 };
            var a5 = new int[] { 5 };
            var a133799 = new int[] { 1, 3, 3, 7, 9, 9 };
            var empty = new int[] { };

            setI.ExceptWith (empty);
            Assert.AreEqual (0, setI.Count);

            setI.ExceptWith (a37);
            Assert.AreEqual (0, setI.Count);

            setI.Add (3); setI.Add (5); setI.Add (7);

            setI.ExceptWith (a133799);
            Assert.AreEqual (1, setI.Count);
            Assert.AreEqual (5, setI.Min);

            setI.ExceptWith (a5);
            Assert.AreEqual (0, setI.Count);
        }

        [Test]
        public void UnitRs_ExceptWithSelf()
        {
            Setup();
            setI.Add (4); setI.Add (2);

            setI.ExceptWith (setI);
            Assert.AreEqual (0, setI.Count);
        }


        [Test]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_IntersectWith_ArgumentNull()
        {
            Setup();
            setI.IntersectWith (null);
        }

        [Test]
        public void UnitRs_IntersectWith()
        {
            Setup(4);
            var a1 = new int[] { 3, 5, 7, 9, 11, 13 };
            var empty = new int[] { };

            setI.IntersectWith (empty);
            Assert.AreEqual (0, setI.Count);

            foreach (var v1 in a1) setI.Add (v1);
            setI.IntersectWith (new int[] { 1 });
            Assert.AreEqual (0, setI.Count);

            setI.Clear();
            foreach (var v1 in a1) setI.Add (v1);
            setI.IntersectWith (new int[] { 15 });
            Assert.AreEqual (0, setI.Count);

            setI.Clear();
            foreach (var v1 in a1) setI.Add (v1);
            setI.IntersectWith (new int[] { 1, 9, 15 });
            Assert.AreEqual (1, setI.Count);
            Assert.AreEqual (9, setI.Min);

            setI.IntersectWith (empty);
            Assert.AreEqual (0, setI.Count);
        }


        [Test]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_SymmetricExceptWith_ArgumentNull()
        {
            Setup();
            setI.SymmetricExceptWith (null);
        }

        [Test]
        public void UnitRs_SymmetricExceptWith()
        {
            Setup (4);

            setI.SymmetricExceptWith (new int[] { });
            Assert.AreEqual (0, setI.Count);

            setI.SymmetricExceptWith (new int[] { 4, 5, 6 });
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 4, 5, 6}, setI));

            setI.SymmetricExceptWith (new int[] { 5, 7, 8 });
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 4, 6, 7, 8 }, setI));

            setI.SymmetricExceptWith (new int[] { 1, 2, 7, 8 });
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 1, 2, 4, 6 }, setI));

            setI.SymmetricExceptWith (new int[] { 2, 3 });
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual(new int[] { 1, 3, 4, 6 }, setI));
        }


        [Test]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_UnionWith_ArgumentNull()
        {
            Setup();
            setI.UnionWith (null);
        }

        [Test]
        public void UnitRs_UnionWith()
        {
            Setup();
            var a357 = new int[] { 3, 5, 7 };
            var a5599 = new int[] { 5, 5, 9, 9 };
            var empty = new int[] { };

            setI.UnionWith (empty);
            Assert.AreEqual (0, setI.Count);

            setI.UnionWith (a357);
            Assert.AreEqual (3, setI.Count);

            setI.UnionWith (a5599);
            Assert.AreEqual (4, setI.Count);
        }


        [Test]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_IsSubsetOf_ArgumentNull()
        {
            Setup();
            bool result = setI.IsSubsetOf (null);
        }

        [Test]
        public void UnitRs_IsSubsetOf()
        {
            Setup();
            var a35779 = new int[] { 3, 5, 7, 7, 9 };
            var a357 = new int[] { 3, 5, 7 };
            var a35 = new int[] { 3, 5 };
            var empty = new int[] { };

            Assert.IsTrue (setI.IsSubsetOf (a35));
            Assert.IsTrue (setI.IsSubsetOf (empty));

            setI.Add (3); setI.Add (5); setI.Add (7);

            Assert.IsTrue (setI.IsSubsetOf (a35779));
            Assert.IsTrue (setI.IsSubsetOf (a357));
            Assert.IsFalse (setI.IsSubsetOf (a35));
            Assert.IsFalse (setI.IsSubsetOf (empty));
            Assert.IsFalse (setI.IsSubsetOf (new int[] { 3, 5, 8 }));
        }


        [Test]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_IsProperSubsetOf_ArgumentNull()
        {
            Setup();
            bool result = setI.IsProperSubsetOf (null);
        }

        [Test]
        public void UnitRs_IsProperSubsetOf()
        {
            Setup();
            var a35779 = new int[] { 3, 5, 7, 7, 9 };
            var a357 = new int[] { 3, 5, 7 };
            var a35 = new int[] { 3, 5 };
            var empty = new int[] { };

            Assert.IsTrue (setI.IsProperSubsetOf (a35));
            Assert.IsFalse (setI.IsProperSubsetOf (empty));

            setI.Add (3); setI.Add (5); setI.Add (7);

            Assert.IsTrue (setI.IsProperSubsetOf (a35779));
            Assert.IsFalse (setI.IsProperSubsetOf (a357));
            Assert.IsFalse (setI.IsProperSubsetOf (a35));
            Assert.IsFalse (setI.IsProperSubsetOf (new int[] { 1, 2, 3, 4 }));
            Assert.IsFalse (setI.IsProperSubsetOf (empty));
        }

        [Test]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_IsSupersetOf_ArgumentNull()
        {
            Setup();
            bool result = setI.IsSupersetOf (null);
        }

        [Test]
        public void UnitRs_IsSupersetOf()
        {
            Setup();
            var a3579 = new int[] { 3, 5, 7, 9 };
            var a357 = new int[] { 3, 5, 7 };
            var a35 = new int[] { 3, 5 };
            var a355 = new int[] { 3, 5, 5 };
            var empty = new int[] { };

            Assert.IsTrue (setI.IsSupersetOf (empty));
            Assert.IsFalse (setI.IsSupersetOf (a35));

            setI.Add (3); setI.Add (5); setI.Add (7);

            Assert.IsFalse (setI.IsSupersetOf (a3579));
            Assert.IsTrue (setI.IsSupersetOf (a357));
            Assert.IsTrue (setI.IsSupersetOf (a35));
            Assert.IsTrue (setI.IsSupersetOf (a355));
            Assert.IsTrue (setI.IsSupersetOf (empty));
        }


        [Test]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_IsProperSupersetOf_ArgumentNull()
        {
            Setup();
            bool result = setI.IsProperSupersetOf (null);
        }

        [Test]
        public void UnitRs_IsProperSupersetOf()
        {
            Setup();
            var a3579 = new int[] { 3, 5, 7, 9 };
            var a357 = new int[] { 3, 5, 7 };
            var a35 = new int[] { 3, 5 };
            var a355 = new int[] { 3, 5, 5 };
            var empty = new int[] { };

            Assert.IsFalse (setI.IsProperSupersetOf (empty));
            Assert.IsFalse (setI.IsProperSupersetOf (a35));

            setI.Add (3); setI.Add (5); setI.Add (7);

            Assert.IsFalse (setI.IsProperSupersetOf (a3579));
            Assert.IsFalse (setI.IsProperSupersetOf (a357));
            Assert.IsTrue (setI.IsProperSupersetOf (a35));
            Assert.IsTrue (setI.IsProperSupersetOf (a355));
            Assert.IsTrue (setI.IsProperSupersetOf (empty));
            Assert.IsFalse (setI.IsProperSupersetOf (new int[] { 2, 4 }));
        }


        [Test]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_Overlaps_ArgumentNull()
        {
            Setup();
            bool result = setI.Overlaps (null);
        }

        [Test]
        public void UnitRs_OverlapsSet()
        {
            Setup();
            setI.Add (3); setI.Add (5); setI.Add (7);
            Assert.IsTrue (setI.Overlaps (setI));

#if TEST_BCL
            var set1 = new SortedSet<int> { 5, 6 };
            var set2 = new SortedSet<int> { 1, 8 };
            var set3 = new SortedSet<int> { 4 };
#else
            var set1 = new RankedSet<int> { 5, 6 };
            var set2 = new RankedSet<int> { 1, 8 };
            var set3 = new RankedSet<int> { 4 };
#endif
            Assert.IsTrue (setI.Overlaps (set1));
            Assert.IsFalse (setI.Overlaps (set2));
            Assert.IsFalse (setI.Overlaps (set3));
        }

        [Test]
        public void UnitRs_OverlapsArray()
        {
            Setup();
            var a35779 = new int[] { 3, 5, 7, 7, 9 };
            var a357 = new int[] { 3, 5, 7 };
            var a35 = new int[] { 3, 5 };
            var a355 = new int[] { 3, 5, 5 };
            var a19 = new int[] { 1, 9 };
            var empty = new int[] { };

            Assert.IsFalse (setI.Overlaps (empty));
            Assert.IsFalse (setI.Overlaps (a35));

            setI.Add (3); setI.Add (5); setI.Add (7);

            Assert.IsTrue (setI.Overlaps (a35779));
            Assert.IsTrue (setI.Overlaps (a357));
            Assert.IsTrue (setI.Overlaps (a35));
            Assert.IsFalse (setI.Overlaps (a19));
            Assert.IsFalse (setI.Overlaps (empty));
        }


        [Test]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_SetEquals_ArgumentNull()
        {
            Setup();
            bool result = setI.SetEquals (null);
        }

        [Test]
        public void UnitRs_SetEquals()
        {
            Setup();
            var a359 = new int[] { 3, 5, 9 };
            var a3557 = new int[] { 3, 5, 5, 7 };
            var a357 = new int[] { 3, 5, 7 };
            var a35 = new int[] { 3, 5 };
            var a355 = new int[] { 3, 5, 5 };
            var a19 = new int[] { 1, 9 };
            var empty = new int[] { };

            Assert.IsTrue (setI.SetEquals (empty));
            Assert.IsFalse (setI.SetEquals (a35));

            setI.Add (3); setI.Add (5); setI.Add (7);

            Assert.IsTrue (setI.SetEquals (a3557));
            Assert.IsTrue (setI.SetEquals (a357));
            Assert.IsFalse (setI.SetEquals (a359));
            Assert.IsFalse (setI.SetEquals (a35));
            Assert.IsFalse (setI.SetEquals (a355));
            Assert.IsFalse (setI.SetEquals (a19));
            Assert.IsFalse (setI.SetEquals (empty));
        }

        [Test]
        public void UnitRs_SetEquals2()
        {
            Setup();

            personSet.Add (new Person ("Fred"));
            personSet.Add (new Person ("Wilma"));

            var pa = new Person[] { new Person ("Wilma"), new Person ("Fred") };

            Assert.IsTrue (personSet.SetEquals (pa));

            personSet.Add (new Person ("Pebbles"));
            Assert.IsFalse (personSet.SetEquals (pa));
        }

        #endregion

        #region Test bonus methods
#if ! TEST_BCL

        [Test]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRsx_ElementsBetweenHotUpdate()
        {
            Setup (4);
            for (int ix=0; ix<10; ++ix) setI.Add (ix);

            int n = 0;
            foreach (int key in setI.ElementsBetween (3, 8))
            {
                if (++n == 2)
                    setI.Add (49);
            }
        }

        [Test]
        public void UnitRsx_ElementsBetween()
        {
            Setup (4);
            for (int ix=0; ix<20; ++ix) setI.Add (ix);

            int expected1 = 5;
            foreach (int key in setI.ElementsBetween (5, 15))
            {
                Assert.AreEqual (expected1, key);
                ++expected1;
            }
            Assert.AreEqual (expected1, 16);

            int expected2 = 15;
            foreach (int key in setI.ElementsBetween (15, 25))
            {
                Assert.AreEqual (expected2, key);
                ++expected2;
            }
            Assert.AreEqual (expected2, 20);
        }


        [Test]
        public void UnitRsx_ElementsFromNull()
        {
            Setup (4);

            foreach (var px in personSet.ElementsFrom (null))
            { }
        }

        [Test]
        public void UnitRsx_ElementsFrom()
        {
            Setup (4);
            for (int ii = 0; ii < 30; ++ii)
                setI.Add (ii);

            int ix = 20;
            foreach (var kx in setI.ElementsFrom (ix))
            {
                Assert.AreEqual (ix, kx);
                ++ix;
            }

            Assert.AreEqual (30, ix);
        }


        [Test]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRsx_ElementsBetweenIndexesA_ArgumentOutOfRange()
        {
            var set = new RankedSet<int> { 0, 1, 2 };
            foreach (var val in set.ElementsBetweenIndexes (-1, 0))
            { }
        }

        [Test]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRsx_ElementsBetweenIndexesB_ArgumentOutOfRange()
        {
            var set = new RankedSet<int> { 0, 1, 2 };
            foreach (var val in set.ElementsBetweenIndexes (3, 0))
            { }
        }

        [Test]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRsx_ElementsBetweenIndexesC_ArgumentOutOfRange()
        {
            var set = new RankedSet<int> { 0, 1, 2 };
            foreach (var val in set.ElementsBetweenIndexes (0, -1))
            { }
        }

        [Test]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRsx_ElementsBetweenIndexesD_ArgumentOutOfRange()
        {
            var set = new RankedSet<int> { 0, 1, 2 };
            foreach (var val in set.ElementsBetweenIndexes (0, 3))
            { }
        }

        [Test]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRsx_ElementsBetweenIndexes_Argument()
        {
            var set = new RankedSet<int> { 0, 1, 2 };
            foreach (var val in set.ElementsBetweenIndexes (2, 1))
            { }
        }

        [Test]
        public void UnitRsx_ElementsBetweenIndexes()
        {
            int n = 33;
            var set = new RankedSet<int> { Capacity=4 };
            for (int ii = 0; ii < n; ++ii)
                set.Add (ii);

            for (int p1 = 0; p1 < n; ++p1)
                for (int p2 = p1; p2 < n; ++p2)
                {
                    int actual = 0;
                    foreach (var val in set.ElementsBetweenIndexes (p1, p2))
                        actual += val;

                    int expected = (p2 - p1 + 1) * (p1 + p2) / 2;
                    Assert.AreEqual (expected, actual);
                }
        }


        [Test]
        public void UnitRsx_IndexOf()
        {
            Setup (4);
            for (int ii = 0; ii <= 98; ii+=2)
                setI.Add (ii);

            var iz = setI.IndexOf (-1);
            var i0 = setI.IndexOf (0);
            var i8 = setI.IndexOf (8);
            var i98 = setI.IndexOf (98);
            var i100 = setI.IndexOf (100);

            Assert.AreEqual (~0, iz);
            Assert.AreEqual (0, i0);
            Assert.AreEqual (4, i8);
            Assert.AreEqual (49, i98);
            Assert.AreEqual (~50, i100);
        }


        [Test]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRsx_RemoveAtA_ArgumentOutOfRange()
        {
            var rs = new RankedSet<int> { 42 };
            rs.RemoveAt (-1);
        }

        [Test]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRsx_RemoveAtB_ArgumentOutOfRange()
        {
            var rs = new RankedSet<int>();
            rs.RemoveAt (0);
        }


        [Test]
        public void UnitRsx_Replace1()
        {
            var rs = new RankedSet<string> (StringComparer.InvariantCultureIgnoreCase)
            { "aa", "BB", "cc" };

            bool r1 = rs.Replace ("bb");
            Assert.IsTrue (r1);

            bool r2 = rs.Replace ("dd");
            Assert.IsFalse (r2);

            bool r3 = rs.Replace ("AA");
            Assert.IsTrue (r3);

            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (rs, new string[] { "AA", "bb", "cc" }));
        }


        [Test]
        public void UnitRsx_Replace2()
        {
            var rs = new RankedSet<string> (StringComparer.InvariantCultureIgnoreCase)
            { "aa", "BB", "cc" };

            bool r1 = rs.Replace ("AA", true);
            Assert.IsTrue (r1);

            bool r2 = rs.Replace ("bb", false);
            Assert.IsTrue (r2);

            bool r3 = rs.Replace ("dd", true);
            Assert.IsFalse (r3);

            bool r4 = rs.Replace ("ee", false);
            Assert.IsFalse (r4);

            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (rs, new string[] { "AA", "bb", "cc", "dd" }));
        }


        [Test]
        public void UnitRsx_TryGet()
        {
            var rs = new RankedSet<string> (StringComparer.InvariantCultureIgnoreCase);

            rs.Add ("AAA");
            rs.Add ("bbb");
            rs.Add ("ccc");

            bool got1 = rs.TryGet ("aaa", out string actual1);
            Assert.IsTrue (got1);
            Assert.AreEqual ("AAA", actual1);

            bool got2 = rs.TryGet ("bb", out string actual2);
            Assert.IsFalse (got2);

            bool got3 = rs.TryGet ("CCC", out string actual3);
            Assert.IsTrue (got3);
            Assert.AreEqual ("ccc", actual3);
        }


        [Test]
        public void UnitRsx_TryGetLELT()
        {
            Setup (4);
            int n = 25;
            for (int ii = 1; ii <= n; ii+=2) setI.Add (ii);

            bool r0a = setS.TryGetLessThanOrEqual ("AA", out string k0a);
            bool r0b = setS.TryGetLessThan ("BB", out string k0b);
            Assert.IsFalse (r0a);
            Assert.AreEqual (default (string), k0a);
            Assert.IsFalse (r0b);
            Assert.AreEqual (default (string), k0b);

            for (int i1 = 3; i1 <= n; i1+=2)
            {
                bool r1a = setI.TryGetLessThanOrEqual (i1, out int k1a);
                bool r1b = setI.TryGetLessThan (i1, out int k1b);

                Assert.IsTrue (r1a);
                Assert.AreEqual (i1, k1a);
                Assert.IsTrue (r1b);
                Assert.AreEqual (i1-2, k1b);
            }

            for (int i2 = 2; i2 <= n+1; i2+=2)
            {
                bool r2a = setI.TryGetLessThanOrEqual (i2, out int k2a);
                bool r2b = setI.TryGetLessThan (i2, out int k2b);

                Assert.IsTrue (r2a);
                Assert.IsTrue (r2b);
                Assert.AreEqual (i2-1, k2a);
                Assert.AreEqual (i2-1, k2b);
            }

            bool r3a = setI.TryGetLessThanOrEqual (1, out int k3a);
            Assert.IsTrue (r3a);
            Assert.AreEqual (1, k3a);

            bool r3b = setI.TryGetLessThan (1, out int k3b);
            Assert.IsFalse (r3b);
            Assert.AreEqual (default (int), k3b);

            bool r4a = setI.TryGetLessThanOrEqual (0, out int k4a);
            Assert.IsFalse (r4a);
            Assert.AreEqual (default (int), k4a);

            bool r4b = setI.TryGetLessThan (0, out int k4b);
            Assert.IsFalse (r4b);
            Assert.AreEqual (default (int), k4b);
        }


        [Test]
        public void UnitRsx_TryGetGEGT()
        {
            Setup (4);
            int n = 99;
            for (int ii = 1; ii <= n; ii+=2) setI.Add (ii);

            bool r0a = setS.TryGetGreaterThanOrEqual ("AA", out string k0a);
            bool r0b = setS.TryGetGreaterThan ("BB", out string k0b);
            Assert.IsFalse (r0a);
            Assert.AreEqual (default (string), k0a);
            Assert.IsFalse (r0b);
            Assert.AreEqual (default (string), k0b);

            for (int i1 = 1; i1 < n; i1+=2)
            {
                bool r1a = setI.TryGetGreaterThanOrEqual (i1, out int k1a);
                bool r1b = setI.TryGetGreaterThan (i1, out int k1b);

                Assert.IsTrue (r1a);
                Assert.AreEqual (i1, k1a);
                Assert.IsTrue (r1b);
                Assert.AreEqual (i1+2, k1b);
            }

            for (int i2 = 0; i2 < n; i2+=2)
            {
                bool r2a = setI.TryGetGreaterThanOrEqual (i2, out int k2a);
                bool r2b = setI.TryGetGreaterThan (i2, out int k2b);

                Assert.IsTrue (r2a);
                Assert.IsTrue (r2b);
                Assert.AreEqual (i2+1, k2a);
                Assert.AreEqual (i2+1, k2b);
            }

            bool r3a = setI.TryGetGreaterThanOrEqual (n, out int k3a);
            Assert.IsTrue (r3a);
            Assert.AreEqual (n, k3a);

            bool r3b = setI.TryGetGreaterThan (n, out int k3b);
            Assert.IsFalse (r3b);
            Assert.AreEqual (default (int), k3b);

            bool r4a = setI.TryGetGreaterThanOrEqual (n+1, out int k4a);
            Assert.IsFalse (r4a);
            Assert.AreEqual (default (int), k4a);

            bool r4b = setI.TryGetGreaterThan (n+1, out int k4b);
            Assert.IsFalse (r4b);
            Assert.AreEqual (default (int), k4b);
        }


        [Test]
        public void UnitRsx_RemoveAt()
        {
            var rs = new RankedSet<int> { Capacity=5 };
#if STRESS
            int n = 5000, m = 10;
#else
            int n = 40, m = 5;
#endif
            for (int ii = 0; ii < n; ++ii)
                rs.Add (ii);

            for (int i2 = n-m; i2 >= 0; i2 -= m)
                rs.RemoveAt (i2);

            for (int i2 = 0; i2 < n; ++i2)
                if (i2 % m == 0)
                    Assert.IsFalse (rs.Contains (i2));
                else
                    Assert.IsTrue (rs.Contains (i2));
        }


        [Test]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRsx_RemoveRangeA_ArgumentOutOfRange()
        {
            var rs = new RankedSet<int>();
            rs.RemoveRange (-1, 0);
        }

        [Test]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRsx_RemoveRangeB_ArgumentOutOfRange()
        {
            var rs = new RankedSet<int>();
            rs.RemoveRange (0, -1);
        }

        [Test]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRsx_RemoveRange_Argument()
        {
            var rs = new RankedSet<int>();
            rs.Add (3); rs.Add (5);
            rs.RemoveRange (1, 2);
        }

        [Test]
        public void UnitRsx_RemoveRange()
        {
            var set0 = new RankedSet<int> { Capacity=7 };
            for (int ii=0; ii<9; ++ii) set0.Add (ii);

            var set1 = new RankedSet<int> { Capacity=4 };
            for (int ii=0; ii<13; ++ii) set1.Add (ii);

            var set2 = new RankedSet<int> { Capacity=4 };
            for (int ii=0; ii<19; ++ii) set2.Add (ii);

            var set3 = new RankedSet<int> { Capacity=4 };
            for (int ii=0; ii<22; ++ii) set3.Add (ii);

            var set4 = new RankedSet<int> { Capacity=7 };
            for (int ii=0; ii<7; ++ii) set4.Add (ii);

            var set5 = new RankedSet<int> { Capacity=7 };
            for (int ii=0; ii<8; ++ii) set5.Add (ii);

            var set6 = new RankedSet<int> { Capacity=7 };
            for (int ii=0; ii<13; ++ii) set6.Add (ii);

            var set7 = new RankedSet<int> { Capacity=7 };
            for (int ii=0; ii<13; ++ii) set7.Add (ii);

            var set8 = new RankedSet<int> { Capacity=5 };
            for (int ii=0; ii<21; ++ii) set8.Add (ii);

            var set9 = new RankedSet<int> { Capacity=7 };
            for (int ii=0; ii<7; ++ii) set9.Add (ii);

            var setA = new RankedSet<int> { Capacity = 6 };
            for (int ii = 0; ii < 31; ++ii) setA.Add (ii);

            var setB = new RankedSet<int> { Capacity = 6 };
            for (int ii = 0; ii < 56; ++ii) setB.Add (ii);

            var setU = new RankedSet<int> { Capacity = 5 };
            for (int ii = 0; ii < 21; ++ii) setU.Add (ii);

            var setV = new RankedSet<int> { Capacity = 5 };
            for (int ii = 0; ii < 21; ++ii) setV.Add (ii);

            var setW = new RankedSet<int> { Capacity = 5 };
            for (int ii = 0; ii < 21; ++ii) setW.Add (ii);

            var setX = new RankedSet<int> { Capacity = 6 };
            for (int ii = 0; ii < 14; ++ii) setX.Add (ii);

            var setY = new RankedSet<int> { Capacity = 7 };
            for (int ii = 0; ii < 500; ++ii) setY.Add (ii);

            var setZ = new RankedSet<int>();

            set0.RemoveRange (0, 2);  Assert.AreEqual (7, set0.Count);
            set1.RemoveRange (10,2);  Assert.AreEqual (11,set1.Count);
            set2.RemoveRange (0, 6);  Assert.AreEqual (13,set2.Count);
            set3.RemoveRange (9, 6);  Assert.AreEqual (16,set3.Count);
            set4.RemoveRange (6, 1);  Assert.AreEqual (6, set4.Count);
            set5.RemoveRange (5, 2);  Assert.AreEqual (6, set5.Count);
            set6.RemoveRange (6, 4);  Assert.AreEqual (9, set6.Count);
            set7.RemoveRange (1, 6);  Assert.AreEqual (7, set7.Count);
            set8.RemoveRange (12,4);  Assert.AreEqual (17,set8.Count);
            set9.RemoveRange (1, 5);  Assert.AreEqual (2, set9.Count);
            setA.RemoveRange (5, 23); Assert.AreEqual (8, setA.Count);
            setB.RemoveRange (5, 40); Assert.AreEqual (16,setB.Count);
            setU.RemoveRange (16, 3); Assert.AreEqual (18,setU.Count);
            setV.RemoveRange (16, 5); Assert.AreEqual (16,setV.Count);
            setW.RemoveRange (0, 16); Assert.AreEqual (5, setW.Count);
            setX.RemoveRange (1, 8);  Assert.AreEqual (6, setX.Count);
            setY.RemoveRange (0,500); Assert.AreEqual (0, setY.Count);
            setZ.RemoveRange (0, 0);  Assert.AreEqual (0, setZ.Count);
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

        [Test]
        public void StressRsx_RemoveRange()
        {
#if STRESS
            int n = 19;
#else
            int n = 11;
#endif
            for (int width = 1; width <= n; ++width)
            {
                for (int count = 0; count <= width; ++count)
                    for (int index = 0; index <= width - count; ++index)
                    {
                        var set = new RankedSet<int> { Capacity = 6 };
                        for (int ii = 0; ii < width; ++ii) set.Add (ii);
                        set.RemoveRange (index, count);
                        Assert.AreEqual (width-count, set.Count);
#if DEBUG
                        set.SanityCheck();
#endif
                    }
            }
        }

#endif
        #endregion

        #region Test enumeration

        [Test]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRs_EtorOverflow_InvalidOperation()
        {
            Setup (4);
            for (int ix=0; ix<10; ++ix) setI.Add (ix);

            var etor = setI.GetEnumerator();
            while (etor.MoveNext())
            { }

            var val = ((System.Collections.IEnumerator) etor).Current;
        }

        [Test]
        public void UnitRs_ocGetEnumerator()
        {
            Setup();
            var oc = ((System.Collections.Generic.ICollection<int>) setI);

            setI.Add (5);
            var xEtor = oc.GetEnumerator();

            xEtor.MoveNext();
            Assert.AreEqual (5, xEtor.Current);
        }

        [Test]
        public void UnitRs_EtorOverflowNoCrash()
        {
            Setup (4);
            for (int ix=0; ix<10; ++ix) setI.Add (ix);

            var etor = setI.GetEnumerator();
            while (etor.MoveNext())
            { }

            var val = etor.Current;
            Assert.AreEqual (default (int), val);
        }

        [Test]
        public void UnitRs_GetEnumerator()
        {
            int e1 = 0, e2 = 0;
            Setup (4);
            for (int ix=0; ix<10; ++ix) setI.Add (ix);

            var etor = setI.GetEnumerator();
            while (etor.MoveNext())
            {
                int gActual = etor.Current;
                object oActual = ((System.Collections.IEnumerator) etor).Current;
                Assert.AreEqual (e1, gActual);
                Assert.AreEqual (e1, oActual);
                ++e1;
            }
            Assert.AreEqual (10, e1);

            int gActualEnd = etor.Current;
            Assert.AreEqual (default (int), gActualEnd);

            bool isValid = etor.MoveNext();
            Assert.IsFalse (isValid);

            ((System.Collections.IEnumerator) etor).Reset();
            while (etor.MoveNext())
            {
                int val = etor.Current;
                Assert.AreEqual (e2, val);
                ++e2;
            }
            Assert.AreEqual (10, e2);
        }

        [Test]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRs_EtorHotUpdate()
        {
            Setup (4);
            for (int ix=0; ix<10; ++ix) setI.Add (ix);

            int n = 0;
            foreach (int kv in setI)
            {
                if (++n == 2)
                    setI.Add (49);
            }
        }

        [Test]
        public void UnitRs_ocCurrent_HotUpdate()
        {
            Setup();
            setI.Add (1);

            System.Collections.ICollection oc = setI;
            System.Collections.IEnumerator etor = oc.GetEnumerator();

            bool ok = etor.MoveNext();
            Assert.IsTrue (ok);
            Assert.AreEqual (1, etor.Current);

            setI.Clear();
            Assert.AreEqual (1, etor.Current);
        }

        [Test]
        public void UnitRs_EtorCurrentHotUpdate()
        {
            Setup();
            setI.Add (1);
            var etor1 = setI.GetEnumerator();
            Assert.AreEqual (default (int), etor1.Current);
            bool ok1 = etor1.MoveNext();
            Assert.IsTrue (ok1);
            Assert.AreEqual (1, etor1.Current);
            setI.Remove (1);
            Assert.AreEqual (1, etor1.Current);

            setS.Add ("AA");
            var etor2 = setS.GetEnumerator();
            Assert.AreEqual (default (string), etor2.Current);
            bool ok2 = etor2.MoveNext();
            Assert.AreEqual ("AA", etor2.Current);
            setS.Clear();
            Assert.AreEqual ("AA", etor2.Current);
        }


        [Test]
        public void UnitRs_oReset()
        {
            Setup (4);
            var ia = new int[] { 1,2,5,8,9 };
            foreach (var x in ia)
                setI.Add (x);

            var etor = setI.GetEnumerator();

            int ix1 = 0;
            while (etor.MoveNext())
            {
                Assert.AreEqual (ia[ix1], etor.Current);
                ++ix1;
            }
            Assert.AreEqual (ia.Length, ix1);

            ((System.Collections.IEnumerator) etor).Reset();

            int ix2 = 0;
            while (etor.MoveNext())
            {
                Assert.AreEqual (ia[ix2], etor.Current);
                ++ix2;
            }
            Assert.AreEqual (ia.Length, ix2);
        }

        #endregion
    }
}
