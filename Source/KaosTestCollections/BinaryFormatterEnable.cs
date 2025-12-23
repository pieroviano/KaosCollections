using System;
using NUnit.Framework;

namespace Kaos.Test.Collections;

[SetUpFixture]
public class BinaryFormatterEnable
{
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        AppContext.SetSwitch("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", true);
    }
}