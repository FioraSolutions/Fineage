using System;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ClickManager : MonoBehaviour
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            ControllerPacketServices.Bind(clicks: this);
            uiDocument = GetComponent<UIDocument>();
        }
        else
        {
            Destroy(this);
        }
    }

    void OnDestroy()
    {
        _instance = null;
    }

    void Start()
    {
        _locator = null;
        WireClickPacketServices();
    }
}
