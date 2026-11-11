using UnityEngine;
using System;

public abstract partial class AbstractAttackEvents
{
    private void CallBackAnimationFinish(string animName)
    {
        foreach (Animation special in _specialsBows)
        {
            if (animName != special.ToString())
            {
                continue;
            }

            if (special.Type == TypesAnimation.MagicAttack && special.Phase != MagicPhase.End)
            {
                return;
            }

            // Melee / SpAtk / Magic return is owned by SMB SwitchToIdle (not Complete).
            // A second WAIT_RETURN CrossFades mid-transition and can freeze the Animator.
            if (special.Type == TypesAnimation.MeleeAttack ||
                special.Type == TypesAnimation.MagicAttack ||
                (!string.IsNullOrEmpty(animName) &&
                 animName.StartsWith("SpAtk", System.StringComparison.Ordinal)))
            {
                return;
            }

            PlayerStatePacketServices.PlayerState.ChangeIntention(Intention.INTENTION_IDLE);
            PlayerStatePacketServices.PlayerState.NotifyEvent(Event.WAIT_RETURN);
            break;
        }
    }

    private void CallBackFinishedHit(string animName)
    {
        if (!IsMeleeAttackShotAnim(animName))
        {
            return;
        }

        Transform[] swordBasePoints = _stateMachine.Player.GetSwordBasePoints();
        if (swordBasePoints != null && swordBasePoints.Length > 1 &&
            SwordCollisionPacketServices.Collision != null)
        {
            SwordCollisionPacketServices.Collision.UnregisterSword(swordBasePoints[0]);
        }

        DomainPacketServices.LocalPlayer.RemoveProceduralPose();
    }

    private void IfMonsterDead(Entity target)
    {
        if (target == null) return;


        if (target != null & target is MonsterEntity)
        {
            MonsterEntity monsterEntity = (MonsterEntity)target;
            //Debug.Log("Попали и увидели что монстр уже должен быть мертвым hp  " + monsterEntity.Hp() + " RemainingHP " + monsterEntity.CalculateRemainingHp());

            if (monsterEntity.IsDead() || monsterEntity.CalculateRemainingHp() <= 0)
            {
                monsterEntity.SetDead(true);
                if (EntityActionPacketServices.Actions != null)
                    EntityActionPacketServices.Actions.Die(monsterEntity, false, "player_hit_hp0");
            }


        }
    }
}
