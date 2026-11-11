using System;

﻿using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class IconManager
{
    private const string _iconFolder = "Data\\SysTextures\\Icon";
    private const string _cursorFolder = "Data\\UI\\Assets\\Cursor";
    private const string _defaultNameIconBackground = "ItemWindow_DF_SlotBox_Default";
    private const string _checkedCheckBoxTexture = "Data/UI/Quest/CheckBox_checked";
    private const string _uncheckedCheckBoxTexture = "Data/UI/Quest/CheckBox_uncheked";
    


    private Texture2D _noImageIcon;

    private Dictionary<int, Texture2D> _icons = new Dictionary<int, Texture2D>();
    private Dictionary<int, Texture2D[]> _otherIcons = new Dictionary<int, Texture2D[]>();
    private Dictionary<int, Texture2D> _otherIconsCooltime = new Dictionary<int, Texture2D>();
    private Dictionary<string, Texture2D> _interfaceIcon = new Dictionary<string, Texture2D>();


    private static IconManager _instance;

    [System.Obsolete("Use IconPacketServices.Icons or [Inject] IconManager instead")]
    public static IconManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new IconManager();
                IconPacketServices.Bind(icons: _instance);
            }

            return _instance;
        }
    }


    private string CleanIconName(string name) => name?.Replace("icon.", "") ?? "";

}
