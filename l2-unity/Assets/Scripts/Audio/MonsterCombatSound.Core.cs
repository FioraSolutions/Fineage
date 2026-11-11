using UnityEngine;

public static partial class MonsterCombatSound
{
    public static void Play(MonsterSoundEvent soundEvent, string npcClassName)
    {
        if (string.IsNullOrEmpty(npcClassName))
            return;

        string npc = npcClassName.ToLowerInvariant();
        string key = soundEvent.ToString().ToLowerInvariant();

        AudioClip clip = null;
        if (soundEvent == MonsterSoundEvent.Dmg)
            clip = LoadVariant(npc, "dmg");
        else if (soundEvent == MonsterSoundEvent.Step)
            clip = LoadVariant(npc, "step");
        else
            clip = LoadEvent(npc, key);

        if (clip != null)
            InterludeSfx.Play(clip, Volume);
    }

    public static void Play(MonsterSoundEvent soundEvent, Entity entity)
    {
        if (entity == null)
            return;
        string name = ResolveNpcName(entity);
        if (string.IsNullOrEmpty(name))
            return;
        Play(soundEvent, name);
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
