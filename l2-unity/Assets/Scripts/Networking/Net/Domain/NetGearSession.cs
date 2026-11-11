using System;
using System.Collections.Generic;
using Fineage.Net.Contract;
using UnityEngine;

public static partial class NetGearSession
{
    static bool _hasUser;
    static NetUserInfo _user;
    static bool _hasItems;
    static bool _itemsApplied;
    static NetItemEntry[] _items;
    static readonly List<NetInventoryChange[]> _updates = new List<NetInventoryChange[]>();
    static bool _warned;



    /// <summary>
    /// Publishes the local player into <see cref="StorageNpc"/>. Prefers the transport's
    /// PlayerInfo (the full record the zone filled in), falling back to the live PlayerEntity
    /// and finally to a minimal record built from the UserInfo message itself.
    /// </summary>



}
