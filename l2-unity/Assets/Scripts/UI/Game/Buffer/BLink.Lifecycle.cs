using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public partial class BLink
{
    public BLink(BufferPanel bufferPanel)
    {
        _dict = new Dictionary<int, DataCell>();
        _filter = new List<int>();
        bufferPanel.StartCoroutine(Blink(_dict , _filter));
    }

    public void AllClearTrackSkillId()
    {
        _trackedSkillIds.Clear();
    }

    public void TrackSkillId(int skillId)
    {
        _trackedSkillIds.Add(skillId);
    }

    public void UntrackSkillId(int skillId)
    {
        _trackedSkillIds.Remove(skillId);
    }

    public void StartBlinking(DataCell element)
    {
        int position = element.GetPosition();
        element.GetElement().style.opacity = opacityShow;
        AddOrUpdate(element, position);
    }

    private void AddOrUpdate(DataCell element, int position)
    {
        // Upsert: a FullEffectSnapshot clears cell data but used to leave stale blink
        // entries. The old ContainsKey→Add path then threw ArgumentException (Key: N)
        // mid-rebuild and left the buff bar empty.
        _dict[position] = element;
    }

    public void StopBlinking(int position)
    {
        _dict.Remove(position);
    }

    public void ClearAllBlinking()
    {
        _dict.Clear();
    }

    System.Collections.IEnumerator Blink(Dictionary<int, DataCell> _dict , List<int> filter)
    {
        while (_isBlinking)
        {

           // Debug.Log("Start blink " + _dict.Count);
            if (_dict.Count == 0) yield return new WaitForSeconds(_blinkInterval);

            yield return new WaitForSeconds(_blinkInterval);

            float elapsedTime = Time.time;
            FilterElementElseNoTimeOut(elapsedTime, filter , _dict);
            RemoveFilterElements(filter);
            SetTextTimeToElement(_dict, elapsedTime);

            HideElements(_dict , elapsedTime);
            ShowElements(_dict , elapsedTime);
            
        }
    }
}
