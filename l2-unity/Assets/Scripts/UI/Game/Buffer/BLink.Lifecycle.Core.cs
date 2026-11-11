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
}
