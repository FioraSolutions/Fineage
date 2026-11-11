using System;
using System.Collections.Generic;
using UnityEngine;

public partial class HitManager
{
    sealed class RemoteMeleeHit
    {
        public int AttackerId;
        public Entity Attacker;
        public Entity Target;
        public bool Soulshot;
        public bool Fired;
        public float FireAt;
        public bool WaitForShot;
        public bool Miss;
        public AttackDto Packet;
        public AnimationEventsBase Events;
        public Action<string> OnShot;
    }



    /// <summary>
    /// Remote melee (Monster / CharInfo). Local PlayerEntity stays on AttackShot + sword collision.
    /// Impact waits for clip AttackShot; timer is only a late fallback.
    /// Bow: ignore AttackShot — the arrow has not left yet. Impact is ArrowStick Hit Time.
    /// </summary>



}
