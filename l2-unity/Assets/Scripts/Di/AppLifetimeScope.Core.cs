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
        LifetimeScopeMonoRegistration.RegisterIfFoundOnHost<GameClient>(this, builder);
        LifetimeScopeMonoRegistration.RegisterIfFoundOnHost<LoginClient>(this, builder);

        builder.Register<TableInitializer>(Lifetime.Singleton);
        builder.Register<IconManager>(Lifetime.Singleton);
        builder.Register<LobbyFlowController>(Lifetime.Singleton).AsSelf().As<ILobbySession>();

        builder.Register<EventProcessorApplyPump>(Lifetime.Singleton).As<IMainThreadApplyPump>();
        builder.Register<PacketApplyQueue>(Lifetime.Singleton).As<IPacketApplyQueue>().AsSelf();
        builder.Register<FineageZoneTransport>(Lifetime.Singleton).As<IZoneTransport>();
        builder.Register<AnimationManager>(Lifetime.Singleton).As<IAnimationManager>().AsSelf();
        RegisterDatTables(builder);

        builder.RegisterBuildCallback(container =>
        {
            App.Container = container;
            AnimationManager animations = container.Resolve<AnimationManager>();
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
            IGameManager manager = container.Resolve<IGameManager>();
            transport.OnDisconnected += () => manager.OnDisconnect();
            AppLoginSessionApplyIncomingPacketServices.BindApp(
                container.Resolve<GameClient>(),
                container.Resolve<LoginClient>(),
                container.Resolve<GameManager>(),
                container.Resolve<PacketApplyQueue>(),
                container.Resolve<EventProcessor>());
            // NetworkingPacketServices.Bind(manager) already done in BindApp above.
            LobbyFlowLog.Wire(() => manager == null || manager.GameState < GameState.IN_GAME);
        });
    }
}
