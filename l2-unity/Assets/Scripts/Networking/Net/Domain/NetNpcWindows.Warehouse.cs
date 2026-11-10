using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class NetNpcWindows
{
    public static void ApplyWarehouseDepositList(NetWarehouseDepositList list) => ShowDeposit(list);

    public static void ApplyWarehouseWithdrawList(NetWarehouseWithdrawList list) => ShowWithdraw(list);

    static void ShowDeposit(NetWarehouseDepositList list)
    {
        NetNpcSession.OpenShop(list.NpcObjectId, 0);
        string title = "Private Warehouse (" + list.FeePerLine + " adena)";
        OpenDealer(title, "Inventory", "Deposit", ProductType.WHDepositList, BagProducts(list.Items), true, 0, list.Adena);
    }

    static void ShowWithdraw(NetWarehouseWithdrawList list)
    {
        NetNpcSession.OpenShop(list.NpcObjectId, 0);
        _balance.Weight = list.CurrentWeight;
        _balance.MaxWeight = list.MaxWeight;
        _hasBalance = true;
        OpenDealer("Private Warehouse", "Warehouse", "Withdraw", ProductType.WHWithdrawList, BagProducts(list.Items), true, 0, _hasBalance ? _balance.Adena : 0);
    }

}
