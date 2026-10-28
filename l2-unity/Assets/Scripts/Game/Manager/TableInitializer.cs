using VContainer;

/// <summary>
/// Loads and caches DAT tables at app start. Extracted from <see cref="GameManager"/> (#141).
/// </summary>
public sealed class TableInitializer
{
    [Inject] ItemTable _items;
    [Inject] ItemNameTable _itemNames;
    [Inject] ItemStatDataTable _itemStats;
    [Inject] ArmorgrpTable _armorGrps;
    [Inject] EtcItemgrpTable _etcItemGrps;
    [Inject] WeapongrpTable _weaponGrps;
    [Inject] NpcDecoEffectsTable _npcDecoEffects;
    [Inject] NpcgrpTable _npcGrps;
    [Inject] NpcNameTable _npcNames;
    [Inject] QuestNameTable _questNames;
    [Inject] RecipeTable _recipes;
    [Inject] ActionNameTable _actionNames;
    [Inject] AnimLeghtTable _animLengths;
    [Inject] SysStringTable _sysStrings;
    [Inject] SkillNameTable _skillNames;
    [Inject] SkillgrpTable _skillGrps;
    [Inject] MobSkillAnimgrpTable _mobSkillAnimGrps;
    [Inject] AnimTweenTable _animTweens;
    [Inject] EnterEventTable _enterEvents;
    [Inject] ModelTable _models;
    [Inject] LogongrpTable _logonGrps;
    [Inject] SystemMessageTable _systemMessages;

    public void LoadTables()
    {
        WireTablePacketServices();

        ItemTable items = TablePacketServices.Items;
        items.Initialize();
        TablePacketServices.ItemNames.Initialize();
        TablePacketServices.ItemStats.Initialize();
        TablePacketServices.ArmorGrps.Initialize();
        TablePacketServices.EtcItemGrps.Initialize();
        TablePacketServices.WeaponGrps.Initialize();
        items.CacheItems();
        NpcDecoEffectsTable decoEffects = TablePacketServices.NpcDecoEffects;
        decoEffects.Initialize();
        TablePacketServices.NpcGrps.Initialize(decoEffects);
        TablePacketServices.QuestNames.Initialize();
        TablePacketServices.Recipes.Initialize();
        TablePacketServices.NpcNames.Initialize();
        TablePacketServices.ActionNames.Initialize();
        TablePacketServices.AnimLengths.Initialize();
        TablePacketServices.SysStrings.Initialize();
        TablePacketServices.SkillNames.Initialize();
        TablePacketServices.SkillGrps.Initialize();
        TablePacketServices.MobSkillAnimGrps.Initialize();
        TablePacketServices.AnimTweens.Initialize();
        TablePacketServices.EnterEvents.Initialize();
        TablePacketServices.Models.Initialize();
        TablePacketServices.LogonGrps.Initialize();
        TablePacketServices.SystemMessages.Initialize();
        IconManager.Instance.Initialize();
        IconManager.Instance.CacheIcons();
        IconManager.Instance.CacheOtherIcons();
        IconManager.Instance.CacheInterfaceIcons();
    }

    private void WireTablePacketServices()
    {
        TablePacketServices.Bind(
            items: _items,
            itemNames: _itemNames,
            itemStats: _itemStats,
            armorGrps: _armorGrps,
            etcItemGrps: _etcItemGrps,
            weaponGrps: _weaponGrps,
            npcDecoEffects: _npcDecoEffects,
            npcGrps: _npcGrps,
            npcNames: _npcNames,
            questNames: _questNames,
            recipes: _recipes,
            actionNames: _actionNames,
            animLengths: _animLengths,
            sysStrings: _sysStrings,
            skillNames: _skillNames,
            skillGrps: _skillGrps,
            mobSkillAnimGrps: _mobSkillAnimGrps,
            animTweens: _animTweens,
            enterEvents: _enterEvents,
            models: _models,
            logonGrps: _logonGrps,
            systemMessages: _systemMessages);
    }
}
