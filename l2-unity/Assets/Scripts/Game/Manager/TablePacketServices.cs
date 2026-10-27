/// <summary>
/// DAT table access with [Inject] wiring and singleton fallbacks (#142 sub-slice 41).
/// </summary>
public static class TablePacketServices
{
    private static ItemTable _items;
    private static ItemNameTable _itemNames;
    private static ItemStatDataTable _itemStats;
    private static ArmorgrpTable _armorGrps;
    private static EtcItemgrpTable _etcItemGrps;
    private static WeapongrpTable _weaponGrps;
    private static NpcDecoEffectsTable _npcDecoEffects;
    private static NpcgrpTable _npcGrps;
    private static NpcNameTable _npcNames;
    private static QuestNameTable _questNames;
    private static RecipeTable _recipes;
    private static ActionNameTable _actionNames;
    private static AnimLeghtTable _animLengths;
    private static SysStringTable _sysStrings;
    private static SkillNameTable _skillNames;
    private static SkillgrpTable _skillGrps;
    private static MobSkillAnimgrpTable _mobSkillAnimGrps;
    private static AnimTweenTable _animTweens;
    private static EnterEventTable _enterEvents;
    private static ModelTable _models;
    private static LogongrpTable _logonGrps;
    private static SystemMessageTable _systemMessages;

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
        SystemMessageTable systemMessages = null)
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
    }

    public static ItemTable Items =>
        _items != null ? _items : ItemTable.Instance;

    public static ItemNameTable ItemNames =>
        _itemNames != null ? _itemNames : ItemNameTable.Instance;

    public static ItemStatDataTable ItemStats =>
        _itemStats != null ? _itemStats : ItemStatDataTable.Instance;

    public static ArmorgrpTable ArmorGrps =>
        _armorGrps != null ? _armorGrps : ArmorgrpTable.Instance;

    public static EtcItemgrpTable EtcItemGrps =>
        _etcItemGrps != null ? _etcItemGrps : EtcItemgrpTable.Instance;

    public static WeapongrpTable WeaponGrps =>
        _weaponGrps != null ? _weaponGrps : WeapongrpTable.Instance;

    public static NpcDecoEffectsTable NpcDecoEffects =>
        _npcDecoEffects != null ? _npcDecoEffects : NpcDecoEffectsTable.Instance;

    public static NpcgrpTable NpcGrps =>
        _npcGrps != null ? _npcGrps : NpcgrpTable.Instance;

    public static NpcNameTable NpcNames =>
        _npcNames != null ? _npcNames : NpcNameTable.Instance;

    public static QuestNameTable QuestNames =>
        _questNames != null ? _questNames : QuestNameTable.Instance;

    public static RecipeTable Recipes =>
        _recipes != null ? _recipes : RecipeTable.Instance;

    public static ActionNameTable ActionNames =>
        _actionNames != null ? _actionNames : ActionNameTable.Instance;

    public static AnimLeghtTable AnimLengths =>
        _animLengths != null ? _animLengths : AnimLeghtTable.Instance;

    public static SysStringTable SysStrings =>
        _sysStrings != null ? _sysStrings : SysStringTable.Instance;

    public static SkillNameTable SkillNames =>
        _skillNames != null ? _skillNames : SkillNameTable.Instance;

    public static SkillgrpTable SkillGrps =>
        _skillGrps != null ? _skillGrps : SkillgrpTable.Instance;

    public static MobSkillAnimgrpTable MobSkillAnimGrps =>
        _mobSkillAnimGrps != null ? _mobSkillAnimGrps : MobSkillAnimgrpTable.Instance;

    public static AnimTweenTable AnimTweens =>
        _animTweens != null ? _animTweens : AnimTweenTable.Instance;

    public static EnterEventTable EnterEvents =>
        _enterEvents != null ? _enterEvents : EnterEventTable.Instance;

    public static ModelTable Models =>
        _models != null ? _models : ModelTable.Instance;

    public static LogongrpTable LogonGrps =>
        _logonGrps != null ? _logonGrps : LogongrpTable.Instance;

    public static SystemMessageTable SystemMessages =>
        _systemMessages != null ? _systemMessages : SystemMessageTable.Instance;
}
