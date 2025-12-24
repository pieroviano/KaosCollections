using System;
using System.Runtime.Serialization;
using Kaos.Collections;
using Xunit;

namespace Kaos.Test.Collections;

[Serializable]
public class ExamBag : RankedBag<Exam>
{
    public ExamBag() : base(new ScoreComparer())
    {
    }

    public ExamBag(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}