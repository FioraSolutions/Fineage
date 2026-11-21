using Fineage.Net.Contract;
using UnityEngine;

public static class NetAugmentInbound
{
    const string MakeWindow = "augmentation/variation-make";
    const string CancelWindow = "augmentation/variation-cancel";

    static void MissingUi(string opcodeName, string window)
    {
        Debug.LogError($"[Net] {opcodeName}: no client {window} window wired (ItemAugmentPacketServices)");
    }

    public static void ShowMakeWindow()
    {
        var ui = ItemAugmentPacketServices.Make;
        if (ui == null)
        {
            MissingUi(nameof(NetOpcode.AugmentShowMakeWindow), MakeWindow);
            return;
        }

        ui.OpenFromZone();
    }

    public static void ShowCancelWindow()
    {
        var ui = ItemAugmentPacketServices.Cancel;
        if (ui == null)
        {
            MissingUi(nameof(NetOpcode.AugmentShowCancelWindow), CancelWindow);
            return;
        }

        ui.OpenFromZone();
    }

    public static void PutItemResult(VariationPutItemNetMsg msg)
    {
        var ui = ItemAugmentPacketServices.Make;
        if (ui == null)
        {
            MissingUi(nameof(NetOpcode.AugmentPutItemResult), MakeWindow);
            return;
        }

        ui.OnPutTarget(msg);
    }

    public static void PutRefinerResult(VariationPutRefinerNetMsg msg)
    {
        var ui = ItemAugmentPacketServices.Make;
        if (ui == null)
        {
            MissingUi(nameof(NetOpcode.AugmentPutRefinerResult), MakeWindow);
            return;
        }

        ui.OnPutRefiner(msg);
    }

    public static void PutGemResult(VariationPutGemNetMsg msg)
    {
        var ui = ItemAugmentPacketServices.Make;
        if (ui == null)
        {
            MissingUi(nameof(NetOpcode.AugmentPutGemResult), MakeWindow);
            return;
        }

        ui.OnPutGemstone(msg);
    }

    public static void VariationResult(VariationResultNetMsg msg)
    {
        var ui = ItemAugmentPacketServices.Make;
        if (ui == null)
        {
            MissingUi(nameof(NetOpcode.AugmentVariationResult), MakeWindow);
            return;
        }

        ui.OnVariationResult(msg);
    }

    public static void PutCancelItemResult(VariationCancelItemNetMsg msg)
    {
        var ui = ItemAugmentPacketServices.Cancel;
        if (ui == null)
        {
            MissingUi(nameof(NetOpcode.AugmentPutCancelItemResult), CancelWindow);
            return;
        }

        ui.OnPutTarget(msg);
    }

    public static void CancelResult(VariationCancelResultNetMsg msg)
    {
        var ui = ItemAugmentPacketServices.Cancel;
        if (ui == null)
        {
            MissingUi(nameof(NetOpcode.AugmentCancelResult), CancelWindow);
            return;
        }

        ui.OnCancelResult(msg);
    }
}