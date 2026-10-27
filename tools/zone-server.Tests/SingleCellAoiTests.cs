using Fineage.ZoneServer.Interest;
using Xunit;

namespace Fineage.ZoneServer.Tests;

public class SingleCellAoiTests
{
    [Fact]
    public void Enter_TracksEntity()
    {
        var aoi = new SingleCellAoi();
        aoi.Enter(1, 0, 0);

        Assert.Equal(1, aoi.Count);
    }

    [Fact]
    public void GetVisible_ExcludesSelf()
    {
        var aoi = new SingleCellAoi();
        aoi.Enter(1, 0, 0);

        var visible = aoi.GetVisible(1);
        Assert.Empty(visible);
    }

    [Fact]
    public void GetVisible_ReturnsSingleCell_AllOthers()
    {
        var aoi = new SingleCellAoi();
        aoi.Enter(1, 0, 0);
        aoi.Enter(2, 100, 200);
        aoi.Enter(3, -500, 1000);

        var visible = aoi.GetVisible(1);
        Assert.Equal(2, visible.Count);
        Assert.Contains(2, visible);
        Assert.Contains(3, visible);
    }

    [Fact]
    public void Leave_RemovesEntity()
    {
        var aoi = new SingleCellAoi();
        aoi.Enter(1, 0, 0);
        aoi.Enter(2, 0, 0);
        aoi.Leave(2);

        Assert.Equal(1, aoi.Count);
        var visible = aoi.GetVisible(1);
        Assert.Empty(visible);
    }

    [Fact]
    public void Move_DoesNotChangeVisibility()
    {
        var aoi = new SingleCellAoi();
        aoi.Enter(1, 0, 0);
        aoi.Enter(2, 0, 0);
        aoi.Move(1, 99999, 99999);

        var visible = aoi.GetVisible(2);
        Assert.Single(visible);
        Assert.Contains(1, visible);
    }

    [Fact]
    public void EmptyAoi_GetVisible_ReturnsEmpty()
    {
        var aoi = new SingleCellAoi();
        var visible = aoi.GetVisible(1);
        Assert.Empty(visible);
    }

    [Fact]
    public void Count_ReflectsState()
    {
        var aoi = new SingleCellAoi();
        Assert.Equal(0, aoi.Count);

        aoi.Enter(1, 0, 0);
        aoi.Enter(2, 0, 0);
        Assert.Equal(2, aoi.Count);

        aoi.Leave(1);
        Assert.Equal(1, aoi.Count);
    }
}
