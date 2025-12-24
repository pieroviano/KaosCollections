using System;
using System.Runtime.Serialization;
using Kaos.Collections;
using Xunit;

namespace Kaos.Test.Collections;

[Serializable]
public class PlayerMap : RankedMap<Player, int>
{
    public PlayerMap() : base(new PlayerComparer())
    {
    }

    public PlayerMap(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}