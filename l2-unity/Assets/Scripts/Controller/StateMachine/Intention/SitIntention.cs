
using UnityEngine;

public class SitIntention : IntentionBase
{
    public SitIntention(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        // Mobius RequestActionUse 0: the zone owns the sit — it refuses while casting/in combat and answers ChangeWaitType.
        NetSession.Ensure().SendActionUse(0);
    }

    public override void Exit() { }
    public override void Update()
    {
      
    }
}
