//
// File: TestRdLinq.cs
//
// Exercise some of the LINQ API derived from Enumerable. This is a partial sample and only
// Last() is required for coverage testing.
//

using System.Linq;
using Xunit;

namespace Kaos.Test.Collections
{
    public partial class TestRd : IClassFixture<BinaryFormatterEnableFixture>
    {
        [Fact]
        public void UnitRd_LinqAny()
        {
            Setup();
            var x1 = dary1.Any();
            dary1.Add(1, 10);
            dary1.Add(3, 30);
            dary1.Add(2, 20);
            var x2 = dary1.Any();
            Assert.False(x1);
            Assert.True(x2);
        }

        [Fact]
        public void UnitRd_LongCount()
        {
            Setup();
            dary1.Add(3, -33);
            dary1.Add(1, -11);
            dary1.Add(2, -22);
            var result = dary1.LongCount();
            var type = result.GetType();
            Assert.Equal(3, result);
            Assert.Equal("Int64", type.Name);
        }
    }
}