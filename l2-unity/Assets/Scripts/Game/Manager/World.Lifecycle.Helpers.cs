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
    [Inject]
    void BindItemDropPresentationRunner()
    {
        ItemDropPresentationRunner runner = GetComponent<ItemDropPresentationRunner>();
        if (runner == null)
            runner = gameObject.AddComponent<ItemDropPresentationRunner>();
        if (_itemDropPresentation != null)
            runner.Bind(_itemDropPresentation);
    }

    [Inject]
    void BindZoneTransitionPlayers(ILocalPlayerSlot players)
    {
        EnsureZoneTransition(players);
    }

    void BindZoneTransitionRunner()
    {
        EnsureZoneTransition(_localPlayer);
    }

    void EnsureZoneTransition(ILocalPlayerSlot players)
    {
        ZoneTransitionService service = GetComponent<ZoneTransitionService>();
        if (service == null)
            service = gameObject.AddComponent<ZoneTransitionService>();
        ZoneNameBannerOverlay banner = GetComponent<ZoneNameBannerOverlay>();
        if (banner == null)
            banner = gameObject.AddComponent<ZoneNameBannerOverlay>();
        if (players == null)
            return;

        service.Use(players, banner);
    }

    void OnDestroy() {
        if (_deadManager != null)
            _deadManager.OnReadyToRemove -= RemoveObject;
        _entityWait.CancelAll();
        _instance = null;
    }
}
