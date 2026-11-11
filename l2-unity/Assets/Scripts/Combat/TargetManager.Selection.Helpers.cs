using System;
using UnityEngine;
using VContainer;

public partial class TargetManager
{
    public bool HasTarget()
    {
        return _target != null && _target.Data.ObjectTransform != null;
    }

    public bool HasAttackTarget()
    {
        return _attackTarget != null;
    }

    public void UnselectedTarget(int objId)
    {
         ClearTarget();
    }

    public void ClearTarget()
    {
        if (HasTarget())
        {
            if (DomainPacketServices.LocalPlayer.TargetId != -1)
            {
                IZoneTransport game = ZoneCommands.Transport;
                if (game != null)
                    ZoneCommands.Send(new RequestTargetCanceldCommand());
                DomainPacketServices.LocalPlayer.TargetId = -1;
                DomainPacketServices.LocalPlayer.Target = null;
            }

            _target = null;
            _attackTarget = null;
        }
    }

    public void ClearTargetSilently()
    {
        if (DomainPacketServices.LocalPlayer != null)
        {
            DomainPacketServices.LocalPlayer.TargetId = -1;
            DomainPacketServices.LocalPlayer.Target = null;
        }

        _target = null;
        _attackTarget = null;
    }
}
