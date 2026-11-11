using System;
using System.Collections.Concurrent;
using UnityEngine;

public partial class EventProcessor : MonoBehaviour
{
    public static EventProcessor EnsureInstance()
    {
        if (_instance != null)
            return _instance;
        EventProcessor found = FindAnyObjectByType<EventProcessor>();
        if (found != null)
        {
            _instance = found;
            return found;
        }

        GameObject host = new GameObject("EventProcessor (fallback)");
        DontDestroyOnLoad(host);
        EventProcessor created = host.AddComponent<EventProcessor>();
        _instance = created;
        Debug.LogWarning("[EventProcessor] none bound; created app-lifetime fallback pump.");
        return created;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogError(
                "[EventProcessor] Duplicate on '" + name + "' scene=" + gameObject.scene.name
                + ". Only AppLifetimeScope may host EventProcessor. Destroying this duplicate.");
            Destroy(this);
            return;
        }

        _instance = this;
        NetworkingPacketServices.Bind(events: this);
    }

    void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }
}
