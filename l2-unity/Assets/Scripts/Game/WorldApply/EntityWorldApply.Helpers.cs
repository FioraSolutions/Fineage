using UnityEngine;

public abstract partial class EntityWorldApply
{
    public virtual void OnAttack(Entity attacker, Entity target, AttackDto dto)
    {
    }

    public virtual void OnMagicSkillUse(Entity entity, MagicSkillUseDto dto)
    {
    }

    public virtual void OnAutoAttackStart(Entity entity, AutoAttackStartDto dto)
    {
    }

    public virtual void OnAutoAttackStop(Entity entity, AutoAttackStopDto dto)
    {
    }

    public virtual void OnSocialAction(Entity entity, SocialActionDto dto)
    {
    }

    public virtual void OnChangeWaitType(Entity entity, ChangeWaitTypeDto dto)
    {
    }

    public virtual void OnMagicSkillCanceled(Entity entity, MagicSkillCanceledDto dto)
    {
    }

    public virtual void OnNpcHtml(Entity entity, NpcHtmlMessageDto dto)
    {
    }
}
