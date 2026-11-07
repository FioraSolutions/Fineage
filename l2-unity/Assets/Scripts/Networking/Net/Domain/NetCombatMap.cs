using System.Collections.Generic;
using Fineage.Net.Contract;

/// <summary>Fineage Net combat/status wire → Interlude-shaped combat DTOs.</summary>
public static class NetCombatMap
{
    public static AttackDto ToAttackDto(NetAttack attack)
    {
        var dto = new AttackDto();
        dto.InitFromNet(
            attack.AttackerObjectId,
            attack.TargetObjectId,
            attack.Damage,
            attack.Flags,
            attack.AttackerX,
            attack.AttackerY,
            attack.AttackerZ,
            attack.TargetX,
            attack.TargetY,
            attack.TargetZ);
        return dto;
    }

    public static List<StatusUpdate.Attribute> ToStatusAttributes(NetStatusUpdate status)
    {
        var list = new List<StatusUpdate.Attribute>(status.AttributeIds.Length);
        for (int i = 0; i < status.AttributeIds.Length; i++)
            list.Add(new StatusUpdate.Attribute(status.AttributeIds[i], status.Values[i]));
        return list;
    }

    public static AbnormalStatusUpdateDto ToAbnormalStatusUpdateDto(NetAbnormalStatusUpdate update) =>
        AbnormalStatusUpdateDto.FromZoneEffects(update.Effects);

    public static DieDto ToDieDto(NetDie die) => DieDto.FromZoneWire(die.ObjectId);

    public static ReviveDto ToReviveDto(NetRevive revive) => new ReviveDto { ObjectId = revive.ObjectId };

    public static TeleportToLocationDto ToTeleportToLocationDto(NetTeleportToLocation teleport) =>
        TeleportToLocationDto.FromZoneWire(teleport.ObjectId, teleport.X, teleport.Y, teleport.Z, teleport.Heading);

    public static SkillListDto ToSkillListDto(NetSkillEntry[] rows)
    {
        var dto = new SkillListDto();
        if (rows == null)
            return dto;
        for (int i = 0; i < rows.Length; i++)
        {
            NetSkillEntry row = rows[i];
            bool passive = IsPassiveSkill(row.SkillId, row.Level);
            bool disabled = row.ReuseDelayMs > 0;
            dto.Skills.Add(new SkillInstance(row.SkillId, row.Level, passive, disabled));
        }

        return dto;
    }

    public static MagicSkillUseDto ToMagicSkillUseDto(NetMagicSkillUse use)
    {
        var dto = new MagicSkillUseDto();
        dto.InitFromNet(
            use.CasterObjectId,
            use.TargetObjectId,
            use.SkillId,
            use.SkillLevel,
            use.HitTimeMs,
            use.ReuseDelayMs);
        return dto;
    }

    static bool IsPassiveSkill(int skillId, int level)
    {
        Skillgrp skill = SkillgrpTable.Instance != null ? SkillgrpTable.Instance.GetSkill(skillId, level) : null;
        return skill != null && skill.OperateType == 2;
    }
}
