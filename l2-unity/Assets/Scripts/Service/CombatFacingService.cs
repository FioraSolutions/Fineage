using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

/// <summary>
/// Continuous yaw-follow toward a live target during bow draw / magic cast.
/// Does not move actors — approach stays server <c>MoveToPawnDto</c> only.
/// Entries keyed by objectId (list-ready for other players later; today only local player).
/// Local player pauses while <see cref="PlayerController.RunningToDestination"/>.
/// </summary>
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

    [Obsolete("Use CombatPacketServices.EnsureFacing or [Inject] CombatFacingService instead")]
    public static CombatFacingService Ensure()
    {
        if (_instance != null)
        {
            return _instance;
        }

        if (App.GameContainer != null)
        {
            try
            {
                CombatFacingService resolved = App.GameContainer.Resolve<CombatFacingService>();
                if (resolved != null)
                    return resolved;
            }
            catch
            {
            }
        }

        GameObject go = new GameObject(nameof(CombatFacingService));
        CombatFacingService created = go.AddComponent<CombatFacingService>();
        App.InjectGameObject(go);
        return created;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        CombatPacketServices.Bind(facing: this);
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    public bool HasFollow(int objectId) => _entries.ContainsKey(objectId);

    /// <summary>
    /// Start / replace tracking for <paramref name="objectId"/> until <see cref="EndFollow(int)"/>.
    /// </summary>
    public void BeginFollow(int objectId, Transform actor, Transform target, int headingUnitsPerSecond = L2PawnYaw.RotationRateYaw)
    {
        if (objectId == 0 || actor == null || target == null || actor == target)
        {
            // Debug.Log(
                // $"{LogPrefix} Begin REJECTED objectId={objectId} " +
                // $"actor={(actor != null ? actor.name : "null")} " +
                // $"target={(target != null ? target.name : "null")} count={_entries.Count}");
            EndFollow(objectId, "begin-rejected");
            return;
        }

        bool replaced = _entries.ContainsKey(objectId);
        _entries[objectId] = new FacingEntry
        {
            Actor = actor,
            Target = target,
            LastTurnLogTime = 0f,
            Rate = headingUnitsPerSecond > 0 ? headingUnitsPerSecond : L2PawnYaw.RotationRateYaw
        };

        // Debug.Log(
            // $"{LogPrefix} Begin ACCEPTED objectId={objectId} " +
            // $"actor={actor.name} target={target.name} " +
            // $"replaced={replaced} count={_entries.Count}");
    }

    public void EndFollow(int objectId)
    {
        EndFollow(objectId, "explicit");
    }

    public void EndFollow(int objectId, string reason)
    {
        if (objectId == 0)
        {
            return;
        }

        if (!_entries.Remove(objectId))
        {
            return;
        }

        // Debug.Log($"{LogPrefix} End REMOVED objectId={objectId} reason={reason} count={_entries.Count}");
    }

    /// <summary>End follow for local player (convenience for current call sites).</summary>
    public void EndFollowLocal(string reason = "local")
    {
        int id = ResolveLocalObjectId();
        if (id != 0)
        {
            EndFollow(id, reason);
        }
    }

    public void EndFollowAll()
    {
        int before = _entries.Count;
        _entries.Clear();
        // Debug.Log($"{LogPrefix} End ALL cleared before={before} count=0");
    }

}
