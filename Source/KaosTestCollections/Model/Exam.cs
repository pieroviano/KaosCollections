using System;
using System.Runtime.Serialization;

namespace Kaos.Test.Collections;

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