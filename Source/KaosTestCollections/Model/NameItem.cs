#if !TEST_BCL
using System;

namespace Kaos.Test.Collections;

public class NameItem
{
    public NameItem(string name)
    {
        this.Name = name;
    }

    public String Name { get; private set; }
}
#endif