using UnityEngine;

public abstract partial class EntityWorldApply
{
    public virtual void OnMoveTo(Entity entity, Vector3 destination, Vector3 current, CharMoveToLocationDto dto)
    {
    }

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
}
