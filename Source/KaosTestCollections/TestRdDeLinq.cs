//
// File: TestRdDeLinq.cs
// Purpose: Test LINQ emulation.
//

using System;
using System.Collections.Generic;
using SLE = System.Linq.Enumerable;
using SCG = System.Collections.Generic;
using NUnit.Framework;
#if TEST_BCL
using System.Linq;
#endif

namespace Kaos.Test.Collections
{
    public partial class TestRd
    {
        #region Test methods (LINQ emulation)

        [Test]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdq_ElementAtA_ArgumentOutOfRange()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.ElementAt (dary1, -1);
#else
            var zz = dary1.ElementAt (-1);
#endif
        }

        [Test]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdq_ElementAtB_ArgumentOutOfRange()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.ElementAt (dary1, 0);
#else
            var zz = dary1.ElementAt (0);
#endif
        }

        [Test]
        public void UnitRdq_ElementAt()
        {
            Setup();
            int n = 800;

            for (int ii = 0; ii <= n; ii+=2)
                dary1.Add (ii, ~ii);

            for (int ii = 0; ii <= n/2; ii+=2)
            {
#if TEST_BCL
                KeyValuePair<int,int> pair = Enumerable.ElementAt (dary1, ii);
#else
                KeyValuePair<int,int> pair = dary1.ElementAt (ii);
#endif
                Assert.AreEqual (ii*2, pair.Key);
                Assert.AreEqual (~(ii*2), pair.Value);
            }
        }


        [Test]
        public void UnitRdq_ElementAtOrDefault()
        {
            Setup();

#if TEST_BCL
            KeyValuePair<string,int> pairN = Enumerable.ElementAtOrDefault (dary2, -1);
            KeyValuePair<string,int> pair0 = Enumerable.ElementAtOrDefault (dary2, 0);
#else
            KeyValuePair<string,int> pairN = dary2.ElementAtOrDefault (-1);
            KeyValuePair<string,int> pair0 = dary2.ElementAtOrDefault (0);
#endif
            Assert.AreEqual (default (string), pairN.Key);
            Assert.AreEqual (default (int), pairN.Value);

            Assert.AreEqual (default (string), pair0.Key);
            Assert.AreEqual (default (int), pair0.Value);

            dary2.Add ("nein", -9);

#if TEST_BCL
            KeyValuePair<string,int> pairZ = Enumerable.ElementAtOrDefault (dary2, 0);
            KeyValuePair<string,int> pair1 = Enumerable.ElementAtOrDefault (dary2, 1);
#else
            KeyValuePair<string,int> pairZ = dary2.ElementAtOrDefault (0);
            KeyValuePair<string,int> pair1 = dary2.ElementAtOrDefault (1);
#endif
            Assert.AreEqual ("nein", pairZ.Key);
            Assert.AreEqual (-9, pairZ.Value);

            Assert.AreEqual (default (string), pair1.Key);
            Assert.AreEqual (default (int), pair1.Value);
        }


        [Test]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRdq_First_InvalidOperation()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.First (dary1);
#else
            var zz = dary1.First();
#endif
        }


        [Test]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRdq_Last_InvalidOperation()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.Last (dary1);
#else
            var zz = dary1.Last();
