using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class HitManager : MonoBehaviour
{
    [Inject] World _world;

    private World GameWorld =>
        _world != null ? _world : GamePacketServices.GameWorld;

    private static HitManager _instance;

    [System.Obsolete("Use HitPacketServices.Hits or [Inject] HitManager instead")]
    public static HitManager Instance { get { return _instance; } }
    private const string ETC_NAME = "etc_";
    private const string IMPACT_DEBUG_TAG = "[HIT_DEBUG]";
    private const int SoulshotImpactEffectId = 99998;
    private const int NormalImpactEffectId = 99997;
    private bool _projectileHitSubscribed;
    private readonly Dictionary<int, RemoteMeleeHit> _remoteHits = new Dictionary<int, RemoteMeleeHit>();
    private readonly List<int> _remoteHitScratch = new List<int>();
    private AttackDto _localAttack;
    private int _localAttackToken;
    private int _localAttackFiredToken;


}
