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

    private void LoadAllCoolTime()
    {
        int cooldownImageCount = 356;
        // Resources folder is lowercase skill_time; backslashes break Load on macOS/Linux.
        string cooldownPath = "Data/UI/ShortCut/skill_time";
        string coolTimeName = "cooltime";

        for (int i = 1; i <= cooldownImageCount; i++)
        {
            string iconName = $"{coolTimeName}{i}";
            Texture2D icon = LoadTextureByName(iconName, cooldownPath);

            if (icon == null || icon.name == "NOIMAGE")
                continue;

            if (!_otherIconsCooltime.ContainsKey(i))
                _otherIconsCooltime.Add(i, icon);
        }
    }

    public int GetSizeOtherCoolTime()
    {
        return _otherIconsCooltime.Count;
    }

    public void CacheInterfaceIcons()
    {
        var grade_a = LoadTextureByName("grade_a");
        var grade_b = LoadTextureByName("grade_b");
        var grade_c = LoadTextureByName("grade_c");
        var grade_d = LoadTextureByName("grade_d");
        var grade_s = LoadTextureByName("grade_s");
        _interfaceIcon.Add("grade_a", grade_a);
        _interfaceIcon.Add("grade_b", grade_b);
        _interfaceIcon.Add("grade_c", grade_c);
        _interfaceIcon.Add("grade_d", grade_d);
        _interfaceIcon.Add("grade_s", grade_s);
    }

    public Texture2D GetInterfaceIcon(string nameIcon)
    {
        if (_interfaceIcon.ContainsKey(nameIcon))
        {
            return _interfaceIcon[nameIcon];
        }
        return _noImageIcon;
    }

    public Texture2D LoadTextureByName(string name , string iconFolder = null)
    {
        Texture2D found = TryLoadTextureByName(name, iconFolder);
        return found != null ? found : _noImageIcon;
    }
}
