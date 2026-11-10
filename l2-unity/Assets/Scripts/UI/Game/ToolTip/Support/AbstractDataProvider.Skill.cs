using UnityEngine;
using UnityEngine.UIElements;

public abstract partial class AbstractDataProvider
{
    public VisualElement SetDataSkillInTemplate(VisualElement container, SkillInstance skill, IDataTips text , float timeleft)
    {
        VisualElement returnElement = null;

        container.Q<Label>("nameSkill").text = text.GetName();


        VisualElement centerBox = container.Q<VisualElement>("CenterBox");
        VisualElement footerBox = container.Q<VisualElement>("FooterBox");

        VisualElement groupTypeLvl = container.Q<VisualElement>("lvlSkill");
        Label lvlSkill = container.Q<Label>("lvlSkill");

        VisualElement groupTypeLabelLvl = container.Q<VisualElement>("LvlLabel");
        Label lvlLabel = container.Q<Label>("LvlLabel");

        if(groupTypeLvl != null) AddElementIfNotEmpty(groupTypeLabelLvl, lvlLabel, "Lv.");
        AddElementIfNotEmpty(groupTypeLvl, lvlSkill, skill.Level.ToString());

        VisualElement groupTypeActiveSkill= container.Q<VisualElement>("SettingType");
        Label typeLabel = container.Q<Label>("typeSkill");
        AddElementIfNotEmpty(groupTypeActiveSkill, typeLabel, skill.GetTypeName());

        VisualElement groupTypeHp = container.Q<VisualElement>("HpText");
        Label hpLabel = container.Q<Label>("hpLabel");
        AddElementIfNot0(groupTypeHp, hpLabel, skill.GetHp());

        VisualElement groupTypeMp = container.Q<VisualElement>("MPText");
        Label mpLabel = container.Q<Label>("mpLabel");
        AddElementIfNot0(groupTypeMp, mpLabel, skill.GetMp());

        VisualElement groupTypeRadius = container.Q<VisualElement>("RadiusText");
        Label radiusLabel = container.Q<Label>("rangeLabel");
        AddElementIfNot0(groupTypeRadius, radiusLabel, skill.GetRange());

        VisualElement groupTypeTimeLeft = container.Q<VisualElement>("TimeLeftText");
        Label timeLeftLabel = container.Q<Label>("timeLeftLabel");


        if(timeleft != 0)
        {
            AddElementIfNotEmpty(groupTypeTimeLeft, timeLeftLabel, TimeUtils.FormatTime(timeleft));
            returnElement =  timeLeftLabel;
        }



        if (skill.GetHp() == 0 && skill.GetMp() == 0 && skill.GetRange() == -1 && timeleft == 0)
        {
            AddBoxIfNotEmpty(centerBox, "");
        }

        if (string.IsNullOrEmpty(text.GetDiscription()))
        {
            AddBoxIfNotEmpty(footerBox, text.GetDiscription());
        }

        VisualElement groupTypeCasting = container.Q<VisualElement>("CastingTimeText");
        Label castingLabel = container.Q<Label>("castlabel");


        VisualElement groupTypeReuse = container.Q<VisualElement>("ReuseTimeText");
        Label reuseLabel = container.Q<Label>("reuselabel");

        AddElementIfNot0(groupTypeReuse, reuseLabel, skill.GetReuseTime());

        AddElementIfNot0(groupTypeCasting, castingLabel, skill.GetHitTime());


        VisualElement groupTypeDesc = container.Q<VisualElement>("DescriptedText");
        Label descLabel = container.Q<Label>("descriptedLabel");

        AddElementIfNotEmpty(groupTypeDesc, descLabel, text.GetDiscription());

        VisualElement groupIcon = container.Q<VisualElement>("GrowIcon");
        var icon = container.Q<VisualElement>("icon");
        AddNamedIconIfPresent(groupIcon, icon, skill.Icon());

        return returnElement;
    }

}
