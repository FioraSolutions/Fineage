using System.Collections.Generic;
using UnityEngine;
using VContainer;

public sealed partial class CombatFacingService
{
    private void Update()
    {
        if (_entries.Count == 0)
        {
            return;
        }

        int localId = ResolveLocalObjectId();
        PlayerController player = Player;
        bool localMoving = player != null && player.RunningToDestination;

        // Snapshot keys — ApplyYaw may write entry back; shoot/Exit may EndFollow mid-frame.
        _iterateIds.Clear();
        foreach (int id in _entries.Keys)
        {
            _iterateIds.Add(id);
        }

        _removeIds.Clear();

        for (int i = 0; i < _iterateIds.Count; i++)
        {
            int objectId = _iterateIds[i];
            if (!_entries.TryGetValue(objectId, out FacingEntry entry))
            {
                continue;
            }

            if (entry.Actor == null || entry.Target == null)
            {
                _removeIds.Add(objectId);
                continue;
            }

            // Only local player's move-path owns rotation while running.
            if (objectId == localId && localMoving)
            {
                continue;
            }

            ApplyYaw(objectId, ref entry);
            if (_entries.ContainsKey(objectId))
            {
                _entries[objectId] = entry;
            }
        }

        for (int i = 0; i < _removeIds.Count; i++)
        {
            int rid = _removeIds[i];
            if (_entries.Remove(rid))
            {
                // Debug.Log(
                    // $"{LogPrefix} End REMOVED objectId={rid} reason=null-actor-or-target count={_entries.Count}");
            }
        }
    }

    private void ApplyYaw(int objectId, ref FacingEntry entry)
    {
        Transform actor = entry.Actor;
        Transform target = entry.Target;

        Vector3 dir = target.position - actor.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f)
        {
            return;
        }

        L2PawnYawDrive drive = actor.GetComponent<L2PawnYawDrive>();
        if (drive != null)
            drive.Stop();

        float desired = Quaternion.LookRotation(dir).eulerAngles.y;
        float current = actor.eulerAngles.y;
        if (_pawnYaw == null)
            return;
        float maxStep = _pawnYaw.DegreesPerSecond(entry.Rate) * Time.deltaTime;
        float next = _pawnYaw.StepDegrees(current, desired, maxStep);
        actor.rotation = Quaternion.Euler(0f, next, 0f);
        float angle = Mathf.Abs(Mathf.DeltaAngle(current, desired));

        float now = Time.unscaledTime;
        if (now - entry.LastTurnLogTime >= TurnLogIntervalSeconds)
        {
            entry.LastTurnLogTime = now;
            // Debug.Log(
                // $"{LogPrefix} Turn objectId={objectId} " +
                // $"actor={actor.name} target={target.name} angle={angle:F1} count={_entries.Count}");
        }
    }

    private static int ResolveLocalObjectId()
    {
        if (DomainPacketServices.LocalPlayer == null ||
            DomainPacketServices.LocalPlayer.Identity == null)
        {
            return 0;
        }

        return DomainPacketServices.LocalPlayer.Identity.Id;
    }

    /// <summary>
    /// Equipped bow via appearance hands, fallback anim name contains "bow".
    /// </summary>
    public static bool IsUsingBow(Entity entity)
    {
        if (entity is PlayerEntity player)
            return IsPlayerUsingBow(player);

        if (entity != null && entity.Appearance != null && Weapons != null)
        {
            if (IsBowItemId(entity.Appearance.RHand) || IsBowItemId(entity.Appearance.LHand))
                return true;
        }

        string anim = null;
        UserEntity user = entity as UserEntity;
        if (user != null)
            anim = user.WeaponAnim;
        else if (entity != null && entity.Gear != null)
            anim = entity.Gear.WeaponAnim;

        return !string.IsNullOrEmpty(anim) &&
               anim.IndexOf("bow", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public static bool IsPlayerUsingBow(PlayerEntity player)
    {
        if (player == null)
        {
            return false;
        }

        if (player.Appearance != null && Weapons != null)
        {
            if (IsBowItemId(player.Appearance.RHand) || IsBowItemId(player.Appearance.LHand))
            {
                return true;
            }
        }

        string anim = player.GetCurrentAnimName();
        return !string.IsNullOrEmpty(anim) &&
               anim.IndexOf("bow", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static bool IsBowItemId(int itemId)
    {
        if (itemId == 0)
        {
            return false;
        }

        Weapongrp weapon = Weapons.GetWeapon(itemId);
        return weapon != null && weapon.WeaponType == WeaponType.bow;
    }
}
