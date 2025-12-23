//
// Library: KaosCollections
// File:    TestRdSerializaton.cs
//

using NUnit.Framework;
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#if TEST_BCL
using System.Collections.Generic;
#else
using Kaos.Collections;
#pragma warning disable SYSLIB0050
#endif

namespace Kaos.Test.Collections
{
    [Serializable]
    public class PlayerComparer : System.Collections.Generic.Comparer<Player>
    {
        public override int Compare (Player x, Player y)
        {
            int cp = String.Compare (x.Clan, y.Clan);
            return cp != 0 ? cp : String.Compare (x.Name, y.Name);
        }
    }

    [Serializable]
    public class Player : ISerializable
    {
        public string Clan { get; private set; }
        public string Name { get; private set; }

        public Player (string clan, string name)
        { this.Clan = clan; this.Name = name; }

        protected Player (SerializationInfo info, StreamingContext context)
        {
            this.Clan = (string) info.GetValue ("Clan", typeof (String));
            this.Name = (string) info.GetValue ("Name", typeof (String));
        }

        public virtual void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            info.AddValue ("Clan", Clan, typeof (String));
            info.AddValue ("Name", Name, typeof (String));
        }

        public override string ToString() => Clan + "." + Name;
    }

    [Serializable]
#if TEST_BCL
    public class PlayerDary : SortedDictionary<Player,int>
#else
    public class PlayerDary : RankedDictionary<Player,int>
#endif
    {
        public PlayerDary() : base (new PlayerComparer())
        { }

#if ! TEST_BCL
        public PlayerDary (SerializationInfo info, StreamingContext context) : base (info, context)
        { }
#endif
    }

    [Serializable]
#if TEST_BCL
    public class BadPlayerDary : SortedDictionary<Player,int>
#else
    public class BadPlayerDary : RankedDictionary<Player,int>, IDeserializationCallback
#endif
    {
        public BadPlayerDary() : base (new PlayerComparer())
        { }

#if ! TEST_BCL
        public BadPlayerDary (SerializationInfo info, StreamingContext context) : base (info, context)
        { }

        void IDeserializationCallback.OnDeserialization (Object sender)
        {
            // This double call is for coverage purposes only.
            OnDeserialization (sender);
            OnDeserialization (sender);
        }
#endif
    }


    public partial class TestRd
    {
#if ! TEST_BCL
        [Test]
        public void CrashRdz_ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var dary = new PlayerDary();
                ((ISerializable) dary).GetObjectData (null!, new StreamingContext());
            });
        }

        [Test]
        public void CrashRdz_NullCB()
        {
            Assert.Throws<SerializationException>(() =>
            {
                var dary = new PlayerDary (null, new StreamingContext());
                ((IDeserializationCallback) dary).OnDeserialization (null);
            });
        }

        [Test]
        public void CrashRdz_BadCount()
        {
            Assert.Throws<SerializationException>(() =>
            {
                string fileName = @"Targets\DaryBadCount.bin";
#pragma warning disable SYSLIB0011
                IFormatter formatter = new BinaryFormatter();
#pragma warning restore SYSLIB0011
                using (var fs = new FileStream (fileName, FileMode.Open))
                  { var dary = (PlayerDary) formatter.Deserialize (fs); }
            });
        }

        [Test]
        public void CrashRdz_MismatchKV()
        {
            Assert.Throws<SerializationException>(() =>
            {
                string fileName = @"Targets\DaryMismatchKV.bin";
#pragma warning disable SYSLIB0011
                IFormatter formatter = new BinaryFormatter();
#pragma warning restore SYSLIB0011
                using (var fs = new FileStream (fileName, FileMode.Open))
                  { var dary = (PlayerDary) formatter.Deserialize (fs); }
            });
        }

        [Test]
        public void CrashRdz_MissingKeys()
        {
            Assert.Throws<SerializationException>(() =>
            {
                string fileName = @"Targets\DaryMissingKeys.bin";
#pragma warning disable SYSLIB0011
                IFormatter formatter = new BinaryFormatter();
#pragma warning restore SYSLIB0011
                using (var fs = new FileStream (fileName, FileMode.Open))
                  { var dary = (PlayerDary) formatter.Deserialize (fs); }
            });
        }

        [Test]
        public void CrashRdz_MissingValues()
        {
            Assert.Throws<SerializationException>(() =>
            {
                string fileName = @"Targets\DaryMissingValues.bin";
#pragma warning disable SYSLIB0011
                IFormatter formatter = new BinaryFormatter();
#pragma warning restore SYSLIB0011
                using (var fs = new FileStream (fileName, FileMode.Open))
                  { var dary = (PlayerDary) formatter.Deserialize (fs); }
            });
        }
#endif

        [Test]
        public void UnitRdz_Serialization()
        {
            string fileName = "DaryScores.bin";
            var p1 = new PlayerDary();
            p1.Add (new Player ("GG", "Floyd"), 11);
            p1.Add (new Player (null, "Betty"), 22);
            p1.Add (new Player (null, "Alvin"), 33);
            p1.Add (new Player ("GG", "Chuck"), 44);
            p1.Add (new Player ("A1", "Ziggy"), 55);
            p1.Add (new Player ("GG", null), 66);

#pragma warning disable SYSLIB0011
            IFormatter formatter = new BinaryFormatter();
#pragma warning restore SYSLIB0011
            using (var fs = new FileStream (fileName, FileMode.Create))
            { formatter.Serialize (fs, p1); }

            PlayerDary p2 = null;
            using (var fs = new FileStream (fileName, FileMode.Open))
            { p2 = (PlayerDary) formatter.Deserialize (fs); }

            Assert.AreEqual (6, p2.Count);
        }


        [Test]
        public void UnitRdz_BadSerialization()
        {
            string fileName = "BadDaryScores.bin";
            var p1 = new BadPlayerDary ();
            p1.Add (new Player ("YY", "Josh"), 88);

#pragma warning disable SYSLIB0011
            IFormatter formatter = new BinaryFormatter();
#pragma warning restore SYSLIB0011
            using (var fs = new FileStream (fileName, FileMode.Create))
            { formatter.Serialize (fs, p1); }

            BadPlayerDary p2 = null;
            using (var fs = new FileStream (fileName, FileMode.Open))
            { p2 = (BadPlayerDary) formatter.Deserialize (fs); }

            Assert.AreEqual (1, p2.Count);
        }
    }
}
