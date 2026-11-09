using NUnit.Framework;

/// <summary>
/// Minimal EditMode smoke so CI can batchmode-compile Scripts via Test Runner (#737).
/// </summary>
public sealed class WireSmokeEditModeTests
{
    [Test]
    public void PacketReader_ReadsOpcodeByte()
    {
        var reader = new PacketReader(new byte[] { 0x01, 0x00, 0x00, 0x00, 0x02 }, skipOpcode: true);
        Assert.AreEqual(2, reader.ReadI());
    }

    [Test]
    public void NetworkingAssembly_IsLoaded()
    {
        Assert.IsNotNull(typeof(PacketReader));
        Assert.IsNotNull(typeof(IWireDto));
    }
}
