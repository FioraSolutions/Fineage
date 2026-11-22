using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

public partial class PlayerInventory : MonoBehaviour
{

    private const string TYPE_RECIPE = "recipe";
    private int _objectIdEtcType = -1;
    private CharacterInventory _book;
    private Dictionary<int , ItemInstance> _playerInventory;
    private Dictionary<int, ItemInstance> _playerEquipInventory;
    private UserInfoDto _currentUser;
    private readonly object _lock = new object();
    public static PlayerInventory _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static PlayerInventory Instance { get { return _instance; } }
}
