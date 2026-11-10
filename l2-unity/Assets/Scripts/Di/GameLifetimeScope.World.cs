using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed partial class GameLifetimeScope
{
    void RegisterWorldServices(IContainerBuilder builder)
    {
        GameScopePlayerControllerRegistration.Register(builder);
        RegisterIfFound<World>(builder);
        RegisterIfFound<WorldCombat>(builder);
        // Late-bound after the scope (#142 slice 4): these live in Input.prefab / UI.prefab
        // and are only searchable once the prefab is instantiated.
        RegisterLateBound<TargetManager>(builder);
        RegisterIfFound<HitManager>(builder);
        RegisterIfFound<ClickManager>(builder);
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
        builder.Register<GameEventBus>(Lifetime.Singleton).As<IGameEventBus>();
        builder.Register<TargetClearOnEntityDiedListener>(Lifetime.Singleton);
        builder.Register<PlayerAttackClearOnEntityDiedListener>(Lifetime.Singleton);
        builder.Register<L2ActorFade>(Lifetime.Singleton);
        builder.Register<DefaultEffectAttachmentResolver>(Lifetime.Singleton).As<IEffectAttachmentResolver>();
        builder.Register<HomeProjectileDualFlightRoots>(Lifetime.Singleton);
        builder.Register<HomeProjectileLauncher>(Lifetime.Singleton);
        builder.Register<HomeProjectileService>(Lifetime.Singleton).As<IHomeProjectileService>();
        builder.RegisterEntryPoint<AppearFadeService>().AsSelf();
        RegisterIfFound<DeadManager>(builder);
        RegisterLiveMono<MoveAllCharacters>(builder);
        RegisterIfFound<NetworkCharacterControllerShare>(builder);
        RegisterIfFound<NetworkTransformReceive>(builder);
        RegisterIfFound<NetworkTransformShare>(builder);
        RegisterIfFound<GravityNpc>(builder);
        RegisterIfFound<FNManagerLight>(builder);
        RegisterIfFound<SwordCollisionService>(builder);
        RegisterIfFound<PathFinderController>(builder);
        RegisterIfFound<Geodata>(builder);
        RegisterIfFound<AudioManager>(builder);
        RegisterIfFound<MusicManager>(builder);
        RegisterIfFound<SurfaceDetector>(builder);
        RegisterIfFound<ThirdPersonListener>(builder);
        RegisterIfFound<PlayerActions>(builder);
        RegisterIfFound<ListenerCharacterGear>(builder);
        RegisterLiveMono<CharacterBuilder>(builder);

    }
}
