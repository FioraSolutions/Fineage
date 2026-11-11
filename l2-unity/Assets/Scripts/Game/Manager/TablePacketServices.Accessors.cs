using VContainer;

public static partial class TablePacketServices
{
    public static ItemTable Items =>
        _items != null ? _items : LegacyItemTableInstance();

    public static ItemNameTable ItemNames
    {
        get
        {
            if (_itemNames != null)
                return _itemNames;
            if (App.HasContainer)
            {
                try
                {
                    return App.Resolve<ItemNameTable>();
                }
                catch
                {
                }
            }

#pragma warning disable CS0618
            return ItemNameTable.Instance;
#pragma warning restore CS0618
        }
    }

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

    public static RecipeTable Recipes
    {
        get
        {
            if (_recipes != null)
                return _recipes;
            if (App.HasContainer)
            {
                try
                {
                    return App.Resolve<RecipeTable>();
                }
                catch
                {
                }
            }

#pragma warning disable CS0618
            return RecipeTable.Instance;
#pragma warning restore CS0618
        }
    }

    public static ActionNameTable ActionNames =>
        _actionNames != null ? _actionNames : ActionNameTable.Instance;

    public static AnimLeghtTable AnimLengths =>
        _animLengths != null ? _animLengths : AnimLeghtTable.Instance;

    public static SysStringTable SysStrings =>
        _sysStrings != null ? _sysStrings : SysStringTable.Instance;

    public static SkillNameTable SkillNames =>
        _skillNames != null ? _skillNames : SkillNameTable.Instance;

    public static SkillgrpTable SkillGrps
    {
        get
        {
            if (_skillGrps != null)
                return _skillGrps;
            if (App.HasContainer)
            {
                try
                {
                    return App.Resolve<SkillgrpTable>();
                }
                catch
                {
                }
            }

#pragma warning disable CS0618
            return SkillgrpTable.Instance;
#pragma warning restore CS0618
        }
    }

    public static MobSkillAnimgrpTable MobSkillAnimGrps =>
        _mobSkillAnimGrps != null ? _mobSkillAnimGrps : MobSkillAnimgrpTable.Instance;

    public static AnimTweenTable AnimTweens =>
        _animTweens != null ? _animTweens : AnimTweenTable.Instance;

    public static EnterEventTable EnterEvents =>
        _enterEvents != null ? _enterEvents : EnterEventTable.Instance;

    public static ModelTable Models =>
        _models != null ? _models : ModelTable.Instance;

    public static LogongrpTable LogonGrps
    {
        get
        {
            if (_logonGrps != null)
                return _logonGrps;
            if (App.HasContainer)
            {
                try
                {
                    return App.Resolve<LogongrpTable>();
                }
                catch
                {
                }
            }

#pragma warning disable CS0618
            return LogongrpTable.Instance;
#pragma warning restore CS0618
        }
    }

    public static SystemMessageTable SystemMessages =>
        _systemMessages != null ? _systemMessages : SystemMessageTable.Instance;

    public static WeaponShotTable WeaponShots
    {
        get
        {
            if (_weaponShots != null)
                return _weaponShots;
#pragma warning disable CS0618
            return WeaponShotTable.Instance;
#pragma warning restore CS0618
        }
    }

    public static KeyImageTable KeyImages
    {
        get
        {
            if (_keyImages != null)
                return _keyImages;
#pragma warning disable CS0618
            return KeyImageTable.Instance;
#pragma warning restore CS0618
        }
    }

#pragma warning disable CS0618 // Locator fallback until ItemTable.Instance is removed
    static ItemTable LegacyItemTableInstance() => ItemTable.Instance;
#pragma warning restore CS0618
}
