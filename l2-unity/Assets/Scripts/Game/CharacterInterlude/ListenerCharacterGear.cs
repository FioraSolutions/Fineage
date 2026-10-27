using UnityEngine;
using VContainer;

public class ListenerCharacterGear : MonoBehaviour
{

    public static ListenerCharacterGear Instance { get; private set; }

    [Inject] EventBus _bus;
    [Inject] World _world;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            App.InjectGameObject(gameObject);
            WireGamePacketServices();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        WireGamePacketServices();
    }

    private EventBus Bus
    {
        get { return _bus != null ? _bus : GamePacketServices.Bus; }
    }

    private World GameWorld
    {
        get { return _world != null ? _world : GamePacketServices.GameWorld; }
    }

    private void WireGamePacketServices()
    {
        GamePacketServices.Bind(bus: _bus, gameWorld: _world);
    }

    private void OnEnable()
    {
        if (Bus != null)
        {
            Bus.OnEquipped += HandleItemEquipped;
            Bus.OnUnEquipped += HandleItemUnequipped;
            // GearFlowLog.Info("ListenerCharacterGear subscribe busSameAsInstance=" +
                // (Bus == EventBus.Instance) +
                // " injected=" + (_bus != null));
        }
        else
        {
            // GearFlowLog.Warn("ListenerCharacterGear OnEnable Bus=null — inventory will not dress PlayerEntity");
        }
    }

    private void OnDisable()
    {
        if (Bus != null)
        {
            Bus.OnEquipped -= HandleItemEquipped;
            Bus.OnUnEquipped -= HandleItemUnequipped;
        }
    }

    private void HandleItemEquipped(ItemInstance item , int objectId)
    {
        Entity entity = GameWorld != null ? GameWorld.GetEntityNoLockSync(objectId) : null;
        PlayerEntity local = PlayerEntity.Instance;
        int localId = local != null && local.Identity != null ? local.Identity.Id : 0;
        // GearFlowLog.Info("Inventory EQUIP objectId=" + objectId +
            // " localPlayerId=" + localId +
            // " itemId=" + (item != null ? item.ItemId : 0) +
            // " cat=" + (item != null ? item.Category.ToString() : "null") +
            // " body=" + (item != null ? item.BodyPart.ToString() : "null") +
            // " " + GearFlowLog.Entity(entity));
        // if (entity == null)
            // GearFlowLog.Warn("Inventory EQUIP lookup miss objectId=" + objectId +
                // " worldNull=" + (GameWorld == null));
        entity?.EquipAndDetermineType(item, objectId);
    }

    private void HandleItemUnequipped(ItemInstance item, int objectId)
    {
        Entity entity = GameWorld != null ? GameWorld.GetEntityNoLockSync(objectId) : null;
        // GearFlowLog.Info("Inventory UNEQUIP objectId=" + objectId +
            // " itemId=" + (item != null ? item.ItemId : 0) +
            // " cat=" + (item != null ? item.Category.ToString() : "null") +
            // " entity=" + (entity != null ? entity.GetType().Name : "null"));
        entity?.UnequipAndDetermineType(item);
    }

}
