using UnityEngine;

/// <summary>
/// Interlude MonSound via <see cref="InterludeSfx"/>.
/// Clips: Resources/Data/Sounds/MonSound/{npc}/{npc}_{event}.wav
/// </summary>
public static class MonsterCombatSound
{
    const string Root = "Data/Sounds/MonSound/";
    public static float Volume = 0.85f;

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

    /// <summary>Convenience when Entity is known (death / atk from visual layer).</summary>
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

    static AudioClip LoadEvent(string npc, string eventKey)
    {
        string[] stems =
        {
            npc + "_" + eventKey,
            npc + "_" + Capitalize(eventKey),
            npc + "_" + eventKey.ToUpperInvariant(),
        };
        for (int i = 0; i < stems.Length; i++)
        {
            AudioClip clip = InterludeSfx.Load(Root + npc + "/" + stems[i]);
            if (clip != null)
                return clip;
        }
        return null;
    }

    static AudioClip LoadVariant(string npc, string baseKey)
    {
        int[] order = { 1, 2, 3 };
        for (int i = order.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int tmp = order[i];
            order[i] = order[j];
            order[j] = tmp;
        }

        for (int i = 0; i < order.Length; i++)
        {
            AudioClip clip = LoadEvent(npc, baseKey + "_" + order[i]);
            if (clip != null)
                return clip;
        }
        return LoadEvent(npc, baseKey);
    }

    static string Capitalize(string s)
    {
        if (string.IsNullOrEmpty(s))
            return s;
        if (s.Length == 1)
            return s.ToUpperInvariant();
        return char.ToUpperInvariant(s[0]) + s.Substring(1).ToLowerInvariant();
    }
}
