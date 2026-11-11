
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

public partial class MemberInfoContent : AbstractClanContent
{
    private const string _templateNameElementChangeRank = "Data/UI/_Elements/Game/Clan/DetailedContent/MemberInfoElement/ChangeRank";
    private const string _templateNameElementChangeTitle = "Data/UI/_Elements/Game/Clan/DetailedContent/MemberInfoElement/ChangeTitle";
    private VisualTreeAsset _tamplateChangeRank;
    private VisualTreeAsset _tamplateChangeTitle;
    private DropdownField _dropdown;
    private TextField _textfield;
    private string _selectMemeberName = "";
    private VisualElement _centerBox;

    private const string _dropdownName = "comboBox";
    private const string _textboxName = "userInputField";
    private const string _cancelButtonName = "CancelButtonBox2";
    private const string _centerBoxContentName = "Center";
    private const string _buttonRankName = "RankButton";
    private const string _buttonTitleName = "TitleButton";
    private const string _buttonDismissName = "DismissButton";

    private const string _labelRankName = "RankLabel";
    private const string _labelTitleName = "TitleLabel";
    private const string _dismissTitleName = "DismissLabel";
    private readonly List<string> _itemsDropdown = new List<string> { "1st level Privilege", "2st level Privilege", "3st level Privilege", "4st level Privilege", "Joint Rank" };


    private const string _insideTitleTemplateButtonOkName = "OkTitleButton";
    private const string _insideTitleTemplateButtonCancelName = "CancelTitleButton";
    private const string _insideTitleTemplateButtonDeleteName = "DelTitleButton";


    private const string _insideRankTemplateButtonOkName = "OkRankButton";
    private const string _insideRankTemplateButtonCancelName = "CancelRankButton";



    private Button cancelButton;


    public Action<string , int> OnOutsideClickRank;
    public Action<string , string> OnOutsideClickTitle;
    public Action<string> OnOutsideClickDismiss;
    public Action<string> OnOutsideClickDeleteTitle;


    private bool _isLeader = false;
}
