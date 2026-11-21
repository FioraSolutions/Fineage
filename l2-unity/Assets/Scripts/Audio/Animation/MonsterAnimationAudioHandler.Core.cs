using UnityEngine;
using System.Collections;

public partial class MonsterAnimationAudioHandler : BaseAnimationAudioHandler
{
    [SerializeField] private string _monsterName;

    protected override void Initialize()
    {
        base.Initialize();
        ResolveMonsterName();
    }

    public void PlaySound(MonsterSoundEvent soundEvent)
    {
        ResolveMonsterName();
        Entity entity = GetComponent<Entity>();
        int entityId = entity != null && entity.Identity != null ? entity.Identity.Id : 0;
        CharacterCombatAudioDispatch.PlayMonster(
            soundEvent, _monsterName, transform.position, entityId);
    }
}
