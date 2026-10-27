using Fineage.ZoneServer.Net;
using Xunit;

namespace Fineage.ZoneServer.Tests;

public class MemoryCacheTests
{
    sealed record TestValue(string Name);

    [Fact]
    public async Task GetAsync_MissingKey_ReturnsNull()
    {
        var cache = new MemoryCache();
        var result = await cache.GetAsync<TestValue>("missing");
        Assert.Null(result);
    }

    [Fact]
    public async Task SetAndGet_ReturnsValue()
    {
        var cache = new MemoryCache();
        var val = new TestValue("hello");

        await cache.SetAsync("key1", val);
        var result = await cache.GetAsync<TestValue>("key1");

        Assert.NotNull(result);
        Assert.Equal("hello", result!.Name);
    }

    [Fact]
    public async Task RemoveAsync_DeletesEntry()
    {
        var cache = new MemoryCache();
        await cache.SetAsync("key1", new TestValue("val"));

        await cache.RemoveAsync("key1");
        var result = await cache.GetAsync<TestValue>("key1");

        Assert.Null(result);
    }

    [Fact]
    public async Task SetAsync_WithTtl_ExpiresImmediately_WhenTtlIsZero()
    {
        var cache = new MemoryCache();
        await cache.SetAsync("key1", new TestValue("val"), ttl: TimeSpan.Zero);

        await Task.Delay(1);
        var result = await cache.GetAsync<TestValue>("key1");
        Assert.Null(result);
    }

    [Fact]
    public async Task SetAsync_WithLongTtl_DoesNotExpire()
    {
        var cache = new MemoryCache();
        await cache.SetAsync("key1", new TestValue("val"), ttl: TimeSpan.FromHours(1));

        var result = await cache.GetAsync<TestValue>("key1");
        Assert.NotNull(result);
        Assert.Equal("val", result!.Name);
    }

    [Fact]
    public async Task SetAsync_OverwritesPreviousValue()
    {
        var cache = new MemoryCache();
        await cache.SetAsync("key1", new TestValue("first"));
        await cache.SetAsync("key1", new TestValue("second"));

        var result = await cache.GetAsync<TestValue>("key1");
        Assert.NotNull(result);
        Assert.Equal("second", result!.Name);
    }

    [Fact]
    public async Task Count_ReflectsEntries()
    {
        var cache = new MemoryCache();
        Assert.Equal(0, cache.Count);

        await cache.SetAsync("a", new TestValue("1"));
        await cache.SetAsync("b", new TestValue("2"));
        Assert.Equal(2, cache.Count);

        await cache.RemoveAsync("a");
        Assert.Equal(1, cache.Count);
    }
}
