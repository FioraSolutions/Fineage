using System;
using UnityEngine;
using System.Collections.Generic;

public partial class IconManager
{
    public Texture2D TryLoadTextureByName(string name, string iconFolder = null)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        var folder = iconFolder == null ? _iconFolder : iconFolder;
        string icon = folder.Replace('\\', '/') + "/" + CleanIconName(name);
        Texture2D texture = Resources.Load<Texture2D>(icon);
        ApplyUiSampling(texture);
        return texture;
    }

    public bool IsNoImage(Texture2D texture) =>
        texture != null && _noImageIcon != null && ReferenceEquals(texture, _noImageIcon);

    public Texture2D LoadTextureOtherSources(string href)
    {
        var result = Resources.Load<Texture2D>(href);

        //Debug.Log($"Loading icon {name}.");
        if (result != null)
        {
            ApplyUiSampling(result);
            return result;
        }

        // Debug.LogWarning($"Missing texture: {href}.");

        return _noImageIcon;
    }

    public Texture2D GetInvetoryDefaultBackground()
    {
        return LoadTextureByName(_defaultNameIconBackground);
    }

    public Texture2D GetCheckedCheckBoxTexture()
    {
        return Resources.Load<Texture2D>(_checkedCheckBoxTexture);

    }
}
