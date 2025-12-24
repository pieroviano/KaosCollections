using System;

namespace Kaos.Test.Collections;

[Serializable]
public class PlayerComparer : System.Collections.Generic.Comparer<Player>
{
    public override int Compare(Player x, Player y)
    {
        int cp = String.Compare(x.Clan, y.Clan);
        return cp != 0 ? cp : String.Compare(x.Name, y.Name);
    }
}