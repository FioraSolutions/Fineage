using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class SkillbarWindow : L2PopupWindow
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _cooldownAnimationService = new CooldownAnimationService();
            WireSkillbarPacketServices();
        }
        else
        {
            Destroy(this);
        }

        _expandCoroutines = new List<Coroutine>();
        _minimizeCoroutines = new List<Coroutine>();

    }

    private void Start()
    {
        WireSkillbarPacketServices();
    }

    private void WireSkillbarPacketServices()
    {
        SkillbarPacketServices.Bind(skillbar: this, shortcuts: _shortcuts, inventory: _inventory);
    }
}
