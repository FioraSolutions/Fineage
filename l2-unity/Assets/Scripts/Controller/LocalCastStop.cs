using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Mobius <c>CreatureAI.thinkCast</c> / <c>Player.useMagic</c> → <c>clientStopMoving(null)</c>:
/// the caster halts on the spot the instant a cast is requested or confirmed.
///
/// The server already halts its authoritative timed move in <c>StopForCast</c> and (by design)
/// sends nothing back to the caster, expecting the client to stop on <c>MagicSkillUse</c>.
/// Blocking new input through <see cref="CastMoveLock"/> alone is not enough: the local
/// <see cref="PlayerController"/> keeps stepping toward its old destination, so the client
/// drifts a few metres ahead of the server, the next ValidatePosition fails and
/// PositionCorrection rubber-bands the hero back to the cast origin.
/// </summary>
public static class LocalCastStop
{
    /// <summary>
    /// Clears the local move order and the locomotion state so the pawn stands still at once.
    /// Safe to call repeatedly; a no-op when the hero is not moving.
    /// </summary>
    public static void Halt()
    {
        PlayerController player = ControllerPacketServices.Player;
        if (player == null)
            return;

        if (!player.RunningToDestination)
            return;

        player.StopMove();
    }

    /// <summary>
    /// Stops the hero and drops the queued click/keyboard destination, then lets the state
    /// machine fall back to idle so the cast animation starts from a standing pose.
    /// </summary>
    public static void HaltForCast()
    {
        Halt();

        PlayerStateMachine machine = PlayerStatePacketServices.PlayerState;
        if (machine == null)
            return;

        if (machine.State == PlayerState.RUNNING || machine.State == PlayerState.WALKING)
        {
            machine.NotifyEvent(Event.ARRIVED);
            machine.ChangeIntention(Intention.INTENTION_IDLE);
        }

        // A queued MoveToPawn chase would otherwise resume the run after the cast.
        machine.IsMoveToPawn = false;
    }
}

/// <summary>
/// Client copy of the server's skill reuse timers, fed by the local MagicSkillUse (reuse delay) and SkillList
/// (remaining reuse). Used only to decide whether a skill press may stop the hero before the zone answers.
///
/// Mobius Player.useMagic: a skill on reuse is refused with SystemMessage "%s is not available at this time" +
/// ActionFailed and the hero is never stopped, so the run goes on. Halting locally on such a press froze the
/// client pawn (run animation on the spot) while the server kept moving it, and the next correction teleported it
/// forward.
/// </summary>
public static class LocalSkillReuse
{
    // Press-side slack: a press a little before the server timer runs out is still refused by the zone, so treat
    // the tail of the reuse window as "still on reuse" and keep running. A press that lands just after the real end
    // is fine either way: MagicSkillUse halts the hero through MagicSkillPresentation.
    const long EarlyPressSlackMs = 250;

    static readonly object Gate = new object();
    static readonly Dictionary<int, long> ReadyAtMs = new Dictionary<int, long>();

    static long NowMs => Stopwatch.GetTimestamp() * 1000L / Stopwatch.Frequency;

    /// <summary>Starts (or restarts) the reuse window for <paramref name="skillId"/>. Thread-safe.</summary>
    public static void Begin(int skillId, long reuseMs)
    {
        if (skillId <= 0)
            return;

        lock (Gate)
        {
            if (reuseMs <= 0)
            {
                ReadyAtMs.Remove(skillId);
                return;
            }

            ReadyAtMs[skillId] = NowMs + reuseMs;
        }
    }

    /// <summary>True while the server would refuse <paramref name="skillId"/> with skill_reuse.</summary>
    public static bool IsOnReuse(int skillId)
    {
        lock (Gate)
        {
            if (!ReadyAtMs.TryGetValue(skillId, out long readyAt))
                return false;

            if (NowMs + EarlyPressSlackMs < readyAt)
                return true;

            ReadyAtMs.Remove(skillId);
            return false;
        }
    }

    public static void Clear()
    {
        lock (Gate)
            ReadyAtMs.Clear();
    }
}
