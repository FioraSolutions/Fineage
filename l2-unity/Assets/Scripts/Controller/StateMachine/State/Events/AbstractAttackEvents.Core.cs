using UnityEngine;

public abstract partial class AbstractAttackEvents : StateBase
{
    public AbstractAttackEvents(int objectId , Animation[] specialsBows, PlayerStateMachine stateMachine = null ) : base(stateMachine)
    {
        _specialsBows = specialsBows;
        _events = ControllerPacketServices.Animations.GetAnimationEvents(objectId);
    }

    public override void Enter()
    {
        base.Enter();
        if (_isSubscribed) return;
        _isSubscribed = true;

        if (_events != null)
        {
            _events.OnAnimationFinished += CallBackAnimationFinish;
            _events.OnAnimationStartShoot += CallBackStartShoot;
            _events.OnAnimationFinishedHit += CallBackFinishedHit;
            _events.OnAnimationStartLoadArrow += CallBackLoadArrow;
            // Melee Hit/SoulShot: AttackShot anim event (not wall-clock).
            // Bow Soulshot/stick: HitManager subscribes to ProjectileManager.OnHitMonster (not AttackDto lifecycle).
            _events.OnAnimationAttackShot += CallBackAttackShot;
        }

        if (ProjectilePacketServices.Projectiles != null)
        {
            SkillExecutor executor = SkillExecutorPacketServices.Executor;
            if (executor != null)
                executor.OnAllAnimationFinished += OnAllAnimationFinishedFromExecutor;
            // EffectOnly magic chain only — bow ArrowStick is HitManager ← OnHitMonster (persistent).
            ProjectilePacketServices.Projectiles.OnHitEffectProjectile += OnHitEffectProjectile;
        }
        if (SwordCollisionPacketServices.Collision != null)
        {
            SwordCollisionPacketServices.Collision.OnHitCollider += OnHitColliderMonster;
        }
    }
}
