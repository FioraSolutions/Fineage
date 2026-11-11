using System;

using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class PositionValidationController : MonoBehaviour
{
    [Inject] World _world;
    [Inject] IAnimationManager _animations;

    private World GameWorld
    {
        get { return _world != null ? _world : ControllerPacketServices.GameWorld; }
    }

    private IAnimationManager Animations
    {
        get { return _animations != null ? _animations : ControllerPacketServices.Animations; }
    }

    private PlayerController Player
    {
        get { return ControllerPacketServices.Player; }
    }
    //<197 unit - он передвигается пешком(игнорирует если он двигается)
    //>197 unit он прыгает останавливая движение и возвращая его когда будет перемещен

    private List<ValidateLocationDto> _validateList;
    private List<ValidateLocationDto> _validateRemove;
    private List<CharMoveToLocationDto> _validateInitPosition;
    private bool validTest = false;
    //197 unit | 3.743f metr
    private float _trigger = 3.743f;

    private static PositionValidationController _instance;
    [System.Obsolete("Use [Inject] instead")]
    public static PositionValidationController Instance { get { return _instance; } }









}
