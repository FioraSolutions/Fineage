using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
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

        RegisterIfFound<L2GameUI>(builder);
        RegisterIfFound<L2SlotManager>(builder);
        RegisterLiveMono<ChatWindow>(builder);
        RegisterLiveMono<HtmlWindow>(builder);
        RegisterIfFound<DealerWindow>(builder);
        RegisterIfFound<MultiSellWindow>(builder);
        RegisterLateBound<TradeWindow>(builder);
        RegisterLateBound<TradeRequestWindow>(builder);
        RegisterLateBound<PartyInvitationWindow>(builder);
        RegisterIfFound<PartyMemberListWindow>(builder);
        RegisterIfFound<EnchantWindow>(builder);
        RegisterIfFound<RecipeBookWindow>(builder);
        RegisterIfFound<CraftingItemWindow>(builder);
        RegisterIfFound<SkillLearnWindow>(builder);
        RegisterIfFound<DescriptionSkillWindow>(builder);
        RegisterLateBound<SkillListWindow>(builder);
        RegisterIfFound<QuestWindow>(builder);
        RegisterIfFound<ClanWindow>(builder);
        RegisterIfFound<ShowListWindow>(builder);
        RegisterIfFound<SeedInfoWindow>(builder);
        RegisterIfFound<SellCropListWindow>(builder);
        RegisterLiveMono<SkillbarWindow>(builder);
        RegisterLiveMono<SystemMessageWindow>(builder);
        RegisterLateBound<BufferPanel>(builder);
        RegisterIfFound<InventoryWindow>(builder);
        RegisterIfFound<DeadWindow>(builder);
        RegisterIfFound<StatusWindow>(builder);
        RegisterLiveMono<CharacterInfoWindow>(builder);

        builder.Register<PlayerSpawner>(Lifetime.Singleton);
        builder.Register<BowArrowVisual>(Lifetime.Singleton);
        builder.RegisterFactory<UserEntity, UserBowArrowEvents>(container =>
        {
            BowArrowVisual bowArrow = container.Resolve<BowArrowVisual>();
            IAnimationManager animations = container.Resolve<IAnimationManager>();
            return user => new UserBowArrowEvents(user, bowArrow, animations);
        }, Lifetime.Singleton);
        builder.Register<UserSpawner>(Lifetime.Singleton);
        builder.Register<NpcDecoCatalog>(Lifetime.Singleton);
        builder.Register<NpcDecoService>(Lifetime.Singleton);
        builder.Register<NpcSpawner>(Lifetime.Singleton);
        builder.Register<MonsterSpawner>(Lifetime.Singleton);
        builder.Register<ItemSpawner>(Lifetime.Singleton);
        builder.Register<ItemDropLayerService>(Lifetime.Singleton);
        builder.Register<ItemDropClickAreaService>(Lifetime.Singleton);
        builder.Register<ItemDropPicker>(Lifetime.Singleton);
        builder.Register<ItemDropGrpCatalog>(Lifetime.Singleton);
        builder.Register<ItemDropPrefabLoader>(Lifetime.Singleton);
        builder.Register<ItemDropMaterialService>(Lifetime.Singleton);
        builder.Register<ItemDropWeaponAligner>(Lifetime.Singleton);
        builder.Register<ItemDropVisualService>(Lifetime.Singleton);
        builder.Register<ItemDropPresentationService>(Lifetime.Singleton);
        builder.Register<PlayerPositionSender>(Lifetime.Singleton);
        builder.Register<L2PawnRange>(Lifetime.Singleton);
        builder.Register<L2PawnYaw>(Lifetime.Singleton);
        builder.RegisterEntryPoint<EntityActionMachine>().AsSelf();
        builder.RegisterEntryPoint<StriderRideService>().AsSelf();
        builder.RegisterEntryPoint<UnityChainTicker>();
        builder.Register(_ => new PlayerWorldApply(), Lifetime.Singleton);
        builder.Register<NpcWorldApply>(Lifetime.Singleton);
        builder.Register<MonsterWorldApply>(Lifetime.Singleton);
        builder.Register<UserWorldApply>(Lifetime.Singleton);
        builder.Register<WorldPacketApply>(Lifetime.Singleton);
        builder.Register<WorldDomainSink>(Lifetime.Singleton).As<IWorldDomainSink>().AsSelf();
        builder.Register<MessagePacketApply>(Lifetime.Singleton);
        builder.Register<GameRuntime>(Lifetime.Singleton);

        // #142 slice 3 pilot: packet applies constructed by the container rather than reached
        // through the static *ApplyIncomingPacketServices relay.
        builder.Register<CombatAttackApplyService>(Lifetime.Singleton);
        builder.Register<CombatDieApplyService>(Lifetime.Singleton);
        builder.Register<CombatTargetApplyService>(Lifetime.Singleton);
        builder.Register<StatusUpdateApplyService>(Lifetime.Singleton);
        builder.Register<DeleteObjectGroundApplyService>(Lifetime.Singleton);
        builder.Register<StatusApplyService>(Lifetime.Singleton);
        builder.Register<SkillApplyService>(Lifetime.Singleton);
        builder.Register<PresenceApplyService>(Lifetime.Singleton);
        builder.Register<InventoryApplyService>(Lifetime.Singleton);
        builder.Register<PartyApplyService>(Lifetime.Singleton);
        builder.Register<TradeApplyService>(Lifetime.Singleton);
        builder.Register<TradeRequestApplyService>(Lifetime.Singleton);

        builder.RegisterBuildCallback(container =>
        {
            App.GameContainer = container;
            try
            {
                GameRuntime runtime = container.Resolve<GameRuntime>();
                container.Resolve<TargetClearOnEntityDiedListener>();
                container.Resolve<PlayerAttackClearOnEntityDiedListener>();
                WorldPacketApply worldApply = container.Resolve<WorldPacketApply>();
                MessagePacketApply messageApply = container.Resolve<MessagePacketApply>();
                IWorldDomainSink domain = container.Resolve<IWorldDomainSink>();
                runtime.BindApply(worldApply, messageApply);
                GameRuntimeBindApplyIncomingPacketServices.BindIncoming(
                    runtime: runtime,
                    worldApply: worldApply,
                    messageApply: messageApply,
                    domain: domain);
                World world = container.Resolve<World>();
                NetworkingPacketServices.Bind(
                    world: world,
                    worldApply: worldApply,
                    domain: domain,
                    manager: runtime.Manager,
                    player: runtime.Player,
                    events: runtime.Events);
                WorldApplyPacketServices.Bind(
                    player: runtime.Player,
                    bus: runtime.Bus,
                    gameWorld: world,
                    manager: runtime.Manager,
                    domain: domain,
                    worldApply: worldApply);
                ItemPacketServices.Bind(
                    gameWorld: world,
                    animations: runtime.Animations,
                    effects: runtime.Effects);
                MoveCostLog.Wire(world);

                // #142 slice 3: hand NetSession the container-built applies via [Inject].
                // NetSession lives outside the scope, so it must be injected explicitly.
                NetSession netSession = NetSession.Instance;
                if (netSession == null)
                    netSession = NetSession.Ensure();
                container.InjectGameObject(netSession.gameObject);
                ApplyServices.Bind(
                    inventory: container.Resolve<InventoryApplyService>(),
                    status: container.Resolve<StatusApplyService>());
                if (App.Container != null)
                {
                    try
                    {
                        EventProcessor events = App.Container.Resolve<EventProcessor>();
                        events.WireGameApply(worldApply);
                    }
                    catch
                    {
                    }
                }

                if (App.Container != null)
                {
                    try
                    {
                        LobbyFlowController lobby = App.Container.Resolve<LobbyFlowController>();
                        lobby.WireGame(
                            container.Resolve<World>(),
                            domain,
                            worldApply,
                            container.Resolve<PacketApplyQueue>(),
                            runtime.Ui);
                        GameManager manager = App.Container.Resolve<GameManager>();
                        if (runtime.Ui != null)
                            manager.WireGameUi(runtime.Ui);
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("GameLifetimeScope BindGame failed: " + ex);
                App.GameContainer = null;
                GameLifetimeScope broken = UnityEngine.Object.FindFirstObjectByType<GameLifetimeScope>();
                if (broken != null)
                    UnityEngine.Object.DestroyImmediate(broken.gameObject);
            }
        });
    }

    private void RegisterIfFound<T>(IContainerBuilder builder) where T : MonoBehaviour
    {
        T component = FindFirstObjectByType<T>(FindObjectsInactive.Include);
        if (component != null)
            builder.RegisterComponent(component);
    }

    /// <summary>
    /// Registers a type that is created after the scope is built (prefab-instantiated
    /// MonoBehaviours such as <see cref="PlayerInventory"/> / <see cref="TargetManager"/>).
    /// The concrete registration is still offered when one exists now; the
    /// <see cref="ILateBound{T}"/> registration is unconditional so consumers can always
    /// inject a provider and read null until the object appears (#142 slice 4).
    /// </summary>
    private void RegisterLateBound<T>(IContainerBuilder builder) where T : MonoBehaviour
    {
        T inScene = FindFirstObjectByType<T>(FindObjectsInactive.Include);
        if (inScene != null)
        {
            builder.RegisterComponent(inScene);
            builder.RegisterInstance<ILateBound<T>>(new LateBoundMono<T>(() => inScene));
            return;
        }

        // Not in the scene: the prefab instantiates it after the scope. Register the
        // concrete type lazily too, because other classes [Inject] it directly, and add
        // the provider whose Value is null until the object exists.
        builder.Register<T>(_ => ResolveLiveMono<T>(), Lifetime.Singleton);
        builder.RegisterInstance<ILateBound<T>>(new LateBoundMono<T>(() => ResolveLiveMono<T>()));
    }

    /// <summary>
    /// Registers a scene MonoBehaviour when present; otherwise a live singleton that resolves
    /// FindInScene → static Instance (spawn-deferred types like <see cref="PlayerController"/>).
    /// </summary>
    private void RegisterLiveMono<T>(IContainerBuilder builder) where T : MonoBehaviour
    {
        T inScene = FindFirstObjectByType<T>(FindObjectsInactive.Include);
        if (inScene != null)
        {
            builder.RegisterComponent(inScene);
            return;
        }

        builder.Register<T>(_ => ResolveLiveMono<T>(), Lifetime.Singleton);
    }

    private static T ResolveLiveMono<T>() where T : MonoBehaviour
    {
        T found = UnityEngine.Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
        if (found != null)
            return found;

        if (typeof(T) == typeof(PlayerInventory))
            return PlayerInventory.Instance as T;
        if (typeof(T) == typeof(TargetManager))
            return TargetManager.Instance as T;
        if (typeof(T) == typeof(TradeWindow))
            return TradeWindow.Instance as T;
        if (typeof(T) == typeof(TradeRequestWindow))
            return TradeRequestWindow.Instance as T;
        if (typeof(T) == typeof(PartyInvitationWindow))
            return PartyInvitationWindow.Instance as T;
        if (typeof(T) == typeof(SkillListWindow))
            return SkillListWindow.Instance as T;
        if (typeof(T) == typeof(BufferPanel))
            return BufferPanel.Instance as T;
        if (typeof(T) == typeof(MoveAllCharacters))
            return MoveAllCharacters.Instance as T;
        if (typeof(T) == typeof(EffectManager))
            return EffectManager.Instance as T;
        if (typeof(T) == typeof(EventBus))
            return EventBus.Instance as T;
        if (typeof(T) == typeof(SkillbarWindow))
            return SkillbarWindow.Instance as T;
        if (typeof(T) == typeof(ChatWindow))
            return ChatWindow.Instance as T;
        if (typeof(T) == typeof(HtmlWindow))
            return HtmlWindow.Instance as T;
        if (typeof(T) == typeof(SystemMessageWindow))
            return SystemMessagePacketServices.Window as T;
        if (typeof(T) == typeof(CameraController))
            return CameraController.Instance as T;
        if (typeof(T) == typeof(CharacterInfoWindow))
            return CharacterInfoWindow.Instance as T;
        if (typeof(T) == typeof(CharacterBuilder))
            return CharacterBuilder.Instance as T;

        return null;
    }
}
