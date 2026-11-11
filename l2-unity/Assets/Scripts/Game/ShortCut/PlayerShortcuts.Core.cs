using System;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

public partial class PlayerShortcuts : MonoBehaviour
{
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
}
