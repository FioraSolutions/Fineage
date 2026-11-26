using System;

using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements.Experimental;
using VContainer;

public partial class World : MonoBehaviour, IWorldSpawnRegistration, IWorldEntityLookup
{
    void Start() {
        if (_deadManager != null)
            _deadManager.OnReadyToRemove += RemoveObject;
        if (GetComponent<L2Ue2MaterialRuntime>() == null)
            gameObject.AddComponent<L2Ue2MaterialRuntime>();
        if (GetComponent<L2CaveTerrainGate>() == null)
            gameObject.AddComponent<L2CaveTerrainGate>();
        if (GetComponent<L2DoorPassage>() == null)
            gameObject.AddComponent<L2DoorPassage>();
        if (GetComponent<L2BrushPortal>() == null)
            gameObject.AddComponent<L2BrushPortal>();
        if (GetComponent<OfficialMusicVolumePlayback>() == null)
            gameObject.AddComponent<OfficialMusicVolumePlayback>();
        if (GetComponent<OfficialAmbientSoundPlayback>() == null)
            gameObject.AddComponent<OfficialAmbientSoundPlayback>();
        if (GetComponent<OfficialWorldEmitterPlayback>() == null)
            gameObject.AddComponent<OfficialWorldEmitterPlayback>();
        if (GetComponent<L2FlameAnim>() == null)
            gameObject.AddComponent<L2FlameAnim>();
        UpdateMasks();
    }

    public void UpdateMasks() {
        _nameplates.SetMask(_entityMask);
        if (_geodata != null)
            _geodata.ObstacleMask = _obstacleMask;
        LayerMask dropMask = _dropLayers != null ? _dropLayers.Mask : 0;
        _clicks.SetMasks(_entityClickAreaMask | dropMask, _clickThroughMask);
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
