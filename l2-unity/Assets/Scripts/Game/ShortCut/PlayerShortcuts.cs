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









}
