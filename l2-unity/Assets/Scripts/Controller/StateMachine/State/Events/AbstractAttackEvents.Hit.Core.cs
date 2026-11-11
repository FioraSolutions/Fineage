using UnityEngine;

public abstract partial class AbstractAttackEvents
{
    protected void RegisterSwordCollision(PlayerEntity entity)
    {
        if (entity == null) return;

        Transform[] swordBasePoints = entity.GetSwordBasePoints();

        if (swordBasePoints != null && swordBasePoints.Length > 1)
        {
            Transform swordBase = swordBasePoints[0];
            Transform swordTip = swordBasePoints[1];
            Entity targetEntity = entity.GetTargetEntity();
            Transform target = targetEntity != null ? targetEntity.transform : DomainPacketServices.LocalPlayer.Target;
            int attackerEntityId = entity.Identity != null ? entity.Identity.Id : 0;
            int targetEntityId = targetEntity != null && targetEntity.Identity != null ? targetEntity.Identity.Id : 0;
            SwordCollisionService collision = SwordCollisionPacketServices.Collision;
            if (collision != null)
                collision.RegisterSwordByEntityId(attackerEntityId, targetEntityId, swordBase, swordTip, target, 0);
        }
        else
        {
            // Debug.LogWarning($"[ATK_HIT_CHAIN] RegisterSword SKIP — no sword points on player");
        }
    }
}
