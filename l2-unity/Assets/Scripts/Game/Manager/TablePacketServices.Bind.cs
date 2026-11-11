using VContainer;

public static partial class TablePacketServices
{
    public static void Bind(
        ItemTable items = null,
        ItemNameTable itemNames = null,
        ItemStatDataTable itemStats = null,
        ArmorgrpTable armorGrps = null,
        EtcItemgrpTable etcItemGrps = null,
        WeapongrpTable weaponGrps = null,
        NpcDecoEffectsTable npcDecoEffects = null,
        NpcgrpTable npcGrps = null,
        NpcNameTable npcNames = null,
        QuestNameTable questNames = null,
        RecipeTable recipes = null,
        ActionNameTable actionNames = null,
        AnimLeghtTable animLengths = null,
        SysStringTable sysStrings = null,
        SkillNameTable skillNames = null,
        SkillgrpTable skillGrps = null,
        MobSkillAnimgrpTable mobSkillAnimGrps = null,
        AnimTweenTable animTweens = null,
        EnterEventTable enterEvents = null,
        ModelTable models = null,
        LogongrpTable logonGrps = null,
        SystemMessageTable systemMessages = null,
        WeaponShotTable weaponShots = null,
        KeyImageTable keyImages = null)
    {
        if (items != null)
            _items = items;
        if (itemNames != null)
            _itemNames = itemNames;
        if (itemStats != null)
            _itemStats = itemStats;
        if (armorGrps != null)
            _armorGrps = armorGrps;
        if (etcItemGrps != null)
            _etcItemGrps = etcItemGrps;
        if (weaponGrps != null)
            _weaponGrps = weaponGrps;
        if (npcDecoEffects != null)
            _npcDecoEffects = npcDecoEffects;
        if (npcGrps != null)
            _npcGrps = npcGrps;
        if (npcNames != null)
            _npcNames = npcNames;
        if (questNames != null)
            _questNames = questNames;
        if (recipes != null)
            _recipes = recipes;
        if (actionNames != null)
            _actionNames = actionNames;
        if (animLengths != null)
            _animLengths = animLengths;
        if (sysStrings != null)
            _sysStrings = sysStrings;
        if (skillNames != null)
            _skillNames = skillNames;
        if (skillGrps != null)
            _skillGrps = skillGrps;
        if (mobSkillAnimGrps != null)
            _mobSkillAnimGrps = mobSkillAnimGrps;
        if (animTweens != null)
            _animTweens = animTweens;
        if (enterEvents != null)
            _enterEvents = enterEvents;
        if (models != null)
            _models = models;
        if (logonGrps != null)
            _logonGrps = logonGrps;
        if (systemMessages != null)
            _systemMessages = systemMessages;
        if (weaponShots != null)
            _weaponShots = weaponShots;
        if (keyImages != null)
            _keyImages = keyImages;
    }

}
