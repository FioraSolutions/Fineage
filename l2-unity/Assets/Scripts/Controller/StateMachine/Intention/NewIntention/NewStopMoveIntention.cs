using TMPro;

public class NewStopMoveIntention : IntentionBase
{
    public NewStopMoveIntention(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter(object arg0)
    {
        
        if (arg0 != null)
        {
            StopMoveDto stop = (StopMoveDto)arg0;
            
            //Debug.Log("StopMove Dist " + VectorUtils.Distance2D(PlayerController.Instance.transform.position, stop.StopPos));

            L2PawnRange.ClearIgnoredPawn(DomainPacketServices.LocalPlayer);
            PlayerStatePacketServices.PlayerState.IsMoveToPawn = false;
            ControllerPacketServices.Player.StopMove();
            //Debug.Log("IsMoveToPawn Двигаемся к цели stopmove1");

            //когда мы не успеваем добежать до точки и находимся в состоянии бега нам нужно предупредить RunningState о том что мы прибежали на место и нам нужно отключить анимацию бега
            //и после этого перейти в режим idle или по другому режим wait ждем дальнейших указаний
            if (PlayerStatePacketServices.PlayerState.State == PlayerState.RUNNING)
            {
                //Debug.Log("IsMoveToPawn Двигаемся к цели stopmove2");

                _stateMachine.NotifyEvent(Event.ARRIVED);
                _stateMachine.ChangeIntention(Intention.INTENTION_IDLE);
                ControllerPacketServices.Player.StopMove();
            }
            else
            {
                //if (PlayerStatePacketServices.PlayerState.IsMoveToPawn == true | PlayerController.Instance.RunningToDestination == true)
                //{
                  //  PlayerStatePacketServices.PlayerState.IsMoveToPawn = false;
                  //  _stateMachine.ChangeIntention(Intention.INTENTION_IDLE);
                  //  PlayerController.Instance.StopMove();
                //}

            }

        }
    }




    public override void Exit() { }
    public override void Update()
    {

    }
}