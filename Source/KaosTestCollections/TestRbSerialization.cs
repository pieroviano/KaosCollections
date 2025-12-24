//
// Library: KaosCollections
// File:    TestRbSerializaton.cs
//

#pragma warning disable SYSLIB0050
#pragma warning disable SYSLIB0011
#if !TEST_BCL
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Kaos.Collections;
using Xunit;

#endif

namespace Kaos.Test.Collections;

#if !TEST_BCL
public partial class TestRb : IClassFixture<BinaryFormatterEnableFixture>
{
    [Fact]
    public void CrashRbz_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var bag = new ExamBag();
            ((ISerializable)bag).GetObjectData(null !, new StreamingContext());
        });
    }

    [Fact]
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

    [Fact]
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

    [Fact]
    public void CrashRbz_NullCB()
    {
        Assert.Throws<SerializationException>(() =>
        {
            var bag = new ExamBag(null, new StreamingContext());
            ((IDeserializationCallback)bag).OnDeserialization(null);
        });
    }

    [Fact]
    public void UnitRbz_BadSerialization()
    {
        var fileName = "BagOfBadExams.bin";
        var bag1 = new BadExamBag
        {
            new Exam(1, "Agness")
        };
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

        Assert.Equal(1, bag2.Count);
    }

    [Fact]
    public void UnitRbz_Serialization()
    {
        var fileName = "BagOfExams.bin";
        var bag1 = new ExamBag
        {
            new Exam(5, "Floyd")
        };
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

        Assert.Equal(1, bag2.Count);
    }
}
#endif
