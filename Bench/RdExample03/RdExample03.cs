//
// Program: RdExample03.cs
// Purpose: Demonstrate LINQ usage and range query.
//

using System;
using System.Linq;
using System.Collections.Generic;
using Kaos.Collections;

namespace ExampleApp;

class RdExample03
{
    static void Main()
    {
        var towns = new RankedDictionary<string,int>
        {
            // Load sample data.
            { "Albany", 43600 },
            { "Bandon", 2960 },
            { "Corvallis", 54462 },
            { "Damascus", 10539 },
            { "Elkton", 195 },
            { "Florence", 8466 },
            { "Glide", 1795 },
            { "Jacksonville", 2235 },
            { "Lebanon", 13140 },
            { "Lookingglass", 855 },
            { "Medford", 75180 },
            { "Powers", 689 },
            { "Riddle", 1020 },
            { "Roseburg", 20480 },
            { "Scio", 710 },
            { "Talent", 6066 },
            { "Umatilla", 6906 },
            { "Winston", 5379 },
            { "Yamhill", 820 }
        };

        // Here's a typical LINQ-To-Objects operation.
        var avg = towns.Average (x => x.Value);
        Console.WriteLine ($"Average population of all towns = {avg:f0}");

        // Lambda expression
        var r1 = towns.Where (t => t.Key.CompareTo ("E") < 0);

        Console.WriteLine ("\nTowns A-D:");
        foreach (var e in r1)
            Console.WriteLine (e.Key);

        // LINQ range: O(n)
        var r2 = towns.SkipWhile (t => t.Key.CompareTo ("E") < 0).TakeWhile (t => t.Key.CompareTo ("J") < 0);

        Console.WriteLine ("\nTowns E-G:");
        foreach (var e in r2)
            Console.WriteLine (e.Key);

        //
        // Use the ElementsBetween iterator to query range.
        // Unlike LINQ SkipWhile and TakeWhile, this will perform an optimized (partial scan) lookup.
        //

        // BtreeDictionary range operator: O(log n)
        var r3 = towns.ElementsBetween ("K", "M");

        Console.WriteLine ("\nTowns K-L:");
        foreach (var town in r3)
            Console.WriteLine (town.Key);

        // Range operator without upper limit: O(log n)
        var r4 = towns.ElementsFrom ("M");

        Console.WriteLine ("\nTowns M-R:");
        foreach (var town in r4)
            // This avoids the issue in the last example where a town named "M" would be included.
            if (town.Key.CompareTo ("S") >= 0)
                break;
            else
                Console.WriteLine (town.Key);

        // Range operator without upper limit: O(log n)
        var r5 = towns.ElementsFrom ("T");

        Console.WriteLine ("\nTowns T-Z:");
        foreach (var town in r5)
            Console.WriteLine (town.Key);
    }

    /* Output:

    Average population of all towns = 13447

    Towns A-D:
    Albany
    Bandon
    Corvallis
    Damascus

    Towns E-G:
    Elkton
    Florence
    Glide

    Towns K-L:
    Lebanon
    Lookingglass

    Towns M-R:
    Medford
    Powers
    Riddle
    Roseburg

    Towns T-Z:
    Talent
    Umatilla
    Winston
    Yamhill

     */
}