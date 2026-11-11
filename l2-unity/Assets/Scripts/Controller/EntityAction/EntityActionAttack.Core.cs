using UnityEngine;

public sealed partial class EntityActionAttack : IEntityActionProcess
{
    public EntityActionAttack(L2PawnYaw pawnYaw)
    {
        _pawnYaw = pawnYaw;
    }
}
