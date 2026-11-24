using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed partial class GameLifetimeScope
{
    void RegisterWorldServices(IContainerBuilder builder)
    {
        builder.Register<WorldEntityRegistry<Entity>>(Lifetime.Singleton);
        builder.Register<SpawnedEntityCatalog>(Lifetime.Singleton).As<ISpawnedEntities>();
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
        RegisterCameraCollision(builder);
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
        RegisterLocalPlayer(builder);
        RegisterWorldPlacement(builder);
    }

    /// <summary>
    /// Register <see cref="WorldPlacementAdapter"/> as <see cref="IWorldPlacement"/> (#131 S41a).
    /// </summary>
    void RegisterWorldPlacement(IContainerBuilder builder)
    {
        builder.Register<WorldPlacementAdapter>(Lifetime.Singleton).As<IWorldPlacement>();
    }

    static void RequireWorldPlacement(IObjectResolver container)
    {
        IWorldPlacement placement;
        try
        {
            placement = container.Resolve<IWorldPlacement>();
        }
        catch (VContainerException ex)
        {
            if (ex.InvalidType == typeof(IWorldPlacement))
            {
                throw new InvalidOperationException(
                    "[GameLifetimeScope] IWorldPlacement missing — WorldPlacementAdapter was not registered as IWorldPlacement.",
                    ex);
            }

            throw;
        }

        if (placement is WorldPlacementAdapter)
            return;

        throw new InvalidOperationException(
            "[GameLifetimeScope] IWorldPlacement is not a WorldPlacementAdapter — WorldPlacementAdapter was not registered as IWorldPlacement.");
    }

    /// <summary>
    /// Register <see cref="IVisibilityRoster"/> in this game scope, the scope that injects it into <see cref="World"/>.
    /// A loaded <see cref="VisibilityManager"/> is reused. Loaded scenes do not contain one, so the scope creates it
    /// and still registers the interface. Skipping the registration throws
    /// <c>No such registration of type: IVisibilityRoster</c> when <see cref="World"/> is resolved.
    /// </summary>
    void RegisterVisibilityRoster(IContainerBuilder builder)
    {
        VisibilityManager found = UnityEngine.Object.FindFirstObjectByType<VisibilityManager>(FindObjectsInactive.Include);
        if (found == null)
        {
            var host = new GameObject("VisibilityManager");
            host.transform.SetParent(transform, false);
            found = host.AddComponent<VisibilityManager>();
        }

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
    /// Bind the scene <see cref="MoveAllCharacters"/> into the app-scoped <see cref="SharedMoveRoster"/>.
    /// Lobby and game resolve the same <see cref="IMoveRoster"/>.
    /// </summary>
    void RegisterMoveRoster(IContainerBuilder builder)
    {
        MoveAllCharacters found = UnityEngine.Object.FindFirstObjectByType<MoveAllCharacters>(FindObjectsInactive.Include);
        if (found == null)
        {
            throw new InvalidOperationException(
                "[GameLifetimeScope] IMoveRoster missing — RegisterMoveRoster found no MoveAllCharacters in loaded scenes.");
        }

        builder.RegisterComponent(found).AsSelf().As<IMoveAllCharacters>();
        builder.RegisterBuildCallback(container => BindSharedMoveRoster(container, found));
    }

    static void BindSharedMoveRoster(IObjectResolver container, MoveAllCharacters scene)
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
                    "[GameLifetimeScope] IMoveRoster missing — AppLifetimeScope did not register SharedMoveRoster.",
                    ex);
            }

            throw;
        }

        SharedMoveRoster shared = roster as SharedMoveRoster;
        if (shared == null)
        {
            throw new InvalidOperationException(
                "[GameLifetimeScope] IMoveRoster is not a SharedMoveRoster — AppLifetimeScope did not register SharedMoveRoster.");
        }

        shared.BindScene(scene);
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
                    "[GameLifetimeScope] IMoveRoster missing — AppLifetimeScope did not register SharedMoveRoster.",
                    ex);
            }

            throw;
        }

        if (roster is SharedMoveRoster shared && shared.HasScene)
            return;

        throw new InvalidOperationException(
            "[GameLifetimeScope] IMoveRoster missing — SharedMoveRoster has no MoveAllCharacters bound.");
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
    /// Register <see cref="LocalPlayerSlot"/> as <see cref="ILocalPlayerSlot"/> (#131 S38r).
    /// The player attaches after spawn.
    /// </summary>
    void RegisterLocalPlayer(IContainerBuilder builder)
    {
        builder.Register<LocalPlayerSlot>(Lifetime.Singleton).AsSelf().As<ILocalPlayerSlot>();
    }

    static void RequireLocalPlayer(IObjectResolver container)
    {
        ILocalPlayerSlot slot;
        try
        {
            slot = container.Resolve<ILocalPlayerSlot>();
        }
        catch (VContainerException ex)
        {
            if (ex.InvalidType == typeof(ILocalPlayerSlot))
            {
                throw new InvalidOperationException(
                    "[GameLifetimeScope] ILocalPlayerSlot missing — LocalPlayerSlot was not registered as ILocalPlayerSlot.",
                    ex);
            }

            throw;
        }

        if (slot is LocalPlayerSlot)
            return;

        throw new InvalidOperationException(
            "[GameLifetimeScope] ILocalPlayerSlot is not a LocalPlayerSlot — LocalPlayerSlot was not registered as ILocalPlayerSlot.");
    }

    /// <summary>
    /// <see cref="PlayerController"/> is <see cref="IPlayerPawn"/> (#131 S40).
    /// The live accessor is null until the pawn is in a loaded scene. A missing registration still throws.
    /// </summary>
    static void RequirePlayerPawn(IObjectResolver container)
    {
        IPlayerPawn pawn;
        try
        {
            pawn = container.Resolve<IPlayerPawn>();
        }
        catch (VContainerException ex)
        {
            if (ex.InvalidType == typeof(IPlayerPawn))
            {
                throw new InvalidOperationException(
                    "[GameLifetimeScope] IPlayerPawn missing — PlayerController was not registered as IPlayerPawn.",
                    ex);
            }

            throw;
        }

        if (pawn == null || pawn is PlayerController)
            return;

        throw new InvalidOperationException(
            "[GameLifetimeScope] IPlayerPawn is not a PlayerController — PlayerController was not registered as IPlayerPawn.");
    }

    static void RequireSpawnedEntities(IObjectResolver container)
    {
        ISpawnedEntities spawned;
        try
        {
            spawned = container.Resolve<ISpawnedEntities>();
        }
        catch (VContainerException ex)
        {
            if (ex.InvalidType == typeof(ISpawnedEntities))
            {
                throw new InvalidOperationException(
                    "[GameLifetimeScope] ISpawnedEntities missing — SpawnedEntityCatalog was not registered as ISpawnedEntities.",
                    ex);
            }

            throw;
        }

        if (spawned is SpawnedEntityCatalog)
            return;

        throw new InvalidOperationException(
            "[GameLifetimeScope] ISpawnedEntities is not a SpawnedEntityCatalog — SpawnedEntityCatalog was not registered as ISpawnedEntities.");
    }

    /// <summary>
    /// Register the scene <see cref="CameraController"/> as <see cref="ICameraCollisionMask"/> (#131 S39a).
    /// </summary>
    void RegisterCameraCollision(IContainerBuilder builder)
    {
        CameraController found = UnityEngine.Object.FindFirstObjectByType<CameraController>(FindObjectsInactive.Include);
        if (found == null)
        {
            throw new InvalidOperationException(
                "[GameLifetimeScope] ICameraCollisionMask missing — RegisterCameraCollision found no CameraController in loaded scenes.");
        }

        builder.RegisterComponent(found).AsSelf().As<ICameraCollisionMask>();
    }

    static void RequireCameraCollision(IObjectResolver container)
    {
        ICameraCollisionMask mask;
        try
        {
            mask = container.Resolve<ICameraCollisionMask>();
        }
        catch (VContainerException ex)
        {
            if (ex.InvalidType == typeof(ICameraCollisionMask))
            {
                throw new InvalidOperationException(
                    "[GameLifetimeScope] ICameraCollisionMask missing — CameraController was not registered as ICameraCollisionMask.",
                    ex);
            }

            throw;
        }

        if (mask is CameraController)
            return;

        throw new InvalidOperationException(
            "[GameLifetimeScope] ICameraCollisionMask is not a CameraController — CameraController was not registered as ICameraCollisionMask.");
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
