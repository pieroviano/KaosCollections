using System;
using System.Runtime.Serialization;
using Kaos.Collections;
using Xunit;

namespace Kaos.Test.Collections;

[Serializable]
#if TEST_BCL
    public class StudentSet : SortedSet<Student>
#else
public class StudentSet : RankedSet<Student>
#endif
{
    public StudentSet() : base(new StudentComparer())
    {
    }

    public StudentSet(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}