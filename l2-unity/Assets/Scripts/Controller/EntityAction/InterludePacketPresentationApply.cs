using UnityEngine;

/// <summary>Applies data-driven packet presentations before cast/skill execution paths.</summary>
public static class InterludePacketPresentationApply
{
  public static bool TryApplyMagicSkillUse(Entity entity, MagicSkillUseDto dto)
    {
        if (entity == null || dto == null)
            return false;

        if (!InterludePacketPresentationCatalog.TryResolveMagicSkillUse(
                dto.SkillId, out InterludePresentationKind kind))
            return false;

        if (LevelUpCelebrationPresentation.TryPlay(
                entity, kind, "MagicSkillUse skillId=" + dto.SkillId))
            return true;

        Debug.LogError(
            "[PacketPresentation] MagicSkillUse skillId=" + dto.SkillId +
            " mapped kind=" + kind + " but presentation failed on entity=" + entity.name);
        return true;
    }

}