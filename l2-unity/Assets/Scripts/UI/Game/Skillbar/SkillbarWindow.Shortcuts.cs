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

    /// <summary>
    /// Repaints every shot slot after the zone confirms an auto-shot toggle (or clears one on a
    /// weapon swap / empty stack). Called from the <c>AutoSoulShot</c> apply path.
    /// </summary>
    public void RefreshAutoShotMarks()
    {
        if (_skillbars == null)
            return;
        _skillbars.ForEach(skillbar => skillbar.RefreshSlotMarks());
    }

    public void RemoveShortcut(int oldSlot)
    {
        int slot = oldSlot % PlayerShortcuts.MAXIMUM_SHORTCUTS_PER_BAR;
        int page = oldSlot / PlayerShortcuts.MAXIMUM_SHORTCUTS_PER_BAR;

        _skillbars.ForEach((skillbar) =>
        {
            if (skillbar.Page == page)
            {
                skillbar.DeleteShortcut(oldSlot);
            }
        });
    }

  
    public Shortcut GetShortcutByPosition(int skillbarListPosition)
    {
        if(skillbarListPosition > PlayerShortcuts.MAXIMUM_SHORTCUTS_PER_BAR * PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT)
        {
            return null;
        }

        int slotPosition = skillbarListPosition % PlayerShortcuts.MAXIMUM_SHORTCUTS_PER_BAR;
        int pagePosition = skillbarListPosition / PlayerShortcuts.MAXIMUM_SHORTCUTS_PER_BAR;

        AbstractSkillbar skillbar = _skillbars[pagePosition];
        //backup
        //foreach (var slot in skillbar.BarSlots)
        //{
        //  if (slot.Position == slotPosition)
        // {
        //     return slot.Shortcut;
        // }
        //}

       
        return GetSlotByPosition(skillbar, slotPosition)?.Shortcut;

        //Debug.LogError($"No skill found at position {skillbarListPosition}");
        //return null;
    }

    private SkillbarSlot GetSlotBySkillId(List<AbstractSkillbar> skillbars, int skillId, int type)
    {
        foreach (var skillbar in skillbars)
        {
            foreach (var slot in skillbar.BarSlots)
            {
                if (slot.Shortcut?.Type == type && slot.Shortcut?.Id == skillId)
                {
                    return slot;
                }
            }
        }
        return null;
    }

    //backup
    //private SkillbarSlot GetSlotBySkillId(List<AbstractSkillbar> skillbars , int skilId , int type)
    //{
    //for(int i =0; i < skillbars.Count; i++)
    //{
    // AbstractSkillbar skillBar = skillbars[0];

    //for (int b = 0; b < skillBar.BarSlots.Count; b++)
    //{
    // SkillbarSlot slot = skillBar.BarSlots[b];

    //if(slot.Shortcut != null)
    // {
    // if (slot.Shortcut.Type == type && slot.Shortcut.Id == skilId)
    //{
    //   return slot;
    //}
    //}

    //}
    // }

    //return null;
    // }

    private SkillbarSlot GetSlotByPosition(AbstractSkillbar skillbar, int slotPosition)
    {
        foreach (var slot in skillbar.BarSlots)
        {
            if (slot.Position == slotPosition)
            {
                return slot;
            }
        }
        return null;
    }

    public void ShowCooldown(int skillId , int shortcutType , int reuseDelay)
    {
        SkillbarSlot skillbarSlot = GetSlotBySkillId(_skillbars, skillId, shortcutType);

        if(skillbarSlot != null)
        {
            StartCoroutine(_cooldownAnimationService.CooldownCoroutine(skillbarSlot.GetReuseElement(), skillbarSlot.GetRechargeMaskElement(), reuseDelay));
        }


    }
}
