public partial class CreatorSets
{
    public void AddSetsElement(VisualElement container, VisualTreeAsset setsTemplate, VisualTreeAsset setsEffectsTemplate, ItemName[] setsName , ItemSets[] setsEffects)
    {
        string element = "setsElements";
        string effects = "setsEffect";

        if (setsName != null && setsName.Length > 0)
        {

            RemoveSetsElements(container, element);
            RemoveSetsElements(container, effects);
            int lastIndex = CreateSetsPart(container, setsTemplate, setsName);
            CreateSetsEffect(lastIndex , container, setsEffectsTemplate, setsEffects);
        }
        else
        {
            RemoveSetsElements(container, effects);
            RemoveSetsElements(container, element);
        }
    }

    private void RemoveSetsElements(VisualElement container, string nameElement)
    {
        var elementsToRemove = container.Query<VisualElement>(nameElement).ToList();

        if (elementsToRemove != null && elementsToRemove.Count > 0)
        {
            foreach (var element in elementsToRemove)
            {
                element.RemoveFromHierarchy();

            }
        }
    }

    private int CreateSetsPart(VisualElement container, VisualTreeAsset setsTemplate, ItemName[] setsName)
    {
        int index = 0;
        for (int i = 0; i < setsName.Length; i++)
        {
            ItemName itemName = setsName[i];
            int id = itemName.Id;
            var weapon = TablePacketServices.WeaponGrps.GetWeapon(id);
            var armor = TablePacketServices.ArmorGrps.GetArmor(id);
            var item = TablePacketServices.EtcItemGrps.GetEtcItem(id);
            bool isEquip = InventoryPacketServices.Inventory.IsItemEquipByItemId(id);

            if (weapon != null)
            {
                if(setsTemplate != null)
                {
                    container.Insert(i, CreateVisualElement(i, weapon, itemName.Name, setsTemplate, isEquip));
                }
            }
            else if (armor != null)
            {
                if (setsTemplate != null)
                {
                    container.Insert(i, CreateVisualElement(i, armor, itemName.Name, setsTemplate, isEquip));
                }
                    
            }
            else if (item != null)
            {
                if (setsTemplate != null)
                {
                    container.Insert(i, CreateVisualElement(i, item, itemName.Name, setsTemplate, isEquip));
                }
                    
            }

            index = i;
        }

        return index + 1;
    }
}
