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
        MonsterCombatSound.Play(soundEvent, _monsterName);
    }
}
