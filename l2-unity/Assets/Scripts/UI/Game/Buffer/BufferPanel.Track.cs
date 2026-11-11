

using NUnit.Framework;
using System;
using System.Linq;
using static UnityEngine.Rendering.DebugUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class BufferPanel
{
    public void TrackSkillId(int skillId)
    {
        _bLink.TrackSkillId(skillId);
    }

    public void UntrackSkillId(int skillId)
    {
        _bLink.UntrackSkillId(skillId);
    }

    public void ResetAllSubscrible()
    {
        foreach (Action<int, float> handler in _allCurrentHandler)
        {
            _bLink.OnSpecificTimeLeftUpdated -= handler;
        }
        _allCurrentHandler.Clear();
        _bLink.AllClearTrackSkillId();
    }

    public void SetSubscrible(Action<int, float> handler)
    {
        _allCurrentHandler.Add(handler);
        OnSpecificTimeLeftUpdated += handler;
    }

    public void MovePanelToChatPosition(float sourceY)
    {
        _windowEle.style.top = 3;
        _windowEle.style.left = sourceY;

    }

}
