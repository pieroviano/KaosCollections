//
// Library: KaosCollections
// File:    TestRbSerializaton.cs
//

#pragma warning disable SYSLIB0050
# if ! TEST_BCL
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Kaos.Collections;
using NUnit.Framework;

#pragma warning disable SYSLIB0011
#endif

namespace Kaos.Test.Collections;

[Serializable]
public class ScoreComparer : Comparer<Exam>
{
    public override int Compare(Exam x1, Exam x2)
    {
        return x1?.Score ?? 0 - x2?.Score ?? 0;
    }
}

[Serializable]
public class Exam : ISerializable
{
    public Exam(int score, string name)
    {
        Score = score;
        Name = name;
    }

    protected Exam(SerializationInfo info, StreamingContext context)
    {
        Score = (int)info.GetValue("Score", typeof(int))!;
        Name = (string)info.GetValue("Name", typeof(string));
    }

    public int Score { get; private set; }
    public string Name { get; private set; }

    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("Score", Score, typeof(int));
        info.AddValue("Name", Name, typeof(string));
    }
}

#if ! TEST_BCL
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

public partial class TestRb
{
    [Test]
    public void CrashRbz_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var bag = new ExamBag();
            ((ISerializable)bag).GetObjectData(null!, new StreamingContext());
        });
    }

    [Test]
    public void CrashRbz_BadCount()
    {
        Assert.Throws<SerializationException>(() =>
        {
            var fileName = @"Targets\BagBadCount.bin";
            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                var bag = (ExamBag)formatter.Deserialize(fs);
            }
        });
    }

    [Test]
    public void CrashRbz_MissingItems()
    {
        Assert.Throws<SerializationException>(() =>
        {
            var fileName = @"Targets\BagMissingItems.bin";
            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                var bag = (ExamBag)formatter.Deserialize(fs);
            }
        });
    }

    [Test]
    public void CrashRbz_NullCB()
    {
        Assert.Throws<SerializationException>(() =>
        {
            var bag = new ExamBag(null, new StreamingContext());
            ((IDeserializationCallback)bag).OnDeserialization(null);
        });
    }

    [Test]
    public void UnitRbz_BadSerialization()
    {
        var fileName = "BagOfBadExams.bin";
        var bag1 = new BadExamBag { new Exam(1, "Agness") };

        IFormatter formatter = new BinaryFormatter();
        using (var fs = new FileStream(fileName, FileMode.Create))
        {
            formatter.Serialize(fs, bag1);
        }

        BadExamBag bag2 = null;
        using (var fs = new FileStream(fileName, FileMode.Open))
        {
            bag2 = (BadExamBag)formatter.Deserialize(fs);
        }

        Assert.AreEqual(1, bag2.Count);
    }

    [Test]
    public void UnitRbz_Serialization()
    {
        var fileName = "BagOfExams.bin";
        var bag1 = new ExamBag { new Exam(5, "Floyd") };

        IFormatter formatter = new BinaryFormatter();
        using (var fs = new FileStream(fileName, FileMode.Create))
        {
            formatter.Serialize(fs, bag1);
        }

        RankedBag<Exam> bag2 = null;
        using (var fs = new FileStream(fileName, FileMode.Open))
        {
            bag2 = (ExamBag)formatter.Deserialize(fs);
        }

        Assert.AreEqual(1, bag2.Count);
    }
}
#endif