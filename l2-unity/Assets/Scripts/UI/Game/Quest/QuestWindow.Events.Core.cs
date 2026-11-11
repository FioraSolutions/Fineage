using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class QuestWindow
{
    private void OnSwitchEventOut(ITab tab, bool isTrade)
    {
        var tabName = tab.GetTabName();
        var element = _builderTabs.GetActiveContent();
        
        switch (tabName)
        {
            case _singleTabName:
                _questTabPanelSingle.GetOrCreateTab(element);
                ToggleDetailedInfo(nonRepeatQuests);
                break;
        }
        switch (tabName)
        {
            case _repeatTabName:
                _questTabPanelRepeat.GetOrCreateTab(element);
                ToggleDetailedInfo(repeatQuests);
                break;
        }

    }

    private void OnClickButton(int open , QuestInstance quest)
    {
        if(open == 0)
        {
            _selectQuest = quest;
            _dataProvider.SetDataInfo(_detailedInfoElement, quest);
            _detailedInfoElement.style.display = DisplayStyle.Flex;
            _creatorRewardTables.AddOtherData(_questsList);
        }
        else
        {
            _selectQuest = null;
            _detailedInfoElement.style.display = DisplayStyle.None;
        }
    }

    private void OnClickButtonDetailedInfo(ClickEvent evt)
    {
        if (_detailedInfoElement.style.display == DisplayStyle.Flex)
        {
            _detailedInfoElement.style.display = DisplayStyle.None;
        }
        else
        {
            if (IsSuccess())
            {
                _detailedInfoElement.style.display = DisplayStyle.Flex;
            }
        }
    }

    private void OnClickCheckBoxAutoNotify(ClickEvent evt)
    {
        if (_labelAutoNotify != null && _imageBoxAutoNotify != null)
        {
            _isAutoNotifyCheckeNotify = !_isAutoNotifyCheckeNotify;
            var texture = _isAutoNotifyCheckeNotify ? IconPacketServices.Icons.GetCheckedCheckBoxTexture() : IconPacketServices.Icons.GetUncheckedCheckBoxTexture();
            _imageBoxAutoNotify.style.backgroundImage = new StyleBackground(texture);
        }
    }

    private void OnClickCheckBoxAutoTracking(ClickEvent evt)
    {
        if (_labelTracking != null && _imageBoxTracking != null)
        {
            _isAutoNotifyCheckedTracking = !_isAutoNotifyCheckedTracking;
            var texture = _isAutoNotifyCheckedTracking ? IconPacketServices.Icons.GetCheckedCheckBoxTexture() : IconPacketServices.Icons.GetUncheckedCheckBoxTexture();
            _imageBoxTracking.style.backgroundImage = new StyleBackground(texture);
        }
    }
}
