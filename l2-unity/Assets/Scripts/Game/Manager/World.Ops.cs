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
        INameplateRoster nameplates = Nameplates();
        if (nameplates != null)
            nameplates.SetMask(_entityMask);
        if (_geodata != null)
            _geodata.ObstacleMask = _obstacleMask;
        IClickMasks clicks = Clicks();
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

    static bool _loggedMissingNameplates;

    static INameplateRoster Nameplates()
    {
        INameplateRoster nameplates = NameplatePacketServices.BoundNameplates;
        if (nameplates == null && !_loggedMissingNameplates)
        {
            _loggedMissingNameplates = true;
            Debug.LogError("[World] INameplateRoster missing — NameplatesManager.Awake did not bind, so nameplates were not updated.");
        }

        return nameplates;
    }

    static bool _loggedMissingClicks;

    static bool _loggedMissingMoves;

    static IMoveRoster Moves()
    {
        IMoveRoster moves = MoveAllCharactersPacketServices.BoundMoves;
        if (moves == null && !_loggedMissingMoves)
        {
            _loggedMissingMoves = true;
            Debug.LogError("[World] IMoveRoster missing — MoveAllCharacters.Awake did not bind, so character moves were not updated.");
        }

        return moves;
    }

    static IClickMasks Clicks()
    {
        IClickMasks clicks = ControllerPacketServices.BoundClicks;
        if (clicks == null && !_loggedMissingClicks)
        {
            _loggedMissingClicks = true;
            Debug.LogError("[World] IClickMasks missing — ClickManager.Awake did not bind, so click masks were not updated.");
        }

        return clicks;
    }
}
