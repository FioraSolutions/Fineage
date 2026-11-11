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

    public void OnStartL2jMoving(bool walking)
    {
       // base.OnStartMoving(walking);
        float speedAnim = walking ? Stats.ScaledAnimWalkSpeed : Stats.ScaledAnimRunSpeed;
        if (walking)
        {

        }
        else
        {
            if (!_networkAnimationReceive.GetBool(_run))
            {
                _networkAnimationReceive.SetAnimationProperty(walking ? (int)MonsterAnimationEvent.Walk : (int)MonsterAnimationEvent.Run, 1f);
            }
            
        }
        
        StartWalkAnim(walking);
    }

    private void StartWalkAnim(bool walking)
    {
        if (walking)
        {
            _networkAnimationReceive.SetBool(_walk, true);
        }
        else
        {
            if (!_networkAnimationReceive.GetBool(_run))
            {
                _networkAnimationReceive.SetBool(_run, true);
            }
            
        }
    }

    public void AttachArrowToNearestBone(GameObject attach , Vector3 hitPointCollider, Transform target , Vector3 hitDirection)
    {
        if(_gear.GetType() != typeof(MonsterGear))
        {
            // Debug.LogWarning("MonsterEntity->AttachArrowToNearestBone: gear invalid cast !!!");
            return;
        }

        MonsterGear gear = (MonsterGear)_gear;
        Transform targetBone = gear.DetermineHitSide(hitPointCollider, target);
        bool isSpecialBone = (targetBone == gear.GetHeadBone() || targetBone == gear.GetTailBone());
        gear.SetPositionArrowRandomlyNearCenter( attach,  targetBone  , hitDirection , isSpecialBone);
    }

    public void OnDestroyDepends()
    {
        if (GetComponent<MonsterAnimationAudioHandler>())
            Destroy(GetComponent<MonsterAnimationAudioHandler>());
    }
}
