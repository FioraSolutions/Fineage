using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class HitManager
{
    sealed class RemoteMeleeHit
    {
        public int AttackerId;
        public Entity Attacker;
        public Entity Target;
        public int NextHitIndex;
        public int HitCount;
        public float CycleStart;
        public float CycleSec;
        public float FireAt;
        public bool WaitForShot;
        public AttackDto Packet;
        public AnimationEventsBase Events;
        public Action<string> OnShot;
    }



    /// <summary>
    /// Melee Attack packets (local and remote) present every hit row on one clock:
    /// Mobius timeAtk/2, then hit i of N at sAtk*(i+1)/N. Bow stays on the arrow.
    /// A skill swing with no Attack hit list still waits for AttackShot.
    /// </summary>



}