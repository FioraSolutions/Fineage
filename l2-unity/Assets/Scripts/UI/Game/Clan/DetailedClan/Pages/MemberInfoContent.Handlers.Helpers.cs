using System;
using System.Collections.Generic;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;
using UnityEngine;

public partial class MemberInfoContent
{
    private void OnRankOkClick() {
        _centerBox.Clear();
        int adjustedIndex = _dropdown != null ? _dropdown.index + 1 : -1;
        OnOutsideClickRank?.Invoke(_selectMemeberName, adjustedIndex);
    } 

    private void OnDismissButtonClick(VisualElement centerBox){ centerBox.Clear(); OnOutsideClickDismiss?.Invoke(_selectMemeberName); }

    private void OnTitleCancelClick(VisualElement centerBox)
    {
        centerBox.Clear();
    }

    public void SetDropdownList(DropdownField dropdown, List<string> items)
    {
        if (items == null)
        {
            dropdown.choices = new List<string>();
            dropdown.SetValueWithoutNotify(string.Empty);
        }
        else
        {
            dropdown.choices = items;
        }

    }

    private void InsideUnregisterAll()
    {
        UnregisterInsideCallBackAllButtons();
    }
}
