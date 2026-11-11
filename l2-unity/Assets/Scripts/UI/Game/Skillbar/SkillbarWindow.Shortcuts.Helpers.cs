using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SkillbarWindow
{
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
