using System;

namespace Kaos.Test.Collections;

public class TS1 : IComparable<TS1>, IComparable
{
    public int K1 { get; private set; }
    public TS1(int k1) { this.K1 = k1; }

    public int CompareTo(TS1 other) { return this.K1 - other.K1; }
    public int CompareTo(object ob) { return this.K1 - ((TS1)ob).K1; }
}