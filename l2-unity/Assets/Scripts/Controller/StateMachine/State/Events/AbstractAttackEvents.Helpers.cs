using UnityEngine;

public abstract partial class AbstractAttackEvents : StateBase
{
    public override void Exit()
    {
        base.Exit();
        CombatPacketServices.Facing?.EndFollow(ResolveFacingObjectId(), "state-exit");
        if (!_isSubscribed) return;
        _isSubscribed = false;

        if (_events != null)
        {
            _events.OnAnimationFinished -= CallBackAnimationFinish;
            _events.OnAnimationStartShoot -= CallBackStartShoot;
            _events.OnAnimationFinishedHit -= CallBackFinishedHit;
            _events.OnAnimationStartLoadArrow -= CallBackLoadArrow;
            _events.OnAnimationAttackShot -= CallBackAttackShot;
        }

        SkillExecutor executor = SkillExecutorPacketServices.Executor;
        if (executor != null)
            executor.OnAllAnimationFinished -= OnAllAnimationFinishedFromExecutor;

        if (ProjectilePacketServices.Projectiles != null)
        {
            ProjectilePacketServices.Projectiles.OnHitEffectProjectile -= OnHitEffectProjectile;
        }
        if (SwordCollisionPacketServices.Collision != null)
        {
            SwordCollisionPacketServices.Collision.OnHitCollider -= OnHitColliderMonster;
        }
    }

    private int ResolveFacingObjectId()
    {
        if (_stateMachine != null)
        {
            return _stateMachine.GetObjectId();
        }

        if (DomainPacketServices.LocalPlayer != null && DomainPacketServices.LocalPlayer.Identity != null)
        {
            return DomainPacketServices.LocalPlayer.Identity.Id;
        }

        return 0;
    }

    private void OnAllAnimationFinishedFromExecutor(AnimationEventsBase actions)
    {
        if (actions == null || _events == null || actions != _events)
        {
            return;
        }

        if (_stateMachine != null && _stateMachine.Player != null)
        {
            int objectId = _stateMachine.Player.Identity.Id;
            ControllerPacketServices.Animations.ResetPlayerAnimatorSpeed(objectId);
        }

        MagicSkillPresentation.EndCastChannel();
        // Pose return is owned by combat SMB (SpAtk / Magic / jatk SwitchToIdle).
    }
}
