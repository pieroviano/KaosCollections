//
// Library: KaosCollections
// File:    TestRdSerializaton.cs
//

#pragma warning disable SYSLIB0050
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

using Xunit;
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#if TEST_BCL
using System.Collections.Generic;
#else

#endif

namespace Kaos.Test.Collections;

public partial class TestRd
{
#if !TEST_BCL
    [Fact]
    public void CrashRdz_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var dary = new PlayerDary();
            ((ISerializable)dary).GetObjectData(null, new StreamingContext());
        });
    }

    [Fact]
    public void CrashRdz_NullCB()
    {
        Assert.Throws<SerializationException>(() =>
        {
            var dary = new PlayerDary(null, new StreamingContext());
            ((IDeserializationCallback)dary).OnDeserialization(null);
        });
    }

    [Fact]
    public void CrashRdz_BadCount()
    {
        Assert.Throws<SerializationException>(() =>
        {
            var fileName = @"Targets\DaryBadCount.bin";
#pragma warning disable SYSLIB0011
            IFormatter formatter = new BinaryFormatter();
#pragma warning restore SYSLIB0011
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                var dary = (PlayerDary)formatter.Deserialize(fs);
            }
        });
    }

    [Fact]
    public void CrashRdz_MismatchKV()
    {
        Assert.Throws<SerializationException>(() =>
        {
            var fileName = @"Targets\DaryMismatchKV.bin";
#pragma warning disable SYSLIB0011
            IFormatter formatter = new BinaryFormatter();
#pragma warning restore SYSLIB0011
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                var dary = (PlayerDary)formatter.Deserialize(fs);
            }
        });
    }

    [Fact]
    public void CrashRdz_MissingKeys()
    {
        Assert.Throws<SerializationException>(() =>
        {
            var fileName = @"Targets\DaryMissingKeys.bin";
#pragma warning disable SYSLIB0011
            IFormatter formatter = new BinaryFormatter();
#pragma warning restore SYSLIB0011
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                var dary = (PlayerDary)formatter.Deserialize(fs);
            }
        });
    }

    [Fact]
    public void CrashRdz_MissingValues()
    {
        Assert.Throws<SerializationException>(() =>
        {
            var fileName = @"Targets\DaryMissingValues.bin";
#pragma warning disable SYSLIB0011
            IFormatter formatter = new BinaryFormatter();
#pragma warning restore SYSLIB0011
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                var dary = (PlayerDary)formatter.Deserialize(fs);
            }
        });
    }

#endif
    [Fact]
    public void UnitRdz_Serialization()
    {
        var fileName = "DaryScores.bin";
        var p1 = new PlayerDary
        {
            { new Player("GG", "Floyd"), 11 },
            { new Player(null, "Betty"), 22 },
            { new Player(null, "Alvin"), 33 },
            { new Player("GG", "Chuck"), 44 },
            { new Player("A1", "Ziggy"), 55 },
            { new Player("GG", null), 66 }
        };
#pragma warning disable SYSLIB0011
        IFormatter formatter = new BinaryFormatter();
#pragma warning restore SYSLIB0011
        using (var fs = new FileStream(fileName, FileMode.Create))
        {
            formatter.Serialize(fs, p1);
        }

        PlayerDary? p2 = null;
        using (var fs = new FileStream(fileName, FileMode.Open))
        {
            p2 = (PlayerDary)formatter.Deserialize(fs);
        }

        Assert.Equal(6, p2.Count);
    }

    [Fact]
    public void UnitRdz_BadSerialization()
    {
        var fileName = "BadDaryScores.bin";
        var p1 = new BadPlayerDary { { new Player("YY", "Josh"), 88 } };
#pragma warning disable SYSLIB0011
        IFormatter formatter = new BinaryFormatter();
#pragma warning restore SYSLIB0011
        using (var fs = new FileStream(fileName, FileMode.Create))
        {
            formatter.Serialize(fs, p1);
        }

        BadPlayerDary? p2 = null;
        using (var fs = new FileStream(fileName, FileMode.Open))
        {
            p2 = (BadPlayerDary)formatter.Deserialize(fs);
        }

        Assert.Equal(1, p2.Count);
    }
}