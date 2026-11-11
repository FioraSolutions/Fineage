using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using static UnityEngine.GraphicsBuffer;

public partial class PlayerActions : MonoBehaviour
{
    [Inject] InputManager _input;
    [Inject] TargetManager _targets;
    private Dictionary<ActionType, L2Action> _actions;
    
    private static PlayerActions _instance;
    [System.Obsolete("Use [Inject] or ShortcutPacketServices.Actions instead")]
    public static PlayerActions Instance
    {
        get { return _instance; }
    }









}
