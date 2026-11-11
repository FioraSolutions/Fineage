using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class InventoryWindow
{
    void ApplyInterludeInventoryTextures()
    {
        // Full Inventory_Back plate (tab cutouts + bag frame). Gear is hitboxes only.
        SetResourcesBackground("Container", "Data/UI/Inventory/Inventory_Interlude_Back");
        VisualElement gear = GetElementById("Gear");
        if (gear != null)
            gear.style.backgroundImage = StyleKeyword.None;

        // Header FrameBack + sys buttons — same load path (resource() is unreliable).
        PaintNamedBackground("Left-Frame-Element", "Data/UI/Frame/Interlude/FrameBackLeft");
        PaintNamedBackground("Center-Frame-Element", "Data/UI/Frame/Interlude/FrameBackMid");
        PaintNamedBackground("Right-Frame-Element", "Data/UI/Frame/Interlude/FrameBackRight");
        // System menu (⋮) — not chatting_option wrench.
        PaintNamedBackground("Options-Window-Button", "Data/UI/Frame/Interlude/FrameOptionBtn");
        PaintNamedBackground("Minimize-Window-Button", "Data/UI/Frame/Interlude/FrameMiniBtn");
        PaintNamedBackground("Close-Window-Button", "Data/UI/Frame/Interlude/FrameCloseBtn");

        VisualElement trashBg = _windowEle?.Q<VisualElement>("TrashBtn")?.Q<VisualElement>("ButtonBg");
        if (trashBg != null)
            L2SlotIconView.TrySetResourcesBackground(
                trashBg,
                "Data/UI/Inventory/Inventory_Interlude_Trash");

        // UITK resource() often fails for these strips — stamp from Resources.Load.
        PaintWeightFillParts();
    }

    void PaintWeightFillParts()
    {
        VisualElement gauge = _weiBarBg ?? GetElementById("WeightGauge");
        if (gauge == null)
            return;
        L2SlotIconView.TrySetResourcesBackground(
            gauge.Q("Left"), "Data/UI/Inventory/Inventory_Interlude_Weight1");
        L2SlotIconView.TrySetResourcesBackground(
            gauge.Q("Center"), "Data/UI/Inventory/Inventory_Interlude_Weight2");
        L2SlotIconView.TrySetResourcesBackground(
            gauge.Q("Right"), "Data/UI/Inventory/Inventory_Interlude_Weight3");
    }

    void SetResourcesBackground(string elementId, string resourcesPath)
    {
        VisualElement ele = GetElementById(elementId);
        if (ele == null)
            return;
        L2SlotIconView.TrySetResourcesBackground(ele, resourcesPath);
    }

    void PaintNamedBackground(string elementName, string resourcesPath)
    {
        VisualElement ele = _windowEle?.Q(elementName);
        if (ele == null)
            return;
        L2SlotIconView.TrySetResourcesBackground(ele, resourcesPath);
    }
}
