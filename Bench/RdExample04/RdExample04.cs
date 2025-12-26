using System;
using Kaos.Collections;

namespace ExampleApp;

class RdExample04
{
    static void Main()
    {
        var dary1 = new RankedDictionary<string,int> (StringComparer.InvariantCultureIgnoreCase)
        {
            { "AAA", 0 },
            { "bbb", 1 },
            { "CCC", 2 },
            { "ddd", 3 }
        };

        Console.WriteLine ("Comparer is case insensitive:");
        foreach (var pair in dary1)
            Console.WriteLine (pair.Key);
        Console.WriteLine();

        var dary2 = new RankedDictionary<string,int> (StringComparer.Ordinal)
        {
            { "AAA", 0 },
            { "bbb", 2 },
            { "CCC", 1 },
            { "ddd", 3 }
        };

        Console.WriteLine ("Comparer is case sensitive:");
        foreach (var pair in dary2)
            Console.WriteLine (pair.Key);
    }

    /* Output:

    Comparer is case insensitive:
    AAA
    bbb
    CCC
    ddd

    Comparer is case sensitive:
    AAA
    CCC
    bbb
    ddd

    */
}