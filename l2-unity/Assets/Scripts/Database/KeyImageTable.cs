using System;

﻿using System.Collections.Generic;
using UnityEngine;

public class KeyImageTable
{
    private string _iconFolder = "Data\\UI\\Assets\\SkillBar\\Shortcuts";
    private Dictionary<string, Texture2D> _keyImages = new Dictionary<string, Texture2D>();

    private static KeyImageTable _instance;

    [System.Obsolete("Use TablePacketServices.KeyImages instead")]
        public static KeyImageTable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new KeyImageTable();
                TablePacketServices.Bind(keyImages: _instance);
            }

            return _instance;
        }
    }

    public static void Bind(KeyImageTable table)
    {
        if (table == null)
            return;
        _instance = table;
        TablePacketServices.Bind(keyImages: table);
    }

    public void Initialize()
    {
        _keyImages = new Dictionary<string, Texture2D>();
        Bind(this);
    }

    public Texture2D LoadTextureByKey(string name)
    {
        if (_keyImages.TryGetValue(name, out Texture2D result))
        {
            return result;
        }

        string icon = _iconFolder + "\\ShortcutWnd_DF_Key_" + name;
        result = Resources.Load<Texture2D>(icon);

        if (result != null)
        {
            _keyImages.Add(name, result);
            return result;
        }

        // Debug.LogWarning($"Missing icon: {name}.");

        return null;
    }
}
