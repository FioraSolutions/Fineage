using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using VContainer;

public partial class PlayerInventory
{
    public bool UseItem(int objectId , bool isShortCut = false)
    {
        if (!TryGetItem(objectId, out ItemInstance item))
        {
            return false;
        }

        if (IsRecipeItem(item))
        {
            UseRecipeItem(_objectIdEtcType , objectId, isShortCut);
        }
        else if (IsEnchantScroll(item))
        {
            // Mobius RequestEnchantItem: the scroll selects itself; the window then asks for a target.
            EnchantWindow window = ItemEnchantPacketServices.Enchant;
            if (window != null)
                window.BeginZoneSelect(item);
            RequestEnchantItem(objectId);
        }
        else
        {
            SendUseItemPacket(objectId);
        }

        return true;
    }

    void RequestEnchantItem(int objectId)
    {
        ZoneCommands.Send(new RequestEnchantItemCommand(objectId));
    }

    static bool IsEnchantScroll(ItemInstance item)
    {
        if (item == null)
            return false;
        EtcItem etcItem = item.ItemData as EtcItem;
        if (etcItem == null)
            return false;
        string etcType = etcItem.EtcItemgrp != null ? etcItem.EtcItemgrp.EtcItemType : null;
        return NetEnchant.IsScrollType(etcType);
    }

    private void UseRecipeItem(int objectIdEtcType , int objectId, bool isShortCut)
    {
        if (isShortCut)
        {
            _objectIdEtcType = objectId;
            SendUseItemPacket(objectId);
        }
        else
        {
            HandleRecipeItem(objectId);
        }
    }

    private bool IsRecipeItem(ItemInstance item)
    {
        if (item.ItemData is EtcItem etcItem)
        {
            return etcItem.EtcItemgrp.EtcItemType == TYPE_RECIPE;
        }
        return false;
    }

    private void HandleRecipeItem(int objectId)
    {
        _objectIdEtcType = objectId;
        SetupMessageWindow();
    }

    private void SendUseItemPacket(int objectId)
    {
        IZoneTransport game = ZoneCommands.Transport;
        if (game == null)
            return;
        ZoneCommands.Send(new UseItemCommand(objectId, 0));
    }
}
