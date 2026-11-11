using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

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

}
