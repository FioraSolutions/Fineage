using UnityEngine;
using UnityEngine.UIElements;

public abstract partial class AbstractDataProvider
{
    public void SetDataAccessoriesInTemplate(VisualElement container, Armorgrp armor, IDataTips text , int price , string accessoriesName)
    {
        container.Q<Label>("nameAccessories").text = text.GetName();

        VisualElement gradeBox = container.Q<VisualElement>("grade");
        Texture2D grade = text.GetGradeTexture();
        SetImageElement(gradeBox, grade);

        VisualElement groupBoxTypeS1 = container.Q<VisualElement>("typeTextS1");
        Label typeS1 = container.Q<Label>("typeLabelS1");
        VisualElement groupBoxMDef = container.Q<VisualElement>("mdeText");

        VisualElement groupBoxIcon = container.Q<VisualElement>("GrowIcon");
        VisualElement icon = container.Q<VisualElement>("icon");
        AddNamedIconIfPresent(groupBoxIcon, icon, armor.Icon);
        AddElementIfNotEmpty(groupBoxTypeS1, typeS1, "");

        Label labelEnchant = container.Q<Label>("enchant");
        AddElementEnchantifNot0(labelEnchant, labelEnchant, text.GetEnchant());


        //EnabledRow(groupBoxMDef);

        Label mpLabel = container.Q<Label>("mpLabel");
        VisualElement mpGroup = container.Q<VisualElement>("mpText");
        AddElementIfNot0(mpGroup, mpLabel, armor.MpBonus);

        Label textMdef = container.Q<Label>("mdefLabel");
        //textMdef.text = armor.MDef.ToString();
        AddElementIfNotEmpty(groupBoxMDef, textMdef, armor.MDef.ToString());

        VisualElement groupTypeLabel = container.Q<VisualElement>("SettingType");
        Label typeLabel = container.Q<Label>("typeLabel");
        AddElementIfNotEmpty(groupTypeLabel, typeLabel, accessoriesName);

        Label groupPriceLabel = (Label)container.Q<Label>("PriceName");
        Label priceLabel = (Label)container.Q<Label>("priceLabel");
        AddElementPriceifNot0(groupPriceLabel, priceLabel, price);



        //typeLabel.text = product.GetTypeAccessoriesName();

        container.Q<Label>("weightlabel").text = armor.Weight.ToString();
        container.Q<Label>("descriptedLabel").text = text.GetItemDiscription();
    }

    protected void AddArmor(VisualElement container, int price, string armorTypeName ,  IDataTips text, Armorgrp armor, VisualTreeAsset setsElements, VisualTreeAsset setsEffects, CreatorSets creator)
    {
        //set icon
        VisualElement groupBoxIcon = container.Q<VisualElement>("GrowIcon");
        
        VisualElement icon = container.Q<VisualElement>("icon");
        AddNamedIconIfPresent(groupBoxIcon, icon, armor.Icon);

        container.Q<Label>("nameAccessories").text = text.GetName();

        VisualElement gradeBox = container.Q<VisualElement>("grade");
        Texture2D grade = text.GetGradeTexture();
        SetImageElement(gradeBox, grade);
    

        //VisualElement groupBoxType = container.Q<VisualElement>("typeText");
        //VisualElement groupBoxMDef = container.Q<VisualElement>("mdeText");

        //EnabledRow(groupBoxType);
        //EnabledRow(groupBoxMDef);


        Label labelEnchant = container.Q<Label>("enchant");
        AddElementEnchantifNot0(labelEnchant, labelEnchant, text.GetEnchant());

        VisualElement groupType = container.Q<VisualElement>("typeText");
        Label typeLabel = container.Q<Label>("typeLabel");

        
        Label groupPriceLabel = (Label)container.Q<Label>("PriceName");
        Label priceLabel = (Label)container.Q<Label>("priceLabel");

        AddElementPriceifNot0(groupPriceLabel, priceLabel, price);
        AddElementIfNotEmpty(groupType, typeLabel, armorTypeName);

        Label pdefLabel = container.Q<Label>("pdefLabel");
        VisualElement pdefGroup = container.Q<VisualElement>("pdeText");

        Label mdefLabel = container.Q<Label>("mdefLabel");
        VisualElement mdefGroup = container.Q<VisualElement>("mdeText");

        Label chancedefLabel = container.Q<Label>("chancedefLabel");
        VisualElement chancedefGroup = container.Q<VisualElement>("chancedeText");

        Label dexLabel = container.Q<Label>("dexLabel");
        VisualElement dexGroup = container.Q<VisualElement>("dexText");

        Label mpLabel = container.Q<Label>("mpLabel");
        VisualElement mpGroup = container.Q<VisualElement>("mpText");



        AddElementIfNot0(mpGroup, mpLabel, armor.MpBonus);
        AddElementIfNot0(pdefGroup, pdefLabel, armor.PDef);
        AddElementIfNot0(mdefGroup, mdefLabel, armor.MDef);
        AddElementIfNot0(chancedefGroup, chancedefLabel, 0);
        AddElementIfNot0(dexGroup, dexLabel, 0);
        AddElementIfNot0(dexGroup, dexLabel, 0);

        VisualElement footer_container = container.Q<VisualElement>("Footer");

        creator.AddSetsElement(footer_container, setsElements, setsEffects , text.GetSets() , text.GetSetsEffect());


        container.Q<Label>("weightlabel").text = armor.Weight.ToString();

        Label descriptionLabel = container.Q<Label>("descriptedLabel");
        AddElementIfNotEmpty(descriptionLabel, descriptionLabel, text.GetItemDiscription());

        //container.Q<Label>("descriptedLabel").text = text.GetItemDiscription();
    }

