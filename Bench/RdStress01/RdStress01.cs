//
// Program: RdStress01.cs
// Purpose: Stress RankedDictionary with permutation deletes.
//
// Usage notes:
// • Not a performance test so run Debug version for full diagnostics.
// • Compilation has an external NuGet dependency for permutations.
//

using System;
using System.Threading;
using Kaos.Collections;
using Kaos.Combinatorics;

namespace StressApp
{
    class RdStress01
    {
        static void Main()
        {
            var dary = new RankedDictionary<int,int>() { Capacity = 4 };

            for (int w = 1; w < 21; ++w)
            {
                foreach (Permutation permAdd in new Permutation (w).GetRows())
                {
                    foreach (Permutation permDel in permAdd.GetRows())
                    {
                        for (int m = 0; m < permAdd.Choices; ++m)
                        {
                            dary.Add (permAdd[m], permAdd[m] + 100);
#if DEBUG
                            if (permDel.Rank == 0)
                            {
                                try
                                {
                                    dary.SanityCheck();
                                }
                                catch (DataMisalignedException ex)
                                {
                                    Console.WriteLine ("Insanity found: {0}", ex.Message);
                                    Console.WriteLine ("Width={0} add.Rank={1} m={2}", w, permAdd.Rank, m);
                                    throw;
                                }
                            }
#endif
                        }

                        for (int m = 0; m < permDel.Choices; ++m)
                        {
                            dary.Remove (permDel[m]);
#if DEBUG
                            try
                            {
                                dary.SanityCheck();
                            }
                            catch (DataMisalignedException ex)
                            {
                                Console.WriteLine ("Insanity found: {0}", ex.Message);
                                Console.WriteLine ("Width={0} add.Rank={1} del.Rank={2} m={3}",
                                                   w, permAdd.Rank, permDel.Rank, m);
                                throw;
                            }
#endif
                        }

                        if (dary.Count != 0)
                            throw new DataMisalignedException ("Count should be zero");
                        dary.Clear();
                    }
                }

                Console.WriteLine ("{2} - Completed Width {0} = {1}", w, new Permutation (w), DateTime.Now);
                Thread.Sleep (250);
            }
        }
    }
}
