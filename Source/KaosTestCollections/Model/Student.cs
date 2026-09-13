using System;
using System.Runtime.Serialization;
using Xunit;

namespace Kaos.Test.Collections;

[Serializable]
public class Student : ISerializable
{
    public string Name { get; private set; }

    public Student(string name)
    {
        this.Name = name;
    }

    protected Student(SerializationInfo info, StreamingContext context)
    {
        this.Name = (string)info.GetValue("Name", typeof(string));
    }

    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("Name", Name, typeof(string));
    }
}