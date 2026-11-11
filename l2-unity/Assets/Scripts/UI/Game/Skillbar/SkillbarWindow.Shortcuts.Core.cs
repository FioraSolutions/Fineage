using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SkillbarWindow
{
    public void UpdateAllShortcuts()
    {
        UpdateAllShortcuts(Shortcuts.Shortcuts);
    }

    public void UpdateAllShortcuts(List<Shortcut> shortcuts)
    {
        if (_skillbars == null || !IsReady)
            return;

        _skillbars.ForEach((skillbar) => skillbar.ResetShortcuts());

        if (shortcuts == null)
            return;

        shortcuts.ForEach((shortcut) =>
        {
            UpdateBarShortcut(shortcut);
        });
    }

    private void UpdateBarShortcut(Shortcut shortcut)
    {
        if (shortcut.Slot < 12)
        {
            _skillbars.ForEach((skillbar) =>
            {
                if (skillbar.Page == shortcut.Page)
                {
                   // Debug.Log($"Add shortcut at page {shortcut.Page} in slot {shortcut.Slot}.");
                    skillbar.UpdateShortcut(shortcut, shortcut.Slot);
                }
            });
        }
        else
        {
            // Debug.LogError($"Slot value {shortcut.Slot} is too high.");
        }
    }

    public void AddShortcut(Shortcut shortcut)
    {
        if (!IsReady)
            return;
        UpdateBarShortcut(shortcut);
    }

    public void RefreshAutoShotMarks()
    {
        if (_skillbars == null)
            return;
        _skillbars.ForEach(skillbar => skillbar.RefreshSlotMarks());
    }
}
