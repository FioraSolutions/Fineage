using UnityEngine;

public partial class PlayerStateJAtk
{
    private void TrySubscribeSwordCollider()
    {
        if (_isColliderSubscribed || SwordCollisionPacketServices.Collision == null)
        {
            return;
        }

        SwordCollisionPacketServices.Collision.OnHitCollider += OnSwordColliderHit;
        _isColliderSubscribed = true;
    }

    private void TryUnsubscribeSwordCollider()
    {
        if (!_isColliderSubscribed || SwordCollisionPacketServices.Collision == null)
        {
            return;
        }

        SwordCollisionPacketServices.Collision.OnHitCollider -= OnSwordColliderHit;
        _isColliderSubscribed = false;
    }
}
