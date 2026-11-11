using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using VContainer;

public partial class HtmlWindow : L2PopupWindow
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            WireGameWindowPacketServices();
        }
        else
            Destroy(this);
    }

    private void Start()
    {
        WireGameWindowPacketServices();
    }

    private void WireGameWindowPacketServices()
    {
        GameWindowPacketServices.Bind(html: this);
    }

    private void Update()
    {
        if (PollInput.KeyDown(Key.B) && CanSend(1100))
        {
            // Debug.Log("Requesting : ShowBoard");
            ZoneCommands.Send(new RequestShowBoardCommand());
        }
    }
}
