

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
