using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public partial class EffectManager
{
    void BuildRegistry()
    {
        _byId.Clear();
        _byComment.Clear();
        _retainedAssets.Clear();
        _registryBuilt = false;

        if (database == null || database.effects == null)
        {
            Debug.LogError("[EffectManager] BuildRegistry: database missing");
            return;
        }

        for (int i = 0; i < database.effects.Count; i++)
        {
            EffectDatabase.EffectData row = database.effects[i];
            if (row == null)
                continue;

            // Weapon-shot rows resolve prefab at play time from weapon_shot.int.
            if (!row.useWeaponShot && row.prefab == null)
            {
                Debug.LogError(
                    "[EffectManager] BuildRegistry missing prefab id=" + row.id +
                    " comment=" + row.comment);
                continue;
            }

            var entry = new RuntimeEffect
            {
                Id = row.id,
                Prefab = row.prefab,
                Settings = row.settings,
                UseWeaponShot = row.useWeaponShot,
                SpiritShot = row.spiritShot,
                IgnoreFlightTimeForCast = row.ignoreFlightTimeForCast,
                Comment = row.comment,
            };
            _byId[row.id] = entry;

            if (!string.IsNullOrEmpty(row.comment) && !_byComment.ContainsKey(row.comment))
                _byComment[row.comment] = entry;

            if (row.prefab != null)
            {
                _retainedAssets.Add(row.prefab);
                if (row.prefab.gameObject != null)
                    _retainedAssets.Add(row.prefab.gameObject);
                row.prefab.CollectReferencedPrefabs(_retainedAssets);
            }

            if (row.settings != null)
                _retainedAssets.Add(row.settings);
        }

        _registryBuilt = _byId.Count > 0;
    }

    void EnsureRegistry()
    {
        if (!_registryBuilt)
            BuildRegistry();
    }

    bool TryGetRuntime(int id, out RuntimeEffect entry)
    {
        EnsureRegistry();
        return _byId.TryGetValue(id, out entry);
    }

    /// <summary>
    /// Session flag from GlobalEffect.ignoreFlightTimeForCast (not live SO read).
    /// </summary>

    public bool ShouldIgnoreFlightTimeForCast(int effectId)
    {
        return TryGetRuntime(effectId, out RuntimeEffect entry) && entry.IgnoreFlightTimeForCast;
    }

    /// <summary>
    /// Exact comment match against the session registry (EnterEventgrp keys).
    /// </summary>

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

    /// <summary>
    /// Debug helper: rows whose comment or prefab name contains <paramref name="token"/>.
    /// </summary>

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
