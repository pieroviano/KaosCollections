using System;
using System.Runtime.Serialization;
using Kaos.Collections;
using Xunit;

namespace Kaos.Test.Collections;

[Serializable]
#if TEST_BCL
    public class BadPlayerDary : SortedDictionary<Player,int>
#else
public class BadPlayerDary : RankedDictionary<Player, int>, IDeserializationCallback
#endif
{
    public BadPlayerDary() : base(new PlayerComparer())
    {
    }

#if !TEST_BCL
    public BadPlayerDary(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    void IDeserializationCallback.OnDeserialization(Object sender)
    {
        // This double call is for coverage purposes only.
        OnDeserialization(sender);
        OnDeserialization(sender);
    }
#endif
}