#endif
        }

        [Test]
        public void UnitRdq_Last()
        {
            Setup();
            for (int ii = 9; ii >= 1; --ii) dary1.Add (ii, -ii);
#if TEST_BCL
            var kv1 = Enumerable.First (dary1);
            var kv9 = Enumerable.Last (dary1);
#else
            var kv1 = dary1.First();
            var kv9 = dary1.Last();
#endif
            Assert.AreEqual (1, kv1.Key, "wrong first key");
            Assert.AreEqual (-1, kv1.Value, "wrong first value");
            Assert.AreEqual (9, kv9.Key, "wrong last key");
            Assert.AreEqual (-9, kv9.Value, "wrong last value");
        }


        [Test]
        public void UnitRdq_Skip()
        {
            Setup (4);

            Assert.AreEqual (0, SLE.Count (dary1.Skip (-1)));
            Assert.AreEqual (0, SLE.Count (dary1.Skip (0)));
            Assert.AreEqual (0, SLE.Count (dary1.Skip (1)));

            dary1.Add (1,11); dary1.Add (2,22);
            var p1 = new SCG.KeyValuePair<int,int> (1,11);
            var p2 = new SCG.KeyValuePair<int,int> (2,22);

            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p1,p2 }, dary1.Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p1,p2 }, dary1.Skip (0)));
            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p2 }, dary1.Skip (1)));

            Assert.AreEqual (0, SLE.Count (dary1.Skip (0).Skip (2)));
            Assert.AreEqual (0, SLE.Count (dary1.Skip (0).Skip (3)));
            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p1,p2 }, dary1.Skip (0).Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p1,p2 }, dary1.Skip (0).Skip (0)));
            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p2 }, dary1.Skip (0).Skip (1)));

            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p2 }, dary1.Skip (1).Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p2 }, dary1.Skip (1).Skip (0)));

            Assert.AreEqual (0, SLE.Count (dary1.Skip (2).Skip (0)));
            Assert.AreEqual (0, SLE.Count (dary1.Skip (2).Skip (1)));

            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p2,p1 }, dary1.Reverse().Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p2,p1 }, dary1.Reverse().Skip (0)));
            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p1 }, dary1.Reverse().Skip (1)));
            Assert.AreEqual (0, SLE.Count (dary1.Reverse().Skip (2)));
            Assert.AreEqual (0, SLE.Count (dary1.Reverse().Skip (3)));
        }


        [Test]
        public void UnitRdq_SkipWhile2Ctor()
        {
            Setup (5);
            var p1 = new SCG.KeyValuePair<int,int> (1,11);
            var p2 = new SCG.KeyValuePair<int,int> (2,22);

            Assert.AreEqual (0, SLE.Count (dary1.SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (dary1.SkipWhile (x => true)));

            dary1.Add (p1.Key, p1.Value); 
            dary1.Add (p2.Key, p2.Value);

            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p1,p2 }, dary1.SkipWhile (x => false)));
            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p2 }, dary1.SkipWhile (x => x.Key%2!=0)));
            Assert.AreEqual (0, SLE.Count (dary1.SkipWhile (x => true)));
        }


        [Test]
        public void UnitRdq_SkipWhile2F()
        {
            Setup (5);
            var p1 = new SCG.KeyValuePair<int,int> (1,-1);
            var p2 = new SCG.KeyValuePair<int,int> (2,-2);
            var p3 = new SCG.KeyValuePair<int,int> (3,-3);

            Assert.AreEqual (0, SLE.Count (dary1.Skip (0).SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (dary1.Skip (0).SkipWhile (x => true)));

            dary1.Add (p1.Key, p1.Value);

            Assert.AreEqual (0, SLE.Count (dary1.Skip (0).SkipWhile (x => true)));
            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p1 }, dary1.Skip(0).SkipWhile (x => false)));

            dary1.Add (p2.Key, p2.Value);
            dary1.Add (p3.Key, p3.Value);

            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p2,p3 }, dary1.Skip(0).SkipWhile (kv => kv.Key%2!=0)));
        }

        [Test]
        public void UnitRdq_SkipWhile2R()
        {
            Setup (5);
            var p1 = new SCG.KeyValuePair<int,int> (1,-1);
            var p2 = new SCG.KeyValuePair<int,int> (2,-2);
            var p3 = new SCG.KeyValuePair<int,int> (3,-3);

            Assert.AreEqual (0, SLE.Count (dary1.Reverse().SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (dary1.Reverse().SkipWhile (x => true)));

            dary1.Add (p1.Key, p1.Value);

            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p1 }, dary1.Reverse().SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (dary1.Reverse().SkipWhile (x => true)));

            dary1.Add (p2.Key, p2.Value);
            dary1.Add (p3.Key, p3.Value);

            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p2,p1 }, dary1.Reverse().SkipWhile (x => x.Key%2!=0)));
        }


        [Test]
        public void UnitRdq_SkipWhile3Ctor()
        {
            Setup (4);
            var p1 = new SCG.KeyValuePair<int,int> (1,11);
            var p2 = new SCG.KeyValuePair<int,int> (2,22);

            Assert.AreEqual (0, SLE.Count (dary1.SkipWhile ((p,i) => false)));
            Assert.AreEqual (0, SLE.Count (dary1.SkipWhile ((p,i) => true)));

            dary1.Add (p1.Key, p1.Value);
            dary1.Add (p2.Key, p2.Value);

            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p1,p2 }, dary1.SkipWhile ((p,i) => false)));
            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p2 }, dary1.SkipWhile ((p,i) => p.Key%2!=0)));
            Assert.AreEqual (0, SLE.Count (dary1.SkipWhile ((p,i) => true)));
        }

        [Test]
        public void UnitRdq_SkipWhile3F()
        {
            Setup (4);
            var p1 = new SCG.KeyValuePair<int,int> (1,-1);
            var p2 = new SCG.KeyValuePair<int,int> (2,-2);
            var p3 = new SCG.KeyValuePair<int,int> (3,-3);

            Assert.AreEqual (0, SLE.Count (dary1.Skip (0).SkipWhile ((p,i) => false)));
            Assert.AreEqual (0, SLE.Count (dary1.Skip (0).SkipWhile ((p,i) => true)));

            dary1.Add (p1.Key, p1.Value);

            Assert.AreEqual (0, SLE.Count (dary1.Skip (0).SkipWhile ((p,i) => true)));
            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p1 }, dary1.Skip(0).SkipWhile ((p,i) => false)));

            dary1.Add (p2.Key, p2.Value);
            dary1.Add (p3.Key, p3.Value);

            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p3 }, dary1.Skip(0).SkipWhile ((p,i) => p.Key%2==0 || i<1)));
            Assert.AreEqual (0, SLE.Count (dary1.Skip (0).SkipWhile ((p,i) => true)));
        }

        [Test]
        public void UnitRdq_SkipWhile3R()
        {
            Setup (4);
            var p1 = new SCG.KeyValuePair<int,int> (1,-1);
            var p2 = new SCG.KeyValuePair<int,int> (2,-2);
            var p3 = new SCG.KeyValuePair<int,int> (3,-3);

            Assert.AreEqual (0, SLE.Count (dary1.Reverse().SkipWhile ((p,i) => false)));
            Assert.AreEqual (0, SLE.Count (dary1.Reverse().SkipWhile ((p,i) => true)));

            dary1.Add (p1.Key, p1.Value);

            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p1 }, dary1.Reverse().SkipWhile ((p,i) => false)));
            Assert.AreEqual (0, SLE.Count (dary1.Reverse().SkipWhile ((p,i) => true)));

            dary1.Add (p2.Key, p2.Value);
            dary1.Add (p3.Key, p3.Value);

            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p1 }, dary1.Reverse().SkipWhile ((p,i) => p.Key%2==0 || i<1)));
            Assert.AreEqual (0, SLE.Count (dary1.Reverse().SkipWhile ((p,i) => true)));
        }

        #endregion

        #region Test bonus (LINQ emulation)
