using UnityEngine;

public partial class MonsterEntity : NetworkEntity
{
    public override void Initialize()
    {
        base.Initialize();
        _monsterAnimationAudioHandler = GetComponent<MonsterAnimationAudioHandler>();
        _characterController = GetComponent<CharacterController>();
        EntityLoaded = true;
    }

    public  CharacterController GetCharacterController()
    {
        return _characterController;
    }

    public ObjectData GetByIdUseLocator(int id , Vector3 sourcePosition)
    {
        var _entitiesInRange = Physics.SphereCastAll(sourcePosition, 50f, transform.forward, 0, _areaMask);

        foreach (var entity in _entitiesInRange)
        {
            if (entity.transform.parent != null)
            {
                var _entity = entity.transform.parent.GetComponent<Entity>();

                if (_entity != null & _entity.Identity.Id == id)
                {
                    return new ObjectData(entity.transform.parent.gameObject);
                }
            }
        }
        return null;
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        _networkAnimationReceive.SetAnimationProperty((int)MonsterAnimationEvent.Death, 1f, true);
    }

    public void OnDeathL2j()
    {
        _networkAnimationReceive.SetAnimationProperty((int)MonsterAnimationEvent.Death, 1f, true);
        _networkAnimationReceive.SetBool("death", true);
    }

    protected override void OnHit(bool criticalHit)
    {
        base.OnHit(criticalHit);
        _monsterAnimationAudioHandler.PlaySound(MonsterSoundEvent.Dmg);
        // Interlude damageaction flinch on hit / DoT tick (StatusUpdate without Attack).
        if (ControllerPacketServices.Animations != null && Identity != null)
            ControllerPacketServices.Animations.PlayMonsterAnimation(
                Identity.Id,
                AnimationNames.MONSTER_DamageAction.ToString());
    }

    public void OnWaitAnim()
    {
        _networkAnimationReceive.SetBool(_wait , true);
    }
}
