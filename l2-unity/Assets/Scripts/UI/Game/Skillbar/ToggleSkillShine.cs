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
    const string FrameRoot = "Data/UI/ShortCut/skill_time/ToggleEffect";
    const int FrameCount = 13;
    const long FrameMs = 90;

    /// <summary>Resolved frames. <c>null</c> until the first load attempt, empty when the art is missing.</summary>
    static Texture2D[] _frames;
    static bool _triedLoad;

    /// <summary>Show/hide the shine for a slot that holds <paramref name="skillId"/>.</summary>
    public static void Apply(VisualElement slot, int skillId, int level, ref VisualElement shine,
        ref IVisualElementScheduledItem pulse)
    {
        if (slot == null)
            return;

        if (!IsRunningToggle(skillId, level))
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

        var list = new List<Texture2D>(FrameCount);
        for (int i = 1; i <= FrameCount; i++)
        {
            Texture2D tex = Resources.Load<Texture2D>(FrameRoot + i.ToString("000"));
            if (tex != null)
                list.Add(tex);
        }
        _frames = list.Count > 0 ? list.ToArray() : null;
        if (_frames == null)
            Debug.LogWarning("[ToggleShine] missing " + FrameRoot + "001..013");
    }
}
