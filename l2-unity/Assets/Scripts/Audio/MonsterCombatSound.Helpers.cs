using UnityEngine;

public static partial class MonsterCombatSound
{
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
