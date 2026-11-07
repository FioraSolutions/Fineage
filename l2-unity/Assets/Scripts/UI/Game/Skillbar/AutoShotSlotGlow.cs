using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Interlude ExAutoSoulShot armed shortcut: a flowing shimmer wave across the icon
/// (not an opacity blink). Uses the ItemEnchant Loading frame strip already in Resources —
/// same sweeping highlight the original client shows on auto-use shortcuts.
/// </summary>
public static class AutoShotSlotGlow
{
    const string FrameRoot = "Data/UI/Enchant/loading_anim/ItemEnchant_DF_Effect_Loading_";
    const int FrameCount = 40;
    const long FrameMs = 40;

    static Texture2D[] _frames;
    static bool _triedLoad;

    public static void EnsureOverlay(VisualElement slot, ref VisualElement glow)
    {
        if (slot == null)
            return;
        if (glow == null)
        {
            glow = new VisualElement
            {
                pickingMode = PickingMode.Ignore,
                name = "AutoShotGlow"
            };
            glow.AddToClassList("auto-shot-glow");
            slot.Add(glow);
        }
        glow.style.display = DisplayStyle.Flex;
        glow.style.opacity = 0.85f;
    }

    public static IVisualElementScheduledItem StartWave(VisualElement glow)
    {
        if (glow == null)
            return null;

        EnsureFrames();
        if (_frames == null || _frames.Length == 0)
            return null;

        int index = 0;
        glow.style.backgroundImage = new StyleBackground(_frames[0]);
        return glow.schedule.Execute(() =>
        {
            if (glow.style.display == DisplayStyle.None)
                return;
            index = (index + 1) % _frames.Length;
            Texture2D frame = _frames[index];
            if (frame != null)
                glow.style.backgroundImage = new StyleBackground(frame);
        }).Every(FrameMs);
    }

    static void EnsureFrames()
    {
        if (_triedLoad)
            return;
        _triedLoad = true;

        var list = new List<Texture2D>(FrameCount);
        for (int i = 1; i <= FrameCount; i++)
        {
            string path = FrameRoot + i.ToString("00");
            Texture2D tex = Resources.Load<Texture2D>(path);
            if (tex != null)
                list.Add(tex);
        }

        _frames = list.Count > 0 ? list.ToArray() : null;
    }
}
