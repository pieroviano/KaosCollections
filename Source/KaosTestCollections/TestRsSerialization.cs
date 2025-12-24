//
// Library: KaosCollections
// File:    TestRsSerializaton.cs
//

#pragma warning disable SYSLIB0050
#pragma warning disable SYSLIB0011

using Xunit;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
#if TEST_BCL
using System.Collections.Generic;
#else
#endif

namespace Kaos.Test.Collections
{
    public partial class TestRs : IClassFixture<BinaryFormatterEnableFixture>
    {
        [Fact]
        public void CrashRsz_ArgumentNull()
        {
            try
            {
                var set = new StudentSet();
                ((ISerializable)set).GetObjectData(null !, new StreamingContext());
                Assert.Fail("Expected exception not thrown.");
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("OK");
            }
        }

        [Fact]
        public void CrashRsz_NullCB()
        {
            try
            {
                var set = new StudentSet(null, new StreamingContext());
                ((IDeserializationCallback)set).OnDeserialization(null);
                Assert.Fail("Expected exception not thrown.");
            }
            catch (SerializationException)
            {
                Console.WriteLine("OK");
            }
        }

#if !TEST_BCL
        [Fact]
        public void CrashRsz_BadCount()
        {
            try
            {
                string fileName = @"Targets\SetBadCount.bin";
                IFormatter formatter = new BinaryFormatter();
                using (var fs = new FileStream(fileName, FileMode.Open))
                {
                    var set = (StudentSet)formatter.Deserialize(fs);
                }

                Assert.Fail("Expected exception not thrown.");
            }
            catch (SerializationException)
            {
                Console.WriteLine("OK");
            }
        }

        [Fact]
        public void CrashRsz_MissingItems()
        {
            try
            {
                string fileName = @"Targets\SetMissingItems.bin";
                IFormatter formatter = new BinaryFormatter();
                using (var fs = new FileStream(fileName, FileMode.Open))
                {
                    var set = (StudentSet)formatter.Deserialize(fs);
                }

                Assert.Fail("Expected exception not thrown.");
            }
            catch (SerializationException)
            {
                Console.WriteLine("OK");
            }
        }

#endif
        [Fact]
        public void UnitRsz_Serialization()
        {
            string fileName = "SetOfStudents.bin";
            var set1 = new StudentSet();
            set1.Add(new Student("Floyd"));
            set1.Add(new Student("Irene"));
            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                formatter.Serialize(fs, set1);
            }

            var set2 = new StudentSet();
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                set2 = (StudentSet)formatter.Deserialize(fs);
            }

            Assert.Equal(2, set2.Count);
        }

        [Fact]
        public void UnitRsz_BadSerialization()
        {
            string fileName = "SetOfBadStudents.bin";
            var set1 = new BadStudentSet();
            set1.Add(new Student("Orville"));
            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                formatter.Serialize(fs, set1);
            }

            var set2 = new BadStudentSet();
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                set2 = (BadStudentSet)formatter.Deserialize(fs);
            }

            Assert.Equal(1, set2.Count);
        }
    }
}