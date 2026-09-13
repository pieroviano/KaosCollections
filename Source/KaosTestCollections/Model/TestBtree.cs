//
// Library: KaosCollections
// File: TestInit.cs
//
// Run this suite against SortedDictionary and SortedSet to demonstrate identical
// behavior between the two API's.  To perform baseline test against BCL, add the
// TEST_BCL compilation symbol in project properties.
//

using System;
using System.Collections.Generic;
#if !TEST_BCL
using Kaos.Collections;
#endif

namespace Kaos.Test.Collections;

public class TestBtree
{
    public static bool IsAlways(int arg) => true;
    public static bool IsEven(int arg) => arg % 2 == 0;
    public static bool IsPairAlways(KeyValuePair<int, int> kv) => true;
    public static bool IsPairEven(KeyValuePair<int, int> kv) => kv.Value % 2 == 0;
    public static bool IsPairLeN100(KeyValuePair<int, int> kv) => kv.Value <= -100;

#if TEST_BCL
        public SortedDictionary<int,int> dary1;
        public SortedDictionary<string,int> dary2;
        public SortedDictionary<string,int?> dary3;
        public SortedDictionary<int,string> dary4;
        public SortedDictionary<string,int> dary5;
        public SortedSet<int> setI;
        public SortedSet<string> setS;
        public SortedSet<TS1> setTS1;
        public SortedSet<Person> personSet;
#else
    public RankedDictionary<int, int>? dary1;
    public RankedDictionary<string, int>? dary2;
    public RankedDictionary<string, int?>? dary3;
    public RankedDictionary<int, string>? dary4;
    public RankedDictionary<string, int>? dary5;
    public RankedSet<int>? setI;
    public RankedSet<string>? setS;
    public RankedSet<TS1>? setTS1;
    public RankedSet<Person>? personSet;
#endif
    ICollection<KeyValuePair<string, int>>? genCol2;
    public ICollection<string>? genKeys2;
    public ICollection<int>? genValues2;

    public System.Collections.IDictionary? objCol1, objCol2, objCol3, objCol4;

    public KeyValuePair<string, int>[] greek = new[]
    {
        new KeyValuePair<string,int> ("alpha", 1),
        new KeyValuePair<string,int> ("beta", 2),
        new KeyValuePair<string,int> ("chi", 22),
        new KeyValuePair<string,int> ("delta", 4),
        new KeyValuePair<string,int> ("epsilon", 5),
        new KeyValuePair<string,int> ("eta", 7),
        new KeyValuePair<string,int> ("iota", 9),
        new KeyValuePair<string,int> ("lambda", 11),
        new KeyValuePair<string,int> ("omega", 24)
    };

    // Must not contain value 50.
    public static int[] iVals1 = new[] { 12, 28, 15, 18, 14, 19, 25 };
    static int[] iVals2 = new[] { 10, 28, 14, 50 };
    public static int[] iVals3 = new[] { 13, 22, 51, 22, 33 };
    static int[] iVals4 = new[] { 14, 15, 19 };

    public void Setup() { Setup(5); }

    public void Setup(int order)
    {
#if TEST_BCL
            dary1 = new SortedDictionary<int,int>();
            dary2 = new SortedDictionary<string,int>();
            dary3 = new SortedDictionary<string,int?>();
            dary4 = new SortedDictionary<int,string>();
            dary5 = new SortedDictionary<string,int> (StringComparer.InvariantCultureIgnoreCase);
            setI = new SortedSet<int>();
            setS = new SortedSet<string>();
            setTS1 = new SortedSet<TS1>();
            personSet = new SortedSet<Person> (new PersonComparer());
#else
        dary1 = new RankedDictionary<int, int>();
        dary2 = new RankedDictionary<string, int>();
        dary3 = new RankedDictionary<string, int?>();
        dary4 = new RankedDictionary<int, string>();
        dary5 = new RankedDictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
        setI = new RankedSet<int>();
        setS = new RankedSet<string>();
        setTS1 = new RankedSet<TS1>();
        personSet = new RankedSet<Person>(new PersonComparer());
        dary1.Capacity = order;
        dary2.Capacity = order;
        dary3.Capacity = order;
        dary4.Capacity = order;
        setI.Capacity = order;
        setS.Capacity = order;
        setTS1.Capacity = order;
        personSet.Capacity = order;
#endif

        var treeType = dary1.GetType();

        // For testing explicit implementations.
        genCol2 = dary2;
        genKeys2 = dary2.Keys;
        genValues2 = dary2.Values;
        objCol1 = dary1;
        objCol2 = dary2;
        objCol3 = dary3;
        objCol4 = dary4;
    }
}