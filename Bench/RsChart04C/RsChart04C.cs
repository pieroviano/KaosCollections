//
// Program: RsChart04C.cs
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
    class RsChart04C
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
            set = new RankedSet<int>() { Capacity=4 };

            Console.WriteLine ("Create tree of order 4:");
            for (int i = 2; i <= 50; i+=2)
                set.Add (i);

            for (int i = 3; i <= 50; i+=6)
                set.Add (i);
            WriteInfo();

            Console.WriteLine ("Add 29,35:");
            set.Add (29);
            set.Add (35);
            WriteInfo();

            Console.WriteLine ("Remove 27:");
            set.Remove (27);
            WriteInfo();

            Console.WriteLine ("Remove 21:");
            set.Remove (21);
            WriteInfo();

            Console.WriteLine ("Remove 22:");
            set.Remove (22);
            WriteInfo();

            Console.WriteLine ("Remove 24:");
            set.Remove (24);
            WriteInfo();
        }

        /* Debug output:

        Create tree of order 4:

        B0: 28
        B1: 10,20 | 38,46
        B2: 4,8 | 14,16 | 22,26 | 32,34 | 40,44 | 50
        L3: 2,3|4,6|8,9 | 10,12|14,15|16,18 | 20,21|22,24|26,27 | 28,30|32,33|34,36 | 38,39|40,42|44,45 | 46,48|50

        Add 29,35:

        B0: 28
        B1: 10,20 | 38,46
        B2: 4,8 | 14,16 | 22,26 | 32,34 | 40,44 | 50
        L3: 2,3|4,6|8,9 | 10,12|14,15|16,18 | 20,21|22,24|26,27 | 28,29,30|32,33|34,35,36 | 38,39|40,42|44,45 | 46,48|50

        Remove 27:

        B0: 29
        B1: 10,20 | 38,46
        B2: 4,8 | 14,16 | 22,26 | 32,34 | 40,44 | 50
        L3: 2,3|4,6|8,9 | 10,12|14,15|16,18 | 20,21|22,24|26,28 | 29,30|32,33|34,35,36 | 38,39|40,42|44,45 | 46,48|50

        Remove 21:

        B0: 29
        B1: 10,20 | 38,46
        B2: 4,8 | 14,16 | 26 | 32,34 | 40,44 | 50
        L3: 2,3|4,6|8,9 | 10,12|14,15|16,18 | 20,22,24|26,28 | 29,30|32,33|34,35,36 | 38,39|40,42|44,45 | 46,48|50

        Remove 22:

        B0: 29
        B1: 10,20 | 38,46
        B2: 4,8 | 14,16 | 26 | 32,34 | 40,44 | 50
        L3: 2,3|4,6|8,9 | 10,12|14,15|16,18 | 20,24|26,28 | 29,30|32,33|34,35,36 | 38,39|40,42|44,45 | 46,48|50

        Remove 24:

        B0: 38
        B1: 10,20 | 46
        B2: 4,8 | 14,16 | 29,32,34 | 40,44 | 50
        L3: 2,3|4,6|8,9 | 10,12|14,15|16,18 | 20,26,28|29,30|32,33|34,35,36 | 38,39|40,42|44,45 | 46,48|50

        */
    }
}
