using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using VContainer;

public partial class ArmorgrpTable
{
    private static ArmorgrpTable _instance;

    [Obsolete("Use [Inject] ArmorgrpTable or TablePacketServices.ArmorGrps instead")]
    public static ArmorgrpTable Instance {
        get {
            if (_instance == null && App.HasContainer) {
                try {
                    _instance = App.Resolve<ArmorgrpTable>();
                } catch {
                }
            }

            if (_instance == null) {
                _instance = new ArmorgrpTable();
            }

            return _instance;
        }
    }


    private Dictionary<int, Armorgrp> _armorgrps;
    public Dictionary<int, Armorgrp> ArmorGrps { get { return _armorgrps; } }

    private int indexPdef = 329;
    private int indexMdef = 330;
    private int indexMpBouns = 331;

    bool IsIndexValid<T>(T[] array, int index)
    {
        return index >= 0 && index < array.Length;
    }
}
