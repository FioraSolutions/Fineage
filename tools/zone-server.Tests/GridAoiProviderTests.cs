using System.Diagnostics;
using Fineage.ZoneServer.Interest;
using Xunit;
using Xunit.Abstractions;

namespace Fineage.ZoneServer.Tests;

public class GridAoiProviderTests
{
    readonly ITestOutputHelper _output;

    public GridAoiProviderTests(ITestOutputHelper output) => _output = output;

    [Fact]
    public void Enter_SameCell_IsVisible()
    {
        var aoi = new GridAoiProvider();
        int cell = GridAoiProvider.CellSize;
        aoi.Enter(1, 0, 0);
        aoi.Enter(2, cell / 2, cell / 2);

        var visible = aoi.GetVisible(1);
        Assert.Single(visible);
        Assert.Contains(2, visible);
    }

    [Fact]
    public void Move_AcrossCell_UpdatesVisibility()
    {
        var aoi = new GridAoiProvider();
        int far = GridAoiProvider.CellSize * 4;
        aoi.Enter(1, 0, 0);
        aoi.Enter(2, far, 0);

        Assert.Empty(aoi.GetVisible(1));

        aoi.Move(2, GridAoiProvider.CellSize / 2, 0);
        var visible = aoi.GetVisible(1);
        Assert.Single(visible);
        Assert.Contains(2, visible);
    }

    [Fact]
    public void Leave_RemovesFromNeighbors()
    {
        var aoi = new GridAoiProvider();
        aoi.Enter(1, 0, 0);
        aoi.Enter(2, 100, 100);
        aoi.Leave(2);

        Assert.Empty(aoi.GetVisible(1));
        Assert.Equal(1, aoi.Count);
    }

    [Fact]
    public void GetVisible_ExcludesSelf()
    {
        var aoi = new GridAoiProvider();
        aoi.Enter(1, 0, 0);
        Assert.Empty(aoi.GetVisible(1));
    }

    [Fact]
    public void PackCell_NegativeCoords_Stable()
    {
        int gx = GridAoiProvider.CellCoord(-84318);
        int gy = GridAoiProvider.CellCoord(244578);
        long key = GridAoiProvider.PackCell(gx, gy);
        Assert.NotEqual(0, key);
    }

    [Fact]
    public void Benchmark_500Entities_RandomMove_GetVisibleUnderBudget()
    {
        var aoi = new GridAoiProvider();
        var rng = new Random(115);
        const int n = 500;
        for (int i = 1; i <= n; i++)
        {
            int x = rng.Next(-50_000, 50_000);
            int y = rng.Next(-50_000, 50_000);
            aoi.Enter(i, x, y);
        }

        var sw = Stopwatch.StartNew();
        const int iterations = 2000;
        for (int i = 0; i < iterations; i++)
        {
            int id = rng.Next(1, n + 1);
            int x = rng.Next(-50_000, 50_000);
            int y = rng.Next(-50_000, 50_000);
            aoi.Move(id, x, y);
            _ = aoi.GetVisible(id);
        }

        sw.Stop();
        _output.WriteLine($"GridAoi 500 entities: {iterations} move+visible in {sw.ElapsedMilliseconds} ms");
        Assert.True(sw.ElapsedMilliseconds < 5000, "AOI benchmark exceeded 5s budget");
    }
}