#if ! TEST_BCL

        [Test]
        public void UnitRdqx_oEtorGetEnumerator()
        {
            Setup (4);
            var ia = new int[] { 2,3,5,6,8 };
            foreach (var x in ia) dary1.Add (x, -x);

            var oAble1 = (System.Collections.IEnumerable) dary1;
            System.Collections.IEnumerator oEtor1 = oAble1.GetEnumerator();
            var oAble2 = (System.Collections.IEnumerable) oEtor1;
            System.Collections.IEnumerator oEtor2 = oAble2.GetEnumerator();

            int ix = 0;
            while (oEtor2.MoveNext())
            {
                var row = (KeyValuePair<int,int>) oEtor2.Current;
                Assert.AreEqual (ia[ix], row.Key);
                Assert.AreEqual (-ia[ix], row.Value);
                ++ix;
            }
            Assert.AreEqual (ia.Length, ix);
        }

#endif
        #endregion


        #region Test Keys methods (LINQ emulation)

        [Test]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRdkq_Contains_ArgumentNull()
        {
            Setup();
            dary2.Add ("pithy", 1);

            var zz = dary2.Keys.Contains (null);
        }

        [Test]
        public void UnitRdkq_Contains()
        {
            Setup (4);
            foreach (var kv in greek)
                dary5.Add (kv.Key, kv.Value);

            Assert.IsTrue (dary5.Keys.Contains ("Iota"));
            Assert.IsTrue (dary5.Keys.Contains ("IOTA"));
            Assert.IsFalse (dary5.Keys.Contains ("Zed"));
        }


        [Test]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdkq_ElementAt_ArgumentOutOfRange1()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.ElementAt (dary1.Keys, -1);
#else
            var zz = dary1.Keys.ElementAt (-1);
#endif
        }

        [Test]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdkq_ElementAt_ArgumentOutOfRange2()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.ElementAt (dary1.Keys, 0);
