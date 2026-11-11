using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public sealed partial class CombatFacingService : MonoBehaviour
{
    private const string LogPrefix = "[CombatFacing]";
    private const float TurnLogIntervalSeconds = 0.35f;

    private struct FacingEntry
    {
        public Transform Actor;
        public Transform Target;
        public float LastTurnLogTime;
        public int Rate;
    }

    private static CombatFacingService _instance;

    [Obsolete("Use CombatPacketServices.Facing or [Inject] CombatFacingService instead")]
    public static CombatFacingService Instance => _instance;

    [Inject] WeapongrpTable _weaponGrps;
    [Inject] L2PawnYaw _pawnYaw;

    private PlayerController Player =>
        ServicePacketServices.Player
        ?? WorldApplyPacketServices.Player
        ?? ControllerPacketServices.Player;

    private static WeapongrpTable Weapons
    {
        get
        {
            if (_instance != null && _instance._weaponGrps != null)
                return _instance._weaponGrps;
            return TablePacketServices.WeaponGrps;
        }
    }


    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _angleThreshold = 3f;

    private readonly Dictionary<int, FacingEntry> _entries = new Dictionary<int, FacingEntry>(8);
    private readonly List<int> _iterateIds = new List<int>(8);
    private readonly List<int> _removeIds = new List<int>(8);

    public int ActiveCount => _entries.Count;
    public bool IsActive => _entries.Count > 0;





    /// <summary>
    /// Start / replace tracking for <paramref name="objectId"/> until <see cref="EndFollow(int)"/>.
    /// </summary>



    /// <summary>End follow for local player (convenience for current call sites).</summary>


}
