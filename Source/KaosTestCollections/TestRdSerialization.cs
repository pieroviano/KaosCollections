//
// Library: KaosCollections
// File:    TestRdSerializaton.cs
//

#pragma warning disable SYSLIB0050

using Xunit;
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#if TEST_BCL
using System.Collections.Generic;
#else

#endif

namespace Kaos.Test.Collections
{
    public partial class TestRd : IClassFixture<BinaryFormatterEnableFixture>
    {
#if !TEST_BCL
        [Fact]
        public void CrashRdz_ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var dary = new PlayerDary();
                ((ISerializable)dary).GetObjectData(null !, new StreamingContext());
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
                string fileName = @"Targets\DaryBadCount.bin";
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
                string fileName = @"Targets\DaryMismatchKV.bin";
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
                string fileName = @"Targets\DaryMissingKeys.bin";
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
                string fileName = @"Targets\DaryMissingValues.bin";
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
            string fileName = "DaryScores.bin";
            var p1 = new PlayerDary();
            p1.Add(new Player("GG", "Floyd"), 11);
            p1.Add(new Player(null, "Betty"), 22);
            p1.Add(new Player(null, "Alvin"), 33);
            p1.Add(new Player("GG", "Chuck"), 44);
            p1.Add(new Player("A1", "Ziggy"), 55);
            p1.Add(new Player("GG", null), 66);
#pragma warning disable SYSLIB0011
            IFormatter formatter = new BinaryFormatter();
#pragma warning restore SYSLIB0011
            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                formatter.Serialize(fs, p1);
            }

            PlayerDary p2 = null;
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                p2 = (PlayerDary)formatter.Deserialize(fs);
            }

            Assert.Equal(6, p2.Count);
        }

        [Fact]
        public void UnitRdz_BadSerialization()
        {
            string fileName = "BadDaryScores.bin";
            var p1 = new BadPlayerDary();
            p1.Add(new Player("YY", "Josh"), 88);
#pragma warning disable SYSLIB0011
            IFormatter formatter = new BinaryFormatter();
#pragma warning restore SYSLIB0011
            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                formatter.Serialize(fs, p1);
            }

            BadPlayerDary p2 = null;
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                p2 = (BadPlayerDary)formatter.Deserialize(fs);
            }

            Assert.Equal(1, p2.Count);
        }
    }
}