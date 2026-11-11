using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class ItemTable
{
    private static ItemTable _instance;

    [Obsolete("Use [Inject] ItemTable or TablePacketServices.Items instead")]
    public static ItemTable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ItemTable();
            }

            return _instance;
        }
    }

    [SerializeField] private List<int> weaponsToLoad = new List<int>();
    [SerializeField] private List<int> armorsToLoad = new List<int>();
    [SerializeField] private List<int> itemsToLoad = new List<int>();

    private Dictionary<int, Weapon> _weapons = new Dictionary<int, Weapon>();
    private Dictionary<int, Armor> _armors = new Dictionary<int, Armor>();
    private Dictionary<int, EtcItem> _etcItems = new Dictionary<int, EtcItem>();

    public Dictionary<int, Weapon> Weapons { get { return _weapons; } }
    public Dictionary<int, Armor> Armors { get { return _armors; } }
    public Dictionary<int, EtcItem> EtcItems { get { return _etcItems; } }

    public static int NAKED_CHEST = 21;
    public static int NAKED_LEGS = 28;
    public static int NAKED_GLOVES = 48;
    public static int NAKED_BOOTS = 35;
    private bool _loadAll = true;












}
