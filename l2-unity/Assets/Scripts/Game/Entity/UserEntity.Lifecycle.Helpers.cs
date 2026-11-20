using UnityEngine;
using VContainer;

public partial class UserEntity : NetworkEntity
{
    public void LogSpeed(string where)
    {
        Stats s = Stats;
        float move = 0f;
        if (s != null)
            move = Running ? s.UnitySpeedRun : s.UnitySpeedWalking;
        string msg = LogTag + " speed " + where +
            " id=" + (Identity != null ? Identity.Id : 0) +
            " running=" + Running +
            " BaseRun=" + (s != null ? s.BaseRunSpeed : 0) +
            " BaseWalk=" + (s != null ? s.BaseWalkingSpeed : 0) +
            " RunReal=" + (s != null ? s.RunRealSpeed : 0f) +
            " WalkReal=" + (s != null ? s.WalkRealSpeed : 0f) +
            " unityRun=" + (s != null ? s.UnitySpeedRun : 0f) +
            " unityWalk=" + (s != null ? s.UnitySpeedWalking : 0f) +
            " moveSpeed=" + move;
        if (move <= 0.001f)
            Debug.LogWarning(msg);
    }

    public void BindCharInfoAnimEvents(UserBowArrowEvents bowArrowEvents)
    {
        _bowArrowEvents?.Exit();
        if (Identity == null)
            return;
        _bowArrowEvents = bowArrowEvents;
        _bowArrowEvents?.Enter();
    }

    void OnDestroy()
    {
        _bowArrowEvents?.Exit();
        _bowArrowEvents = null;
    }
}