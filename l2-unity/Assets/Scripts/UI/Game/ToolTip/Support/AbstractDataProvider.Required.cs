using UnityEngine;
using UnityEngine.UIElements;

public abstract partial class AbstractDataProvider
{
    public void SetRequired(VisualElement container, IDataTips text, string etcIcon, int priceItem, string accessoriesName, string weight , int requiredCount , int countInventory)
    {
        SetOther(container, text, etcIcon, priceItem, accessoriesName, weight);
        SetRequiredField(container , requiredCount , countInventory);
    }

    private void SetRequiredField(VisualElement container, int requiredCount , int inventoryCount)
    {
        Label groupRequiredLabel = (Label)container.Q<Label>("RequredName");
        Label RequiredLabel = (Label)container.Q<Label>("requiredLabel");
        AddElementIfNotEmpty(groupRequiredLabel, RequiredLabel, requiredCount.ToString());


        Label groupInStockLabel = (Label)container.Q<Label>("InStockName");
        Label inStockLabel = (Label)container.Q<Label>("inStockLabel");
        AddElementIfNotEmpty(groupInStockLabel, inStockLabel, inventoryCount.ToString());
        
    }

    protected void SetDataIngredient(VisualElement container, IDataTips text, string icon, int count, string name)
    {

        VisualElement gradeBox = container.Q<VisualElement>("grade");
        Texture2D grade = text.GetGradeTexture();
        SetImageElement(gradeBox, grade);

        VisualElement groupIcon = container.Q<VisualElement>("GrowIcon");
        var iconElement = container.Q<VisualElement>("icon");
        AddNamedIconIfPresent(groupIcon, iconElement, icon);

        VisualElement groupPriceLabel = container.Q<VisualElement>("GrowPrice");
        Label priceLabel = (Label)container.Q<Label>("price");
        AddElementCountXifNot0(groupPriceLabel, priceLabel, count);

        VisualElement groupName = container.Q<VisualElement>("GrowName");
        Label labelName = container.Q<Label>("nameLabel");
        AddElementIfNotEmpty(groupName, labelName, name);
    }

}
