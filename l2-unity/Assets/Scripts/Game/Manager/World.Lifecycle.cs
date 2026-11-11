using System;

using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements.Experimental;
using VContainer;

public partial class World : MonoBehaviour, IWorldSpawnContext
{
    static bool ShouldEnsureGameScope()
    {
        if (SystemUIPacketServices.Ui != null)
            return true;

        GameManager manager = WorldPacketServices.Manager ?? GamePacketServices.Manager;
        if (manager != null && manager.GameState == GameState.LOGIN_SCREEN)
            return false;

        return true;
    }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else if (_instance != this) {
            Destroy(this);
            return;
        }

        if (ShouldEnsureGameScope())
            DiBootstrap.EnsureGameScope();
        // Prefer [Inject] EventProcessor; NetworkingPacketServices is the #142 fallback bridge.
        if (_eventProcessor == null)
            _eventProcessor = NetworkingPacketServices.Events;
        _playerPlaceholder = Resources.Load<GameObject>("Prefab/Player_FDarkElf");
        _userPlaceholder = Resources.Load<GameObject>("Prefab/User_FDarkElf");
        _npcPlaceHolder = Resources.Load<GameObject>("Prefab/Npc");
        _monsterPlaceholder = Resources.Load<GameObject>("Data/Animations/LineageMonsters/gremlin_m00/gremlin_m00");
        _npcsContainer = GameObject.Find("Npcs");
        _monstersContainer = GameObject.Find("Monsters");
        _usersContainer = GameObject.Find("Users");
        _itemsContainer = GameObject.Find("Items");
        if (_itemsContainer == null)
        {
            _itemsContainer = new GameObject("Items");
            _itemsContainer.transform.SetParent(transform, false);
        }

        BindItemDropPresentationRunner();
    }

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
