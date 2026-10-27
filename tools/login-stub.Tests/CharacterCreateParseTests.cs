using Fineage.LoginStub;
using Xunit;

namespace Fineage.LoginStub.Tests;

public sealed class CharacterCreateParseTests
{
    [Fact]
    public void Parses_human_fighter_create_packet()
    {
        byte[] nameUtf16 = System.Text.Encoding.Unicode.GetBytes("Alice");
        var packet = new PacketBuilder()
            .C(0x0B)
            .C((byte)nameUtf16.Length)
            .B(nameUtf16)
            .C(0)
            .D(0).D(0).D(0)
            .D(21).D(40).D(43).D(25).D(30).D(11)
            .D(2).D(3).D(4)
            .ToArray();

        Assert.True(InterludeGamePackets.TryParseCharacterCreate(packet, out var req));
        Assert.Equal("Alice", req.Name);
        Assert.Equal(0, req.Race);
        Assert.Equal(0, req.ClassId);
        Assert.Equal(2, req.HairStyle);
    }
}
