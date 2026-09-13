using System;

namespace Kaos.Test.Collections;

[Serializable]
public class PlayerComparer : System.Collections.Generic.Comparer<Player>
{
    public override int Compare(Player? x, Player? y)
    {
        var cp = String.CompareOrdinal(x?.Clan, y?.Clan);
        return cp != 0 ? cp : String.CompareOrdinal(x?.Name, y?.Name);
    }
}