using System;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine;

public partial class MemberInfoContent
{
    private void OnRankButtonClick(VisualElement centerBox)
    {
        centerBox.Clear();

        InsideUnregisterAll();

        VisualElement elementChangeRank = ToolTipsUtils.CloneOne(_tamplateChangeRank);
        _dropdown = elementChangeRank.Q<DropdownField>(_dropdownName);
        _dropdown.value = "";

        var _insideOkButton = elementChangeRank.Q<Button>(_insideRankTemplateButtonOkName);
        var _insideCancelButton = elementChangeRank.Q<Button>(_insideRankTemplateButtonCancelName);

        var _isInsideRankOkSubscribed = GetStatusInsideCallBack(0);
        var _isInsideRankCancelSubscribed = GetStatusInsideCallBack(1);

        var _rankOkCallback = new EventCallback<ClickEvent>(evt => OnRankOkClick());
        var _rankCancelCallback = new EventCallback<ClickEvent>(evt => OnTitleCancelClick(centerBox));

        RegisterInsideCallBackAllButtons(new Button[2] { _insideOkButton , _insideCancelButton } , new EventCallback<ClickEvent>[2] { _rankOkCallback  , _rankCancelCallback } );


        SetDropdownList(_dropdown, _itemsDropdown);
        centerBox?.Add(elementChangeRank);
    }

    private void OnTitleButtonClick(VisualElement centerBox)
    {
        centerBox.Clear();

        InsideUnregisterAll();

        VisualElement elementChangeTitle= ToolTipsUtils.CloneOne(_tamplateChangeTitle);
        _textfield = elementChangeTitle.Q<TextField>(_textboxName);


        var _insideOkButton = elementChangeTitle.Q<Button>(_insideTitleTemplateButtonOkName);
        var _insideCancelButton = elementChangeTitle.Q<Button>(_insideTitleTemplateButtonCancelName);
        var _insideDeleteButton = elementChangeTitle.Q<Button>(_insideTitleTemplateButtonDeleteName);


        var _titleOkCallback = new EventCallback<ClickEvent>(evt => OnTitleOkClick());
        var _titleCancelCallback = new EventCallback<ClickEvent>(evt => OnTitleCancelClick(centerBox));
        var _titleDeleteCallback = new EventCallback<ClickEvent>(evt => OnTitleDeleteClick());

        RegisterInsideCallBackAllButtons(new Button[3] { _insideOkButton, _insideCancelButton , _insideDeleteButton }, new EventCallback<ClickEvent>[3] { _titleOkCallback, _titleCancelCallback , _titleDeleteCallback });

        centerBox?.Add(elementChangeTitle);
    }

    private void OnTitleOkClick(){
        _centerBox.Clear();
        OnOutsideClickTitle?.Invoke(_selectMemeberName, _textfield?.value);
    }

    private void OnTitleDeleteClick()
    {
        _centerBox.Clear();
        OnOutsideClickDeleteTitle?.Invoke(_selectMemeberName);
    }
}
