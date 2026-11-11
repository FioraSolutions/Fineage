public sealed partial class ItemDropGrpCatalog
{
    public ItemDropGrpCatalog(ItemTable items)
    {
        _items = items;
    }

    public Abstractgrp ResolveGrp(int itemId)
    {
        ItemTable items = _items != null ? _items : TablePacketServices.Items;
        if (items == null)
            return null;

        Weapon weapon = items.GetWeapon(itemId);
        if (weapon != null && weapon.Weapongrp != null)
            return weapon.Weapongrp;

        Armor armor = items.GetArmor(itemId);
        if (armor != null && armor.Armorgrp != null)
            return armor.Armorgrp;

        EtcItem etc = items.GetEtcItem(itemId);
        if (etc != null && etc.EtcItemgrp != null)
            return etc.EtcItemgrp;

        return null;
    }

    public string ResolveEquipModel(Abstractgrp grp)
    {
        if (grp is Weapongrp weapon)
            return weapon.Model;
        if (grp is EtcItemgrp etc)
            return etc.Model;
        return null;
    }

    public bool IsAdenaDropVisual(int itemId)
    {
        Abstractgrp grp = ResolveGrp(itemId);
        if (grp is EtcItemgrp etc && etc.ConsumeType == ConsumeCategory.Asset)
            return true;
        return IsCoinDropModel(grp != null ? grp.DropModel : null);
    }
}
