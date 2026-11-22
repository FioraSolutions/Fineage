using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed partial class GameLifetimeScope
{
    void RegisterWorldServices(IContainerBuilder builder)
    {
        GameScopePlayerControllerRegistration.Register(builder);
        RegisterWorld(builder);
        RegisterIfFound<WorldCombat>(builder);
        // Late-bound after the scope (#142 slice 4): these live in Input.prefab / UI.prefab
        // and are only searchable once the prefab is instantiated.
        RegisterLateBound<TargetManager>(builder);
        RegisterIfFound<HitManager>(builder);
        RegisterClickMasks(builder);
        RegisterIfFound<KeyboardMoveController>(builder);
        RegisterIfFound<NpcCursorManager>(builder);
        // Late-bound after the scope (#142 slice 4): these live in Input.prefab / UI.prefab
        // and are only searchable once the prefab is instantiated.
        RegisterLateBound<PlayerInventory>(builder);
        RegisterIfFound<PlayerShortcuts>(builder);
        RegisterLiveMono<EffectManager>(builder);
        RegisterIfFound<ProjectileManager>(builder);
        RegisterIfFound<ObjectPoolManager>(builder);
        RegisterLiveMono<CameraController>(builder);
        RegisterIfFound<InputManager>(builder);
        RegisterIfFound<PositionValidationController>(builder);
        RegisterIfFound<SkillExecutor>(builder);
        RegisterIfFound<CombatFacingService>(builder);
        RegisterLiveMono<EventBus>(builder);
        GameScopeCycleSafeRegistration.Register(builder);
        builder.Register<TargetClearOnEntityDiedListener>(Lifetime.Singleton);
        builder.Register<PlayerAttackClearOnEntityDiedListener>(Lifetime.Singleton);
        builder.Register<L2ActorFade>(Lifetime.Singleton);
        builder.Register<DefaultEffectAttachmentResolver>(Lifetime.Singleton).As<IEffectAttachmentResolver>();
        builder.Register<HomeProjectileDualFlightRoots>(Lifetime.Singleton);
        builder.Register<HomeProjectileLauncher>(Lifetime.Singleton);
        builder.Register<HomeProjectileService>(Lifetime.Singleton).As<IHomeProjectileService>();
        builder.RegisterEntryPoint<AppearFadeService>().AsSelf();
        RegisterIfFound<DeadManager>(builder);
        RegisterMoveRoster(builder);
        RegisterNameplateRoster(builder);
        RegisterVisibilityRoster(builder);
        RegisterIfFound<NetworkCharacterControllerShare>(builder);
        RegisterIfFound<NetworkTransformReceive>(builder);
        RegisterIfFound<NetworkTransformShare>(builder);
        RegisterIfFound<GravityNpc>(builder);
        RegisterIfFound<FNManagerLight>(builder);
        RegisterIfFound<SwordCollisionService>(builder);
        RegisterIfFound<PathFinderController>(builder);
        RegisterIfFound<Geodata>(builder);
        RegisterLateBound<AudioManager>(builder);
        RegisterLateBound<AudioService>(builder);
        RegisterLateBound<MusicManager>(builder);
        RegisterLateBound<ThirdPersonListener>(builder);
        RegisterIfFound<SurfaceDetector>(builder);
        RegisterIfFound<PlayerActions>(builder);
        RegisterIfFound<ListenerCharacterGear>(builder);
        RegisterLiveMono<CharacterBuilder>(builder);
        RegisterPlayerRebirth(builder);
    }

    /// <summary>
    /// Register the scene <see cref="VisibilityManager"/> as <see cref="IVisibilityRoster"/> (#131 S38h).
    /// </summary>
    void RegisterVisibilityRoster(IContainerBuilder builder)
    {
        VisibilityManager found = UnityEngine.Object.FindFirstObjectByType<VisibilityManager>(FindObjectsInactive.Include);
        if (found != null)
            builder.RegisterComponent(found).AsSelf().As<IVisibilityRoster>();
    }

    static void RequireVisibilityRoster(IObjectResolver container)
    {
        IVisibilityRoster roster;
        try
        {
            roster = container.Resolve<IVisibilityRoster>();
        }
        catch (VContainerException ex)
        {
            if (ex.InvalidType == typeof(IVisibilityRoster))
            {
                throw new InvalidOperationException(
                    "[GameLifetimeScope] IVisibilityRoster missing — VisibilityManager was not registered as IVisibilityRoster.",
                    ex);
            }

            throw;
        }

        if (roster is VisibilityManager)
            return;

        throw new InvalidOperationException(
            "[GameLifetimeScope] IVisibilityRoster is not a VisibilityManager — VisibilityManager was not registered as IVisibilityRoster.");
    }

    static void RequireEntityActionRoster(IObjectResolver container)
    {
        IEntityActionRoster roster;
        try
        {
            roster = container.Resolve<IEntityActionRoster>();
        }
        catch (VContainerException ex)
        {
            if (ex.InvalidType == typeof(IEntityActionRoster))
            {
                throw new InvalidOperationException(
                    "[GameLifetimeScope] IEntityActionRoster missing — EntityActionMachine was not registered as IEntityActionRoster.",
                    ex);
            }

            throw;
        }

        if (roster is EntityActionMachine)
            return;

        throw new InvalidOperationException(
            "[GameLifetimeScope] IEntityActionRoster is not an EntityActionMachine — EntityActionMachine was not registered as IEntityActionRoster.");
    }

    /// <summary>
    /// Register the scene <see cref="MoveAllCharacters"/> as <see cref="IMoveRoster"/> (#131 S38l).
    /// </summary>
    void RegisterMoveRoster(IContainerBuilder builder)
    {
        MoveAllCharacters found = UnityEngine.Object.FindFirstObjectByType<MoveAllCharacters>(FindObjectsInactive.Include);
        if (found == null)
        {
            throw new InvalidOperationException(
                "[GameLifetimeScope] IMoveRoster missing — RegisterMoveRoster found no MoveAllCharacters in loaded scenes.");
        }

        builder.RegisterComponent(found).AsSelf().As<IMoveAllCharacters>().As<IMoveRoster>();
    }

    static void RequireMoveRoster(IObjectResolver container)
    {
        IMoveRoster roster;
        try
        {
            roster = container.Resolve<IMoveRoster>();
        }
        catch (VContainerException ex)
        {
            if (ex.InvalidType == typeof(IMoveRoster))
            {
                throw new InvalidOperationException(
                    "[GameLifetimeScope] IMoveRoster missing — MoveAllCharacters was not registered as IMoveRoster.",
                    ex);
            }

            throw;
        }

        if (roster is MoveAllCharacters)
            return;

        throw new InvalidOperationException(
            "[GameLifetimeScope] IMoveRoster is not a MoveAllCharacters — MoveAllCharacters was not registered as IMoveRoster.");
    }

    /// <summary>
    /// Register the scene <see cref="NameplatesManager"/> as <see cref="INameplateRoster"/> (#131 S38n).
    /// </summary>
    void RegisterNameplateRoster(IContainerBuilder builder)
    {
        NameplatesManager found = UnityEngine.Object.FindFirstObjectByType<NameplatesManager>(FindObjectsInactive.Include);
        if (found == null)
        {
            throw new InvalidOperationException(
                "[GameLifetimeScope] INameplateRoster missing — RegisterNameplateRoster found no NameplatesManager in loaded scenes.");
        }

        builder.RegisterComponent(found).AsSelf().As<INameplateRoster>();
    }

    static void RequireNameplateRoster(IObjectResolver container)
    {
        INameplateRoster roster;
        try
        {
            roster = container.Resolve<INameplateRoster>();
        }
        catch (VContainerException ex)
        {
            if (ex.InvalidType == typeof(INameplateRoster))
            {
                throw new InvalidOperationException(
                    "[GameLifetimeScope] INameplateRoster missing — NameplatesManager was not registered as INameplateRoster.",
                    ex);
            }

            throw;
        }

        if (roster is NameplatesManager)
            return;

        throw new InvalidOperationException(
            "[GameLifetimeScope] INameplateRoster is not a NameplatesManager — NameplatesManager was not registered as INameplateRoster.");
    }

    /// <summary>
    /// Register the scene <see cref="ClickManager"/> as <see cref="IClickMasks"/> (#131 S38o).
    /// </summary>
    void RegisterClickMasks(IContainerBuilder builder)
    {
        ClickManager found = UnityEngine.Object.FindFirstObjectByType<ClickManager>(FindObjectsInactive.Include);
        if (found == null)
        {
            throw new InvalidOperationException(
                "[GameLifetimeScope] IClickMasks missing — RegisterClickMasks found no ClickManager in loaded scenes.");
        }

        builder.RegisterComponent(found).AsSelf().As<IClickMasks>();
    }

    static void RequireClickMasks(IObjectResolver container)
    {
        IClickMasks masks;
        try
        {
            masks = container.Resolve<IClickMasks>();
        }
        catch (VContainerException ex)
        {
            if (ex.InvalidType == typeof(IClickMasks))
            {
                throw new InvalidOperationException(
                    "[GameLifetimeScope] IClickMasks missing — ClickManager was not registered as IClickMasks.",
                    ex);
            }

            throw;
        }

        if (masks is ClickManager)
            return;

        throw new InvalidOperationException(
            "[GameLifetimeScope] IClickMasks is not a ClickManager — ClickManager was not registered as IClickMasks.");
    }

    /// <summary>
    /// Register <see cref="PlayerRebirthRelay"/> as <see cref="IPlayerRebirth"/> (#131 S38q).
    /// The live <see cref="PlayerStateMachine"/> attaches after the player spawns.
    /// </summary>
    void RegisterPlayerRebirth(IContainerBuilder builder)
    {
        builder.Register<PlayerRebirthRelay>(Lifetime.Singleton).AsSelf().As<IPlayerRebirth>();
    }

    static void RequirePlayerRebirth(IObjectResolver container)
    {
        IPlayerRebirth rebirth;
        try
        {
            rebirth = container.Resolve<IPlayerRebirth>();
        }
        catch (VContainerException ex)
        {
            if (ex.InvalidType == typeof(IPlayerRebirth))
            {
                throw new InvalidOperationException(
                    "[GameLifetimeScope] IPlayerRebirth missing — PlayerRebirthRelay was not registered as IPlayerRebirth.",
                    ex);
            }

            throw;
        }

        if (rebirth is PlayerRebirthRelay)
            return;

        throw new InvalidOperationException(
            "[GameLifetimeScope] IPlayerRebirth is not a PlayerRebirthRelay — PlayerRebirthRelay was not registered as IPlayerRebirth.");
    }

    /// <summary>
    /// Register scene <see cref="World"/> as itself, <see cref="IWorldEntityLookup"/>, and <see cref="IWorldSpawnContext"/> (#131).
    /// </summary>
    void RegisterWorld(IContainerBuilder builder)
    {
        World local = UnityEngine.Object.FindFirstObjectByType<World>(FindObjectsInactive.Include);
        if (local != null)
            builder.RegisterComponent(local).AsSelf().As<IWorldEntityLookup>().As<IWorldSpawnContext>();
    }
}