#else
            var zz = dary1.Keys.ElementAt (0);
#endif
        }

        [Test]
        public void UnitRdkq_ElementAt()
        {
            Setup();
            var keys = dary2.Keys;
            dary2.Add ("aa", 0);
            dary2.Add ("bb",-1);
            dary2.Add ("cc",-2);

#if TEST_BCL
            Assert.AreEqual ("aa", Enumerable.ElementAt (keys, 0));
            Assert.AreEqual ("bb", Enumerable.ElementAt (keys, 1));
            Assert.AreEqual ("cc", Enumerable.ElementAt (keys, 2));
#else
            Assert.AreEqual ("aa", keys.ElementAt (0));
            Assert.AreEqual ("bb", keys.ElementAt (1));
            Assert.AreEqual ("cc", keys.ElementAt (2));
#endif
        }


        [Test]
        public void UnitRdkq_ElementAtOrDefault()
        {
            Setup();
            var keys = dary2.Keys;
            dary2.Add ("aa", 0);
            dary2.Add ("bb",-1);
            dary2.Add ("cc",-2);

#if TEST_BCL
            Assert.AreEqual ("aa", Enumerable.ElementAtOrDefault (keys, 0));
            Assert.AreEqual ("bb", Enumerable.ElementAtOrDefault (keys, 1));
            Assert.AreEqual ("cc", Enumerable.ElementAtOrDefault (keys, 2));
            Assert.AreEqual (default (string), Enumerable.ElementAtOrDefault (keys, -1));
            Assert.AreEqual (default (string), Enumerable.ElementAtOrDefault (keys, 3));
#else
            Assert.AreEqual ("aa", keys.ElementAtOrDefault (0));
            Assert.AreEqual ("bb", keys.ElementAtOrDefault (1));
            Assert.AreEqual ("cc", keys.ElementAtOrDefault (2));
            Assert.AreEqual (default (string), keys.ElementAtOrDefault (-1));
            Assert.AreEqual (default (string), keys.ElementAtOrDefault (3));
#endif
        }


        [Test]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRdkq_First_InvalidOperation()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.First (dary1.Keys);
#else
            var zz = dary1.Keys.First();
#endif
        }

        [Test]
        public void UnitRdkq_First()
        {
            Setup (4);
            for (int ii = 1; ii <= 9; ++ii) dary1.Add (ii,-ii);
#if TEST_BCL
            var k1 = Enumerable.First (dary1.Keys);
#else
            var k1 = dary1.Keys.First();
#endif
            Assert.AreEqual (1, k1);
        }


        [Test]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRdkq_Last_InvalidOperation()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.Last (dary1.Keys);
#else
            var zz = dary1.Keys.Last();
