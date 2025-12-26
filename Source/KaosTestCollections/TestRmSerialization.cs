//
// Library: KaosCollections
// File:    TestRmSerializaton.cs
//

#pragma warning disable SYSLIB0011
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Xunit;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace Kaos.Test.Collections;

public partial class TestRm
{
    [Fact]
    public void CrashRmz_ArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var map = new PlayerMap();
#pragma warning disable SYSLIB0050
            ((ISerializable)map).GetObjectData(null, new StreamingContext());
#pragma warning restore SYSLIB0050
        });
    }

    [Fact]
    public void CrashRmz_NullCB()
    {
        Assert.Throws<SerializationException>(() =>
        {
            var map = new PlayerMap(null, new StreamingContext());
            ((IDeserializationCallback)map).OnDeserialization(null);
        });
    }

    [Fact]
    public void CrashRmz_BadCount()
    {
        Assert.Throws<SerializationException>(() =>
        {
            var fileName = @"Targets\MapBadCount.bin";
            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                var map = (PlayerMap)formatter.Deserialize(fs);
            }
        });
    }

    [Fact]
    public void CrashRmz_MismatchKV()
    {
        Assert.Throws<SerializationException>(() =>
        {
            var fileName = @"Targets\MapMismatchKV.bin";
            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                var map = (PlayerMap)formatter.Deserialize(fs);
            }
        });
    }

    [Fact]
    public void CrashRmz_MissingKeys()
    {
        Assert.Throws<SerializationException>(() =>
        {
            var fileName = @"Targets\MapMissingKeys.bin";
            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                var map = (PlayerMap)formatter.Deserialize(fs);
            }
        });
    }

    [Fact]
    public void CrashRmz_MissingValues()
    {
        Assert.Throws<SerializationException>(() =>
        {
            var fileName = @"Targets\MapMissingValues.bin";
            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                var map = (PlayerMap)formatter.Deserialize(fs);
            }
        });
    }

    [Fact]
    public void UnitRmz_Serialization()
    {
        var fileName = "MapScores.bin";
        var map1 = new PlayerMap
        {
            { new Player("GG", "Floyd"), 11 },
            { new Player(null, "Betty"), 22 },
            { new Player(null, "Alvin"), 33 },
            { new Player("GG", "Chuck"), 44 },
            { new Player("A1", "Ziggy"), 55 },
            { new Player("GG", null), 66 }
        };
        IFormatter formatter = new BinaryFormatter();
        using (var fs = new FileStream(fileName, FileMode.Create))
        {
            formatter.Serialize(fs, map1);
        }

        PlayerMap? map2;
        using (var fs = new FileStream(fileName, FileMode.Open))
        {
            map2 = (PlayerMap)formatter.Deserialize(fs);
        }

        Assert.Equal(6, map2.Count);
    }

    [Fact]
    public void UnitRmz_BadSerialization()
    {
        var fileName = "BadMapScores.bin";
        var map1 = new BadPlayerMap { { new Player("VV", "Vicky"), 11 } };
        IFormatter formatter = new BinaryFormatter();
        using (var fs = new FileStream(fileName, FileMode.Create))
        {
            formatter.Serialize(fs, map1);
        }

        BadPlayerMap? map2;
        using (var fs = new FileStream(fileName, FileMode.Open))
        {
            map2 = (BadPlayerMap)formatter.Deserialize(fs);
        }

        Assert.Equal(1, map2.Count);
    }
}