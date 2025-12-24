using System;
using System.Runtime.Serialization;
using Kaos.Collections;
using Xunit;

namespace Kaos.Test.Collections;

[Serializable]
#if TEST_BCL
    public class BadStudentSet : SortedSet<Student>, IDeserializationCallback
#else
public class BadStudentSet : RankedSet<Student>, IDeserializationCallback
#endif
{
    public BadStudentSet() : base(new StudentComparer())
    {
    }

    public BadStudentSet(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    void IDeserializationCallback.OnDeserialization(Object sender)
    {
        // This double call is for coverage purposes only.
        OnDeserialization(sender);
        OnDeserialization(sender);
    }
}