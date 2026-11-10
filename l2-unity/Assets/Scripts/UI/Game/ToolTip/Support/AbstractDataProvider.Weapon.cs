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

    protected void SetOther(VisualElement container , IDataTips text , string etcIcon , int priceItem ,  string accessoriesName , string weight)
    {
     
            container.Q<Label>("nameAccessories").text = text.GetName(true);

            //set icon
            VisualElement groupBoxIcon = container.Q<VisualElement>("GrowIcon");
            VisualElement icon = container.Q<VisualElement>("icon");
            AddNamedIconIfPresent(groupBoxIcon, icon, etcIcon);

            VisualElement gradeBox = container.Q<VisualElement>("grade");
            Texture2D grade = text.GetGradeTexture();
            SetImageElement(gradeBox, grade);

            Label labelEnchant = container.Q<Label>("enchant");
            AddElementEnchantifNot0(labelEnchant, labelEnchant, text.GetEnchant());


            Label mpLabel = container.Q<Label>("mpLabel");
            VisualElement mpGroup = container.Q<VisualElement>("mpText");
            AddElementIfNot0(mpGroup, mpLabel, 0);

            Label groupPriceLabel = (Label)container.Q<Label>("PriceName");
            Label priceLabel = (Label)container.Q<Label>("priceLabel");
            AddElementPriceifNot0(groupPriceLabel, priceLabel, priceItem);
            //price.style.color = ToolTipsUtils.GetColorPrice(text.GetDiscription());
            //price.text = ToolTipsUtils.ConvertToPrice(priceItem) + " Adena";


            VisualElement groupBoxMdef = (VisualElement)container.Q<VisualElement>("mdeText");
            Label lebelMdef = (Label)container.Q<Label>("mdefLabel");

            //set mdef
            AddElementIfNot0(groupBoxMdef, lebelMdef, 0);

            //set type
            VisualElement groupTypeLabel = container.Q<VisualElement>("SettingType");
            Label typeLabel = container.Q<Label>("typeLabel");
            AddElementIfNotEmpty(groupTypeLabel, typeLabel, accessoriesName);

            VisualElement groupBoxWeight = container.Q<VisualElement>("weightText");
            Label weightLabel = container.Q<Label>("weightlabel");
            AddElementIfNotEmpty(groupBoxWeight, weightLabel, weight);

            Label descriptionLabel = container.Q<Label>("descriptedLabel");
            AddElementIfNotEmpty(descriptionLabel, descriptionLabel, text.GetItemDiscription());
        //container.Q<Label>("descriptedLabel").text = text.GetItemDiscription();

        
    }

}
