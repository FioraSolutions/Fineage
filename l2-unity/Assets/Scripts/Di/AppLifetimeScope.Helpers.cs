using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed partial class AppLifetimeScope : LifetimeScope
{
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

    /// <summary>
    /// Register scene <see cref="GameManager"/> as itself and <see cref="IGameManager"/> (#131).
    /// </summary>
    private void RegisterGameManager(IContainerBuilder builder)
    {
        GameManager local = GetComponent<GameManager>();
        if (local == null)
            local = Object.FindFirstObjectByType<GameManager>(FindObjectsInactive.Include);
        if (local != null)
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
        if (local != null)
            builder.RegisterComponent(local).AsSelf().As<ISceneLoader>();
    }
}
