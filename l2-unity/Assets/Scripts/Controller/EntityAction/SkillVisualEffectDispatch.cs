using UnityEngine;

/// <summary>
/// Every physical and magical cast, for players and NPCs, asks <see cref="SkillVisualCatalog"/>.
/// skillgrp desc, otherwise the skill.usk row. Skills with neither play ani_char and sound only.
/// </summary>
public static class SkillVisualEffectDispatch
{
    public static void PlayIfKnown(Entity entity, MagicSkillUseDto magic, AnimationCombo combo = null)
    {
        if (entity == null || magic == null)
            return;

        int targetId = ResolveEffectTargetId(entity, magic);
        MagicCastData cast = WithTarget(entity.GetMagicCastData(), targetId);
        Play(entity, magic.SkillId, magic.SkillLvl, cast, targetId);
    }

    public static void PlayFromSkill(Entity entity, Skillgrp skill, MagicCastData cast, AnimationCombo combo)
    {
        if (entity == null || skill == null)
            return;

        int packetTarget = cast != null ? cast.TargetObjectId : 0;
        int targetId = ResolveEffectTargetId(entity, packetTarget, skill);
        Play(entity, skill.Id, skill.Level, WithTarget(cast, targetId), targetId);
    }

    public static int ResolveEffectTargetId(Entity entity, MagicSkillUseDto magic)
    {
        if (entity == null || entity.Identity == null || magic == null)
            return 0;

        Skillgrp grp = magic.SkillGrp;
        if (grp == null && TablePacketServices.SkillGrps != null)
            grp = TablePacketServices.SkillGrps.GetSkill(magic.SkillId, magic.SkillLvl);
        return ResolveEffectTargetId(entity, magic.TargetId, grp);
    }

    static void Play(Entity entity, int skillId, int level, MagicCastData cast, int targetId)
    {
        SkillVisualCatalog catalog;
        if (!SkillVisualRuntime.TryCatalog(out catalog))
            return;

        SkillVisualResolution resolution = catalog.Resolve(skillId, level, HasPrefab);
        SkillVisualRuntime.ReportOfficialFlightGap(catalog, resolution);
        SkillVisualPlayback.Play(entity, resolution, cast, targetId);
    }

    static bool HasPrefab(string assetKey)
    {
        BaseEffect prefab;
        string path;
        return SkillVisualRuntime.TryLoadPrefab(assetKey, out prefab, out path);
    }

    static int ResolveEffectTargetId(Entity entity, int packetTargetId, Skillgrp grp)
    {
        if (entity == null || entity.Identity == null)
            return 0;

        int casterId = entity.Identity.Id;
        if (packetTargetId == casterId)
            return casterId;
        if (grp != null && grp.CastRange < 0)
            return casterId;
        return packetTargetId;
    }

    static MagicCastData WithTarget(MagicCastData cast, int targetObjectId)
    {
        if (cast == null || targetObjectId <= 0)
            return cast;
        cast.TargetObjectId = targetObjectId;
        return cast;
    }
}
