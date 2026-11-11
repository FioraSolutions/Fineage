using System;
using UnityEngine;
using UnityEngine.Rendering;

public partial class L2CloudSimple : MonoBehaviour
{
    public static L2CloudSimple EnsureOn(GameObject host)
    {
        if (Instance != null)
            return Instance;
        if (host == null)
            return null;
        L2CloudSimple c = host.GetComponent<L2CloudSimple>();
        return c != null ? c : host.AddComponent<L2CloudSimple>();
    }

    void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            enabled = false;
            return;
        }

        Instance = this;
        SkyPacketServices.Bind(clouds: this);
        StripStaleChildren();
        EnsureChunks();
    }
}
