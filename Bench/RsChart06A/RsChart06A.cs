//
// Program: RsChart06A.cs
// Purpose: Show various tree mutation scenarios.
//
// Usage notes:
// • To include diagnostics and charts, run Debug build.
//

using System;
using System.Reflection;
using Kaos.Collections;

namespace ChartApp
{
    class RsChart06A
    {
        static RankedSet<int> set;

        static void WriteInfo (bool showStats=false)
        {
            Console.WriteLine();
#if DEBUG
            foreach (var lx in set.GenerateTreeText())
                Console.WriteLine (lx);

            set.SanityCheck();
            Console.WriteLine();

            if (showStats)
            {
                Console.WriteLine (set.GetTreeStatsText());
                Console.WriteLine();
            }
#endif
        }

        static void Main()
        {
            set = new RankedSet<int>() { Capacity=6 };

            Console.WriteLine ("Create sequentially loaded tree of order 6:");
            for (int i = 2; i <= 60; i+=2)
                set.Add (i);
            WriteInfo (true);

            Console.WriteLine ("Create rightmost nodes with 1 key by appending 62:");
                set.Add (62);
            WriteInfo();

            Console.WriteLine ("Split leaf by adding 5:");
            set.Add (5);
            WriteInfo();

            Console.WriteLine ("Split leaf, cascade split branches by adding 25:");
            set.Add (25);
            WriteInfo();

            Console.WriteLine ("Balance leaves, replace 2 pivots by removing 6:");
            set.Remove (6);
            WriteInfo();

            Console.WriteLine ("Prune leaf by removing 62:");
            set.Remove (62);
            WriteInfo();

            Console.WriteLine ("Remove 54-60:");
            set.Remove (54);
            set.Remove (56);
            set.Remove (58);
            set.Remove (60);
            WriteInfo();

            Console.WriteLine ("Prune leaf and branch by removing 52:");
            set.Remove (52);
            WriteInfo();
        }

        /* Debug output:

        Create sequentially loaded tree of order 6:

        B0: 12,22,32,42,52
        L1: 2,4,6,8,10|12,14,16,18,20|22,24,26,28,30|32,34,36,38,40|42,44,46,48,50|52,54,56,58,60

        --- height = 2, branch fill = 100%, leaf fill = 100%

        Create rightmost nodes with 1 key by appending 62:

        B0: 52
        B1: 12,22,32,42 | 62
        L2: 2,4,6,8,10|12,14,16,18,20|22,24,26,28,30|32,34,36,38,40|42,44,46,48,50 | 52,54,56,58,60|62

        Split leaf by adding 5:

        B0: 52
        B1: 6,12,22,32,42 | 62
        L2: 2,4,5|6,8,10|12,14,16,18,20|22,24,26,28,30|32,34,36,38,40|42,44,46,48,50 | 52,54,56,58,60|62

        Split leaf, cascade split branches by adding 25:

        B0: 26,52
        B1: 6,12,22 | 32,42 | 62
        L2: 2,4,5|6,8,10|12,14,16,18,20|22,24,25 | 26,28,30|32,34,36,38,40|42,44,46,48,50 | 52,54,56,58,60|62

        Balance leaves, replace 2 pivots by removing 6:

        B0: 26,52
        B1: 8,16,22 | 32,42 | 62
        L2: 2,4,5|8,10,12,14|16,18,20|22,24,25 | 26,28,30|32,34,36,38,40|42,44,46,48,50 | 52,54,56,58,60|62

        Prune leaf by removing 62:

        B0: 26,52
        B1: 8,16,22 | 32,42 |
        L2: 2,4,5|8,10,12,14|16,18,20|22,24,25 | 26,28,30|32,34,36,38,40|42,44,46,48,50 | 52,54,56,58,60

        Remove 54-60:

        B0: 26,52
        B1: 8,16,22 | 32,42 |
        L2: 2,4,5|8,10,12,14|16,18,20|22,24,25 | 26,28,30|32,34,36,38,40|42,44,46,48,50 | 52

        Prune leaf and branch by removing 52:

        B0: 26
        B1: 8,16,22 | 32,42
        L2: 2,4,5|8,10,12,14|16,18,20|22,24,25 | 26,28,30|32,34,36,38,40|42,44,46,48,50

        */
    }
}
