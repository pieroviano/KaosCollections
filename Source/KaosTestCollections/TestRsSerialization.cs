//
// Library: KaosCollections
// File:    TestRsSerializaton.cs
//

using NUnit.Framework;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
# if TEST_BCL
using System.Collections.Generic;
#else
using Kaos.Collections;
#endif

namespace Kaos.Test.Collections
{
    [Serializable]
    public class StudentComparer : System.Collections.Generic.Comparer<Student>
    {
        public override int Compare (Student x, Student y)
        { return x==null ? (y==null ? 0 : -1) : (y==null ? 1 : String.Compare (x.Name, y.Name)); }
    }

    [Serializable]
    public class Student : ISerializable
    {
        public string Name { get; private set; }

        public Student (string name)
        { this.Name = name;  }

        protected Student (SerializationInfo info, StreamingContext context)
        { this.Name = (string) info.GetValue ("Name", typeof (string)); }

        public virtual void GetObjectData (SerializationInfo info, StreamingContext context)
        { info.AddValue ("Name", Name, typeof (string)); }
    }

    [Serializable]
#if TEST_BCL
    public class StudentSet : SortedSet<Student>
#else
    public class StudentSet : RankedSet<Student>
#endif
    {
        public StudentSet() : base (new StudentComparer())
        { }

        public StudentSet (SerializationInfo info, StreamingContext context) : base (info, context)
        { }
    }


    [Serializable]
#if TEST_BCL
    public class BadStudentSet : SortedSet<Student>, IDeserializationCallback
#else
    public class BadStudentSet : RankedSet<Student>, IDeserializationCallback
#endif
    {
        public BadStudentSet() : base (new StudentComparer())
        { }

        public BadStudentSet (SerializationInfo info, StreamingContext context) : base (info, context)
        { }

        void IDeserializationCallback.OnDeserialization (Object sender)
        {
            // This double call is for coverage purposes only.
            OnDeserialization (sender);
            OnDeserialization (sender);
        }
    }


    public partial class TestRs
    {
        [Test]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRsz_ArgumentNull()
        {
            var set = new StudentSet();
            ((ISerializable) set).GetObjectData (null, new StreamingContext());
        }

        [Test]
        [ExpectedException (typeof (SerializationException))]
        public void CrashRsz_NullCB()
        {
            var set = new StudentSet ((SerializationInfo) null, new StreamingContext());
            ((IDeserializationCallback) set).OnDeserialization (null);
        }

#if ! TEST_BCL
        [Test]
        [ExpectedException (typeof (SerializationException))]
        public void CrashRsz_BadCount()
        {
            string fileName = @"Targets\SetBadCount.bin";
            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream (fileName, FileMode.Open))
              { var set = (StudentSet) formatter.Deserialize (fs); }
        }

        [Test]
        [ExpectedException (typeof (SerializationException))]
        public void CrashRsz_MissingItems()
        {
            string fileName = @"Targets\SetMissingItems.bin";
            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream (fileName, FileMode.Open))
              { var set = (StudentSet) formatter.Deserialize (fs); }
        }
#endif

        [Test]
        public void UnitRsz_Serialization()
        {
            string fileName = "SetOfStudents.bin";
            var set1 = new StudentSet();
            set1.Add (new Student ("Floyd"));
            set1.Add (new Student ("Irene"));

            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream (fileName, FileMode.Create))
            { formatter.Serialize (fs, set1); }

            var set2 = new StudentSet();
            using (var fs = new FileStream (fileName, FileMode.Open))
            { set2 = (StudentSet) formatter.Deserialize (fs); }

            Assert.AreEqual (2, set2.Count);
        }


        [Test]
        public void UnitRsz_BadSerialization()
        {
            string fileName = "SetOfBadStudents.bin";
            var set1 = new BadStudentSet();
            set1.Add (new Student ("Orville"));

            IFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream (fileName, FileMode.Create))
            { formatter.Serialize (fs, set1); }

            var set2 = new BadStudentSet();
            using (var fs = new FileStream (fileName, FileMode.Open))
            { set2 = (BadStudentSet) formatter.Deserialize (fs); }

            Assert.AreEqual (1, set2.Count);
        }
    }
}
