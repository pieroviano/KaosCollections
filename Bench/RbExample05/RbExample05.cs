using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Kaos.Collections;

namespace ExampleApp
{
    [Serializable]
    public class ExamComparer : Comparer<Exam>
    {
        public override int Compare (Exam x1, Exam x2) => x1.Score - x2.Score;
    }

    [Serializable]
    public class Exam : ISerializable
    {
        public int Score { get; private set; }
        public string Name { get; private set; }

        public Exam (int score, string name)
        { this.Score = score; this.Name = name; }

        protected Exam (SerializationInfo info, StreamingContext context)
        {
            this.Score = (int) info.GetValue ("Score", typeof (int));
            this.Name = (string) info.GetValue ("Name", typeof (string));
        }

        public virtual void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            info.AddValue ("Score", Score, typeof (int));
            info.AddValue ("Name", Name, typeof (string));
        }

        public override string ToString() => Score + ", " + Name;
    }

    class RbExample05
    {
        static void Main()
        {
            var bag1 = new RankedBag<Exam> (new ExamComparer());
            bag1.Add (new Exam (5, "Jack"));
            bag1.Add (new Exam (2, "Ned"));
            bag1.Add (new Exam (2, "Betty"));
            bag1.Add (new Exam (3, "Paul"));
            bag1.Add (new Exam (5, "John"));

            Console.WriteLine ("Items are inserted after other items that compare equally:");
            foreach (var item in bag1)
                Console.WriteLine ($"  {item}");

            string fileName = "Exams.bin";
            IFormatter formatter = new BinaryFormatter();

            SerializeExams (fileName, bag1, formatter);
            Console.WriteLine ($"\nWrote {bag1.Count} items to file '{fileName}'.");
            Console.WriteLine ();

            RankedBag<Exam> bag2 = DeserializeExams (fileName, formatter);
            Console.WriteLine ($"Read back {bag2.Count} items:");

            foreach (var p2 in bag2)
                Console.WriteLine ($"  {p2}");
        }

        static void SerializeExams (string fn, RankedBag<Exam> bag, IFormatter formatter)
        {
            using (var fs = new FileStream (fn, FileMode.Create))
            { formatter.Serialize (fs, bag); }
        }

        static RankedBag<Exam> DeserializeExams (string fn, IFormatter formatter)
        {
            using (var fs = new FileStream (fn, FileMode.Open))
            { return (RankedBag<Exam>) formatter.Deserialize (fs); }
        }

        /* Output:

        Items are inserted after other items that compare equally:
          2, Ned
          2, Betty
          3, Paul
          5, Jack
          5, John

        Wrote 5 items to file 'Exams.bin'.

        Read back 5 items:
          2, Ned
          2, Betty
          3, Paul
          5, Jack
          5, John

        */
    }
}
