using UnityEngine;

public abstract partial class AbstractAttackEvents : StateBase
{
    private const int WOODEN_ARROW = 17;
    private const string PROJECTILE_CHAIN_LOG = "[PROJECTILE_CHAIN]";
    protected AnimationEventsBase _events;
    private Animation[] _specialsBows;
    private bool _isSubscribed;
  
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
