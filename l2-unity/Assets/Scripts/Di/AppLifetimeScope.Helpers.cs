using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed partial class AppLifetimeScope : LifetimeScope
{
    private static void RegisterDatTables(IContainerBuilder builder)
    {
        // Fresh instances — never TablePacketServices.* getters (they log empty MetaTable fallbacks pre-bind).
        builder.Register<ItemTable>(Lifetime.Singleton);
        builder.Register<ItemNameTable>(Lifetime.Singleton);
        builder.Register<ItemStatDataTable>(Lifetime.Singleton);
        builder.Register<ArmorgrpTable>(Lifetime.Singleton);
        builder.Register<EtcItemgrpTable>(Lifetime.Singleton);
        builder.Register<WeapongrpTable>(Lifetime.Singleton);
        builder.Register<NpcDecoEffectsTable>(Lifetime.Singleton);
        builder.Register<NpcgrpTable>(Lifetime.Singleton);
        builder.Register<NpcNameTable>(Lifetime.Singleton);
        builder.Register<QuestNameTable>(Lifetime.Singleton);
        builder.Register<RecipeTable>(Lifetime.Singleton);
        builder.Register<ActionNameTable>(Lifetime.Singleton);
        builder.Register<AnimLeghtTable>(Lifetime.Singleton);
        builder.Register<SysStringTable>(Lifetime.Singleton);
        builder.Register<SkillNameTable>(Lifetime.Singleton).AsSelf().As<ISkillNameTable>();
        builder.Register<SkillgrpTable>(Lifetime.Singleton).AsSelf().As<ISkillgrpLookup>();
        builder.Register<MobSkillAnimgrpTable>(Lifetime.Singleton);
        builder.Register<AnimTweenTable>(Lifetime.Singleton);
        builder.Register<EnterEventTable>(Lifetime.Singleton);
        builder.Register<ModelTable>(Lifetime.Singleton);
        builder.Register<LogongrpTable>(Lifetime.Singleton);
        builder.Register<SystemMessageTable>(Lifetime.Singleton);
    }

    private void RegisterAppEventProcessor(IContainerBuilder builder)
    {
        EventProcessor events = GetComponent<EventProcessor>();
        if (events == null)
            events = gameObject.AddComponent<EventProcessor>();
        builder.RegisterComponent(events).AsSelf().As<IEventQueue>();
    }

    /// <summary>
    /// Register scene <see cref="GameManager"/> as itself and <see cref="IGameManager"/> (#131).
    /// </summary>
    private void RegisterGameManager(IContainerBuilder builder)
    {
        GameManager local = GetComponent<GameManager>();
        if (local == null)
            local = Object.FindFirstObjectByType<GameManager>(FindObjectsInactive.Include);
        if (local == null)
        {
            throw new InvalidOperationException(
                "[AppLifetimeScope] IGameManager missing — RegisterGameManager found no GameManager on the host or in loaded scenes.");
        }

        builder.RegisterComponent(local).AsSelf().As<IGameManager>();
    }

    /// <summary>
    /// Register scene <see cref="SceneLoader"/> as itself and <see cref="ISceneLoader"/> (#131).
    /// </summary>
    private void RegisterSceneLoader(IContainerBuilder builder)
    {
        SceneLoader local = GetComponent<SceneLoader>();
        if (local == null)
            local = Object.FindFirstObjectByType<SceneLoader>(FindObjectsInactive.Include);
        if (local == null)
        {
            Debug.LogError("[AppLifetimeScope] SceneLoader missing — RegisterSceneLoader found none on the host or in loaded scenes, so ISceneLoader is not registered.");
            return;
        }
        builder.RegisterComponent(local).AsSelf().As<ISceneLoader>();
    }

    void HandleTransportDisconnected()
    {
        if (_boundManager != null)
            _boundManager.OnDisconnect();
    }
}
