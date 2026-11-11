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
    [Inject]
    void BindItemDropPresentationRunner()
    {
        ItemDropPresentationRunner runner = GetComponent<ItemDropPresentationRunner>();
        if (runner == null)
            runner = gameObject.AddComponent<ItemDropPresentationRunner>();
        if (_itemDropPresentation != null)
            runner.Bind(_itemDropPresentation);
    }

    void OnDestroy() {
        if (_deadManager != null)
            _deadManager.OnReadyToRemove -= RemoveObject;
        _instance = null;
    }
}
