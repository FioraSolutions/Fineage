using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VContainer;

public partial class ItemNameTable
{
    private static ItemNameTable _instance;

    [Obsolete("Use [Inject] ItemNameTable or TablePacketServices.Items instead")]
    public static ItemNameTable Instance {
        get {
            if (_instance == null && App.HasContainer) {
                try {
                    _instance = App.Resolve<ItemNameTable>();
                } catch {
                }
            }

            if (_instance == null) {
                _instance = new ItemNameTable();
            }

            return _instance;
        }
    }

    private Dictionary<int, ItemName> _itemNames;
    public Dictionary<int, ItemName> ItemNames { get { return _itemNames; } }
}
