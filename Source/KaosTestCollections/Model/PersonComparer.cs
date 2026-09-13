using System;

namespace Kaos.Test.Collections;

public class PersonComparer : System.Collections.Generic.Comparer<Person>
{
    public override int Compare(Person? x, Person? y)
    { return x == null ? (y == null ? 0 : -1) : (y == null ? 1 : String.CompareOrdinal(x.Name, y.Name)); }
}