#endif
        }

        [Test]
        public void UnitRdkq_Last()
        {
            Setup (4);
            for (int ii = 1; ii <= 9; ++ii) dary1.Add (ii,-ii);
#if TEST_BCL
            var k9 = Enumerable.Last (dary1.Keys);
#else
            var k9 = dary1.Keys.Last();
#endif
            Assert.AreEqual (9, k9);
        }


        [Test]
        public void UnitRdkq_Skip()
        {
            Setup();

            Assert.AreEqual (0, SLE.Count (dary1.Keys.Skip (-1)));
            Assert.AreEqual (0, SLE.Count (dary1.Keys.Skip (0)));
            Assert.AreEqual (0, SLE.Count (dary1.Keys.Skip (1)));

            dary1.Add (1,-1);
            dary1.Add (2,-2);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1,2 }, dary1.Keys.Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1,2 }, dary1.Keys.Skip (0)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2 },   dary1.Keys.Skip (1)));
            Assert.AreEqual (0, SLE.Count (dary1.Keys.Skip (2)));
            Assert.AreEqual (0, SLE.Count (dary1.Keys.Skip (3)));

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1,2 }, dary1.Keys.Skip (0).Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1,2 }, dary1.Keys.Skip (0).Skip (0)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2 },   dary1.Keys.Skip (0).Skip (1)));
            Assert.AreEqual (0, SLE.Count (dary1.Skip(0).Skip (3)));

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2,1 }, dary1.Keys.Reverse().Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2,1 }, dary1.Keys.Reverse().Skip (0)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1 },   dary1.Keys.Reverse().Skip (1)));
            Assert.AreEqual (0, SLE.Count (dary1.Keys.Reverse().Skip (2)));
            Assert.AreEqual (0, SLE.Count (dary1.Keys.Reverse().Skip (3)));
        }

        [Test]
        public void UnitRdkq_SkipWhile2Ctor()
        {
            Setup();

            Assert.AreEqual (0, SLE.Count (dary1.Keys.SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (dary1.Keys.SkipWhile (x => true)));

            dary1.Add (1,-1);
            dary1.Add (2,-2);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1,2 }, dary1.Keys.SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (dary1.Keys.SkipWhile (x => true)));

            dary1.Add (3,-3);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2,3 }, dary1.Keys.SkipWhile (x => x%2!=0)));
        }

        [Test]
        public void UnitRdkq_SkipWhile2F()
        {
            Setup();

            Assert.AreEqual (0, SLE.Count (dary1.Keys.Skip(0).SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (dary1.Keys.Skip(0).SkipWhile (x => true)));

            dary1.Add (1,-1);

            Assert.AreEqual (0, SLE.Count (dary1.Keys.Skip(0).SkipWhile (x => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1 }, dary1.Keys.Skip(0).SkipWhile (x => false)));

            dary1.Add (2,-2);
            dary1.Add (3,-3);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2,3 }, dary1.Keys.Skip(0).SkipWhile (x => x%2!=0)));
        }

        [Test]
        public void UnitRdkq_SkipWhile2R()
        {
            Setup();

            Assert.AreEqual (0, SLE.Count (dary1.Keys.Reverse().SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (dary1.Keys.Reverse().SkipWhile (x => true)));

            dary1.Add (1,-1);

            Assert.AreEqual (0, SLE.Count (dary1.Keys.Reverse().SkipWhile (x => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1 }, dary1.Keys.Reverse().SkipWhile (x => false)));

            dary1.Add (2,-2);
            dary1.Add (3,-3);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2,1 }, dary1.Keys.Reverse().SkipWhile (x => x%2!=0)));
        }


        [Test]
        public void UnitRdkq_SkipWhile3Ctor()
        {
            Setup (4);

            Assert.AreEqual (0, SLE.Count (dary1.Keys.SkipWhile ((k,i) => false)));
            Assert.AreEqual (0, SLE.Count (dary1.Keys.SkipWhile ((k,i) => true)));

            dary1.Add (1,-1);
            dary1.Add (2,-2);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1,2 }, dary1.Keys.SkipWhile ((k,i) => false)));
            Assert.AreEqual (0, SLE.Count (dary1.Keys.SkipWhile ((k,i) => true)));

            dary1.Add (3,-3);
            dary1.Add (4,-4);
            dary1.Add (5,-5);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 4,5 }, dary1.Keys.SkipWhile ((k,i) => i<2 || k%2!=0)));
        }

        [Test]
        public void UnitRdkq_SkipWhile3F()
        {
            Setup (4);

            Assert.AreEqual (0, SLE.Count (dary1.Keys.Skip(0).SkipWhile ((k,i) => false)));
            Assert.AreEqual (0, SLE.Count (dary1.Keys.Skip(0).SkipWhile ((k,i) => true)));

            dary1.Add (1,-1);
            dary1.Add (2,-2);

            Assert.AreEqual (0, SLE.Count (dary1.Keys.Skip(0).SkipWhile ((k,i) => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1,2 }, dary1.Keys.Skip(0).SkipWhile ((k,i) => false)));

            dary1.Add (3,-3);
            dary1.Add (4,-4);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 3,4 }, dary1.Keys.Skip(0).SkipWhile ((k,i) => i<1 || k%2==0)));
        }

        [Test]
        public void UnitRdkq_SkipWhile3R()
        {
            Setup (4);

            Assert.AreEqual (0, SLE.Count (dary1.Keys.Reverse().SkipWhile ((k,i) => false)));
            Assert.AreEqual (0, SLE.Count (dary1.Keys.Reverse().SkipWhile ((k,i) => true)));

            dary1.Add (1,-1);
            dary1.Add (2,-2);

            Assert.AreEqual (0, SLE.Count (dary1.Keys.Reverse().SkipWhile ((k,i) => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2,1 }, dary1.Keys.Reverse().SkipWhile ((k,i) => false)));

            dary1.Add (3,-3);
            dary1.Add (4,-4);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2,1 }, dary1.Keys.Reverse().SkipWhile ((k,i) => i<1 || k%2!=0)));
        }

        #endregion

        #region Test Keys enumeration (LINQ emulation)

        [Test]
