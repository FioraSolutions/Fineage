using UnityEngine;
using UnityEngine.UIElements;

public abstract partial class AbstractDataProvider
{
    protected void SetDataWeaponInTemplate(VisualElement container, Weapongrp weapon, int price , IDataTips text)
    {
        container.Q<Label>("nameWeapon").text = text.GetName();

        VisualElement gradeBox = container.Q<VisualElement>("grade");
        Texture2D grade = text.GetGradeTexture();
        SetImageElement(gradeBox, grade);

        VisualElement groupPriceLabel = container.Q<VisualElement>("PriceName");
        Label priceLabel = (Label)container.Q<Label>("priceLabel");
        AddElementPriceifNot0(groupPriceLabel, priceLabel, price);

        VisualElement groupType = container.Q<VisualElement>("SettingType");
        Label typeWeapon = container.Q<Label>("typeWeapon");
        AddElementIfNotEmpty(groupType, typeWeapon, WeaponTypeParser.WeaponTypeName(weapon.WeaponType));


        Label labelEnchant = container.Q<Label>("enchant");
        AddElementEnchantifNot0(labelEnchant, labelEnchant, text.GetEnchant());

        VisualElement groupPhys = container.Q<VisualElement>("phisAtkText");
        Label physLabel = container.Q<Label>("physLabel");
        AddElementIfNotEmpty(groupPhys, physLabel, weapon.PAtk.ToString());

        VisualElement groupMag = container.Q<VisualElement>("magAtkText");
        Label magLabel = container.Q<Label>("magLabel");
        AddElementIfNotEmpty(groupMag, magLabel, weapon.Matk.ToString());



        VisualElement groupSpd = container.Q<VisualElement>("speedText");
        Label spdLabel = container.Q<Label>("spdLabel");
        AddElementIfNotEmpty(groupSpd, spdLabel, weapon.GetSpeedName());



        VisualElement groupCrit = container.Q<VisualElement>("critText");
        Label critLabel = container.Q<Label>("critLabel");
        AddElementIfNotEmpty(groupCrit, critLabel, weapon.CriticalRate.ToString());



        VisualElement groupSoul = container.Q<VisualElement>("soulText");
        Label soulLabel = container.Q<Label>("soullabel");
        AddElementIfNotEmpty(groupSoul, soulLabel, "X" + weapon.Soulshot.ToString());



        VisualElement groupSpirit = container.Q<VisualElement>("spiritText");
        Label spiritLabel = container.Q<Label>("spiritlabel");
        AddElementIfNotEmpty(groupSpirit, spiritLabel, "X" + weapon.Spiritshot.ToString());


        VisualElement groupWeight = container.Q<VisualElement>("weightText");
        Label weightLabel = container.Q<Label>("weightlabel");
        AddElementIfNotEmpty(groupWeight, weightLabel, weapon.Weight.ToString());


        VisualElement groupDescr = container.Q<VisualElement>("descriptedText");
        Label descrLabel = container.Q<Label>("descriptedLabel");
        AddElementIfNotEmpty(groupDescr, descrLabel, text.GetItemDiscription());


        VisualElement groupIcon = container.Q<VisualElement>("GrowIcon");
        var icon = container.Q<VisualElement>("icon");
        AddNamedIconIfPresent(groupIcon, icon, weapon.Icon);
    }
}
