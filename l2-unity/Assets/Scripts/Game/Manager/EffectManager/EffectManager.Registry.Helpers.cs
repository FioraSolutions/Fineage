using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public partial class EffectManager
{
    public bool ShouldIgnoreFlightTimeForCast(int effectId)
    {
        return TryGetRuntime(effectId, out RuntimeEffect entry) && entry.IgnoreFlightTimeForCast;
    }

    /// <summary>
    /// False only for a V2 composite that authors no flying part (see <see cref="CompositeEffectV2.HasFlightStage"/>).
    /// Legacy / non-composite effects and missing rows keep the old distance-based flight timing.
    /// </summary>
    public bool HasFlightStage(int effectId)
    {
        if (!TryGetRuntime(effectId, out RuntimeEffect entry) || entry.UseWeaponShot)
            return true;

        CompositeEffectV2 v2 = entry.Prefab as CompositeEffectV2;
        return v2 == null || v2.HasFlightStage;
    }

    public bool TryGetByComment(
        string comment,
        out int id,
        out BaseEffect prefab,
        out EffectSettings settings)
    {
        id = 0;
        prefab = null;
        settings = null;
        if (string.IsNullOrEmpty(comment))
            return false;

        EnsureRegistry();
        if (!_byComment.TryGetValue(comment, out RuntimeEffect entry) || entry.Prefab == null)
            return false;

        id = entry.Id;
        prefab = entry.Prefab;
        settings = entry.Settings;
        return true;
    }

    public int CountCommentOrPrefabMatches(string token)
    {
        if (string.IsNullOrEmpty(token))
            return 0;

        EnsureRegistry();
        int count = 0;
        foreach (RuntimeEffect entry in _byId.Values)
        {
            string prefabName = entry.Prefab != null ? entry.Prefab.name : string.Empty;
            string comment = entry.Comment != null ? entry.Comment : string.Empty;
            if (prefabName.IndexOf(token, System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                comment.IndexOf(token, System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                count++;
            }
        }

        return count;
    }
}
