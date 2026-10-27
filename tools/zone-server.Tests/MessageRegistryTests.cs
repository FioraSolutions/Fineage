using Fineage.ZoneServer.Net;
using Fineage.ZoneServer.Wire;
using Xunit;

namespace Fineage.ZoneServer.Tests;

public class MessageRegistryTests
{
    [Fact]
    public void CreateDefault_RegistersAllMsgTypes()
    {
        var reg = MessageRegistry.CreateDefault();

        Assert.True(reg.IsKnown((ushort)MsgType.Hello));
        Assert.True(reg.IsKnown((ushort)MsgType.Auth));
        Assert.True(reg.IsKnown((ushort)MsgType.EnterWorld));
        Assert.True(reg.IsKnown((ushort)MsgType.MoveTo));
        Assert.True(reg.IsKnown((ushort)MsgType.HelloAck));
        Assert.True(reg.IsKnown((ushort)MsgType.AuthOk));
        Assert.True(reg.IsKnown((ushort)MsgType.PlayerSpawned));
        Assert.True(reg.IsKnown((ushort)MsgType.MoveToAck));
        Assert.True(reg.IsKnown((ushort)MsgType.Error));
    }

    [Fact]
    public void Resolve_KnownType_ReturnsClrType()
    {
        var reg = MessageRegistry.CreateDefault();
        var type = reg.Resolve((ushort)MsgType.Hello, protocolVersion: 1);

        Assert.NotNull(type);
        Assert.Equal(typeof(HelloMsg), type);
    }

    [Fact]
    public void Resolve_UnknownType_ReturnsNull()
    {
        var reg = MessageRegistry.CreateDefault();
        var type = reg.Resolve(9999, protocolVersion: 1);

        Assert.Null(type);
    }

    [Fact]
    public void Resolve_OldProtocolVersion_ReturnsNull()
    {
        var reg = new MessageRegistry();
        reg.Register<HelloMsg>(1, minProtocolVersion: 5);

        Assert.Null(reg.Resolve(1, protocolVersion: 4));
        Assert.NotNull(reg.Resolve(1, protocolVersion: 5));
        Assert.NotNull(reg.Resolve(1, protocolVersion: 6));
    }

    [Fact]
    public void IsKnown_UnregisteredType_ReturnsFalse()
    {
        var reg = new MessageRegistry();
        Assert.False(reg.IsKnown(12345));
    }

    [Fact]
    public void Register_Overwrites_PreviousMapping()
    {
        var reg = new MessageRegistry();
        reg.Register<HelloMsg>(1);
        reg.Register<AuthMsg>(1);

        var type = reg.Resolve(1, protocolVersion: 1);
        Assert.Equal(typeof(AuthMsg), type);
    }
}
