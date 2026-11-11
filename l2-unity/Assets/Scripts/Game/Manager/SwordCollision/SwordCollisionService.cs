using System;

using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class SwordCollisionService : MonoBehaviour
{
    [Inject] World _world;

    private World GameWorld =>
        _world != null ? _world : GamePacketServices.GameWorld;

    private const string TIMER_LOG = "[SWORD_TIMER]";
    private const string CHAIN_LOG = "[ATK_HIT_CHAIN]";
    private const float DEFAULT_HIT_FRACTION = 0.88f;
    private const float DEFAULT_ATTACK_DURATION_MS = 1000f;

    private sealed class AttackTimingContext
    {
        public int EntityId;
        public int TargetEntityId;
        public Transform AttackerTransform;
        public Transform TargetTransform;
        public float StartTimeSec;
        public float DurationMs;
        public float HitFraction;
        public float LastElapsedMs;
        public int Epoch;
        public bool AttackShotFired;
    }

    private sealed class TimedSwordHit
    {
        public TrackedSword Tracked;
        public int AttackerEntityId;
        public int TargetEntityId;
        public float StartTimeSec;
        public float HitAtSec;
        public bool Fired;

        public TimedSwordHit(TrackedSword tracked, int attackerEntityId, int targetEntityId, float startTimeSec, float hitAtSec)
        {
            Tracked = tracked;
            AttackerEntityId = attackerEntityId;
            TargetEntityId = targetEntityId;
            StartTimeSec = startTimeSec;
            HitAtSec = hitAtSec;
            Fired = false;
        }
    }

    private static SwordCollisionService _instance;

    [System.Obsolete("Use SwordCollisionPacketServices.Collision or [Inject] SwordCollisionService instead")]
    public static SwordCollisionService Instance { get { return _instance; } }

    public LayerMask _entityMask;
    private readonly List<TimedSwordHit> _activeTimedHits = new List<TimedSwordHit>();
    private readonly Dictionary<Transform, HashSet<int>> _hitRegistry = new Dictionary<Transform, HashSet<int>>();
    private readonly Dictionary<int, AttackTimingContext> _attackContextsByEntityId = new Dictionary<int, AttackTimingContext>();

    public event Action<Transform, Transform, Vector3, Vector3> OnHitCollider;

    /// <returns>AttackDto epoch — pass to EndAttack so an older swing cannot clear a newer BeginAttack.</returns>





    /// <summary>
    /// Melee Hit/SoulShot from Unity Animation Event AttackShot (L2 AnimNotify_AttackShot).
    /// AttackShot is the source of truth — do not drop FX when BeginAttack context is missing
    /// (EndAttack / packet race can clear it before the clip notify fires).
    /// </summary>










    // --- OLD wall-clock schedule helpers (kept for reference / quick rollback). ---
    // private void CreateTimedHit(int attackerEntityId, int targetEntityId, Transform swordBase, Transform swordTip, Transform target, float extraRange)
    // {
    //     float now = Time.time;
    //     float delaySec = ResolveHitDelaySec(swordBase, attackerEntityId, targetEntityId, extraRange);
    //     var tracked = new TrackedSword(swordBase, swordTip, target, extraRange);
    //     _activeTimedHits.Add(new TimedSwordHit(tracked, attackerEntityId, targetEntityId, now, now + delaySec));
    //     ...
    // }
    //
    // private void RecreateTimedHit(...)
    // private float ResolveHitDelaySec(...)



}
