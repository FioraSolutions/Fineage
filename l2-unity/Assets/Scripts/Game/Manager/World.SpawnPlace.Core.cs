using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;
using System.Security.Principal;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements.Experimental;
using System.Collections;

public partial class World
{
    public void PickupGroundItem(int itemObjectId, int pickerCharObjId, Vector3 unityPos)
    {
        if (_itemDropPresentation != null)
            _itemDropPresentation.PlayPickup(itemObjectId, pickerCharObjId, unityPos);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // DropMeshLifetimeLog.NotifyRemove(itemObjectId, "GetItem_pickup");
#endif
        RemoveObject(itemObjectId);

        // Let authored PicItem finish before forcing gait (do not stretch the clip).
        IMoveRoster moves = Moves();
        if (pickerCharObjId != 0 && moves != null && moves.IsMoving(pickerCharObjId))
        {
            StartCoroutine(ResumeGaitAfterPickup(pickerCharObjId));
        }
    }

    System.Collections.IEnumerator ResumeGaitAfterPickup(int pickerCharObjId)
    {
        yield return new WaitForSeconds(AnimPlayback.PickupFallbackSeconds);
        IMoveRoster moves = Moves();
        if (moves == null || !moves.IsMoving(pickerCharObjId))
            yield break;

        MovementData move = MoveAllCharactersPacketServices.Moves.GetMove(pickerCharObjId);
        if (move == null || move.GetEntity() == null)
            yield break;

        move.ForceReplayUserGait();
    }

    public void SpawnPlayer(EntityIdentity identity, PlayerStatus status, PlayerStats stats, PlayerAppearance appearance)
    {
        if (_playerSpawner != null)
            _playerSpawner.Spawn(identity, status, stats, appearance, this);
    }

    public void SpawnNpc(EntityIdentity identity, NpcStatusInterlude status, Stats stats)
    {
        SpawnNpc(identity, status, stats, out _);
    }
}
