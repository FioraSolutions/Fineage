using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public static partial class EntitySpawnShared
{
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
}
