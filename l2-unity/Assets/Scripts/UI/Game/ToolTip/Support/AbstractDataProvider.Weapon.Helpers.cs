using UnityEngine;
using UnityEngine.UIElements;

public abstract partial class AbstractDataProvider
{
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
