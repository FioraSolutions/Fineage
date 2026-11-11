using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ClanWindow : L2TwoPanels
{
    private void WireClanPacketServices()
    {
        ClanPacketServices.Bind(clan: this);
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Clan/ClanWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {

        InitWindow(root);

        yield return new WaitForEndOfFrame();

        defaultHeight = 447;
        defaultWidth = 538;

        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);


        _dropdown = _windowEle.Q<DropdownField>("clanComboBox");
        _dropdown.pickingMode = PickingMode.Position;
        DisableEventOnOver(_dropdown);
        _dropdown.RegisterCallback<PointerDownEvent>(OnDropdownPointer, TrickleDown.TrickleDown);

        var master_table_content = GetElementByClass("master-table-list");

        _creatorTableWindows.InitTable(master_table_content);
        _creatorTableWindows.LoadAsset(LoadAsset);
        _masterClan.ForEachClan(_creatorTableWindows);
        _creatorTableWindows.OnRowClicked += _masterClan.SelectMember;
        _creatorTableWindows.OnRowClicked += SelectMember;

        _detailedClan.LoadAssets(LoadAsset);

        var memberButton = (UnityEngine.UIElements.Button)GetElementById("MemberButton");
        memberButton?.RegisterCallback<ClickEvent>(evt => OnClickShowMember(evt));

        var privilegesButton = (UnityEngine.UIElements.Button)GetElementById("PrivilegesButton");
        privilegesButton?.RegisterCallback<ClickEvent>(evt => OnClickPrivileges(evt));
   
        var editAuthButton = (UnityEngine.UIElements.Button)GetElementById("EditAuthButton");
        editAuthButton?.RegisterCallback<ClickEvent>(evt => OnClickEditAuthButton(evt));

        var clanInfo = (UnityEngine.UIElements.Button)GetElementById("ClanInoButton");
        clanInfo?.RegisterCallback<ClickEvent>(evt => OnClickClanInfo(evt));

        var penaltyButton = (UnityEngine.UIElements.Button)GetElementById("PenaltyButton");
        penaltyButton?.RegisterCallback<ClickEvent>(evt => OnClickPenalty(evt));

        var leaveButton = (UnityEngine.UIElements.Button)GetElementById("LeaveButton");
        leaveButton?.RegisterCallback<ClickEvent>(evt => OnClickLeave(evt));

        var inviteButton = (UnityEngine.UIElements.Button)GetElementById("InviteButton");
        inviteButton?.RegisterCallback<ClickEvent>(evt => OnClickInvite(evt));

        BuildZoneActions();
        _creatorTableWindows.OnRowClicked += OnZoneRowClicked;

        _detailedInfoElement = (VisualElement)GetElementById("detailedInfo");
        var masterClan = (VisualElement)GetElementById("masterClan");

        _detailedClan.SetDetailedInfoElement(_detailedInfoElement);

        _labelEditAuth = (UnityEngine.UIElements.Label)GetElementById("EditAuthLabel");
        _labelMemberInfo = (UnityEngine.UIElements.Label)GetElementById("MemberInfoLabel");
        _labelPrivileges = (UnityEngine.UIElements.Label)GetElementById("PrivilegesLabel");
        _labelClanInfo = (UnityEngine.UIElements.Label)GetElementById("ClanInfoLabel");
        _labelPenalty = (UnityEngine.UIElements.Label)GetElementById("PenaltyLabel");
        _labelLeave = (UnityEngine.UIElements.Label)GetElementById("LeaveLabel");
        _labelClanEntry = (UnityEngine.UIElements.Label)GetElementById("ClanEntryLabel");
        _labelInvite = (UnityEngine.UIElements.Label)GetElementById("InviteLabel");


        SetMouseOverDetectionSubElement(_detailedInfoElement);
        SetMouseOverDetectionRefreshTargetElement(masterClan);


        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);
        HideDetailedInfo();

        OnCenterScreen(_root);
        EnabledUnkButtons();

        if (_packet != null)
            ApplyClanData(_packet);
    }
}
