using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class QuestWindow
{
    private void OnClickAbortButton(ClickEvent evt)
    {
        SystemMessagePacketServices.Window.OnButtonOk += OkAbort;
        SystemMessagePacketServices.Window.OnButtonClosed += OnCancel;
        SystemMessagePacketServices.Window.ShowWindowDialogYesOrNot("Do you really want to abort the quest?");
    }

    private void OkAbort()
    {
        if(_selectQuest != null)
        {
            ZoneCommands.Send(new RequestQuestAbortCommand(_selectQuest.QuestID));
            CancelEvent();
        }

    }

    public void OnCancel()
    {
        CancelEvent();
    }

    private void CancelEvent()
    {
        SystemMessagePacketServices.Window.OnButtonOk -= OkAbort;
        SystemMessagePacketServices.Window.OnButtonClosed -= OnCancel;
    }

    private bool IsSuccess()
    {
        if (_builderTabs.GetNameActiveTab() == _singleTabName)
        {
            if (nonRepeatQuests != null && nonRepeatQuests.Any(q => q.IsComplete() != true))
            {
                return true;
            }
        }
        else if (_builderTabs.GetNameActiveTab() == _repeatTabName)
        {
            if (repeatQuests != null && repeatQuests.Any(q => q.IsComplete() != true))
            {
                return true;
            }
        }
        return false;
    }
}
