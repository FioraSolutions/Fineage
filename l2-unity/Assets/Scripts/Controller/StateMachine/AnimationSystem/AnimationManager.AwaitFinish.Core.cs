using System;
using System.Threading.Tasks;
using UnityEngine;

public partial class AnimationManager
{
    public void OnAnimationFinished(string name, int objectId)
    {
        TryCompleteAwaitFromPhaseFinish(objectId, name, "clip_complete");
    }

    public void NotifyMagicPhaseFinished(int objectId, string phaseName)
    {
        MagicGaitTrace.EventId(objectId, "NotifyMagicPhaseFinished", "phase=" + phaseName);
        TryCompleteAwaitFromPhaseFinish(objectId, phaseName, "smb_phase");
    }
}
