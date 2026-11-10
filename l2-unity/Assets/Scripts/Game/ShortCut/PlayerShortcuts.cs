using System;

﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

public class PlayerShortcuts : MonoBehaviour
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
            _shortcuts.Add(shortcut.Slot + shortcut.Page * MAXIMUM_SHORTCUTS_PER_BAR, shortcut);
        }

        SkillbarPacketServices.Skillbar.UpdateAllShortcuts(shortcuts);
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
        SkillbarPacketServices.Skillbar.AddShortcut(shortcut);
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
        SkillbarPacketServices.Skillbar.RemoveShortcut(slot);
        _shortcuts.Remove(slot);
    }

    private void RemoveShotcutNoVisible(int slot)
    {
        //SkillbarPacketServices.Skillbar.RemoveShortcut(slot);
        _shortcuts.Remove(slot);
    }


    public void UpdatePageMapping(int skillbarIndex, int page)
    {
        _pageMap[skillbarIndex] = page;
    }

    #region ShortcutClientRequests
    // Shortcut dragged onto skillbar
    public void AddShortcut(int slot, int id, int type , int level)
    {
        Shortcut oldShortcut = GetShortcutBySlot(slot);
        if(oldShortcut != null) {
            RemoveShotcutNoVisible(slot);
        }
        
        //GameClient.Instance.ClientPacketHandler.RequestAddShortcut(type, id, slot);
        IZoneTransport game = ZoneCommands.Transport;
        if (game != null)
            ZoneCommands.Send(new RequestShortCutRegCommand(type, slot, id, level));
    }

    // Shortcut dragged within bar
    public void MoveShortcut(int oldSlot, int newSlot)
    {
        Shortcut oldShortcut = GetShortcutBySlot(oldSlot);
        Shortcut newShortcut = GetShortcutBySlot(newSlot);
        RemoveShotcutNoVisible(newSlot);
        RemoveShotcutNoVisible(oldSlot);

        if (oldShortcut == null)
        {
            // Debug.LogError($"MoveShortcut. Old slot is null at {oldSlot}.");
            return;
        }


        // Debug.Log("Event Reuqets Add ShrtCut 2 ");
        IZoneTransport game = ZoneCommands.Transport;
        if (game != null)
            ZoneCommands.Send(new RequestShortCutRegCommand(oldShortcut.Type, newSlot, oldShortcut.Id, oldShortcut.Level));

        // Swap slots
        if (newShortcut != null)
        {
            if (game != null)
                ZoneCommands.Send(new RequestShortCutRegCommand(newShortcut.Type, oldSlot, newShortcut.Id, newShortcut.Level));
        }
        else
        {
            DeleteShortcut(oldSlot, true);
        }
    }

    // Shortcut dragged out of bar
    public void DeleteShortcut(int oldSlot , bool locRemove)
    {
        if(locRemove) RemoveShotcutLocally(oldSlot);

        // Debug.Log("Нужно реализовать удаление shortcut");
        //GameClient.Instance.ClientPacketHandler.RequestRemoveShortcut(oldSlot);
        IZoneTransport game = ZoneCommands.Transport;
        if (game != null)
            ZoneCommands.Send(new RequestShortCutDelCommand(oldSlot));
    }

    #endregion
}
