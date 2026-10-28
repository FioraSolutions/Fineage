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
        RegisterIfFound<TargetManager>(builder);
        RegisterIfFound<HitManager>(builder);
        RegisterIfFound<ClickManager>(builder);
        RegisterIfFound<NpcCursorManager>(builder);
        RegisterIfFound<PlayerInventory>(builder);
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
        RegisterIfFound<ZoneV2Client>(builder);
        RegisterIfFound<GravityNpc>(builder);
        RegisterIfFound<FNManagerLight>(builder);
        RegisterIfFound<SwordCollisionService>(builder);
        RegisterIfFound<EffectSkillsmanager>(builder);
        RegisterIfFound<PathFinderController>(builder);
        RegisterIfFound<Geodata>(builder);
        RegisterIfFound<AudioManager>(builder);
        RegisterIfFound<MusicManager>(builder);
        RegisterIfFound<SurfaceDetector>(builder);
        RegisterIfFound<ThirdPersonListener>(builder);
        RegisterIfFound<SkillsManager>(builder);
        RegisterIfFound<PlayerActions>(builder);
        RegisterIfFound<ListenerCharacterGear>(builder);
        RegisterLiveMono<CharacterBuilder>(builder);

        RegisterIfFound<L2GameUI>(builder);
        RegisterIfFound<L2SlotManager>(builder);
        RegisterLiveMono<ChatWindow>(builder);
        RegisterLiveMono<HtmlWindow>(builder);
        RegisterIfFound<DealerWindow>(builder);
        RegisterIfFound<MultiSellWindow>(builder);
        RegisterIfFound<TradeWindow>(builder);
        RegisterIfFound<TradeRequestWindow>(builder);
        RegisterIfFound<PartyInvitationWindow>(builder);
        RegisterIfFound<EnchantWindow>(builder);
        RegisterIfFound<RecipeBookWindow>(builder);
        RegisterIfFound<CraftingItemWindow>(builder);
        RegisterIfFound<SkillLearnWindow>(builder);
        RegisterIfFound<DescriptionSkillWindow>(builder);
        RegisterIfFound<SkillListWindow>(builder);
        RegisterIfFound<QuestWindow>(builder);
        RegisterIfFound<QuestListWindow>(builder);
        RegisterIfFound<ClanWindow>(builder);
        RegisterIfFound<ShowListWindow>(builder);
        RegisterIfFound<SeedInfoWindow>(builder);
        RegisterIfFound<SellCropListWindow>(builder);
        RegisterLiveMono<SkillbarWindow>(builder);
        RegisterLiveMono<SystemMessageWindow>(builder);
        RegisterIfFound<BufferPanel>(builder);
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
                IncomingPacketActions.BindGame(runtime, worldApply, messageApply, domain);
                IncomingPacketServices.Bind(
                    game: runtime.Game,
                    login: runtime.Login,
                    manager: runtime.Manager,
                    gameWorld: runtime.World,
                    worldApply: worldApply,
                    messageApply: messageApply,
                    domain: domain,
                    inventory: runtime.Inventory,
                    targets: runtime.Targets,
                    shortcuts: runtime.Shortcuts,
                    positionValidation: runtime.PositionValidation,
                    html: runtime.Html,
                    dealer: runtime.Dealer,
                    multiSell: runtime.MultiSell,
                    trade: runtime.Trade,
                    tradeRequest: runtime.TradeRequest,
                    partyInvite: runtime.PartyInvite,
                    enchant: runtime.Enchant,
                    recipeBook: runtime.RecipeBook,
                    crafting: runtime.Crafting,
                    skillLearn: runtime.SkillLearn,
                    skillDesc: runtime.SkillDesc,
                    skillList: runtime.SkillList,
                    quest: runtime.Quest,
                    questList: runtime.QuestList,
                    clan: runtime.Clan,
                    showList: runtime.ShowList,
                    seedInfo: runtime.SeedInfo,
                    sellCrop: runtime.SellCrop,
                    buffer: runtime.Buffer);
                ControllerPacketServices.Bind(
                    game: runtime.Game,
                    player: runtime.Player,
                    targets: runtime.Targets,
                    input: runtime.Input);
                NetworkingPacketServices.Bind(
                    world: runtime.World,
                    worldApply: worldApply,
                    domain: domain,
                    manager: runtime.Manager,
                    player: runtime.Player);
                AudioPacketServices.Bind(
                    gameWorld: runtime.World,
                    player: runtime.Player);
                MovementPacketLatest.Wire(domain);
                World world = container.Resolve<World>();
                MoveCostLog.Wire(world);
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

                try
                {
                    ZoneV2Client zoneV2 = container.Resolve<ZoneV2Client>();
                    zoneV2.WireGame(domain, worldApply);
                }
                catch
                {
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
            return SystemMessageWindow.Instance as T;
        if (typeof(T) == typeof(CameraController))
            return CameraController.Instance as T;
        if (typeof(T) == typeof(CharacterInfoWindow))
            return CharacterInfoWindow.Instance as T;
        if (typeof(T) == typeof(CharacterBuilder))
            return CharacterBuilder.Instance as T;

        return null;
    }
}
