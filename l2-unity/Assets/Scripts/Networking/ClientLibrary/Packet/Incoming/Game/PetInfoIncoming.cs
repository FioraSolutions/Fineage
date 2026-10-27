[IncomingGamePacket(GameServerPacketType.PetInfo)]
public sealed class PetInfoIncoming : IncomingPacket<NpcInfoDto>
{
    public override NpcInfoDto Read(PacketReader reader)
    {
        NpcInfoDto dto = new NpcInfoDto();
        dto.ReadPet(reader);
        return dto;
    }

    public override void Apply(NpcInfoDto packet) =>
        NpcIncomingPacketServices.ApplyPetInfo(packet);
}
