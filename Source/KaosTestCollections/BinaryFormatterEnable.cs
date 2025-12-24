using System;
using Xunit;

namespace Kaos.Test.Collections;
public class BinaryFormatterEnable : IClassFixture<BinaryFormatterEnableFixture>
{
    public void OneTimeSetup()
    {
        AppContext.SetSwitch("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", true);
    }
}

public class BinaryFormatterEnableFixture
{
    public BinaryFormatterEnableFixture()
    {
        AppContext.SetSwitch("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", true);
    }
}