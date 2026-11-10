using System;
using System.Collections.Generic;
using UnityEngine;

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
        if (item.ItemData is EtcItem etcItem)
            return NetEnchant.IsScrollType(etcItem.EtcItemgrp.EtcItemType.ToString());
        return false;
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

    private void SetupMessageWindow()
    {
        SystemMessagePacketServices.Window.OnButtonOk += OkUse;
        SystemMessagePacketServices.Window.OnButtonClosed += OnCancel;
        SystemMessagePacketServices.Window.ShowWindowDialogYesOrNot("Component registration is not reversible, do you want to continue?");
    }

    private bool TryGetItem(int objectId, out ItemInstance item)
    {
        if (!_playerInventory.TryGetValue(objectId, out item))
        {
            // Debug.Log("Item not found");
            return false;
        }
        return true;
    }

    private void OkUse()
    {
        SendUseItemPacket(_objectIdEtcType);
        CleanupMessageWindow();
    }

    private void OnCancel()
    {
        _objectIdEtcType = -1;
        CleanupMessageWindow();
    }

    private void CleanupMessageWindow()
    {
        SystemMessageUtils.CancelEvent(SystemMessagePacketServices.Window, OkUse, OnCancel);
    }

    public void DestroyItem(int objectId, int quantity)
    {

        //Cache Name for Message Equip
       
        if (_playerInventory.ContainsKey(objectId))
        {

            ItemInstance item = _playerInventory[objectId];
            //getInstance().AddS1Items(new VariableItem(item.ItemData.ItemName.Name, objectId));
            //AudioPacketServices.Audio.PlayEquipSound("trash_basket");
            IZoneTransport game = ZoneCommands.Transport;
            if (game == null)
                return;
            ZoneCommands.Send(new RequestDestroyItemCommand(objectId, quantity));
        }
        else
        {
            // Debug.Log("Destroy item not found delete " + objectId);
        }
    }

    /// <summary>
    /// Sends RequestDropItem. Prefer worldPos from inventory drag release (mouse→ground).
    /// Distance is validated by the server (CANNOT_DISCARD_DISTANCE_TOO_FAR).
    /// </summary>
    public void DropItem(int objectId, int quantity, Vector3? dropWorldPos = null)
    {
        if (!_playerInventory.ContainsKey(objectId))
        {
            // Debug.Log("Drop item not found " + objectId);
            return;
        }

        IZoneTransport game = ZoneCommands.Transport;
        if (game == null || DomainPacketServices.LocalPlayer == null)
            return;

        Transform player = DomainPacketServices.LocalPlayer.transform;
        Vector3 dropPos = dropWorldPos ?? player.position + player.forward * 0.6f;

        Vector3 l2 = VectorUtils.ConvertPosUnityToL2j(dropPos);
        // Debug.Log($"DropItem obj={objectId} qty={quantity} world={dropPos} l2=({(int)l2.x},{(int)l2.y},{(int)l2.z})");
        ZoneCommands.Send(new RequestDropItemCommand(
            objectId,
            quantity,
            (int)l2.x,
            (int)l2.y,
            (int)l2.z));
    }

    public bool IsContaineInventory(int objectId)
    {
        return _playerInventory.ContainsKey(objectId);
    }

}
