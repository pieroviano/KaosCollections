#if !TEST_BCL
using System;
using Kaos.Collections;
using Kaos.Test.Collections;
using System.Runtime.Serialization;
using Xunit;

namespace Kaos.Test.Collections;

[Serializable]
public class BadExamBag : RankedBag<Exam>, IDeserializationCallback
{
    public BadExamBag() : base(new ScoreComparer())
    {
    }

    public BadExamBag(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    void IDeserializationCallback.OnDeserialization(object sender)
    {
        // This double call is for coverage purposes only.
        OnDeserialization(sender);
        OnDeserialization(sender);
    }
}

#endif