#if ! TEST_BCL
        [ExpectedException (typeof (InvalidOperationException))]
#endif
        public void CrashRdkq_ReverseHotUpdate()
        {
            Setup (5);
            for (int ii = 9; ii > 0; --ii) dary1.Add (ii, -ii);

#if TEST_BCL
            foreach (int k1 in Enumerable.Reverse (dary1.Keys))
#else
            foreach (int k1 in dary1.Keys.Reverse())
#endif
                if (k1 == 4)
                    dary1.Clear();
        }

        [Test]
        public void UnitRdkq_Reverse()
        {
            Setup (5);
            int n = 100;

            for (int i1 = 1; i1 <= n; ++i1)
                dary1.Add (i1, -i1);

            int a0 = 0, an = 0;
#if TEST_BCL
            foreach (var k0 in Enumerable.Reverse (dary2.Keys)) ++a0;
            foreach (var kn in Enumerable.Reverse (dary1.Keys)) ++an;
#else
            foreach (var k0 in dary2.Keys.Reverse()) ++a0;
            foreach (var kn in dary1.Keys.Reverse()) ++an;
#endif
            Assert.AreEqual (0, a0);
            Assert.AreEqual (n, an);
        }

        #endregion

        #region Test Keys bonus (LINQ emulation)
#if ! TEST_BCL

        [Test]
        public void UnitRdkqx_oEtorGetEnumerator()
        {
            Setup (4);
            var ia = new int[] { 2,3,5,6,8 };
            foreach (var x in ia) dary1.Add (x, -x);

            var oAble1 = (System.Collections.IEnumerable) dary1.Keys;
            System.Collections.IEnumerator oEtor1 = oAble1.GetEnumerator();
            var oAble2 = (System.Collections.IEnumerable) oEtor1;
            System.Collections.IEnumerator oEtor2 = oAble2.GetEnumerator();

            int ix = 0;
            while (oEtor2.MoveNext())
            {
                object oValue = oEtor2.Current;
                Assert.IsTrue (oValue.GetType().IsValueType);
                Assert.AreEqual (ia[ix], oValue);
                ++ix;
            }
            Assert.AreEqual (ia.Length, ix);
        }

#endif
        #endregion


        #region Test Values methods (LINQ emulation)

        [Test]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdvq_ElementAt_ArgumentOutOfRange1()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.ElementAt (dary1.Values, -1);
#else
            var zz = dary1.Values.ElementAt (-1);
#endif
        }

        [Test]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdvq_ElementAt_ArgumentOutOfRange2()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.ElementAt (dary1.Values, 0);
#else
            var zz = dary1.Values.ElementAt (0);
#endif
        }

        [Test]
        public void UnitRdvq_ElementAt()
        {
            Setup (4);
            foreach (var kv in greek) dary2.Add (kv.Key, kv.Value);
#if TEST_BCL
            var tree1 = Enumerable.ElementAt (dary2.Values, 6);
#else
            Assert.AreEqual (9, dary2.Values.ElementAt (6));
#endif
        }


        [Test]
        public void UnitRdvq_ElementAtOrDefault()
        {
            Setup (4);
            foreach (var kv in greek) dary3.Add (kv.Key, kv.Value);
#if TEST_BCL
            Assert.IsNull (Enumerable.ElementAtOrDefault (dary3.Values, -1));
            Assert.AreEqual (22, Enumerable.ElementAtOrDefault (dary3.Values, 2));
            Assert.IsNull (Enumerable.ElementAtOrDefault (dary3.Values, dary3.Count));
#else
            Assert.IsNull (dary3.Values.ElementAtOrDefault (-1));
            Assert.AreEqual (22, dary3.Values.ElementAtOrDefault (2));
            Assert.IsNull (dary3.Values.ElementAtOrDefault (dary3.Count));
#endif
        }


        [Test]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRdvq_First()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.First (dary1.Values);
#else
            var zz = dary1.Values.First();
#endif
        }

        [Test]
        public void UnitRdvq_First()
        {
            Setup();
            for (int ii = 9; ii >= 1; --ii) dary1.Add (ii, -ii);
#if TEST_BCL
            Assert.AreEqual (-1, Enumerable.First (dary1.Values));
#else
            Assert.AreEqual (-1, dary1.Values.First());
#endif
        }

        [Test]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRdvq_Last()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.Last (dary1.Values);
