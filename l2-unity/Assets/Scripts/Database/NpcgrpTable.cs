using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using VContainer;

public partial class NpcgrpTable
{
    private static NpcgrpTable _instance;

    [Obsolete("Use [Inject] NpcgrpTable or TablePacketServices.NpcGrps instead")]
    public static NpcgrpTable Instance {
        get {
            if (_instance == null && App.HasContainer) {
                try {
                    _instance = App.Resolve<NpcgrpTable>();
                } catch {
                }
            }

            if (_instance == null) {
                _instance = new NpcgrpTable();
            }

            return _instance;
        }
    }


    private Dictionary<int, Npcgrp> _npcgrps;
    private NpcDecoEffectsTable _decoEffects;

    public Dictionary<int, Npcgrp> Npcgrps { get { return _npcgrps; } }





    /// <summary>
    /// property_list={4416;13;47399;1;…} — alternating skillId;level (Interlude TargetStatus icons).
    /// </summary>
}
