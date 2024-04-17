//
// Program: RsStress02.cs
// Purpose: Stress test RemoveRange operation.
//
// Usage notes:
// • Run in Debug build for tree checks e.g. underfill.
// • Run forever to indicate success.
//

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Kaos.Collections;

namespace StressApp
{
    class RsStress02
    {
        static RankedSet<int> set;

        static void Main()
        {
            for (int width = 1; ; ++width)
            {
                Console.Write (width + " ");
                for (int count = 0; count <= width; ++count)
                    for (int index = 0; index <= width-count; ++index)
                        for (int order = 4; order <= 7; ++order)
                        {
                            set = new RankedSet<int> { Capacity=order };
                            for (int ii = 0; ii < width; ++ii)
                                set.Add (ii);

                            try
                            {
                                set.RemoveRange (index, count);

                                Debug.Assert (set.Count == width-count);
                                Debug.Assert (set.Count == set.Count());
                                Debug.Assert (set.Count == set.Reverse().Count());
#if DEBUG
                                set.SanityCheck();
#endif
                            }
                            catch (Exception)
                            {
                                Console.WriteLine ($"\nwidth={width}, index={index}, count={count}, order={order}");
                                throw;
                            }
                        }
            }
        }
    }
}
