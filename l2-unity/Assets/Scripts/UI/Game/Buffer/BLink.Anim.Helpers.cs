using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public partial class BLink
{
    private void RemoveFilterElements(List<int> filter)
    {
        foreach (int position in filter)
        {
            _dict[position].ResetData();
            _dict[position].ShowCell(false);
            _dict.Remove(position);
        }

        filter.Clear();
    }

    public bool IsBLink()
    {
        return _isBlinking;
    }

    public float  GetLeftTimeBySkillId(int skillId)
    {
        
        foreach (KeyValuePair<int, DataCell> entry in _dict)
        {
            if (entry.Value.GetSkillId() == skillId)
            {
                return entry.Value.GetLastRemainingTime();
            }
        }

        return 0;
    }

    public float GetLeftTimeByPosition(int position)
    {

        if (_dict.ContainsKey(position))
        {
            return _dict[position].GetLastRemainingTime();
        }

        return 0;
    }

    public void OnDisable()
    {
        _isBlinking = false;
        _dict.Clear();
        _lastUpdateTime.Clear();
        OnTimeLeftUpdated = null;
        OnSpecificTimeLeftUpdated = null;
    }
}
