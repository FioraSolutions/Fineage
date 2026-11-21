using Fineage.Net.Contract;

public sealed partial class NetZoneDispatch
{
    static bool HandleCursedWeapons(ZoneApplyPacket packet) => packet.Opcode switch
    {
        NetOpcode.CursedWeaponList => Apply(NetCursedWeaponInbound.ApplyList, (CursedWeaponListNetMsg)packet.Body),
        NetOpcode.CursedWeaponLocation => Apply(NetCursedWeaponInbound.ApplyLocation, (CursedWeaponLocationNetMsg)packet.Body),
        _ => false
    };
}