using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class WeapongrpTable
{
    private static WeapongrpTable _instance;

    [Obsolete("Use [Inject] WeapongrpTable or TablePacketServices.WeaponGrps instead")]
    public static WeapongrpTable Instance {
        get {
            if (_instance == null && App.HasContainer) {
                try {
                    _instance = App.Resolve<WeapongrpTable>();
                } catch {
                }
            }

            if (_instance == null) {
                _instance = new WeapongrpTable();
            }

            return _instance;
        }
    }


    private Dictionary<int, Weapongrp> _weaponGrps;
    public Dictionary<int, Weapongrp> WeaponGrps { get { return _weaponGrps; } }




    

    //41 P atk
    //42 M atk
    //45 Crit rate
    //50 P tak speed
    //51 Mana (MpConsume)
    //52 Soulshot
    //53 SpiritShot

    //49 - shield_rate
    //47 - dex
    //48 - shield_pdef
}
