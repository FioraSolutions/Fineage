

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
        if (swordBasePoints != null && swordBasePoints.Length > 1)
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

    private void CallBackStartShoot(string animName)
    {
        // Bow / magic: release aim-follow at projectile launch (not during flight).
        CombatPacketServices.Facing?.EndFollow(ResolveFacingObjectId(), "shoot");
        // ReplayAttackTrace.Write(
            // "SHOOT_EVENT",
            // DomainPacketServices.LocalPlayer,
            // DomainPacketServices.LocalPlayer != null ? DomainPacketServices.LocalPlayer.GetTargetEntity() : null,
            // "anim=" + animName);

        foreach (Animation special in _specialsBows)
        {
            if (animName == special.ToString())
            {

                if (special.Type == TypesAnimation.BowAttack)
                {
                    Entity attacker = DomainPacketServices.LocalPlayer;
                    // UnityReplaySession.OnBowShot(
                        // attacker,
                        // attacker != null ? attacker.GetTargetEntity() : null,
                        // attacker != null ? attacker.GetPositionRightHand() : Vector3.zero);
                    // SpAtk01_bow / jatk*_bow: same ArrowStick path. Ensure wooden arrow exists
                    // if LoadArrow notify was skipped (state exited early before equip).
                    if (DomainPacketServices.LocalPlayer.GetGoEtcItem() == null)
                    {
                        DomainPacketServices.LocalPlayer.EquipArrow(WOODEN_ARROW);
                    }

                    GameObject go = DomainPacketServices.LocalPlayer.GetGoEtcItem();
                    Transform target = DomainPacketServices.LocalPlayer.Target;

                    if (DomainPacketServices.LocalPlayer == null ||
                        go == null ||
                        target == null)
                    {
                        // Debug.LogError(
                            // $"NewAttackState->CallBackStartShoot: missing arrow/target anim={animName} " +
                            // $"go={(go != null)} target={(target != null)}");
                        return;
                    }

                    Vector3 startPos = DomainPacketServices.LocalPlayer.GetPositionRightHand();
                    Vector3 aimPos = VectorWorldUtils.GetCollision(startPos, target);
                    float dist3d = Vector3.Distance(startPos, aimPos);
                    // ANProjectile NArrow / s_u003_d: dirMul=3000, fly=sqrt(2·Dist/3000), path (t/T)².
                    float flyAccel = ProjectileFlightTimeCalculator.CalculateL2ArrowFlightTimeSeconds(dist3d);
                    float avgSpeed = dist3d / Mathf.Max(flyAccel, 0.05f);

                    ProjectileData settings = new ProjectileData(go, target, startPos, target);
                    settings.impactType = ProjectileImpactType.ArrowStick;
                    settings.speed = avgSpeed;
                    settings.flytime = flyAccel;
                    settings.lifetime = flyAccel;

                    bool isSpAtkBow = animName.IndexOf("SpAtk", System.StringComparison.OrdinalIgnoreCase) >= 0;
                    // Debug.Log(
                        // $"[BOW_ARROW] SHOOT anim={animName} spAtkBow={isSpAtkBow} " +
                        // $"dist3d={dist3d:F3} flyAccel={flyAccel:F3}s avgSpeed={avgSpeed:F3} " +
                        // $"uuAccel=3000 path=(t/T)^2 (compare vs decompile ANSkillProjectileTick.log)");

                    ProjectilePacketServices.Projectiles.LaunchProjectile(go, startPos, target, settings);
                    break;
                }

            }
        }
    }

    private void CallBackLoadArrow(string animName)
    {
        DomainPacketServices.LocalPlayer.EquipArrow(WOODEN_ARROW);
    }

    /// <summary>
    /// L2-like AttackShot: fire melee Hit/SoulShot when clip notify fires.
    /// Accepts jatk* and SpAtk* melee (_1HS / _2HS / _dual / _pole).
    /// Bow: StartShoot + ArrowStickTimeHit (end of flytime / Hit Time), not this path.
    /// </summary>

    private void CallBackAttackShot(string animName)
    {
        if (!IsMeleeAttackShotAnim(animName))
        {
            // Debug.Log(
                // $"[HIT_FX] 2.CallBackAttackShot SKIP not-melee-attack-shot frame={Time.frameCount} anim={animName}");
            return;
        }

        PlayerEntity player = _stateMachine != null ? _stateMachine.Player : null;
        if (player == null || SwordCollisionPacketServices.Collision == null)
        {
            // Debug.LogWarning(
                // $"[HIT_FX] 2.CallBackAttackShot SKIP playerNull={player == null} " +
                // $"swordSvcNull={SwordCollisionPacketServices.Collision == null} anim={animName}");
            return;
        }

        Transform[] swordBasePoints = player.GetSwordBasePoints();
        if (swordBasePoints == null || swordBasePoints.Length <= 1)
        {
            // Debug.LogWarning($"[HIT_FX] 2.CallBackAttackShot SKIP no sword points anim={animName}");
            return;
        }

        Entity targetEntity = player.GetTargetEntity();
        Transform target = targetEntity != null ? targetEntity.transform : player.Target;
        int attackerEntityId = player.Identity != null ? player.Identity.Id : 0;
        int targetEntityId = targetEntity != null && targetEntity.Identity != null
            ? targetEntity.Identity.Id
            : 0;

        if (target == null)
        {
            // Debug.LogWarning(
                // $"[HIT_FX] 2.CallBackAttackShot SKIP target=null anim={animName} " +
                // $"attackerId={attackerEntityId} targetEntityId={targetEntityId}");
            return;
        }

        // Debug.Log(
            // $"[HIT_FX] 2.CallBackAttackShot OK frame={Time.frameCount} t={Time.time:F3} " +
            // $"anim={animName} attackerId={attackerEntityId} targetId={targetEntityId} " +
            // $"target={target.name} → EmitHitFromAttackShot");

        var hits = CombatPacketServices.Hits;
        if (hits != null)
            hits.FireRememberedExtraHits(player);

        if (SwordCollisionPacketServices.Collision != null &&
            attackerEntityId > 0 &&
            player.Stats != null)
        {
            float serverCycleMs = AttackTimingHelper.ResolveServerLikeAttackDurationMs(player);
            float serverHitMs = AttackTimingHelper.ResolveServerLikeHitMs(player);
            // Debug.Log(
                // $"[ATK_TIMING_CMP] AttackShot fire anim={animName} " +
                // $"serverTimeAtkMs={serverCycleMs:F1} serverHitMs={serverHitMs:F1} " +
                // $"(compare with Enter AttackShotWallMs and Exit wallElapsedMs)");
        }

        SwordCollisionPacketServices.Collision.EmitHitFromAttackShot(
            attackerEntityId,
            targetEntityId,
            swordBasePoints[0],
            swordBasePoints[1],
            target);
    }

    /// <summary>jatk01_1HS / SpAtk01_1HS / … — melee AttackShot path, not bow.</summary>

    private static bool IsMeleeAttackShotAnim(string animName)
    {
        if (string.IsNullOrEmpty(animName))
        {
            return false;
        }

        if (animName.IndexOf("bow", System.StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return false;
        }

        bool isJatk = animName.IndexOf("jatk", System.StringComparison.OrdinalIgnoreCase) >= 0;
        bool isSpAtk = animName.IndexOf("SpAtk", System.StringComparison.OrdinalIgnoreCase) >= 0;
        return isJatk || isSpAtk;
    }

}
