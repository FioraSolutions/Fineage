using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed class AppLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        RegisterIfFound<EventProcessor>(builder);
        RegisterIfFound<GameManager>(builder);
        RegisterIfFound<SceneLoader>(builder);
        RegisterIfFound<GameClient>(builder);
        RegisterIfFound<LoginClient>(builder);
        RegisterIfFound<L2LoginUI>(builder);

        builder.Register<TableInitializer>(Lifetime.Singleton);
        builder.Register<LobbyFlowController>(Lifetime.Singleton);

        builder.Register<IncomingPacketAutoRegistry>(Lifetime.Singleton)
            .AsSelf()
            .As<IIncomingPacketAutoRegistry>()
            .As<INetworkHandlers>();
        builder.Register<OutgoingPacketAutoRegistry>(Lifetime.Singleton)
            .AsSelf()
            .As<IOutgoingPacketAutoRegistry>();
        builder.Register<InterludeProtocol>(Lifetime.Singleton).As<IProtocol>().AsSelf();
        builder.Register<PacketApplyQueue>(Lifetime.Singleton);
        builder.Register<NetworkDispatcher>(Lifetime.Singleton).As<INetworkDispatcher>().AsSelf();
        builder.Register<IncomingGameQueue>(Lifetime.Singleton);
        builder.Register<SendGameDataQueue>(Lifetime.Singleton);
        builder.Register<IncomingLoginDataQueue>(Lifetime.Singleton);
        builder.Register<SendLoginDataQueue>(Lifetime.Singleton);
        builder.Register<NetworkRuntime>(Lifetime.Singleton);
        builder.Register<FineageZoneTransport>(Lifetime.Singleton).As<IZoneTransport>();
        builder.Register<AnimationManager>(Lifetime.Singleton).As<IAnimationManager>().AsSelf();
        RegisterDatTables(builder);

        builder.RegisterBuildCallback(container =>
        {
            App.Container = container;
            AnimationManager animations = container.Resolve<AnimationManager>();
            AnimationManager.Bind(animations);
            GameManager manager = container.Resolve<GameManager>();
            IncomingPacketActions.BindApp(
                container.Resolve<EventProcessor>(),
                container.Resolve<GameClient>(),
                container.Resolve<LoginClient>(),
                manager,
                animations,
                container.Resolve<PacketApplyQueue>());
            LobbyFlowLog.Wire(manager);
        });
    }

    private static void RegisterDatTables(IContainerBuilder builder)
    {
        builder.RegisterInstance(ItemTable.Instance);
        builder.RegisterInstance(ItemNameTable.Instance);
        builder.RegisterInstance(ItemStatDataTable.Instance);
        builder.RegisterInstance(ArmorgrpTable.Instance);
        builder.RegisterInstance(EtcItemgrpTable.Instance);
        builder.RegisterInstance(WeapongrpTable.Instance);
        builder.RegisterInstance(NpcDecoEffectsTable.Instance);
        builder.RegisterInstance(NpcgrpTable.Instance);
        builder.RegisterInstance(NpcNameTable.Instance);
        builder.RegisterInstance(QuestNameTable.Instance);
        builder.RegisterInstance(RecipeTable.Instance);
        builder.RegisterInstance(ActionNameTable.Instance);
        builder.RegisterInstance(AnimLeghtTable.Instance);
        builder.RegisterInstance(SysStringTable.Instance);
        builder.RegisterInstance(SkillNameTable.Instance);
        builder.RegisterInstance(SkillgrpTable.Instance);
        builder.RegisterInstance(MobSkillAnimgrpTable.Instance);
        builder.RegisterInstance(AnimTweenTable.Instance);
        builder.RegisterInstance(EnterEventTable.Instance);
        builder.RegisterInstance(ModelTable.Instance);
        builder.RegisterInstance(LogongrpTable.Instance);
        builder.RegisterInstance(SystemMessageTable.Instance);
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
