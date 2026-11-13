using System;

using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements.Experimental;
using VContainer;

public partial class World : MonoBehaviour, IWorldSpawnContext, IWorldEntityLookup
{
    void Start() {
        if (_deadManager != null)
            _deadManager.OnReadyToRemove += RemoveObject;
        if (GetComponent<L2VillageStatuePanner>() == null)
            gameObject.AddComponent<L2VillageStatuePanner>();
        if (GetComponent<L2CaveTerrainGate>() == null)
            gameObject.AddComponent<L2CaveTerrainGate>();
        if (GetComponent<L2DarkElfVillageMusic>() == null)
            gameObject.AddComponent<L2DarkElfVillageMusic>();
        UpdateMasks();
    }

    public void UpdateMasks() {
        NameplatesManager nameplates = NameplatePacketServices.Nameplates;
        if (nameplates != null)
            nameplates.SetMask(_entityMask);
        if (_geodata != null)
            _geodata.ObstacleMask = _obstacleMask;
        ClickManager clicks = ControllerPacketServices.Clicks;
        if (clicks != null)
        {
            LayerMask dropMask = _dropLayers != null ? _dropLayers.Mask : 0;
            clicks.SetMasks(_entityClickAreaMask | dropMask, _clickThroughMask);
        }
        if (_camera != null)
            _camera.SetMask(_obstacleMask);
    }

    public void ClearEntities() {
        _entities.Clear();
        _entityWait.CancelAll();
    }

    /// <summary>Cancel GetEntityAsync waiters without tearing down spawned objects (scene unload owns GO teardown).</summary>
    public void CancelPendingEntityWaits() => _entityWait.CancelAll();

    public bool ContainsNpc(int id)
    {
        return _entities.ContainsNpc(id);
    }
}
