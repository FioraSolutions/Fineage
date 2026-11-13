using System;
using System.Collections.Generic;
using VContainer;
using System.Collections;
using System.IO;
using UnityEngine;

public partial class SysStringTable
{
    private static SysStringTable _instance;
    private Dictionary<int, SysStringData> _strings;

    [Obsolete("Use [Inject] / PacketServices instead")]
    public static SysStringTable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SysStringTable();
            }

            return _instance;
        }
    }

    /// <summary>Interlude <c>&amp;$id;</c> lookup. Empty when the id is missing.</summary>
    public string Get(int id)
    {
        if (_strings == null)
            return "";
        return _strings.TryGetValue(id, out SysStringData data) && data != null
            ? (data.Name ?? "")
            : "";
    }
}
