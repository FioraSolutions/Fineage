using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

public partial class PlayerInventory : MonoBehaviour
{

    private const string TYPE_RECIPE = "recipe";
    private int _objectIdEtcType = -1;
    private Dictionary<int , ItemInstance> _playerInventory;
    private Dictionary<int, ItemInstance> _playerEquipInventory;
    private ChangeInventoryData _changeInventoryData;
    public static PlayerInventory _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static PlayerInventory Instance { get { return _instance; } }



}
