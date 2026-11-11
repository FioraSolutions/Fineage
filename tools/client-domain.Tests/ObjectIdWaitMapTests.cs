using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fineage.Client.Domain.Tests;

public class ObjectIdWaitMapTests
{
    [Fact]
    public async Task WaitAsync_returns_existing_immediately()
    {
        var map = new ObjectIdWaitMap<string>();
        Task<string> wait = map.WaitAsync(7, id => id == 7 ? "ready" : null, TimeSpan.FromSeconds(1));
        Assert.True(wait.IsCompletedSuccessfully);
        Assert.Equal("ready", await wait);
    }

    [Fact]
    public async Task WaitAsync_completes_on_Notify()
    {
        var map = new ObjectIdWaitMap<string>();
        Task<string> wait = map.WaitAsync(3, _ => null, TimeSpan.FromSeconds(2));
        Assert.False(wait.IsCompleted);
        map.Notify(3, "spawned");
        Assert.Equal("spawned", await wait);
    }

    [Fact]
    public async Task WaitAsync_times_out_with_null()
    {
        var map = new ObjectIdWaitMap<string>();
        string result = await map.WaitAsync(9, _ => null, TimeSpan.FromMilliseconds(30));
        Assert.Null(result);
    }

    [Fact]
    public async Task WaitAsync_honors_cancellation()
    {
        var map = new ObjectIdWaitMap<string>();
        using var cts = new CancellationTokenSource();
        Task<string> wait = map.WaitAsync(1, _ => null, TimeSpan.FromSeconds(5), cts.Token);
        cts.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => wait);
    }

    [Fact]
    public async Task CancelAll_completes_pending_with_null()
    {
        var map = new ObjectIdWaitMap<string>();
        Task<string> wait = map.WaitAsync(4, _ => null, TimeSpan.FromSeconds(5));
        map.CancelAll();
        Assert.Null(await wait);
    }
}
