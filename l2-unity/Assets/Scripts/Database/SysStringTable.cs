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


}
