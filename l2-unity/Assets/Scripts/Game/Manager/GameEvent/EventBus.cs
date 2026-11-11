using System;

using UnityEngine;
using VContainer;

public partial class EventBus : MonoBehaviour
{
    private static EventBus _instance;
    [System.Obsolete("Use [Inject] or GamePacketServices.Bus instead")]
    public static EventBus Instance { get { return _instance; } }

    public event Action<Entity, int> OnLevelUp;
    public event Action<ItemInstance , int> OnEquipped;
    public event Action<ItemInstance, int> OnWeaponChanged;
    public event Action<ItemInstance , int> OnUnEquipped;








}
