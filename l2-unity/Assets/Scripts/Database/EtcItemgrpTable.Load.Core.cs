using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class EtcItemgrpTable
{
    public static void Bind(EtcItemgrpTable table) {
        if (table != null)
            _instance = table;
    }

    public EtcItemgrp GetEtcItem(int id)
    {
        EtcItemgrp item;
        EtcItemGrps.TryGetValue(id, out item);
        return item;
    }
}
