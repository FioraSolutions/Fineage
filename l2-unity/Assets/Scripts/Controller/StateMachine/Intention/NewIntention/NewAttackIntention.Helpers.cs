using UnityEngine;

public partial class NewAttackIntention : IntentionBase
{
    static void SelectPacketTarget(Entity targetEntity)
    {
        if (ControllerPacketServices.Targets != null)
        {
            ControllerPacketServices.Targets.SetTarget(new ObjectData(targetEntity.gameObject), "#ffffff");
            return;
        }

        if (DomainPacketServices.LocalPlayer == null)
            return;
        DomainPacketServices.LocalPlayer.TargetId = targetEntity.Identity != null ? targetEntity.Identity.Id : 0;
        DomainPacketServices.LocalPlayer.Target = targetEntity.transform;
    }

    public override void Exit() { }

    public override void Update()
    {

    }
}
