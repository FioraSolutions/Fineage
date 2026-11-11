using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using VContainer;

public partial class EtcItemgrpTable
{
    private static EtcItemgrpTable _instance;

    [Obsolete("Use [Inject] EtcItemgrpTable or TablePacketServices.EtcItemGrps instead")]
    public static EtcItemgrpTable Instance {
        get {
            if (_instance == null && App.HasContainer) {
                try {
                    _instance = App.Resolve<EtcItemgrpTable>();
                } catch {
                }
            }

            if (_instance == null) {
                _instance = new EtcItemgrpTable();
            }

            return _instance;
        }
    }

    private Dictionary<int, EtcItemgrp> _etcItemGrps;
    public Dictionary<int, EtcItemgrp> EtcItemGrps { get { return _etcItemGrps; } }

    const int InterludeId = 1;
    const int InterludeDropType = 2;
    const int InterludeDropAnimType = 3;
    const int InterludeDropRadius = 4;
    const int InterludeDropHeight = 5;
    const int InterludeDropMesh1 = 7;
    const int InterludeDropTex1 = 10;
    const int InterludeIcon = 13;
    const int InterludeEquipMesh = 24;
}
