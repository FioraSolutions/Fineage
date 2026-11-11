using System;
using UnityEngine;
using VContainer;

public partial class ListenerCharacterGear : MonoBehaviour
{
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

    private void WireGamePacketServices()
    {
        GamePacketServices.Bind(bus: _bus, gameWorld: _world);
    }
}
