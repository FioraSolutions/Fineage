using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed class AppLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        RegisterAppEventProcessor(builder);
        RegisterIfFound<GameManager>(builder);
        RegisterIfFound<SceneLoader>(builder);
        RegisterIfFound<GameClient>(builder);
        RegisterIfFound<LoginClient>(builder);

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
            ZoneCommands.Bind(container.Resolve<IZoneTransport>());
            GameManager manager = container.Resolve<GameManager>();
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
#pragma warning disable CS0618
        builder.RegisterInstance(ItemNameTable.Instance);
#pragma warning restore CS0618
#pragma warning disable CS0618
        builder.RegisterInstance(ItemStatDataTable.Instance);
        builder.RegisterInstance(ArmorgrpTable.Instance);
        builder.RegisterInstance(EtcItemgrpTable.Instance);
        builder.RegisterInstance(WeapongrpTable.Instance);
#pragma warning restore CS0618
        builder.RegisterInstance(NpcDecoEffectsTable.Instance);
#pragma warning disable CS0618
        builder.RegisterInstance(NpcgrpTable.Instance);
        builder.RegisterInstance(NpcNameTable.Instance);
#pragma warning restore CS0618
        builder.RegisterInstance(QuestNameTable.Instance);
        builder.RegisterInstance(RecipeTable.Instance);
#pragma warning disable CS0618
        builder.RegisterInstance(ActionNameTable.Instance);
#pragma warning restore CS0618
        builder.RegisterInstance(AnimLeghtTable.Instance);
        builder.RegisterInstance(SysStringTable.Instance);
#pragma warning disable CS0618
        builder.RegisterInstance(SkillNameTable.Instance);
#pragma warning restore CS0618
#pragma warning disable CS0618
        builder.RegisterInstance(SkillgrpTable.Instance);
#pragma warning restore CS0618
        builder.RegisterInstance(MobSkillAnimgrpTable.Instance);
        builder.RegisterInstance(AnimTweenTable.Instance);
        builder.RegisterInstance(EnterEventTable.Instance);
        builder.RegisterInstance(ModelTable.Instance);
        builder.RegisterInstance(LogongrpTable.Instance);
        builder.RegisterInstance(SystemMessageTable.Instance);
    }

    private void RegisterAppEventProcessor(IContainerBuilder builder)
    {
        EventProcessor events = GetComponent<EventProcessor>();
        if (events == null)
            events = gameObject.AddComponent<EventProcessor>();
        builder.RegisterComponent(events);
    }

    private T ResolveComponent<T>() where T : MonoBehaviour
    {
        T local = GetComponent<T>();
        if (local != null)
            return local;
        return FindFirstObjectByType<T>(FindObjectsInactive.Include);
    }

    private void RegisterIfFound<T>(IContainerBuilder builder) where T : MonoBehaviour
    {
        T component = ResolveComponent<T>();
        if (component != null)
            builder.RegisterComponent(component);
    }
}
