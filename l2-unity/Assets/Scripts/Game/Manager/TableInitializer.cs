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

    private static T Table<T>(T injected, T fallback) where T : class
    {
        return injected != null ? injected : fallback;
    }

    public void LoadTables()
    {
        ItemTable items = Table(_items, ItemTable.Instance);
        items.Initialize();
        Table(_itemNames, ItemNameTable.Instance).Initialize();
        Table(_itemStats, ItemStatDataTable.Instance).Initialize();
        Table(_armorGrps, ArmorgrpTable.Instance).Initialize();
        Table(_etcItemGrps, EtcItemgrpTable.Instance).Initialize();
        Table(_weaponGrps, WeapongrpTable.Instance).Initialize();
        items.CacheItems();
        NpcDecoEffectsTable decoEffects = Table(_npcDecoEffects, NpcDecoEffectsTable.Instance);
        decoEffects.Initialize();
        Table(_npcGrps, NpcgrpTable.Instance).Initialize(decoEffects);
        Table(_questNames, QuestNameTable.Instance).Initialize();
        Table(_recipes, RecipeTable.Instance).Initialize();
        Table(_npcNames, NpcNameTable.Instance).Initialize();
        Table(_actionNames, ActionNameTable.Instance).Initialize();
        Table(_animLengths, AnimLeghtTable.Instance).Initialize();
        Table(_sysStrings, SysStringTable.Instance).Initialize();
        Table(_skillNames, SkillNameTable.Instance).Initialize();
        Table(_skillGrps, SkillgrpTable.Instance).Initialize();
        Table(_mobSkillAnimGrps, MobSkillAnimgrpTable.Instance).Initialize();
        Table(_animTweens, AnimTweenTable.Instance).Initialize();
        Table(_enterEvents, EnterEventTable.Instance).Initialize();
        Table(_models, ModelTable.Instance).Initialize();
        Table(_logonGrps, LogongrpTable.Instance).Initialize();
        Table(_systemMessages, SystemMessageTable.Instance).Initialize();
        IconManager.Instance.Initialize();
        IconManager.Instance.CacheIcons();
        IconManager.Instance.CacheOtherIcons();
        IconManager.Instance.CacheInterfaceIcons();
    }
}
