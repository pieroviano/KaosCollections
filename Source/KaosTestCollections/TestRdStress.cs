using NUnit.Framework;

namespace Kaos.Test.Collections
{
    public partial class TestRd
    {
        [Test]
        public void StressRd_WithLongPermutations()
        {
#if STRESS
            int n = 8, m = 500;
#else
            int n = 5, m = 20;
#endif
            for (int order = 5; order <= n; ++order)
            {
                Setup (order);

                for (int w = 1; w <= m; ++w)
                {
                    for (int v = 0; v < w; v++)
                        dary1.Add (v, v + 1000);
                    for (int v = w - 1; v >= 0; --v)
                        dary1.Remove (v);

                    Assert.AreEqual (0, dary1.Count);
                }
            }
        }


        [Test]
        public void StressRd_RemoveForLongBranchShift()
        {
            Setup (6);
#if STRESS
            int n=90;
#else
            int n=10;
#endif

            for (int size = 1; size < n; ++size)
            {
                for (int i = 1; i <= size; ++i)
                    dary1.Add (i, i+200);

                for (int i = 1; i <= size; ++i)
                {
                    dary1.Remove (i);

                    foreach (var kv in dary1)
                        Assert.AreEqual (kv.Key+200, kv.Value);
#if (! TEST_BCL && DEBUG)
                    dary1.SanityCheck();
#endif
                }
            }
        }


        [Test]
        public void StressRd_RemoveSlidingWindowForCoalesce()
        {
            Setup (5);
#if STRESS
            int n1=65, n2=75;
#else
            int n1=11, n2=12;
#endif
            for (int size = n1; size <= n2; ++size)
            {
                for (int a = 1; a <= size; ++a)
                    for (int b = a; b <= size; ++b)
                    {
                        dary1.Clear();
                        for (int i = 1; i <= size; ++i)
                            dary1.Add (i, i + 100);

                        for (int i = a; i <= b; ++i)
                            dary1.Remove (i);

                        foreach (var kv in dary1)
                            Assert.AreEqual (kv.Key+100, kv.Value);

#if (! TEST_BCL && DEBUG)
                        dary1.SanityCheck();
#endif
                    }
            }
        }


        [Test]
        public void StressRd_RemoveSpanForNontrivialCoalesce()
        {
            Setup();

            for (int key = 1; key < 70; ++key)
                dary1.Add (key, key + 100);

            for (int key = 19; key <= 25; ++key)
                dary1.Remove (key);
        }


        [Test]
        public void StressRd_RemoveSpanForBranchBalancing()
        {
            Setup (6);

            for (int key = 1; key <= 46; ++key)
                dary1.Add (key, key + 100);

            for (int key = 1; key <= 46; ++key)
            {
                dary1.Remove (key);

#if (! TEST_BCL && DEBUG)
                dary1.SanityCheck();
#endif
            }
        }


        [Test]
        public void StressRd_AddForSplits()
        {
            Setup (5);

            for (int k = 0; k < 99; k += 8)
                dary1.Add (k, k + 100);

            dary1.Add (20, 1);
            dary1.Add (50, 1);
            dary1.Add (66, 132);
            dary1.Remove (20);
            dary1.Add (38, 147);
            dary1.Add (35, 142);
            dary1.Add (12, 142);
            dary1.Add (10, 147);
            dary1.Add (36, 147);
            dary1.Remove (12);
            dary1.Remove (8);
            dary1.Remove (10);
            dary1.Remove (88);

            dary1.Remove (56);
            dary1.Remove (80);
            dary1.Remove (96);
            dary1.Add (18, 118);
            dary1.Add (11, 111);

#if (! TEST_BCL && DEBUG)
            dary1.SanityCheck();
#endif
        }
    }
}
