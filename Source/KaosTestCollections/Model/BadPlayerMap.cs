using System;
using System.Runtime.Serialization;
using Kaos.Collections;
using Xunit;

namespace Kaos.Test.Collections;

[Serializable]
public class BadPlayerMap : RankedMap<Player, int>, IDeserializationCallback
{
    public BadPlayerMap() : base(new PlayerComparer())
    {
    }

    public BadPlayerMap(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    void IDeserializationCallback.OnDeserialization(Object sender)
    {
        // This double call is for coverage purposes only.
        OnDeserialization(sender);
        OnDeserialization(sender);
    }
}