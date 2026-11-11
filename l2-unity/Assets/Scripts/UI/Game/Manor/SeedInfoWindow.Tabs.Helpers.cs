using System;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class SeedInfoWindow
{
    public void OnClickAllButton1(ClickEvent evt)
    {

        GameWindowPacketServices.Html.UseActionCommand(commandButton1);
    }

    public void OnClickAllButton2(ClickEvent evt)
    {
        GameWindowPacketServices.Html.UseActionCommand(commandButton0);
    }

    public void ShowWindowActiveTabSeed()
    {
        _creatorWindow.SwitchTab(0 , false , true);
        AddContent0Footer(_creatorWindow, _footerContainer);
        base.ShowWindow();
    }

    public void ShowWindowActiveTabAllDefault()
    {
        _creatorWindow.SwitchTab(2, false, true);
        AddContent2Footer(_creatorWindow, _footerContainer);
        base.ShowWindow();
    }
}
