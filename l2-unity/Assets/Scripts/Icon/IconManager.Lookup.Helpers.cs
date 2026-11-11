using System;
using UnityEngine;
using System.Collections.Generic;

public partial class IconManager
{
    public Texture2D GetUncheckedCheckBoxTexture()
    {
        return Resources.Load<Texture2D>(_uncheckedCheckBoxTexture);
    }

    public Texture2D LoadCursorByName(string name)
    {
        // Same slash normalization as LoadTextureByName — Resources.Load on macOS
        // does not resolve Windows-style backslashes, so SetCursor got null (OS cursor).
        string icon = _cursorFolder.Replace('\\', '/') + "/" + name;
        return Resources.Load<Texture2D>(icon);
    }

    public Texture2D GetIcon(int id)
    {
        Texture2D icon;
        _icons.TryGetValue(id, out icon);

        if (icon == null)
        {
            _icons.Add(id, _noImageIcon);
            return _noImageIcon;
        }

        return icon;
    }

    public Texture2D GetOtherIcon(int id , int indexIcon)
    {
        Texture2D[] alIcon;
        _otherIcons.TryGetValue(id, out alIcon);

        if (alIcon == null)
        {
            _icons.Add(id, _noImageIcon);
            return _noImageIcon;
        }

        if(ArrayUtils.IsValidIndexArray(alIcon, indexIcon))
        {
            return alIcon[indexIcon];
        }

        return null;
    }

    public Texture2D GetOtherIconByType(int id , int type)
    {
        return type == 0 && _otherIconsCooltime.TryGetValue(id, out Texture2D icon) ? icon : null;
    }

    private Texture2D GetNoImageIcon()
    {
        return Resources.Load<Texture2D>(_iconFolder + "\\" + "NOIMAGE");
    }
}
