using System.IO;
using UnityEngine;
using FMOD.Studio;

public partial class NewMoveToPawnIntention : IntentionBase
{
    public NewMoveToPawnIntention(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter(object arg0)
    {
        
        if (arg0 != null)
        {
            
            if (_stateMachine.State == PlayerState.DEAD)
                return;

            ModelMovePawn model = (ModelMovePawn)arg0;
            if (model.TarEntity() == null)
                return;

            Vector3 tarPosition = model.TarEntity().transform.position;
            Vector3 sourPosition = model.SourObj();
            float distance = VectorUtils.Distance2D(sourPosition, tarPosition);

            // APawn::ReachedDestination on a pawn goal replaces CollisionRadius with
            // MoveToPawnAdjust and then adds DestinationOffset. The packet distance is that adjust.
            // Debug.Log("NewMoveToPawnIntention > dist " + distance + " stopDist " + model.Distance());

            if (distance > model.Distance())
            {
                // Stop ring is the packet distance. ReachedDestination does not add collision radii for a pawn goal.
                L2PawnRange.IgnorePawnCollision(DomainPacketServices.LocalPlayer, model.TarEntity());
                PlayerStatePacketServices.PlayerState.IsMoveToPawn = true;
                bool running = DomainPacketServices.LocalPlayer != null && DomainPacketServices.LocalPlayer.Running;
                ControllerPacketServices.Player.MoveToPoint(new MovementTarget(model.TarEntity().transform, model.Distance(), running));
                StartAnimMoveTo();
            }
            else
            {
                L2PawnRange.ClearIgnoredPawn(DomainPacketServices.LocalPlayer);
                if (_stateMachine.State == PlayerState.SITTING ||
                    _stateMachine.State == PlayerState.SIT_WAIT ||
                    _stateMachine.State == PlayerState.STANDING)
                {
                    _stateMachine.ChangeState(PlayerState.IDLE);
                    _stateMachine.NotifyEvent(Event.ARRIVED);
                }
            }

        }
    }
}
