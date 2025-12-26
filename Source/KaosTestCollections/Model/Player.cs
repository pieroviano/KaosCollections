using System;
using System.Runtime.Serialization;
using Xunit;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8601 // Possible null reference assignment.

namespace Kaos.Test.Collections;

[Serializable]
public class Player : ISerializable
{
    public string Clan { get; private set; }
    public string Name { get; private set; }

    public Player(string clan, string name)
    {
        this.Clan = clan;
        this.Name = name;
    }

    protected Player(SerializationInfo info, StreamingContext context)
    {
        this.Clan = (string)info.GetValue("Clan", typeof(String));
        this.Name = (string)info.GetValue("Name", typeof(String));
    }

    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("Clan", Clan, typeof(String));
        info.AddValue("Name", Name, typeof(String));
    }

    public override string ToString() => Clan + "." + Name;
}