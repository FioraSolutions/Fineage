using System;
using System.Collections.Generic;

/// <summary>
/// Live "which toggles are running" set for the HUD (Mobius abnormal list, self only).
/// <para>
/// The zone fans the owner's abnormal row out on every toggle on/off, so this is the client-side truth the
/// shortcut bar and the skill window use to draw the <c>l2_skilltime.ToggleEffect</c> shine over a toggle icon
/// — exactly the way the Interlude client marks an active toggle.
/// </para>
/// </summary>
public static class SkillToggleState
{
    static readonly HashSet<int> _active = new HashSet<int>();

    /// <summary>Raised after the set changed so skill UI can repaint its toggle marks.</summary>
    public static event Action Changed;

    public static bool IsActive(int skillId) => skillId > 0 && _active.Contains(skillId);

    public static IReadOnlyCollection<int> Active => _active;

    /// <summary>Full snapshot (zone sends the whole effect list on every abnormal update).</summary>
    public static void Replace(IList<EffectHolder> effects)
    {
        var next = new HashSet<int>();
        if (effects != null)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                EffectHolder holder = effects[i];
                if (holder == null)
                    continue;
                if (SkillToggleRules.IsToggle(holder.Id, holder.Value))
                    next.Add(holder.Id);
            }
        }

        if (next.SetEquals(_active))
            return;
        _active.Clear();
        foreach (int id in next)
            _active.Add(id);
        Changed?.Invoke();
    }

    /// <summary>Death clears the whole abnormal row, so no toggle can stay marked.</summary>
    public static void Clear()
    {
        if (_active.Count == 0)
            return;
        _active.Clear();
        Changed?.Invoke();
    }
}
