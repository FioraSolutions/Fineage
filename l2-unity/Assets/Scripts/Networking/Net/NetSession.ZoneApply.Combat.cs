using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetSession
{


    /// <summary>
    /// The zone dropped the selection. Clear the target window and nameplate without echoing another
    /// <c>RequestTargetCanceld</c> back: this arrived because that request was already answered.
    /// </summary>




    /// <summary>
    /// Mobius <c>AutoAttackStart</c>: the pawn raises its weapon. The zone was already fanning this,
    /// but with no case here a Net client never put an actor into attack stance.
    /// </summary>


    /// <summary>Mobius <c>AutoAttackStop</c>: stance ended, so the pawn drops back to its wait pose.</summary>





    /// <summary>Mobius ChangeWaitType: sit/stand on the actor, self included so a sit is acknowledged.</summary>


    /// <summary>Mobius SocialAction: a gesture on the actor (15 = level up).</summary>












}
