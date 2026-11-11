using System;
using System.Collections.Generic;
using Fineage.Net.Contract;
using UnityEngine;

public static partial class NetGearSession
{
    static void ApplyToLivePlayer(PlayerEntity player, NetUserInfo user)
    {
        PlayerAppearance appearance = player.Appearance as PlayerAppearance;
        PlayerStats stats = player.Stats as PlayerStats;
        PlayerStatus status = player.Status as PlayerStatus;
        if (player.Identity == null || appearance == null || stats == null)
            return;

        var info = new PlayerInfoInterlude
        {
            Identity = player.Identity,
            Appearance = appearance,
            Stats = stats,
            Status = status
        };
        OverlayUserInfo(info, user);
        if (player.Identity != null)
            L2OverheadAnchor.Invalidate(player.Identity.Id);
        player.Running = appearance.Running;
        if (stats.RunRealSpeed > 0.001f)
            player.UpdateRunSpeed(stats.RunRealSpeed);
        if (stats.WalkRealSpeed > 0.001f)
            player.UpdateWalkSpeed(stats.WalkRealSpeed);
        // Overlay writes PAtkSpd/MAtkSpd onto Stats; drive anim converters like spawn/CharInfo do.
        if (stats.PAtkSpd > 0)
            player.UpdatePAtkSpeed(stats.PAtkSpd);
        if (stats.MAtkSpd > 0)
            player.UpdateMAtkSpeed(stats.MAtkSpd);
        player.RefreshVisuals();
        CharacterInfoWindow characterInfo = HudCorePacketServices.CharacterInfo;
        if (characterInfo != null)
            characterInfo.UpdateValues();
        InventoryWindow inventory = InventoryPacketServices.Window;
        if (inventory != null)
            inventory.UpdateStats(stats);
    }

    static bool CanApplyInventory()
    {
        if (InventoryPacketServices.Inventory == null)
        {
            WarnBlocked("PlayerInventory");
            return false;
        }
        // The inventory window used to be a precondition here. That made the snapshot wait for
        // a window which itself stays empty until the snapshot arrives -- nobody could ever
        // open the bag. PlayerInventory now caches the rows and pushes them into the window
        // once it exists, so the data path no longer depends on the UI being built.
        if (!DispatcherPacketServices.Exists || GamePacketServices.Bus == null)
        {
            WarnBlocked("dispatcher/EventBus");
            return false;
        }
        if (TablePacketServices.Items.Weapons.Count == 0)
        {
            WarnBlocked("ItemTable.Weapons");
            return false;
        }
        UserInfoDto user = StorageNpc.getInstance().GetFirstUser();
        if (user == null)
        {
            WarnBlocked("StorageNpc user");
            return false;
        }
        if (user.PlayerInfoInterlude.Identity == null)
        {
            WarnBlocked("user identity");
            return false;
        }
        return true;
    }
}
