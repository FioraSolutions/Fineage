using System;

using UnityEngine;

public partial class EventBus : MonoBehaviour
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            GamePacketServices.Bind(bus: this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }
}
