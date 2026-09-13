using System;
using System.Collections.Generic;
using Xunit;

namespace Kaos.Test.Collections;

[Serializable]
public class ScoreComparer : Comparer<Exam>
{
    public override int Compare(Exam x1, Exam x2)
    {
        return x1?.Score ?? 0 - x2?.Score ?? 0;
    }
}