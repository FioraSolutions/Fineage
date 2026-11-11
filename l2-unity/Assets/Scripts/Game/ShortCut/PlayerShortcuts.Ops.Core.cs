using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using VContainer;

public partial class PlayerShortcuts : MonoBehaviour
{
    public void SetShortcutList(List<Shortcut> shortcuts)
    {
        if (_shortcuts == null)
        {
            _shortcuts = new Dictionary<int, Shortcut>();
        }
        else
        {
            _shortcuts.Clear();
        }

        for (int i = 0; i < shortcuts.Count; i++)
        {
            Shortcut shortcut = shortcuts[i];
            _shortcuts[shortcut.Slot + shortcut.Page * MAXIMUM_SHORTCUTS_PER_BAR] = shortcut;
        }

        // Always keep the dict; paint only when the skillbar slots exist (EnterWorld races BuildWindow).
        SkillbarWindow skillbar = SkillbarPacketServices.Skillbar;
        if (skillbar != null && skillbar.IsReady)
            skillbar.UpdateAllShortcuts(shortcuts);
    }

    public void RegisterShortcut(Shortcut shortcut)
    {
        if (_shortcuts == null)
        {
            _shortcuts = new Dictionary<int, Shortcut>();
        }

        int slot = shortcut.Slot + shortcut.Page * MAXIMUM_SHORTCUTS_PER_BAR;
        // Server ShortCutRegister is authoritative: overwrite a stale local row instead of
        // silently dropping the echo (TryAdd failed left soulshot / item icons blank).
        _shortcuts[slot] = shortcut;
        SkillbarWindow skillbar = SkillbarPacketServices.Skillbar;
        if (skillbar != null && skillbar.IsReady)
            skillbar.AddShortcut(shortcut);
    }

    public Shortcut GetShortcutBySlot(int slot)
    {
        if (_shortcuts.TryGetValue(slot, out Shortcut shortcut))
        {
            return shortcut;
        }

        return null;
    }

    public void RemoveShotcutLocally(int slot)
    {
        SkillbarWindow skillbar = SkillbarPacketServices.Skillbar;
        if (skillbar != null && skillbar.IsReady)
            skillbar.RemoveShortcut(slot);
        _shortcuts.Remove(slot);
    }
}
