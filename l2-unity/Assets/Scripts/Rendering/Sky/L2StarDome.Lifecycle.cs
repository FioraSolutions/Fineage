using System;
using UnityEngine;

public partial class L2StarDome : MonoBehaviour
{
    public static L2StarDome EnsureOn(GameObject host)
    {
        if (Instance != null)
        {
            return Instance;
        }

        L2StarDome dome = host != null ? host.GetComponent<L2StarDome>() : null;
        if (dome == null && host != null)
        {
            dome = host.AddComponent<L2StarDome>();
        }

        return dome;
    }

    void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            enabled = false;
            return;
        }

        Instance = this;
        EnsureDome();
    }

    void OnDisable()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        DestroyDome();
    }
}
