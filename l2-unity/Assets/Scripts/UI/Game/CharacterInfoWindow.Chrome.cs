using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;

public partial class CharacterInfoWindow
{
    /// <summary>
    /// UITK <c>resource()</c> often fails for Interlude sprites — stamp via Resources.Load.
    /// </summary>
    void ApplyInterludeTextures()
    {
        Paint("Left-Frame-Element", "Data/UI/Frame/Interlude/FrameBackLeft");
        Paint("Center-Frame-Element", "Data/UI/Frame/Interlude/FrameBackMid");
        Paint("Right-Frame-Element", "Data/UI/Frame/Interlude/FrameBackRight");
        Paint("Minimize-Window-Button", "Data/UI/Frame/Interlude/FrameMiniBtn");
        Paint("Close-Window-Button", "Data/UI/Frame/Interlude/FrameCloseBtn");

        Paint("MyInfoBack", "Data/UI/Status/Interlude/Status_Interlude_MyInfo_Back");
        Paint("MainWndTabBack", "Data/UI/Status/Interlude/Status_Interlude_MainWndTabBack");

        for (int i = 1; i <= 5; i++)
        {
            string btnPath = i == 1
                ? "Data/UI/Status/Interlude/Status_Interlude_MainWndTabBtn1On"
                : $"Data/UI/Status/Interlude/Status_Interlude_MainWndTabBtn{i}";
            Paint($"MainWndTab{i}", btnPath);
            Paint($"MainWndTabIcon{i}", $"Data/UI/Status/Interlude/Status_Interlude_MainWndTabIcon{i}");
        }

        // myinfo_back troughs are blanked; paint empty tracks + fills (Interlude ps_*).
        Paint("HpBg", "Data/UI/Status/Interlude/Status_Interlude_Ps_HpBar_Back");
        Paint("MpBg", "Data/UI/Status/Interlude/Status_Interlude_Ps_MpBar_Back");
        Paint("CpBgB", "Data/UI/Status/Interlude/Status_Interlude_Ps_CpBar_Back");
        Paint("ExpBg", "Data/UI/Status/Interlude/Status_Interlude_Ps_ExpBar_Back");
        ClearBg("WeightBg"); // weight trough + bag stay on myinfo_back
        Paint("HpGauge", "Data/UI/Status/Interlude/Status_Interlude_Ps_HpBar");
        Paint("MpGauge", "Data/UI/Status/Interlude/Status_Interlude_Ps_MpBar");
        Paint("CpGaugeB", "Data/UI/Status/Interlude/Status_Interlude_Ps_CpBar");
        Paint("ExpGauge", "Data/UI/Status/Interlude/Status_Interlude_Ps_ExpBar");
        Paint("WeightGauge", "Data/UI/Status/Interlude/Status_Interlude_Ps_WeightBar1");
    }

    void ClearBg(string elementName)
    {
        VisualElement ele = _windowEle?.Q(elementName);
        if (ele != null)
            ele.style.backgroundImage = StyleKeyword.None;
    }

    void Paint(string elementName, string resourcesPath)
    {
        VisualElement ele = _windowEle?.Q(elementName);
        if (ele == null)
            return;
        L2SlotIconView.TrySetResourcesBackground(ele, resourcesPath);
        // Stretch the 8px Interlude strip across the trough; ScaleToFit letterboxes a gap on the right.
        ele.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
    }
}
