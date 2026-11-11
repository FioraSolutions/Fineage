using System;

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

}