    protected void AddArmorShield(VisualElement container, int price, string armorTypeName , IDataTips text, Weapongrp armor)
    {
        //set icon
        VisualElement groupBoxIcon = container.Q<VisualElement>("GrowIcon");
        VisualElement icon = container.Q<VisualElement>("icon");
        AddNamedIconIfPresent(groupBoxIcon, icon, armor.Icon);

        container.Q<Label>("nameAccessories").text = text.GetName();

        VisualElement gradeBox = container.Q<VisualElement>("grade");
        Texture2D grade = text.GetGradeTexture();
        SetImageElement(gradeBox, grade);

        VisualElement groupBoxType = container.Q<VisualElement>("typeText");
        VisualElement groupBoxMDef = container.Q<VisualElement>("mdeText");

        EnabledRow(groupBoxType);
        EnabledRow(groupBoxMDef);



        VisualElement groupType = container.Q<VisualElement>("typeText");
        Label typeLabel = container.Q<Label>("typeLabel");

        Label groupPriceLabel = (Label)container.Q<Label>("PriceName");
        Label priceLabel = (Label)container.Q<Label>("priceLabel");

        Label labelEnchant = container.Q<Label>("enchant");
        AddElementEnchantifNot0(labelEnchant, labelEnchant, text.GetEnchant());

        AddElementPriceifNot0(groupPriceLabel, priceLabel, price);
        AddElementIfNotEmpty(groupType, typeLabel, armorTypeName);

        Label pdefLabel = container.Q<Label>("pdefLabel");
        VisualElement pdefGroup = container.Q<VisualElement>("pdeText");

        Label mdefLabel = container.Q<Label>("mdefLabel");
        VisualElement mdefGroup = container.Q<VisualElement>("mdeText");

        Label chancedefLabel = container.Q<Label>("chancedefLabel");
        VisualElement chancedefGroup = container.Q<VisualElement>("chancedeText");

        Label dexLabel = container.Q<Label>("dexLabel");
        VisualElement dexGroup = container.Q<VisualElement>("dexText");

        Label mpLabel = container.Q<Label>("mpLabel");
        VisualElement mpGroup = container.Q<VisualElement>("mpText");
        AddElementIfNot0(mpGroup, mpLabel, 0);

        AddElementIfNot0(pdefGroup, pdefLabel, armor.ShieldPdef);
        AddElementIfNot0(mdefGroup, mdefLabel, 0);
        AddElementIfNot0(chancedefGroup, chancedefLabel, armor.ShieldRate);
        AddElementIfNot0(dexGroup, dexLabel, armor.Dex);

        container.Q<Label>("weightlabel").text = armor.Weight.ToString();

        Label descriptionLabel = container.Q<Label>("descriptedLabel");
        AddElementIfNotEmpty(descriptionLabel, descriptionLabel, text.GetItemDiscription());


        //container.Q<Label>("descriptedLabel").text = text.GetItemDiscription();
    }

}
