using System;

public partial class IconManager
{
    public void Initialize()
    {
        _noImageIcon = GetNoImageIcon();
    }

    public void CacheIcons()
    {
        var weaponValues = TablePacketServices.Items.Weapons.Values;
        var armorValues = TablePacketServices.Items.Armors.Values;
        var etcalues = TablePacketServices.Items.EtcItems.Values;

        foreach (Weapon weapon in TablePacketServices.Items.Weapons.Values)
        {
            Texture2D icon = LoadTextureByName(weapon.Icon);
            _icons.Add(weapon.Id, icon);
        }

        foreach (Armor armor in TablePacketServices.Items.Armors.Values)
        {
            if(armor.Id == 1146)
            {
                // Debug.Log("");
            }
            Texture2D icon = LoadTextureByName(armor.Icon);
            _icons.Add(armor.Id, icon);
        }

        foreach (EtcItem etcItem in TablePacketServices.Items.EtcItems.Values)
        {
            Texture2D icon = LoadTextureByName(etcItem.Icon);
            if (!_icons.ContainsKey(etcItem.Id))
            {
                _icons.Add(etcItem.Id, icon);
            }
            
        }
    }

    public void CacheOtherIcons()
    {

        LoadAllOther();
        LoadAllCoolTime();
    }

    private void LoadAllOther()
    {
        foreach (Armor armor in TablePacketServices.Items.Armors.Values)
        {
            string[] otherIcon = armor.Itemgrp.OtherIcon;

            if (otherIcon != null)
            {
                Texture2D[] arrayIcon = new Texture2D[otherIcon.Length];

                if (otherIcon.Length > 1)
                {
                    for (int n = 0; n < otherIcon.Length; n++)
                    {
                        arrayIcon[n] = LoadTextureByName(otherIcon[n]);
                    }

                    _otherIcons.Add(armor.Id, arrayIcon);
                }
            }

        }
    }
}
