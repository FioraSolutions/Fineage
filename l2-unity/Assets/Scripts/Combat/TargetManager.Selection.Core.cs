using System;
using UnityEngine;
using VContainer;

public partial class TargetManager
{
    public void SetTarget(ObjectData target , string hexColor)
    {
        if (target == null)
        {
            ClearTarget();
            return;
        }

        // New select is Target (blue) until AttackDto/AutoAttackStart marks attack target again.
        int newId = -1;
        Entity newEntity = target.Entity;
        if (newEntity == null && target.ObjectTransform != null)
        {
            newEntity = target.ObjectTransform.GetComponent<Entity>();
        }

        if (newEntity != null && newEntity.Identity != null)
        {
            newId = newEntity.Identity.Id;
        }

        if (_target != null && _target.Identity != null && _target.Identity.Id != newId)
        {
            ClearAttackTarget();
        }

        _target = new TargetData(target);
        _target.SetColor(hexColor);

        DomainPacketServices.LocalPlayer.TargetId = _target.Identity.Id;
        DomainPacketServices.LocalPlayer.Target = _target.Data.ObjectTransform;
    }

    public void SetAttackTarget()
    {
        _attackTarget = _target;
    }

    public void ClearAttackTarget()
    {
        _attackTarget = null;
    }

    public bool IsAttackTargetSet()
    {
        if (_target == null || _attackTarget == null)
        {
            return false;
        }

        if (_target.Identity == null || _attackTarget.Identity == null)
        {
            return _target == _attackTarget;
        }

        return _target.Identity.Id == _attackTarget.Identity.Id;
    }
}
