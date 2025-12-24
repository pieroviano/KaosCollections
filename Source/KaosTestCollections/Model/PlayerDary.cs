using System;
using System.Runtime.Serialization;
using Kaos.Collections;
using Xunit;

namespace Kaos.Test.Collections;

[Serializable]
#if TEST_BCL
    public class PlayerDary : SortedDictionary<Player,int>
#else
public class PlayerDary : RankedDictionary<Player, int>
#endif
{
    public PlayerDary() : base(new PlayerComparer())
    {
    }

#if !TEST_BCL
    public PlayerDary(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
#endif
}