#else
            var zz = dary1.Values.Last();
#endif
        }

        [Test]
        public void UnitRdvq_Last()
        {
            Setup (4);
            for (int ii = 9; ii >= 1; --ii) dary1.Add (ii, -ii);
#if TEST_BCL
            Assert.AreEqual (-9, Enumerable.Last (dary1.Values));
#else
            Assert.AreEqual (-9, dary1.Values.Last());
#endif
        }


        [Test]
        public void UnitRdvq_Skip()
        {
            Setup (4);

            Assert.AreEqual (0, SLE.Count (dary1.Values.Skip (-1)));
            Assert.AreEqual (0, SLE.Count (dary1.Values.Skip (0)));
            Assert.AreEqual (0, SLE.Count (dary1.Values.Skip (1)));

            dary1.Add (1,11);
            dary1.Add (2,22);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 11,22 }, dary1.Values.Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 11,22 }, dary1.Values.Skip (0)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 22 }, dary1.Values.Skip (1)));

            Assert.AreEqual (0, SLE.Count (dary1.Values.Skip (0).Skip (2)));
            Assert.AreEqual (0, SLE.Count (dary1.Values.Skip (0).Skip (3)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 11, 22 }, dary1.Values.Skip (0).Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 11, 22 }, dary1.Values.Skip (0).Skip (0)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 22 }, dary1.Values.Skip (0).Skip (1)));

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 22 }, dary1.Values.Skip (1).Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 22 }, dary1.Values.Skip (1).Skip (0)));

            Assert.AreEqual (0, SLE.Count (dary1.Values.Skip (2).Skip (0)));
            Assert.AreEqual (0, SLE.Count (dary1.Values.Skip (2).Skip (1)));

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 22,11 }, dary1.Values.Reverse().Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 22,11 }, dary1.Values.Reverse().Skip (0)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 11 }, dary1.Values.Reverse().Skip (1)));
            Assert.AreEqual (0, SLE.Count (dary1.Values.Reverse().Skip (2)));
            Assert.AreEqual (0, SLE.Count (dary1.Values.Reverse().Skip (3)));
        }


        [Test]
        public void UnitRdvq_SkipWhile2Ctor()
        {
            Setup (4);

            Assert.AreEqual (0, SLE.Count (dary1.Values.SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (dary1.Values.SkipWhile (x => true)));

            dary1.Add (1,-1);
            dary1.Add (2,-2);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { -1,-2 }, dary1.Values.SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (dary1.Values.SkipWhile (x => true)));

            dary1.Add (3,-3);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { -2,-3 }, dary1.Values.SkipWhile (x => x%2!=0)));
        }

        [Test]
        public void UnitRdvq_SkipWhile2F()
        {
            Setup (4);

            Assert.AreEqual (0, SLE.Count (dary1.Values.Skip(0).SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (dary1.Values.Skip(0).SkipWhile (x => true)));

            dary1.Add (1,-1);

            Assert.AreEqual (0, SLE.Count (dary1.Values.Skip(0).SkipWhile (x => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { -1 }, dary1.Values.Skip(0).SkipWhile (x => false)));

            dary1.Add (2,-2);
            dary1.Add (3,-3);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { -2,-3 }, dary1.Values.Skip(0).SkipWhile (x => x%2!=0)));
        }

        [Test]
        public void UnitRdvq_SkipWhile2R()
        {
            Setup (4);

            Assert.AreEqual (0, SLE.Count (dary1.Values.Reverse().SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (dary1.Values.Reverse().SkipWhile (x => true)));

            dary1.Add (1,-1);

            Assert.AreEqual (0, SLE.Count (dary1.Values.Reverse().SkipWhile (x => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { -1 }, dary1.Values.Reverse().SkipWhile (x => false)));

            dary1.Add (2,-2);
            dary1.Add (3,-3);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { -2,-1 }, dary1.Values.Reverse().SkipWhile (x => x%2!=0)));
        }


        [Test]
        public void UnitRdvq_SkipWhile3Ctor()
        {
            Setup (4);

            Assert.AreEqual (0, SLE.Count (dary1.Values.SkipWhile ((v,i) => false)));
            Assert.AreEqual (0, SLE.Count (dary1.Values.SkipWhile ((v,i) => true)));

            dary1.Add (1,-1);
            dary1.Add (2,-2);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { -1,-2 }, dary1.Values.SkipWhile ((v,i) => false)));
            Assert.AreEqual (0, SLE.Count (dary1.Values.SkipWhile ((v,i) => true)));

            dary1.Add (3,-3);
            dary1.Add (4,-4);
            dary1.Add (5,-5);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { -4,-5 }, dary1.Values.SkipWhile ((v,i) => v>-3 || i%2==0)));
        }

        [Test]
        public void UnitRdvq_SkipWhile3F()
        {
            Setup (4);

            Assert.AreEqual (0, SLE.Count (dary1.Values.Skip(0).SkipWhile ((v,i) => false)));
            Assert.AreEqual (0, SLE.Count (dary1.Values.Skip(0).SkipWhile ((v,i) => true)));

            dary1.Add (1,-1);
            dary1.Add (2,-2);

            Assert.AreEqual (0, SLE.Count (dary1.Values.Skip(0).SkipWhile ((v,i) => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { -1,-2 }, dary1.Values.Skip(0).SkipWhile ((v,i) => false)));

            dary1.Add (3,-3);
            dary1.Add (4,-4);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { -3,-4 }, dary1.Values.Skip(0).SkipWhile ((v,i) => v>-2 || i%2!=0)));
        }

        [Test]
        public void UnitRdvq_SkipWhile3R()
        {
            Setup (4);

            Assert.AreEqual (0, SLE.Count (dary1.Values.Reverse().SkipWhile ((v,i) => false)));
            Assert.AreEqual (0, SLE.Count (dary1.Values.Reverse().SkipWhile ((v,i) => true)));

            dary1.Add (1,-1);
            dary1.Add (2,-2);

            Assert.AreEqual (0, SLE.Count (dary1.Values.Reverse().SkipWhile ((v,i) => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { -2,-1 }, dary1.Values.Reverse().SkipWhile ((v,i) => false)));

            dary1.Add (3,-3);
            dary1.Add (4,-4);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { -2,-1 }, dary1.Values.Reverse().SkipWhile ((v,i) => v<-3 || i%2!=0)));
        }

        #endregion

        #region Test Values enumeration (LINQ emulation)

        [Test]
