using System;

using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class TargetWindow : L2Window
{
    private void TooogleHide(bool isHideHpBar)
    {
        if (isHideHpBar){
            HideBar();
        }
        else{
            ShowBar();
        }
    }

    private void HideBar()
    {
        _HPBarBG.style.display = DisplayStyle.None;
        _HPBar.style.display = DisplayStyle.None;
    }

    private void ShowBar()
    {
        _HPBarBG.style.display = DisplayStyle.Flex;
        _HPBar.style.display = DisplayStyle.Flex;
    }

    // TargetStatusWnd.uc HandleTargetUpdate: the layout extract creates every xdat control of the window, but
    // retail only shows the clan / alliance block (txtPledge "Clan", txtAlliance "Ally", crests, names) for a
    // player target in expanded mode, RankName for a player rank, and barMP for self / own pet. Our expand
    // exists only for NPC properties and ShouldHideHpBar hides the gauges for self / players, so none of
    // these show; left visible they spilled "Cla.. All.." below the 46 px plate and drew an empty MP strip.
    static readonly string[] RetailHiddenControls =
    {
        "RankName", "barMP",
        "txtPledge", "texPledgeCrest", "txtPledgeName",
        "txtAlliance", "texPledgeAllianceCrest", "txtPledgeAllianceName",
    };

    readonly VisualElement[] _retailHidden = new VisualElement[RetailHiddenControls.Length];

    void ApplyRetailControlVisibility()
    {
        for (int i = 0; i < RetailHiddenControls.Length; i++)
        {
            VisualElement element = _retailHidden[i];
            if (element == null)
            {
                // Created by InterludeWindowLayout once the window has a size; picked up on the next frame.
                element = _windowEle.Q<VisualElement>(RetailHiddenControls[i]);
                if (element == null)
                    continue;
                _retailHidden[i] = element;
            }
            if (element.style.display != DisplayStyle.None)
                element.style.display = DisplayStyle.None;
        }
    }
}
