using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class WeapongrpTable
{
    public Weapongrp GetWeapon(int id)
    {
        if (_weaponGrps.ContainsKey(id))
        {
            return _weaponGrps[id];
        }
        return null;
    }
}
