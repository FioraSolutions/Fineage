using System;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

public partial class PlayerShortcuts : MonoBehaviour
{
    private void Update()
    {
        if (_shortcuts == null)
        {
            return;
        }

        VerifySkillbarInputs();
    }

    private void VerifySkillbarInputs()
    {
        foreach (Shortcut shortcut in _shortcuts.Values)
        {
            for (int i = 0; i < _pageMap.Length; i++)
            {
                if (_pageMap[i] == shortcut.Page)
                {
                    InputManager input = Input;
                    if (input == null)
                        continue;
                    bool shortcutUsed = input.SkillbarInputs[i, shortcut.Slot];
                    if (shortcutUsed)
                    {
                        UseShortcut(shortcut);
                    }
                }
            }
        }
    }

    public string GetKeybindForShortcut(int skillbarId, int slot)
    {
        InputManager input = Input;
        if (input == null)
            return string.Empty;
        InputAction action = input.SkillbarActions[skillbarId, slot];
        return action.GetBindingDisplayString(0).ToUpper();
    }

    public void UseShortcut(Shortcut shortcut)
    {
        // Debug.LogWarning($"Use shortcut {shortcut.Page * MAXIMUM_SHORTCUTS_PER_BAR + shortcut.Slot}.");
        switch (shortcut.Type)
        {
            case Shortcut.TYPE_ITEM:
            {
                PlayerInventory inventory = Inventory;
                if (inventory == null)
                    break;

                // Left click / hotkey: use the item once (Mobius UseItem). Auto-shot is right-click only.
                // Level carries the template itemId so a restacked shot can still be resolved.
                ItemInstance item = inventory.FindByAllInventory(shortcut.Id);
                if (item == null && shortcut.Level > 0)
                    item = inventory.GetItemByItemId(shortcut.Level);
                inventory.UseItem(item != null ? item.ObjectId : shortcut.Id, true);
                break;
            }
            case Shortcut.TYPE_ACTION:
                PlayerActions actions = Actions;
                if (actions != null)
                    actions.UseAction((ActionType)shortcut.Id);
                break;
            case Shortcut.TYPE_SKILL:
                PlayerActions skillActions = Actions;
                if (skillActions != null)
                    skillActions.UseSkill(shortcut.Id);
                break;
            default:
                break;
        }
    }
}
