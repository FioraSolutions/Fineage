using System.Collections.Generic;
using UnityEngine;

/// <summary>Shared catalog ↔ cart transfer rules for <see cref="DealerWindow"/> / <see cref="TradeItemMover"/>.</summary>
public static class ShopPanelTransfer
{
    public static ShopTransferMode FromModifySellFlag(bool isModifySell) =>
        isModifySell ? ShopTransferMode.PlayerBagCommit : ShopTransferMode.NpcMerchantBuy;

    public static Product CreateCartLine(ShopTransferMode mode, Product source, int quantity)
    {
        if (source == null)
        {
            Debug.LogError("[Shop] CreateCartLine source=null list mode=" + mode);
            return null;
        }

        if (source.ItemId <= 0)
        {
            Debug.LogError("[Shop] CreateCartLine itemId=" + source.ItemId + " mode=" + mode);
            return null;
        }

        if (quantity <= 0)
        {
            Debug.LogError("[Shop] CreateCartLine itemId=" + source.ItemId + " quantity=" + quantity + " mode=" + mode);
            return null;
        }

        if (mode == ShopTransferMode.NpcMerchantBuy)
        {
            Product line = source.Clone();
            line.SetCount(quantity);
            return line;
        }

        return source;
    }

    public static void ApplyCatalogQuantityTaken(ShopTransferMode mode, Product catalogRow, int quantityTaken)
    {
        if (mode != ShopTransferMode.PlayerBagCommit || catalogRow == null)
            return;
        if (quantityTaken <= 0)
        {
            Debug.LogError("[Shop] ApplyCatalogQuantityTaken itemId=" + (catalogRow?.ItemId ?? 0) + " qty=" + quantityTaken);
            return;
        }

        catalogRow.SetCount(catalogRow.Count - quantityTaken);
    }

    public static bool ReturnsToCatalog(ShopTransferMode mode) =>
        mode == ShopTransferMode.PlayerBagCommit;
}