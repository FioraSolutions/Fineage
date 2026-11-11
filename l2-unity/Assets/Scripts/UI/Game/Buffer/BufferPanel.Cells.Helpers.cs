using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;
using System.Collections;
using UnityEngine;
using VContainer;

public partial class BufferPanel
{
    public void DeleteDataCell(int skillId, int skillLevel)
    {
        if(skillLevel == 0)
        {
            DataCell data = _filterData.FilterBySkillId(skillId);

            if(data != null)
            {
                data.RefreshData(-1, false, 0);
                data.ShowCell(false);
            }
        }
    }

    private void CreateItemsPanel1()
    {
        for(int i = 1; i < 25; i++)
        {
            VisualElement cell = GetElementById("SlotBuffer"+i);
            VisualElement labelTemplate = GetElementById("LabelBuffer" + i);
            VisualElement slot = GetElementById("border"+i);

            if (cell != null)
            {
                 cell.style.display = DisplayStyle.None;
                 labelTemplate.style.display = DisplayStyle.None;
                _dictElement.Add(i, new DataCell(-1, cell , labelTemplate  , slot ,  i));
            }
        }
    }

    public float GetLeftTimeBySkillId(int skillId)
    {
        if(_bLink.IsBLink() == true)
        {
            return _bLink.GetLeftTimeBySkillId(skillId);
        }

        return 0;
    }

    public float GetLeftTimeByPosition(int position)
    {
        if (_bLink.IsBLink() == true)
        {
            return _bLink.GetLeftTimeBySkillId(position);
        }

        return 0;
    }

    private void RebindCellElseOnlyPenalty()
    {
        if (_filterData.IsPassiveFirstRows() != null)
        {
            MoveAllPassiveTo2Rows();
        }
    }

    public void  MoveAllPassiveTo2Rows()
    {
        List<DataCell> list = _filterData.GetAllPassive(1, 12);

        foreach (DataCell moveCell in list)
        {
            //position 13 -24 (rows2)
            DataCell empty = _filterData.GetEmptyCell(13, 24);
            empty.RefreshData(moveCell.GetSkillId(), true, moveCell.GetLevel());
            empty.ShowCell(true);

            _bLink?.StopBlinking(moveCell.GetPosition());
            moveCell.ResetData();
            moveCell.ShowCell(false);
            _dictElement[moveCell.GetPosition()] = moveCell;
        }
    }

    public SkillInstance GetCellByPosition(int position)
    {
        DataCell data = _filterData.GetCellByPosition(position);
        return (data != null)? data.GetSkillInstance() : null;
    }
}
