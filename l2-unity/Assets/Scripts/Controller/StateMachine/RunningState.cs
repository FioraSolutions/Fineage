using UnityEngine;

public class RunningState : StateBase
{
    public RunningState(PlayerStateMachine stateMachine) : base(stateMachine) { }



    public override void HandleEvent(Event evt, object payload = null)
    {
        switch (evt)
        {
            case Event.ARRIVED:
                if (ControllerPacketServices.Targets.HasAttackTarget())
                {
                    //_stateMachine.ChangeIntention(Intention.INTENTION_ATTACK, AttackIntentionType.TargetReached);
                }
                else
                {
                    //if(PlayerStateMachine.Instance.IsAutoAttack == true)
                    //{
                     //   _stateMachine.ChangeIntention(Intention.INTENTION_IDLE);
                     //   _stateMachine.NotifyEvent(Event.WAIT_RETURN);
                   // }
                   // else
                   // {
                    //    _stateMachine.ChangeIntention(Intention.INTENTION_IDLE);
                   // }
                }
                break;

        }
    }

    public override void Update()
    {
        //Arrived to destination
        if (!ControllerPacketServices.Input.Move && !ControllerPacketServices.Player.RunningToDestination)
        {
            ////Debug.Log("Input event Move" + InputManager.Instance.Move);
            //Debug.Log("Input event RunningToDestination" + PlayerController.Instance.RunningToDestination);

            // Debug.Log("Character position : x " + PlayerController.Instance.GetPlayerPosition().x + " y " + PlayerController.Instance.GetPlayerPosition().y + PlayerController.Instance.GetPlayerPosition().z);
            SendValidatePosition(ControllerPacketServices.Player.GetPlayerPosition());
            _stateMachine.NotifyEvent(Event.ARRIVED);
        }

        // If move input is pressed while running to target
        if (ControllerPacketServices.Targets.HasAttackTarget() && ControllerPacketServices.Input.Move)
        {
            // Cancel follow target
            ControllerPacketServices.Targets.ClearAttackTarget();
        }
    }

    private void SendValidatePosition(Vector3 playerPosition)
    {
        int heading = 0;
        if (ControllerPacketServices.Player != null)
            heading = PlayerPositionSender.ToL2Heading(ControllerPacketServices.Player.transform.eulerAngles.y);
        ControllerPacketServices.Game.Send(new ValidatePositionCommand(playerPosition.x, playerPosition.y, playerPosition.z, heading));
    }
}