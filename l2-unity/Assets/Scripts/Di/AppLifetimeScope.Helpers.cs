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
}
