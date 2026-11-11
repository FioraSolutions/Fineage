using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using System.Collections.Concurrent;
using UnityEngine;

public sealed partial class NetSession
{




    /// <summary>
    /// True when <paramref name="groundObjectId"/> matches the outstanding pickup request.
    /// Clears the pending id so a later despawn of the same object is a normal delete.
    /// </summary>


    /// <summary>Mobius <c>RequestDestroyItem</c>: delete the bag slot, no ground spawn.</summary>


    /// <summary>
    /// Mobius <c>RequestAutoSoulShot</c>: right-clicking a shot on the hotbar turns automatic use on/off.
    /// <paramref name="itemId"/> is the shot template id the client carries.
    /// </summary>


    /// <summary>
    /// Registers one hotbar slot. <paramref name="worldSlot"/> is the client world slot
    /// (slot + page * 12); the zone echoes the result as <c>ShortCutRegister</c> or
    /// <c>ShortCutDelete</c>.
    /// </summary>

    /// <summary>Clears one hotbar slot (world slot = slot + page * 12).</summary>

    /// <summary>
    /// Mobius <c>RequestActionUse</c>: 0 = sit/stand, 1 = walk/run, 12/13/14 = social gestures.
    /// The zone answers with ChangeWaitType / ChangeMoveType / SocialAction.
    /// </summary>

    /// <summary>
    /// Position heartbeat (L2 coordinates + heading). The zone reconciles the client's
    /// position against its own copy; without it a walk drifts and then snaps back.
    /// </summary>




    /// <summary>
    /// Mobius <c>RequestAcquireSkillInfo</c>: ask the zone for the confirmation page of one trainer offer.
    /// Called by the trainer window when a row is clicked.
    /// </summary>

    /// <summary>Mobius <c>RequestAcquireSkill</c>: buy the offer (SP is spent by the zone).</summary>

    /// <summary>Mobius <c>RequestEnchantItem</c>: the bag scroll the player clicked opens the window.</summary>

    /// <summary>The item placed in the enchant slot; the zone rolls the scroll against it.</summary>

    /// <summary>The player closed the window without rolling. The zone clears the scroll.</summary>



















    /// <summary>Mobius <c>RequestCreatePledge</c>: found a clan with this name.</summary>

    /// <summary>Mobius <c>RequestJoinPledge</c>: ask the owner of this clan to enrol the player.</summary>

    /// <summary>Mobius <c>RequestAnswerJoinPledge</c>. Response 0 declines.</summary>

    /// <summary>Mobius <c>RequestWithdrawalPledge</c>.</summary>

    /// <summary>Mobius <c>RequestOustPledgeMember</c>, by object id.</summary>

    /// <summary>Mobius <c>RequestDissolvePledge</c>.</summary>

    /// <summary>Mobius <c>RequestPledgeReorganizeMember</c>-side cancel of a running dissolution.</summary>

    /// <summary>Mobius <c>RequestTargetCanceld</c>: drop the selection on the zone side too.</summary>
}
