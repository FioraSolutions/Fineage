using UnityEngine;

public abstract class EntityWorldApply
{
    public virtual void OnMoveTo(Entity entity, Vector3 destination, Vector3 current, CharMoveToLocationDto dto)
    {
    }

    /// <summary>
    /// Server PositionCorrection: snap the entity to the authoritative point.
    /// Must not go through <see cref="OnMoveTo"/> (dest==origin fights approach locomotion).
    /// </summary>
    public virtual void OnPositionCorrection(Entity entity, Vector3 position)
    {
        SnapTransform(entity, position);
    }

    public static void SnapTransform(Entity entity, Vector3 position)
    {
        if (entity == null)
            return;
        CharacterController cc = entity.GetComponent<CharacterController>();
        if (cc != null && cc.enabled)
            cc.enabled = false;
        entity.transform.position = position;
        if (cc != null)
            cc.enabled = true;
    }

    public virtual void OnStopMove(Entity entity, StopMoveDto dto)
    {
    }

    public virtual void OnMoveToPawn(Entity entity, MoveToPawnDto dto)
    {
    }

    public virtual void OnDie(Entity entity, DieDto dto)
    {
    }

    public virtual void OnRevive(Entity entity, ReviveDto dto)
    {
    }

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
