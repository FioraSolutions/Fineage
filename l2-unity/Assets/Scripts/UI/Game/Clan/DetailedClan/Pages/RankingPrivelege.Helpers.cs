using FMOD.Studio;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class RankingPrivelege : AbstractClanContent
{
    private void CreateTable(PledgePowerGradeListDto rankingList , VisualElement master_table_content , Func<string, VisualTreeAsset> loaderFunc)
    {
        if(master_table_content != null)
        {
            _creatorTableWindows.InitTable(master_table_content);
            _creatorTableWindows.LoadAsset(loaderFunc);
            RankedClanTableColumn.ForEachClan(
                rankingList.GradeList.ConvertAll(g => g.GetPower()),
                _creatorTableWindows);
        }

    }

    public void SelectRank(int select , string name)
    {
        if (ArrayUtils.IsValidIndexList(_rankingList.GradeList, select))
        {
            GradeList gradeItem = _rankingList.GradeList[select];
            _selectRank = gradeItem;
        }
    }

    private void OnClickEdit(ClickEvent evt)
    {
        if (_selectRank != null)
        {

            ZoneCommands.Send(new RequestPledgePowerCommand(_selectRank.GetRank(), 1, _selectRank.GetPower()));

            //OnSwitchRank?.Invoke(EDIT_MODE_ALL_NOT_CHECKED);

        }

    }

    private void ClearClick(VisualElement content)
    {
        Button editButton = content.Q<Button>(button_edit_auth);
        editButton?.UnregisterCallback<ClickEvent>(evt => OnClickEdit(evt));
    }
}
