using UnityEngine;

public static class AbstractgrpDatParser
{
    public static bool ParseBaseAbstractItemGrpDat(Abstractgrp abstractgrp, string key, string value) {

        switch (key) {
            case "object_id":
                abstractgrp.ObjectId = int.Parse(value);
                break;
            case "drop_texture": //{{[dropitems.drop_mfighter_m001_t02_u_m00];{[MFighter.MFighter_m001_t02_u]}}}
                string[] modTex = DatUtils.ParseArray(value);
                abstractgrp.DropTexture = modTex[1];
                abstractgrp.DropModel = modTex[0];
                break;
            case "icon": // {[icon.armor_t02_u_i00];[None];[None];[None];[None]}
                if(abstractgrp.ObjectId == 356)
                {
                    // Debug.Log("");
                }
                var allIcon = DatUtils.ParseOtherIcon(value);
                abstractgrp.Icon = allIcon[0];
                if (allIcon.Length > 1) abstractgrp.SetOtherIcon(allIcon);
                break;
            case "weight":
                abstractgrp.Weight = int.Parse(value);
                break;
            case "material_type":
                abstractgrp.Material = ItemMaterialParser.Parse(value);
                break;
            case "crystal_type": //crystal_type=d
                abstractgrp.Grade = ItemGradeParser.Parse(value);
                break;
            case "drop_sound": // [ItemSound.itemdrop_dagger]
                abstractgrp.DropSound = DatUtils.CleanupString(value);
                break;
            case "crystallizable":
                abstractgrp.Crystallizable = value == "1";
                break;
            case "equip_sound":
                abstractgrp.EquipSound = DatUtils.CleanupString(value);
                break;
            case "inventory_type":
                abstractgrp.InventoryType = value;
                break;
            case "drop_type":
                abstractgrp.DropType = int.Parse(value);
                break;
            case "drop_anim_type":
                abstractgrp.DropAnimType = int.Parse(value);
                break;
            case "drop_radius":
                abstractgrp.DropRadius = int.Parse(value);
                break;
            case "drop_height":
                abstractgrp.DropHeight = int.Parse(value);
                break;
            default:
                return false;
        }


        return true;
    }
}
