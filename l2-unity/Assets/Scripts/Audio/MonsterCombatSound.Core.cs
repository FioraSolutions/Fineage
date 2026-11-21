using UnityEngine;

public static partial class MonsterCombatSound
{
    public static void Play(MonsterSoundEvent soundEvent, string npcClassName)
    {
        AudioService service = RequiredAudioServices.RequireService(nameof(MonsterCombatSound));
        service?.LogCombatAudioError(
            "MonsterCombatSound.Play requires Entity position",
            entityId: 0,
            chrSoundBankPath: npcClassName,
            soundEvent.ToString());
    }

    public static void Play(MonsterSoundEvent soundEvent, Entity entity)
    {
        if (entity == null)
            return;
        string name = ResolveNpcName(entity);
        int entityId = entity.Identity != null ? entity.Identity.Id : 0;
        Vector3 position = entity.transform.position;
        CharacterCombatAudioDispatch.PlayMonster(soundEvent, name, position, entityId);
    }

    internal static bool TryLoadClip(MonsterSoundEvent soundEvent, string npcClassName, out AudioClip clip)
    {
        clip = null;
        if (string.IsNullOrEmpty(npcClassName))
            return false;

        string npc = npcClassName.ToLowerInvariant();
        string key = soundEvent.ToString().ToLowerInvariant();

        if (soundEvent == MonsterSoundEvent.Dmg)
            clip = LoadVariant(npc, "dmg");
        else if (soundEvent == MonsterSoundEvent.Step)
            clip = LoadVariant(npc, "step");
        else
            clip = LoadEvent(npc, key);

        return clip != null;
    }

    public static string ResolveNpcName(Entity entity)
    {
        if (entity == null)
            return null;

        if (entity.Identity != null && !string.IsNullOrEmpty(entity.Identity.NpcClass))
        {
            string npcClass = entity.Identity.NpcClass;
            string[] parts = npcClass.Split('.');
            if (parts.Length > 1)
                return parts[1].ToLowerInvariant();
            return npcClass.ToLowerInvariant();
        }

        Animator animator = entity.GetComponentInChildren<Animator>();
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            string n = animator.runtimeAnimatorController.name.ToLowerInvariant();
            int cut = n.IndexOf('.');
            if (cut >= 0)
                n = n.Substring(0, cut);
            return n;
        }
        return null;
    }
}
