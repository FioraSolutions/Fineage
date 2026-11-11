using System;
using UnityEngine;
using UnityEngine.Rendering;

public partial class L2CloudMyst
{
    public static L2CloudMyst EnsureOn(GameObject host)
    {
        if (!PortEnabled)
            return null;
        if (Instance != null)
            return Instance;
        if (host == null)
            return null;
        L2CloudMyst c = host.GetComponent<L2CloudMyst>();
        return c != null ? c : host.AddComponent<L2CloudMyst>();
    }

    void OnEnable()
    {
        if (!PortEnabled)
        {
            enabled = false;
            return;
        }

        if (Instance != null && Instance != this)
        {
            enabled = false;
            return;
        }

        Instance = this;
        EnsureMesh();
    }
}
