using System;
using UnityEngine;
using VContainer;
using System.Collections.Generic;

public partial class PlayerStateMachine : MonoBehaviour
{
    private bool _enableLogs = true;
 
    private static PlayerStateMachine _instance;

    [System.Obsolete("Use PlayerStatePacketServices.PlayerState or [Inject] PlayerStateMachine instead")]
    public static PlayerStateMachine Instance => _instance;


    private PlayerState _currentState;
    private Intention _currentIntention;

    public Intention Intention { get { return _currentIntention; } }
    public Transform Follow { get; set; }

    public PlayerEntity Player { get; set; }
    public bool IsMoveToPawn { get; set; }
    public PlayerState State { get { return _currentState; } }

    private StateBase _stateInstance;
    private IntentionBase _intentionInstance;
    private Dictionary<PlayerState, StateBase> _dictState = new Dictionary<PlayerState, StateBase>();
    private Dictionary<Intention, IntentionBase> _dictIntention = new Dictionary<Intention, IntentionBase>();

    public bool Riding
    {
        get
        {
            StriderRideService striderRide = AnimationPacketServices.StriderRide;
            return Player != null
                && striderRide != null
                && striderRide.IsRiding(Player);
        }
    }
}
