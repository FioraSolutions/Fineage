using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Globalization;
using VContainer;

public partial class ItemStatDataTable
{
    private static ItemStatDataTable _instance;

    [Obsolete("Use [Inject] ItemStatDataTable or TablePacketServices.ItemStats instead")]
    public static ItemStatDataTable Instance {
        get {
            if (_instance == null && App.HasContainer) {
                try {
                    _instance = App.Resolve<ItemStatDataTable>();
                } catch {
                }
            }

            if (_instance == null) {
                _instance = new ItemStatDataTable();
            }

            return _instance;
        }
    }


    private Dictionary<int, ItemStatData> _itemStatData;
    public Dictionary<int, ItemStatData> ItemsStatData { get { return _itemStatData; } }



}
