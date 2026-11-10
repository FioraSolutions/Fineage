using System;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ClickManager
{
    public void SetMasks(LayerMask entityMask, LayerMask clickThroughMask)
    {
        _entityMask = entityMask;
        _clickThroughMask = clickThroughMask;
    }

    static Camera ResolvePickCamera()
    {
        CameraController controller = CameraPacketServices.Controller;
        if (controller != null)
        {
            Camera cam = controller.GetComponent<Camera>();
            if (cam != null && cam.enabled && cam.gameObject.activeInHierarchy)
                return cam;
        }

        return Camera.main;
    }

    public void OnClickOnEntity()
    {
        TargetData clickTarget = new TargetData(_targetObjectData);
        if (clickTarget == null || clickTarget.Identity == null)
        {
            return;
        }

        IZoneTransport game = ZoneCommands.Transport;
        if (game == null)
            return;

        // Already selected monster → melee attack intent. Ctrl force-attacks players / NPCs / mobs
        // (FineageZoneTransport also keys off GameCtrl.IsHeld so the first Ctrl-click swings).
        Entity clickedEntity = clickTarget.GetEntity();
        bool sameSelected = Targets != null &&
            Targets.HasTarget() &&
            Targets.Target.Identity != null &&
            Targets.Target.Identity.Id == clickTarget.Identity.Id &&
            !clickTarget.IsDead();
        bool isMonster = clickedEntity is MonsterEntity &&
            !PetVisuals.IsPet(clickTarget.Identity.Owned, clickTarget.Identity.NpcId);
        if (Targets != null && sameSelected && (isMonster || GameCtrl.IsHeld))
            Targets.SetAttackTarget();

        var l2jpos = clickTarget.Identity.GetL2jPos();
        ZoneCommands.Send(new ClickActionCommand(
            clickTarget.Identity.Id, (int)l2jpos.x, (int)l2jpos.y, (int)l2jpos.z, 0));
    }

}
