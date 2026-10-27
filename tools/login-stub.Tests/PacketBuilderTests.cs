using System.Buffers.Binary;
using Fineage.LoginStub;
using Xunit;

namespace Fineage.LoginStub.Tests;

public class PacketBuilderTests
{
    [Fact]
    public void C_AppendsByte()
    {
        var result = new PacketBuilder().C(0xAB).ToArray();
        Assert.Single(result);
        Assert.Equal(0xAB, result[0]);
    }

    [Fact]
    public void D_AppendsInt32LittleEndian()
    {
        var result = new PacketBuilder().D(0x11223344).ToArray();
        Assert.Equal(4, result.Length);
        Assert.Equal(0x11223344, BinaryPrimitives.ReadInt32LittleEndian(result));
    }

    [Fact]
    public void H_AppendsInt16LittleEndian()
    {
        var result = new PacketBuilder().H(0x1234).ToArray();
        Assert.Equal(2, result.Length);
        Assert.Equal(0x1234, BinaryPrimitives.ReadInt16LittleEndian(result));
    }

    [Fact]
    public void Q_AppendsInt64LittleEndian()
    {
        var result = new PacketBuilder().Q(0x0102030405060708).ToArray();
        Assert.Equal(8, result.Length);
        Assert.Equal(0x0102030405060708, BinaryPrimitives.ReadInt64LittleEndian(result));
    }

    [Fact]
    public void S_AppendsUtf16LE_NullTerminated()
    {
        var result = new PacketBuilder().S("Hi").ToArray();
        // "Hi" = 'H' (0x48 0x00) + 'i' (0x69 0x00) + null (0x00 0x00) = 6 bytes
        Assert.Equal(6, result.Length);
        Assert.Equal(0x48, result[0]);
        Assert.Equal(0x00, result[1]);
        Assert.Equal(0x69, result[2]);
        Assert.Equal(0x00, result[3]);
        Assert.Equal(0x00, result[4]);
        Assert.Equal(0x00, result[5]);
    }

    [Fact]
    public void S_Empty_AppendsOnlyNullTerminator()
    {
        var result = new PacketBuilder().S("").ToArray();
        Assert.Equal(2, result.Length);
        Assert.Equal(0x00, result[0]);
        Assert.Equal(0x00, result[1]);
    }

    [Fact]
    public void B_AppendsRawBytes()
    {
        byte[] data = [0x01, 0x02, 0x03];
        var result = new PacketBuilder().B(data).ToArray();
        Assert.Equal(data, result);
    }

    [Fact]
    public void Chaining_ProducesCorrectLayout()
    {
        var result = new PacketBuilder()
            .C(0x00)
            .D(42)
            .H(7)
            .ToArray();

        Assert.Equal(7, result.Length);
        Assert.Equal(0x00, result[0]);
        Assert.Equal(42, BinaryPrimitives.ReadInt32LittleEndian(result.AsSpan(1)));
        Assert.Equal(7, BinaryPrimitives.ReadInt16LittleEndian(result.AsSpan(5)));
    }
}
