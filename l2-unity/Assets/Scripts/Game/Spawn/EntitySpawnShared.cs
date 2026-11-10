using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class EntitySpawnShared
{
    /// <summary>npcIds already reported as unresolvable, so a repeated NpcInfo does not spam logs.</summary>
    static readonly HashSet<int> ReportedUnresolvedNpcIds = new HashSet<int>();
    const int MaxUnresolvedReports = 20;
    static bool _unresolvedReportsSuppressed;

    public static bool TryResolveNpc(
        EntityIdentity identity,
        NpcgrpTable npcGrps,
        NpcNameTable npcNames,
        ModelTable models,
        out NpcSpawnRequest request)
    {
        request = null;
        if (identity == null)
            return false;

        npcGrps = npcGrps != null ? npcGrps : TablePacketServices.NpcGrps;
        npcNames = npcNames != null ? npcNames : TablePacketServices.NpcNames;
        models = models != null ? models : TablePacketServices.Models;

        int visualNpcId = identity.NpcId;
        if (npcGrps.GetNpcgrp(visualNpcId) == null)
            visualNpcId = PetVisuals.VisualNpcId(identity.NpcId);

        Npcgrp npcgrp = npcGrps.GetNpcgrp(visualNpcId);
        NpcName npcName = npcNames.GetNpcName(identity.NpcId);
        if (npcName == null)
            npcName = npcNames.GetNpcName(visualNpcId);
        if (npcName == null || npcgrp == null)
        {
            ReportUnresolved(
                identity.NpcId,
                "no entry in Npcgrp_Classic.txt / NpcName_Classic-eu.txt");
            return false;
        }

        GameObject prefab = models.GetNpc(npcgrp.Mesh);
        string usedMesh = npcgrp.Mesh;
        if (prefab == null)
        {
            bool resolved = IsMonsterMesh(npcgrp.Mesh)
                ? TryResolveFallbackMonsterPrefab(models, npcgrp.Mesh, out prefab, out usedMesh)
                : TryResolveFallbackNpcPrefab(models, npcgrp.Mesh, out prefab, out usedMesh);
            if (!resolved)
            {
                ReportUnresolved(
                    identity.NpcId,
                    "mesh '" + npcgrp.Mesh + "' has no prefab at Resources/Data/Animations/"
                    + npcgrp.Mesh.Replace('.', '/')
                    + " and no fallback mesh resolved");
                return false;
            }
            ReportSubstituted(identity.NpcId, npcgrp.Mesh, usedMesh);
        }

        // Wire Attackable (RememberNpc from ApplyNpcSpawn) wins over prefab MonsterEntity /
        // NpcEntity — Folk on monster meshes stay NPC; attackable mobs stay Monster.
        EntityType fromPrefab = ResolveEntityType(prefab, npcgrp.ClassName);
        identity.EntityType = NetNpcSession.IsAttackable(
            identity.Id,
            fromPrefab == EntityType.Monster)
            ? EntityType.Monster
            : EntityType.NPC;

        request = new NpcSpawnRequest
        {
            Identity = identity,
            Npcgrp = npcgrp,
            NpcName = npcName,
            Prefab = prefab
        };
        return true;
    }

    /// <summary>
    /// One line per npcId when the client cannot build a spawn request. Without this the NPC
    /// silently disappears, which looks identical to the server never sending NpcInfo.
    /// </summary>
    static void ReportUnresolved(int npcId, string reason)
    {
        if (!ReportedUnresolvedNpcIds.Add(npcId))
            return;

        if (_unresolvedReportsSuppressed)
            return;
        if (ReportedUnresolvedNpcIds.Count > MaxUnresolvedReports)
        {
            _unresolvedReportsSuppressed = true;
            FineageLog.Warn(
                FineageLogCategory.World,
                "NpcInfo unresolved reports capped at " + MaxUnresolvedReports
                + " distinct npcIds; further ids are not logged.");
            return;
        }

        FineageLog.Warn(
            FineageLogCategory.World,
            "NpcInfo npcId=" + npcId + " not rendered: " + reason);
    }

    /// <summary>
    /// Reported once per substituted mesh so a missing asset pack variant is visible in the
    /// log instead of being silently papered over.
    /// </summary>
    static void ReportSubstituted(int npcId, string wanted, string used)
    {
        string key = wanted + " -> " + used;
        if (!ReportedSubstitutedMeshes.Add(key))
            return;
        // Art-pack gap (prefab absent under Resources/Data/Animations/...), not a broken texture.
        // Once per wanted→used pair per play session so enter-world knownlist does not flood the console.
        FineageLog.Info(
            FineageLogCategory.World,
            "NpcInfo npcId=" + npcId + " art pack missing mesh '" + wanted
            + "'; stand-in '" + used + "'");
    }
}
