using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public sealed partial class WeaponShotTable
{
    const string RelativePath = "Data/Meta/weapon_shot.int";

    static WeaponShotTable _instance;

    readonly Dictionary<string, string> _prefabPaths =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    readonly Dictionary<string, string> _spiritPrefabPaths =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    readonly Dictionary<string, BaseEffect> _loaded =
        new Dictionary<string, BaseEffect>(StringComparer.OrdinalIgnoreCase);

    [System.Obsolete("Use TablePacketServices.WeaponShots instead")]
        public static WeaponShotTable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new WeaponShotTable();
                TablePacketServices.Bind(weaponShots: _instance);
            }
            return _instance;
        }
    }


    WeaponShotTable()
    {
        Load();
    }





}
