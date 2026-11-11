using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public sealed partial class CombatFacingService : MonoBehaviour
{
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
}
