using System;
using Xunit;

namespace Kaos.Test.Collections;

[Serializable]
public class StudentComparer : System.Collections.Generic.Comparer<Student>
{
    public override int Compare(Student? x, Student? y)
    {
        return x == null ? (y == null ? 0 : -1) : (y == null ? 1 : String.CompareOrdinal(x.Name, y.Name));
    }
}