using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public partial class BLink
{
    private void SetTextTimeToElement(Dictionary<int, DataCell> _dict, float elapsedTime)
    {
        foreach (KeyValuePair<int, DataCell> entry in _dict)
        {
            DataCell cell = entry.Value;
            float time = cell.GetRemainingTime(elapsedTime);

            if(time <= 60)
            {
                int roundTime = (int)Math.Round(time);
                cell.SetText(roundTime.ToString());



                // Check if this skill ID is being tracked
                if (_trackedSkillIds.Contains(cell.GetSkillId()))
                {
                    // Check if enough time has passed since last update
                    if (!CheckAndUpdateLastTime(cell.GetSkillId(), elapsedTime))
                    {
                        OnSpecificTimeLeftUpdated?.Invoke(cell.GetSkillId(), time);
                    }
                }

                // Check if enough time has passed since last update
                if (!CheckAndUpdateLastTime(cell.GetSkillId(), elapsedTime))
                {
                    OnTimeLeftUpdated?.Invoke(cell.GetSkillId(), time);
                }
            }
        
        }
        
    }

    private bool CheckAndUpdateLastTime(int skillId, float currentTime)
    {
        if (_lastUpdateTime.TryGetValue(skillId, out float lastTime))
        {
            if (currentTime - lastTime < _updateInterval)
            {
                return true; // Skip update
            }
        }

        _lastUpdateTime[skillId] = currentTime;
        return false; // Allow update
    }

    private void HideElements(Dictionary<int, DataCell> _dict , float elapsedTime)
    {
        foreach (KeyValuePair<int, DataCell> entry in _dict)
        {
            DataCell cell = entry.Value;

           // Debug.Log("Start blink add element isHide INTERVAL USE" + cell.GetIntervalUse() + " remaning time " + cell.GetRemainingTime(elapsedTime));

            if (cell.GetRemainingTime(elapsedTime) <= cell.GetIntervalUse() & cell.IsHideElements())
            {
                cell.SetTimeDeltaTime(Time.deltaTime);
                float newOpacity = Mathf.Lerp(opacityShow, opacityHide, cell.GetSmoothTime() / durationOpacity);
               
                cell.GetElement().style.opacity = newOpacity; 
            }
        }
    }

    private void ShowElements(Dictionary<int, DataCell> _dict , float elapsedTime)
    {
        foreach (KeyValuePair<int, DataCell> entry in _dict)
        {
            DataCell cell = entry.Value;
            VisualElement element = cell.GetElement();

            if (element != null)
            {
               // Debug.Log("Start blink add element INTERVAL USE" + cell.GetIntervalUse() + " remaning time " + cell.GetRemainingTime(elapsedTime));
                if (cell.GetRemainingTime(elapsedTime) <= cell.GetIntervalUse() & cell.IsShowElements())
                {
                    cell.SetTimeDeltaTime(Time.deltaTime);
                    float newOpacity = Mathf.Lerp(opacityHide, opacityShow, cell.GetSmoothTime() / durationOpacity);
                   
                    cell.GetElement().style.opacity = newOpacity;
                }
            }
        }
    }

    private void FilterElementElseNoTimeOut(float currentTime , List<int> filter , Dictionary<int, DataCell> _dict)
    {
        foreach (KeyValuePair<int, DataCell> entry in _dict)
        {
            int position = entry.Key;
            DataCell cell = entry.Value;
            float remainingTime = cell.GetRemainingTime(currentTime);
            //Debug.Log("Start blink remaining time " + remainingTime);
            if (remainingTime <= 0)
            {
                filter.Add(position);
            }
        }
    }
}
