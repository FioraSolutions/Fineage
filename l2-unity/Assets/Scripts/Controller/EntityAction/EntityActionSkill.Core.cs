using System;
using System.Collections.Generic;
using UnityEngine;

public partial class EntityActionSkill : IEntityActionProcess
{
    public EntityActionSkill(L2PawnYaw pawnYaw)
    {
        _pawnYaw = pawnYaw;
    }
}
