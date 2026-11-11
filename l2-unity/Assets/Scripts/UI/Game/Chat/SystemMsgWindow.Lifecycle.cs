using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SystemMsgWindow : L2Window
{
    public static SystemMsgWindow Ensure()
    {
        if (_instance != null)
        {
            // Lobby→world can disable UI hosts; coroutines need an active GO.
            if (!_instance.gameObject.activeInHierarchy)
                _instance.gameObject.SetActive(true);
            return _instance;
        }
        var go = new GameObject(nameof(SystemMsgWindow));
        if (SystemUIPacketServices.Ui != null)
            go.transform.SetParent(SystemUIPacketServices.Ui.transform, false);
        return go.AddComponent<SystemMsgWindow>();
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
    }

    void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Chat/SystemMsgWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        yield return new WaitForEndOfFrame();

        _list = _windowEle.Q<ScrollView>("SystemMsgList");
        _content = _windowEle.Q<VisualElement>("SystemMsgContent");
        _windowEle.style.position = Position.Absolute;
        _windowEle.style.height = DefaultHeight;

        if (_chatAnchor != null)
            BindDock(_chatAnchor);

        _ready = true;
        for (int i = 0; i < _pending.Count; i++)
            AppendLineNow(_pending[i]);
        _pending.Clear();
    }
}
