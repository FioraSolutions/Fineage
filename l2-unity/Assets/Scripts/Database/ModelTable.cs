using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class ModelTable : AbstractCache
{
    private static ModelTable _instance;

    [Obsolete("Use [Inject] ModelTable or TablePacketServices.Models instead")]
    public static ModelTable Instance
    {
        get
        {
            if (_instance == null)
                _instance = new ModelTable();
            return _instance;
        }
    }

}
