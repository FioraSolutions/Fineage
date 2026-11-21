using Fineage.Net.Contract;
using UnityEngine;

public static class NetCursedWeaponInbound
{
    public static void ApplyList(CursedWeaponListNetMsg list)
    {
        Debug.LogError(
            $"[Net] {nameof(NetOpcode.CursedWeaponList)}: cursed-weapon list UI not wired "
            + $"(item ids={list.ItemIds?.Length ?? 0})");
    }

    public static void ApplyLocation(CursedWeaponLocationNetMsg location)
    {
        Debug.LogError(
            $"[Net] {nameof(NetOpcode.CursedWeaponLocation)}: cursed-weapon map pins UI not wired "
            + $"(rows={location.Rows?.Length ?? 0})");
    }
}