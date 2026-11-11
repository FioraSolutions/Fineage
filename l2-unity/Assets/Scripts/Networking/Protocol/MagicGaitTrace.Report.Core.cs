using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static partial class MagicGaitTrace
{
    public static void Decision(Entity entity, string tag, string detail)
    {
        if (!Enabled || !IsMagic(entity) || entity.Identity == null)
            return;
        Write(entity, tag, detail, dumpAnim: true, stack: false);
    }

    public static void BeginPostShotWatch(Entity entity)
    {
        if (!Enabled || !IsMagic(entity) || entity.Identity == null)
            return;
        WatchUntil[entity.Identity.Id] = Time.unscaledTime + 4f;
        Event(entity, "post-shot-WATCH", "dense-sync 4s");
    }

    static void Write(Entity entity, string tag, string detail, bool dumpAnim, bool stack)
    {
        int id = entity.Identity.Id;
        string nick = entity.Identity.Name ?? ("#" + id);
        UserEntity user = entity as UserEntity;

        Sb.Length = 0;
        Sb.Append("[MAGIC_GAIT] t=").Append(Time.unscaledTime.ToString("F3"))
            .Append(" frame=").Append(Time.frameCount)
            .Append(" ").Append(tag)
            .Append(" nick=").Append(nick)
            .Append(" id=").Append(id)
            .Append(" race=").Append(entity.RaceId)
            .Append(" weapon=").Append(user != null ? user.WeaponAnim : "?")
            .Append(" running=").Append(entity.Running)
            .Append(" inCombat=").Append(entity.InCombat)
            .Append(" action=").Append(entity.ActionSlot != null ? entity.ActionSlot.Action.ToString() : "?")
            .Append(" attackVis=").Append(user != null && user.IsAttackVisualPlaying())
            .Append(" visLeft=").Append(user != null ? user.AttackVisualLeftSec().ToString("F3") : "-")
            .Append(" isMoving=").Append(
                MoveAllCharactersPacketServices.Moves != null && MoveAllCharactersPacketServices.Moves.IsMoving(id));

        MagicCastData cast = entity.GetMagicCastData();
        if (cast != null)
        {
            Sb.Append(" speedShot=").Append(cast.SpeedShot.ToString("F3"))
                .Append(" speedMid=").Append(cast.SpeedMid.ToString("F3"))
                .Append(" speedEnd=").Append(cast.SpeedEnd.ToString("F3"));
        }

        if (!string.IsNullOrEmpty(detail))
            Sb.Append(" | ").Append(detail);

        if (dumpAnim)
            AppendAnimatorDump(Sb, user, id);

        if (stack)
        {
            string st = new StackTrace(2, false).ToString();
            Sb.Append(" | stack=");
            AppendShortStack(Sb, st);
        }

        string text = Sb.ToString();
        Debug.Log(text);

        try
        {
            string dir = Path.GetDirectoryName(LogPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.AppendAllText(LogPath, text + "\n");
        }
        catch (Exception ex)
        {
            Debug.LogWarning("MagicGaitTrace file: " + ex.Message);
        }
    }
}
