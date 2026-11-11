using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public sealed partial class CombatFacingService : MonoBehaviour
{
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
