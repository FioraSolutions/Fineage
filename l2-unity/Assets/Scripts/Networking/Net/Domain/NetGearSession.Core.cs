using System;
using System.Collections.Generic;
using Fineage.Net.Contract;
using UnityEngine;

public static partial class NetGearSession
{
    public static void Reset()
    {
        _hasUser = false;
        _user = default;
        _hasItems = false;
        _itemsApplied = false;
        _items = null;
        _updates.Clear();
        _warned = false;
        _lastBlock = null;
    }

    public static void NoteUserInfo(NetUserInfo user)
    {
        _user = user;
        _hasUser = true;
        int rhand = user.HasPaperdoll ? NetPaperdoll.RightHandItemId(user.Paperdoll) : 0;
        Debug.Log("[Net] UserInfo objectId=" + user.ObjectId
            + " class=" + user.ClassId
            + " rhand=" + rhand);

        PlayerInfoInterlude localInfo = ZoneCommands.LocalPlayerInfo;
        if (localInfo.Identity != null && localInfo.Identity.Id == user.ObjectId)
            OverlayUserInfo(localInfo, user);

        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player != null && player.Identity != null && player.Identity.Id == user.ObjectId)
            ApplyToLivePlayer(player, user);

        // StorageNpc is how the rest of the client (inventory, clan, status) looks up "me".
        RegisterSelfInStorage(user, localInfo, player);
        // ItemList may have arrived before UserInfo; apply now that StorageNpc has "me".
        FlushPending();
    }

    static void RegisterSelfInStorage(NetUserInfo user, PlayerInfoInterlude localInfo, PlayerEntity player)
    {
        PlayerInfoInterlude info;
        if (localInfo.Identity != null)
        {
            info = localInfo;
        }
        else
        {
            // Last resort: the identity is all the inventory window needs to key its rows.
            info = new PlayerInfoInterlude
            {
                Identity = new EntityIdentity
                {
                    Id = user.ObjectId,
                    Name = user.Name,
                    PlayerClass = user.ClassId
                }
            };
        }

        if (info.Identity == null)
            return;

        StorageNpc.getInstance().AddUserInfo(new UserInfoDto(info));
    }
}
