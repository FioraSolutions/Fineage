using Xunit;

namespace Fineage.Client.Domain.Tests;

public class ZoneEpochGateTests
{
    [Theory]
    [InlineData(0, 0, true)]
    [InlineData(3, 3, true)]
    [InlineData(2, 3, false)]
    [InlineData(4, 1, false)]
    public void IsCurrent_compares_epochs(int captured, int live, bool expected)
    {
        Assert.Equal(expected, ZoneEpochGate.IsCurrent(captured, live));
    }
}
