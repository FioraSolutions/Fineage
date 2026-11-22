using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed partial class AppLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        RegisterAppEventProcessor(builder);
        RegisterGameManager(builder);
        RegisterSceneLoader(builder);
        AppScopeCycleSafeRegistration.Register(this, builder);

        builder.Register<TableInitializer>(Lifetime.Singleton).AsSelf().As<ITableInitializer>();
        builder.Register<IconManager>(Lifetime.Singleton);
        builder.Register<LobbyFlowController>(Lifetime.Singleton).AsSelf().As<ILobbySession>();

        builder.Register<EventProcessorApplyPump>(Lifetime.Singleton).As<IMainThreadApplyPump>();
        builder.Register<FineageZoneTransport>(Lifetime.Singleton).As<IZoneTransport>();
        builder.Register<AnimationManager>(Lifetime.Singleton).As<IAnimationManager>().As<IAnimationRoster>().AsSelf();
        RegisterDatTables(builder);

        builder.RegisterBuildCallback(container =>
        {
            AnimationManager animations = RequireAnimationRoster(container);
            RequireAnimationManager(container);
            RequireEventQueue(container);
            RequireGameManager(container);
            AudioSessionBootstrap.EnsureSession();
            App.Container = container;
            AnimationManager.Bind(animations);
            LogongrpTable.Bind(container.Resolve<LogongrpTable>());
            ItemNameTable.Bind(container.Resolve<ItemNameTable>());
            SkillgrpTable.Bind(container.Resolve<SkillgrpTable>());
            ItemStatDataTable.Bind(container.Resolve<ItemStatDataTable>());
            ArmorgrpTable.Bind(container.Resolve<ArmorgrpTable>());
            EtcItemgrpTable.Bind(container.Resolve<EtcItemgrpTable>());
            WeapongrpTable.Bind(container.Resolve<WeapongrpTable>());
            NpcgrpTable.Bind(container.Resolve<NpcgrpTable>());
            NpcNameTable.Bind(container.Resolve<NpcNameTable>());
            ActionNameTable.Bind(container.Resolve<ActionNameTable>());
            SkillNameTable.Bind(container.Resolve<SkillNameTable>());
            IZoneTransport transport = container.Resolve<IZoneTransport>();
            ZoneCommands.Bind(transport);
            // Single NetSession host owned by composition root (GPT A02) — not FineageZoneTransport.Ensure.
            NetSession netSession = NetSession.CreateForCompositionRoot();
            NetworkingPacketServices.Bind(session: netSession);
            IGameManager manager = container.Resolve<IGameManager>();
            GameManager gameManager = container.Resolve<GameManager>();
            LoginClient login = container.Resolve<LoginClient>();
            EventProcessor events = container.Resolve<EventProcessor>();
            _boundTransport = transport;
            _boundManager = manager;
            _boundGameManager = gameManager;
            _boundLogin = login;
            _boundEvents = events;
            _boundSession = netSession;
            NetGameHost.Bind(manager);
            _onDisconnected = HandleTransportDisconnected;
            transport.OnDisconnected += _onDisconnected;
            AppLoginSessionApplyIncomingPacketServices.BindApp(
                container.Resolve<GameClient>(),
                login,
                gameManager,
                container.Resolve<PacketApplyQueue>(),
                events);
            // NetworkingPacketServices.Bind(manager) already done in BindApp above; session bound above.
            LobbyFlowLog.Wire(() => manager == null || manager.GameState < GameState.IN_GAME);
        });
    }

    static AnimationManager RequireAnimationRoster(IObjectResolver container)
    {
        IAnimationRoster roster;
        try
        {
            roster = container.Resolve<IAnimationRoster>();
        }
        catch (VContainerException ex)
        {
            if (ex.InvalidType == typeof(IAnimationRoster))
            {
                throw new InvalidOperationException(
                    "[AppLifetimeScope] IAnimationRoster missing — AnimationManager was not registered as IAnimationRoster.",
                    ex);
            }

            throw;
        }

        AnimationManager animations = roster as AnimationManager;
        if (animations == null)
        {
            throw new InvalidOperationException(
                "[AppLifetimeScope] IAnimationRoster is not an AnimationManager — AnimationManager was not registered as IAnimationRoster.");
        }

        return animations;
    }

    static void RequireAnimationManager(IObjectResolver container)
    {
        IAnimationManager manager;
        try
        {
            manager = container.Resolve<IAnimationManager>();
        }
        catch (VContainerException ex)
        {
            if (ex.InvalidType == typeof(IAnimationManager))
            {
                throw new InvalidOperationException(
                    "[AppLifetimeScope] IAnimationManager missing — AnimationManager was not registered as IAnimationManager.",
                    ex);
            }

            throw;
        }

        if (manager is AnimationManager)
            return;

        throw new InvalidOperationException(
            "[AppLifetimeScope] IAnimationManager is not an AnimationManager — AnimationManager was not registered as IAnimationManager.");
    }

    static void RequireEventQueue(IObjectResolver container)
    {
        IEventQueue queue;
        try
        {
            queue = container.Resolve<IEventQueue>();
        }
        catch (VContainerException ex)
        {
            if (ex.InvalidType == typeof(IEventQueue))
            {
                throw new InvalidOperationException(
                    "[AppLifetimeScope] IEventQueue missing — EventProcessor was not registered as IEventQueue.",
                    ex);
            }

            throw;
        }

        if (queue is EventProcessor)
            return;

        throw new InvalidOperationException(
            "[AppLifetimeScope] IEventQueue is not an EventProcessor — EventProcessor was not registered as IEventQueue.");
    }

    static void RequireGameManager(IObjectResolver container)
    {
        IGameManager manager;
        try
        {
            manager = container.Resolve<IGameManager>();
        }
        catch (VContainerException ex)
        {
            if (ex.InvalidType == typeof(IGameManager))
            {
                throw new InvalidOperationException(
                    "[AppLifetimeScope] IGameManager missing — GameManager was not registered as IGameManager.",
                    ex);
            }

            throw;
        }

        if (manager is GameManager)
            return;

        throw new InvalidOperationException(
            "[AppLifetimeScope] IGameManager is not a GameManager — GameManager was not registered as IGameManager.");
    }
}