#if ! TEST_BCL
        [ExpectedException (typeof (InvalidOperationException))]
#endif
        public void CrashRdvq_ReverseHotUpdate()
        {
            Setup (4);
            for (int ii = 9; ii > 0; --ii) dary1.Add (ii, -ii);
#if TEST_BCL
            foreach (int v1 in Enumerable.Reverse (dary1.Values))
#else
            foreach (int v1 in dary1.Values.Reverse())
#endif
                if (v1 == -4)
                    dary1.Clear();
        }

        [Test]
        public void UnitRdvq_Reverse()
        {
            Setup (5);
            int n = 100;

            for (int i1 = 1; i1 <= n; ++i1)
                dary1.Add (i1, -i1);

            int a0 = 0, an = 0;
#if TEST_BCL
            foreach (var v0 in Enumerable.Reverse (dary2.Values)) ++a0;
            foreach (var vn in Enumerable.Reverse (dary1.Values))
#else
            foreach (var v0 in dary2.Values.Reverse()) ++a0;
            foreach (var vn in dary1.Values.Reverse())
#endif
            {
                Assert.AreEqual (an-n, vn);
                ++an;
            }

            Assert.AreEqual (0, a0);
            Assert.AreEqual (n, an);
        }

        #endregion

        #region Test Values bonus (LINQ emulation)
#if ! TEST_BCL

        [Test]
        public void UnitRdvqx_oEtorGetEnumerator()
        {
            Setup (4);
            var ia = new int[] { 2,3,5,6,8 };
            foreach (var x in ia) dary1.Add (x, -x);

            var oVals = (System.Collections.IEnumerable) dary1.Values;
            System.Collections.IEnumerator oEtor1 = oVals.GetEnumerator();
            var oAble = (System.Collections.IEnumerable) oEtor1;
            System.Collections.IEnumerator oEtor2 = oAble.GetEnumerator();

            int ix = 0;
            while (oEtor2.MoveNext())
            {
                object oValue = oEtor2.Current;
                Assert.AreEqual (-ia[ix], oValue);
                ++ix;
            }
            Assert.AreEqual (ia.Length, ix);
        }

#endif
        #endregion
    }
}
