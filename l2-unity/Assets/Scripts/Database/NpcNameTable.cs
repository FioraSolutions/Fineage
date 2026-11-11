using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VContainer;

public partial class NpcNameTable
{
    private static NpcNameTable _instance;

    [Obsolete("Use [Inject] NpcNameTable or TablePacketServices.NpcNames instead")]
    public static NpcNameTable Instance {
        get {
            if (_instance == null && App.HasContainer) {
                try {
                    _instance = App.Resolve<NpcNameTable>();
                } catch {
                }
            }

            if (_instance == null) {
                _instance = new NpcNameTable();
            }

            return _instance;
        }
    }


    private Dictionary<int, NpcName> _npcNames;

    public Dictionary<int, NpcName> NpcNames { get { return _npcNames; } }



}
