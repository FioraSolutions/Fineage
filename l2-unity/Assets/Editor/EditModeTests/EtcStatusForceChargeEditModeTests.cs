using NUnit.Framework;

public sealed class EtcStatusForceChargeEditModeTests
{
    [Test]
    public void Dto_defaultForceCharge_usesTraySkill4271()
    {
        var dto = new EtcStatusUpdateDto();
        Assert.AreEqual(4271, dto.ForceCharge[0]);
        Assert.AreEqual(0, dto.ForceCharge[1]);
    }

    [Test]
    public void Dto_ReadFrom_storesChargeLevel()
    {
        // Mobius field order: charges, weight, chat, danger, weapon grade, charm, death
        var payload = new byte[]
        {
            0x00, // opcode skipped
            0x03, 0x00, 0x00, 0x00, // charges = 3 (big-endian wire)
            0x00, 0x00, 0x00, 0x00, // weight
            0x00, 0x00, 0x00, 0x00, // chat
            0x00, 0x00, 0x00, 0x00, // danger
            0x00, 0x00, 0x00, 0x00, // weapon grade
            0x00, 0x00, 0x00, 0x00, // charm
            0x00, 0x00, 0x00, 0x00  // death
        };
        var reader = new PacketReader(payload, skipOpcode: true);
        var dto = new EtcStatusUpdateDto();
        dto.ReadFrom(reader);
        Assert.AreEqual(3, dto.ForceCharge[1]);
    }
}