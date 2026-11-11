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
}
