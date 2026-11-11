
using UnityEngine;

public partial class MonsterEntity : NetworkEntity
{
    public override void OnStopMoving()
    {
        if (_networkAnimationReceive.GetAnimationProperty((int)MonsterAnimationEvent.Atk01) == 0f)
        {
            _networkAnimationReceive.SetAnimationProperty((int)MonsterAnimationEvent.Wait, 1f);
        }
        else
        {
            _networkAnimationReceive.SetAnimationProperty((int)NpcAnimationEvent.Wait, 1f);
        }
    }

    public void OnStopL2jMoving()
    {
        if (_networkAnimationReceive == null)
            return;

        _networkAnimationReceive.SetBool(_walk, false);
        _networkAnimationReceive.SetBool(_run, false);
        _networkAnimationReceive.SetBool(_wait, true);
        _networkAnimationReceive.SetAnimationProperty((int)NpcAnimationEvent.Wait, 1f);
    }

    public override void OnStartMoving(bool walking)
    {
        // _baseAnimationController.SetBool("walk_" + WeaponAnim, false);
        _networkAnimationReceive.SetAnimationProperty(walking ? (int)MonsterAnimationEvent.Walk : (int)MonsterAnimationEvent.Run, 1f);
    }
}
