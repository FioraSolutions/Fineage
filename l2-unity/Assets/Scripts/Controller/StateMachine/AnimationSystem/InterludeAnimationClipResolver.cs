using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Single clip resolve path: AnimTween / skill data → exact Resources take name → load or log.
/// </summary>
public static class InterludeAnimationClipResolver
{
    static readonly HashSet<string> LoggedMissing = new HashSet<string>(StringComparer.Ordinal);

    static readonly string[] CreaturePackages =
    {
        "LineageMonsters",
        "LineageNPCs",
        "LineageNPCs4",
        "LineageNpcs",
    };

    public static string DeriveMeshAnimKey(string meshStem)
    {
        if (string.IsNullOrEmpty(meshStem))
            return null;

        string stem = meshStem;
        int m = stem.IndexOf("_m", StringComparison.OrdinalIgnoreCase);
        if (m > 0)
            stem = stem.Substring(0, m);
        return stem + "_anim";
    }

    public static string BuildResourcesPath(string folder, string meshStem, string resourcesTake)
    {
        return folder + meshStem + ".ao_" + resourcesTake;
    }

    public static AnimationClip LoadExact(string resourcesPathWithoutExtension)
    {
        if (string.IsNullOrEmpty(resourcesPathWithoutExtension))
            return null;
        return Resources.Load<AnimationClip>(resourcesPathWithoutExtension);
    }

    public static void LogMissing(string context, string resourcesPath)
    {
        if (!LoggedMissing.Add(resourcesPath ?? context))
            return;
        Debug.LogError(
            "[InterludeAnim] clip missing "
            + context
            + " expectedResources="
            + resourcesPath);
    }

    public static string BuildPlayerSpAtkQuery(int spAtkNumber, string weaponToken, string race)
    {
        string weapon = string.IsNullOrEmpty(weaponToken) ? "hand" : weaponToken.TrimStart('_').ToLowerInvariant();
        var sb = new StringBuilder();
        sb.Append("spatk");
        sb.Append(spAtkNumber.ToString("00", System.Globalization.CultureInfo.InvariantCulture));
        sb.Append('_');
        sb.Append(weapon);
        sb.Append('_');
        sb.Append(race);
        return sb.ToString();
    }

    public static AnimationClip TryLoadPlayerSpAtkCombat(
        string raceName,
        int spAtkNumber,
        string weaponToken,
        string normalizedState)
    {
        SkillAnimationDatabase.RaceClipProfile profile = SkillAnimationDatabase.ResolveRaceProfile(raceName);
        if (profile == null)
            return null;

        string meshAnim = profile.Race + "_anim";
        string query = BuildPlayerSpAtkQuery(spAtkNumber, weaponToken, profile.Race);
        AnimTweenTable table = TablePacketServices.AnimTweens;
        if (table == null || !table.TryResolveExactSequence(meshAnim, query, out string animTweenSequence))
        {
            LogMissing(
                "AnimTween sequence mesh=" + meshAnim + " query=" + query + " state=" + normalizedState,
                null);
            return null;
        }

        string take = InterludeAnimResourceTakeTable.ResourcesTakeName(meshAnim, animTweenSequence);
        string[] meshStems = SkillAnimationDatabase.MeshStemsForRaceProfile(profile);
        for (int i = 0; i < meshStems.Length; i++)
        {
            string path = BuildResourcesPath(profile.Folder, meshStems[i], take);
            AnimationClip clip = LoadExact(path);
            if (clip != null)
                return clip;
        }

        string expected = BuildResourcesPath(profile.Folder, meshStems[0], take);
        LogMissing(
            "player SpAtk animTween=" + animTweenSequence + " state=" + normalizedState,
            expected);
        return null;
    }

    public static AnimationClip TryLoadCreatureLocomotion(
        string meshStem,
        string meshAnim,
        string slotFamily,
        string weaponSuffix)
    {
        if (string.IsNullOrEmpty(meshStem) || string.IsNullOrEmpty(meshAnim))
            return null;

        AnimTweenTable table = TablePacketServices.AnimTweens;
        if (table == null
            || !table.TryResolveLocomotionSequence(meshAnim, slotFamily, weaponSuffix, out string animTweenSequence))
        {
            LogMissing(
                "AnimTween locomotion meshAnim=" + meshAnim + " slot=" + slotFamily + " weapon=" + weaponSuffix,
                null);
            return null;
        }

        string take = InterludeAnimResourceTakeTable.ResourcesTakeName(meshAnim, animTweenSequence);
        for (int p = 0; p < CreaturePackages.Length; p++)
        {
            string folder = "Data/Animations/" + CreaturePackages[p] + "/" + meshStem + "/";
            string path = BuildResourcesPath(folder, meshStem, take);
            AnimationClip clip = LoadExact(path);
            if (clip != null)
                return clip;
            folder = "Data/Animations/" + CreaturePackages[p] + "/" + meshStem + "/Model/";
            path = BuildResourcesPath(folder, meshStem, take);
            clip = LoadExact(path);
            if (clip != null)
                return clip;
        }

        string expected = BuildResourcesPath(
            "Data/Animations/LineageMonsters/" + meshStem + "/",
            meshStem,
            take);
        LogMissing(
            "creature locomotion animTween=" + animTweenSequence + " meshStem=" + meshStem,
            expected);
        return null;
    }

    /// <summary>
    /// MeshAnimation name for this pawn: race + "_anim" for players, npcgrp mesh stem for NPCs.
    /// Null when the pawn has no race and no npcgrp mesh. Callers must not substitute another package.
    /// </summary>
    public static string OwnerMeshAnimation(Entity entity)
    {
        if (entity == null)
            return null;
        if (entity is PlayerEntity || entity is UserEntity)
        {
            string race = entity.RaceId.ToString();
            if (string.IsNullOrEmpty(race))
                return null;
            return race + "_anim";
        }

        if (entity.Identity == null || entity.Identity.NpcId <= 0)
            return null;
        Npcgrp grp = TablePacketServices.NpcGrps != null
            ? TablePacketServices.NpcGrps.GetNpcgrp(entity.Identity.NpcId)
            : null;
        if (grp == null || string.IsNullOrEmpty(grp.Mesh))
            return null;

        string mesh = grp.Mesh;
        int dot = mesh.LastIndexOf('.');
        if (dot >= 0 && dot < mesh.Length - 1)
            mesh = mesh.Substring(dot + 1);
        int slash = Math.Max(mesh.LastIndexOf('/'), mesh.LastIndexOf('\\'));
        if (slash >= 0 && slash < mesh.Length - 1)
            mesh = mesh.Substring(slash + 1);
        return DeriveMeshAnimKey(mesh);
    }
}