using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed partial class GameLifetimeScope
{
    void RegisterUiServices(IContainerBuilder builder)
    {
        RegisterIfFound<L2GameUI>(builder);
        RegisterIfFound<L2SlotManager>(builder);
        RegisterIfFound<DragIcon>(builder);
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

        builder.Register<StatsConverter>(Lifetime.Singleton);
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
                AnimationPacketServices.Bind(
                    animations: runtime.Animations,
                    applyQueue: container.Resolve<PacketApplyQueue>(),
                    striderRide: container.Resolve<StriderRideService>());
                StatsConverter statsConverter = container.Resolve<StatsConverter>();
                StatsConverterPacketServices.Bind(converter: statsConverter);
                CombatPacketServices.Bind(statsConverter: statsConverter);
                ServicePacketServices.Bind(statsConverter: statsConverter);
                MoveCostLog.Wire(world);

                // #142 slice 3: hand NetSession the container-built applies via [Inject].
                // NetSession lives outside the scope, so it must be injected explicitly.
                NetSession netSession = NetworkingPacketServices.Session;
                if (netSession == null)
                    netSession = NetSession.CreateForCompositionRoot();
                NetworkingPacketServices.Bind(session: netSession);
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
}
