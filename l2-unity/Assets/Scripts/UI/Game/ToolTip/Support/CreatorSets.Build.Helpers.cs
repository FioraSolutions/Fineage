public partial class CreatorSets
{
    private void CreateSetsEffect(int lastIndex, VisualElement container, VisualTreeAsset effectTemplate, ItemSets[] setsEffect)
    {
        if(effectTemplate != null)
        {
            for (int i = 0; i < setsEffect.Length; i++)
            {

                ItemSets itemName = setsEffect[i];
                string peace = GetTextPeace(itemName);
                container.Insert(lastIndex, CreateVisualElementEffect(peace, itemName.GetDescription(), effectTemplate));
                lastIndex++;
            }
        }
   
    }

    private string GetTextPeace(ItemSets itemName)
    {
        if(itemName.GetArrayId().Length > 1)
        {
            return itemName.GetArrayId().Length.ToString() + "Piece";
        }
        else if (itemName.GetArrayId().Length == 1)
        {
            int id_i = itemName.GetArrayId()[0];
            Weapongrp weapon = TablePacketServices.WeaponGrps.GetWeapon(id_i);
            Armorgrp armor = TablePacketServices.ArmorGrps.GetArmor(id_i);

            if (weapon != null && weapon.WeaponType == WeaponType.shield)
                return "Shield";

            if(armor != null)
            {
                if (weapon.BodyPart == ItemSlot.gloves)
                {
                    return "Gloves";
                }
                else if (weapon.BodyPart == ItemSlot.head)
                {
                    return "Helmet";
                }
            }

            return itemName.GetArrayId().Length.ToString() + "Piece";
        }
        return "";
    }

    private TemplateContainer CreateVisualElementEffect(string peace , string text , VisualTreeAsset effectTemplate)
    {
        var template = effectTemplate.CloneTree();
        AddCountPiece(peace, template);
        AddTextEffect(text, template);

        return template;
    }
}
