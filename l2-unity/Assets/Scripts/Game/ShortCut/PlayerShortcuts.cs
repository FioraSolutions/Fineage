using System;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

public partial class PlayerShortcuts : MonoBehaviour
{
    [Inject] PlayerInventory _inventory;
    [Inject] InputManager _input;
    [Inject] PlayerActions _actions;

    private InputManager Input => ShortcutPacketServices.Input;

    private PlayerActions Actions => ShortcutPacketServices.Actions;

    private PlayerInventory Inventory => ShortcutPacketServices.Inventory;
    public const int MAXIMUM_SHORTCUTS_PER_BAR = 12;
    public const int MAXIMUM_SKILLBAR_COUNT = 5;
    private int[] _pageMap;

    private Dictionary<int, Shortcut> _shortcuts;
    public List<Shortcut> Shortcuts { get { return _shortcuts.Values.ToList(); } }

    private static PlayerShortcuts _instance;
    [System.Obsolete("Use [Inject] or ShortcutPacketServices.Shortcuts instead")]
    public static PlayerShortcuts Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }

        _shortcuts = new Dictionary<int, Shortcut>();
        _pageMap = new int[5] { 0, 1, 2, 3, 4 };
        WireShortcutPacketServices();
    }

    private void Start()
    {
        WireShortcutPacketServices();
    }

    private void WireShortcutPacketServices()
    {
        ShortcutPacketServices.Bind(
            inventory: _inventory,
            input: _input,
            actions: _actions,
            shortcuts: this);
    }

    private void OnDestroy()
    {
        _instance = null;
    }

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

                // The original client treats a soulshot / spiritshot shortcut as an auto-use switch: the
                // click or the key arms automatic charging instead of spending a single shot by hand.
                ItemInstance item = inventory.FindByAllInventory(shortcut.Id);
                if (item != null && NetAutoShot.IsShotItem(item.ItemId))
                {
                    NetAutoShot.Toggle(item.ItemId);
                    SkillbarWindow skillbar = SkillbarPacketServices.Skillbar;
                    if (skillbar != null)
                        skillbar.RefreshAutoShotMarks();
                    break;
                }

                inventory.UseItem(shortcut.Id, true);
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
