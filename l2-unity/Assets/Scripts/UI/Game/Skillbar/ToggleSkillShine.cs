using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Interlude toggle marker: the <c>l2_skilltime.ToggleEffect</c> shine drawn over a running toggle's icon.
/// <para>
/// The original client paints this animated frame on a shortcut slot and on the skill-window slot while the
/// toggle's abnormal is live (Fake Death, Silent Mode, Vicious Stance, Focus Skill Mastery, …). It is purely a
/// marker — the icon stays underneath and the slot still takes clicks.
/// </para>
/// </summary>
public static partial class ToggleSkillShine
{
    const long FrameMs = ToggleEffectFrames.FrameMs;

    /// <summary>Resolved frames. <c>null</c> until the first load attempt, empty when the art is missing.</summary>
    static Texture2D[] _frames;
    static bool _triedLoad;

    /// <summary>Show/hide the shine for a slot that holds <paramref name="skillId"/>.</summary>
    public static void Apply(VisualElement slot, int skillId, int level, ref VisualElement shine,
        ref IVisualElementScheduledItem pulse)
    {
        Apply(slot, IsRunningToggle(skillId, level), ref shine, ref pulse);
    }

    /// <summary>
    /// Same ToggleEffect shine used for a running toggle, also used on a shortcut-bar shot
    /// whose auto-use the zone has confirmed via <c>AutoSoulShot</c>. Inventory slots must not call this.
    /// </summary>
    public static void Apply(VisualElement slot, bool show, ref VisualElement shine,
        ref IVisualElementScheduledItem pulse)
    {
        if (slot == null)
            return;

        if (!show)
        {
            Hide(slot, ref shine, ref pulse);
            return;
        }

        EnsureFrames();
        if (_frames == null || _frames.Length == 0)
            return;

        if (shine == null)
        {
            shine = new VisualElement { pickingMode = PickingMode.Ignore, name = "ToggleShine" };
            shine.AddToClassList("toggle-shine");
            slot.Add(shine);
        }

        shine.style.display = DisplayStyle.Flex;
        if (pulse != null)
            return;

        int index = 0;
        VisualElement target = shine;
        target.style.backgroundImage = new StyleBackground(_frames[0]);
        pulse = target.schedule.Execute(() =>
        {
            if (target.style.display == DisplayStyle.None)
                return;
            index = (index + 1) % _frames.Length;
            Texture2D frame = _frames[index];
            if (frame != null)
                target.style.backgroundImage = new StyleBackground(frame);
        }).Every(FrameMs);
    }

    public static void Hide(VisualElement slot, ref VisualElement shine, ref IVisualElementScheduledItem pulse)
    {
        if (pulse != null)
        {
            pulse.Pause();
            pulse = null;
        }
        if (shine != null)
            shine.style.display = DisplayStyle.None;
        slot?.RemoveFromClassList("toggle-on");
    }

    static bool IsRunningToggle(int skillId, int level)
    {
        if (skillId <= 0)
            return false;
        if (!SkillToggleRules.IsToggle(skillId, level))
            return false;
        return SkillToggleState.IsActive(skillId);
    }

    static void EnsureFrames()
    {
        if (_triedLoad)
            return;
        _triedLoad = true;

        var list = new List<Texture2D>(ToggleEffectFrames.Count);
        for (int i = 1; i <= ToggleEffectFrames.Count; i++)
        {
            Texture2D tex = Resources.Load<Texture2D>(ToggleEffectFrames.ResourcePath(i));
            if (tex != null)
                list.Add(tex);
        }
        _frames = list.Count > 0 ? list.ToArray() : null;
        if (_frames == null)
            Debug.LogWarning("[ToggleShine] missing " + ToggleEffectFrames.ResourcePath(1) + ".." + ToggleEffectFrames.ResourcePath(ToggleEffectFrames.Count));
    }
}
