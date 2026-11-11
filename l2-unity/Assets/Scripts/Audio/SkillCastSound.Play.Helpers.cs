using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static partial class SkillCastSound
{
    public static void NotifyLocalCastBarFinished()
    {
        if (_localReleaseCasterId <= 0)
            return;
        PendingCast pending;
        string shot = "";
        string throwKind = "";
        if (_pending.TryGetValue(_localReleaseCasterId, out pending))
        {
            shot = pending.ShotClip;
            throwKind = pending.ThrowVoiceKind;
            Debug.Log(
                "[CastSync] SFX.castBarDone skill=" + pending.SkillId +
                " t=" + (Time.realtimeSinceStartup - pending.StartTime).ToString("F3") +
                " plannedShot=" + pending.ShotDelaySec.ToString("F3") +
                " alreadyShot=" + pending.ShotPlayed);
        }
        PlayRelease(_localReleaseCasterId, DomainPacketServices.LocalPlayer, shot, throwKind, "castBar");
    }
}
