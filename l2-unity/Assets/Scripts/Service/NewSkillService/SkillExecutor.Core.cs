using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using VContainer;

public partial class SkillExecutor : MonoBehaviour
{
    public SkillExecutor()
    {
        _animRunner = new SkillAnimationRunner();

    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        WireServicePacketServices();
    }

    private void WireServicePacketServices()
    {
        ServicePacketServices.Bind(animations: _animations, effects: _effects);
    }
}
