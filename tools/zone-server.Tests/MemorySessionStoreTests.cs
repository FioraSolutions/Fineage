using Fineage.ZoneServer.Net;
using Xunit;

namespace Fineage.ZoneServer.Tests;

public class MemorySessionStoreTests
{
    sealed class StubSession : ISession
    {
        public int SessionId { get; init; }
        public string Account { get; init; } = "";
        public bool Authenticated { get; init; }
        public bool InWorld { get; init; }
        public int EntityId { get; init; }
        public int X { get; init; }
        public int Y { get; init; }
        public int Z { get; init; }
        public ValueTask SendAsync<T>(ushort msgType, T payload, CancellationToken ct) => ValueTask.CompletedTask;
        public void Disconnect(string reason) { }
    }

    [Fact]
    public void TryAdd_NewSession_Succeeds()
    {
        var store = new MemorySessionStore();
        var session = new StubSession { SessionId = 1, Account = "alice" };

        Assert.True(store.TryAdd(session));
        Assert.Equal(1, store.Count);
    }

    [Fact]
    public void TryAdd_DuplicateId_Fails()
    {
        var store = new MemorySessionStore();
        var s1 = new StubSession { SessionId = 1, Account = "alice" };
        var s2 = new StubSession { SessionId = 1, Account = "bob" };

        Assert.True(store.TryAdd(s1));
        Assert.False(store.TryAdd(s2));
        Assert.Equal(1, store.Count);
    }

    [Fact]
    public void Get_ReturnsCorrectSession()
    {
        var store = new MemorySessionStore();
        var session = new StubSession { SessionId = 5, Account = "charlie" };
        store.TryAdd(session);

        var found = store.Get(5);
        Assert.NotNull(found);
        Assert.Equal("charlie", found!.Account);
    }

    [Fact]
    public void Get_UnknownId_ReturnsNull()
    {
        var store = new MemorySessionStore();
        Assert.Null(store.Get(999));
    }

    [Fact]
    public void GetByAccount_CaseInsensitive()
    {
        var store = new MemorySessionStore();
        var session = new StubSession { SessionId = 1, Account = "Alice" };
        store.TryAdd(session);

        Assert.NotNull(store.GetByAccount("alice"));
        Assert.NotNull(store.GetByAccount("ALICE"));
        Assert.NotNull(store.GetByAccount("Alice"));
    }

    [Fact]
    public void TryRemove_RemovesSession()
    {
        var store = new MemorySessionStore();
        var session = new StubSession { SessionId = 1, Account = "alice" };
        store.TryAdd(session);

        Assert.True(store.TryRemove(1));
        Assert.Equal(0, store.Count);
        Assert.Null(store.Get(1));
        Assert.Null(store.GetByAccount("alice"));
    }

    [Fact]
    public void TryRemove_UnknownId_ReturnsFalse()
    {
        var store = new MemorySessionStore();
        Assert.False(store.TryRemove(999));
    }

    [Fact]
    public void All_ReturnsSnapshot()
    {
        var store = new MemorySessionStore();
        store.TryAdd(new StubSession { SessionId = 1, Account = "a" });
        store.TryAdd(new StubSession { SessionId = 2, Account = "b" });
        store.TryAdd(new StubSession { SessionId = 3, Account = "c" });

        var all = store.All;
        Assert.Equal(3, all.Count);
    }

    [Fact]
    public void DuplicateAccount_OverwritesLookup()
    {
        var store = new MemorySessionStore();
        var s1 = new StubSession { SessionId = 1, Account = "alice" };
        var s2 = new StubSession { SessionId = 2, Account = "alice" };

        store.TryAdd(s1);
        store.TryAdd(s2);

        var found = store.GetByAccount("alice");
        Assert.NotNull(found);
        Assert.Equal(2, found!.SessionId);
    }
}
