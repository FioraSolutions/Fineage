using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed class AppLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        RegisterAppEventProcessor(builder);
        LifetimeScopeMonoRegistration.RegisterIfFoundOnHost<GameManager>(this, builder);
        LifetimeScopeMonoRegistration.RegisterIfFoundOnHost<SceneLoader>(this, builder);
        LifetimeScopeMonoRegistration.RegisterIfFoundOnHost<GameClient>(this, builder);
        LifetimeScopeMonoRegistration.RegisterIfFoundOnHost<LoginClient>(this, builder);

        builder.Register<TableInitializer>(Lifetime.Singleton);
        builder.Register<IconManager>(Lifetime.Singleton);
        builder.Register<LobbyFlowController>(Lifetime.Singleton);

        builder.Register<PacketApplyQueue>(Lifetime.Singleton);
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
            GameManager manager = container.Resolve<GameManager>();
            transport.OnDisconnected += () => manager.OnDisconnect();
            AppLoginSessionApplyIncomingPacketServices.BindApp(
                container.Resolve<GameClient>(),
                container.Resolve<LoginClient>(),
                manager,
                container.Resolve<PacketApplyQueue>(),
                container.Resolve<EventProcessor>());
            // NetworkingPacketServices.Bind(manager) already done in BindApp above.
            LobbyFlowLog.Wire(() => manager == null || manager.GameState < GameState.IN_GAME);
        });
    }

    private static void RegisterDatTables(IContainerBuilder builder)
    {
        builder.RegisterInstance(TablePacketServices.Items);
        builder.RegisterInstance(TablePacketServices.ItemNames);
        builder.RegisterInstance(TablePacketServices.ItemStats);
        builder.RegisterInstance(TablePacketServices.ArmorGrps);
        builder.RegisterInstance(TablePacketServices.EtcItemGrps);
        builder.RegisterInstance(TablePacketServices.WeaponGrps);
        builder.RegisterInstance(TablePacketServices.NpcDecoEffects);
        builder.RegisterInstance(TablePacketServices.NpcGrps);
        builder.RegisterInstance(TablePacketServices.NpcNames);
        builder.RegisterInstance(TablePacketServices.QuestNames);
        builder.RegisterInstance(TablePacketServices.Recipes);
        builder.RegisterInstance(TablePacketServices.ActionNames);
        builder.RegisterInstance(TablePacketServices.AnimLengths);
        builder.RegisterInstance(TablePacketServices.SysStrings);
        builder.RegisterInstance(TablePacketServices.SkillNames);
        builder.RegisterInstance(TablePacketServices.SkillGrps);
        builder.RegisterInstance(TablePacketServices.MobSkillAnimGrps);
        builder.RegisterInstance(TablePacketServices.AnimTweens);
        builder.RegisterInstance(TablePacketServices.EnterEvents);
        builder.RegisterInstance(TablePacketServices.Models);
        builder.RegisterInstance(TablePacketServices.LogonGrps);
        builder.RegisterInstance(TablePacketServices.SystemMessages);
    }

    private void RegisterAppEventProcessor(IContainerBuilder builder)
    {
        EventProcessor events = GetComponent<EventProcessor>();
        if (events == null)
            events = gameObject.AddComponent<EventProcessor>();
        builder.RegisterComponent(events);
    }

}
