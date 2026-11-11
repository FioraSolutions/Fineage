using System;
using UnityEngine;
using System.Collections.Generic;

public partial class IconManager
{
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
