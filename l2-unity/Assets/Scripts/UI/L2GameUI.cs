using System;

﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class L2GameUI : L2UI
{
    private NativeFunctions.NativeCoords _lastMousePosition;
    [SerializeField] private bool _mouseEnabled = true;

    private List<L2PopupWindow> _openedWindows;

    private static L2GameUI _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static L2GameUI Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DiBootstrap.EnsureGameScope();
            WireSystemUIPacketServices();
        }
        else
        {
            Destroy(this);
        }

        _openedWindows = new List<L2PopupWindow>();
    }

    private void Start()
    {
        WireSystemUIPacketServices();
    }

    private void WireSystemUIPacketServices()
    {
        SystemUIPacketServices.Bind(ui: this);
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    protected override void Update()
    {
        base.Update();

        InputManager input = ControllerPacketServices.Input;
        if (input != null && input.TurnCamera)
        {
            DisableMouse();
        }
        else
        {
            EnableMouse();
        }

        CheckUIInputs();
    }









    static void EnsurePartyMemberListWindow()
    {
        if (PartyPacketServices.MemberList != null || Instance == null)
            return;
        Instance.gameObject.AddComponent<PartyMemberListWindow>();
    }
}
