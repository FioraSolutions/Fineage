using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Interlude skill cast/shot SFX (#522). Clips extracted from the retail skillsound*.uax packages
/// into Resources/Data/Sounds/SkillSound as "{skill_name}_cast" / "_shot" / "_explotion".
/// Plays through Unity audio so the FMOD SkillSound bank does not need a rebuild.
/// </summary>
public static class SkillCastSound
{
    const string Root = "Data/Sounds/SkillSound/";
    static readonly Dictionary<string, AudioClip> _cache = new Dictionary<string, AudioClip>();
    static readonly HashSet<string> _missing = new HashSet<string>();

    public static float Volume = 0.8f;

    public static void OnMagicSkillUse(Entity caster, MagicSkillUseDto dto)
    {
        if (caster == null || dto == null)
            return;
        string key = KeyFor(dto.SkillId, dto.SkillLvl);
        if (string.IsNullOrEmpty(key))
            return;

        Vector3 pos = caster.transform.position;
        Play(key + "_cast", pos);

        AudioManager host = AudioManager.Instance;
        if (host == null)
            return;
        float delay = Mathf.Max(0f, dto.HitTime / 1000f);
        host.StartCoroutine(PlayLater(key, caster, delay));
    }

    static IEnumerator PlayLater(string key, Entity caster, float delay)
    {
        yield return new WaitForSeconds(delay);
        Vector3 pos = caster != null ? caster.transform.position : Vector3.zero;
        if (!Play(key + "_shot", pos))
            Play(key + "_explotion", pos);
    }

    static bool Play(string clipName, Vector3 pos)
    {
        AudioClip clip = Load(clipName);
        if (clip == null)
            return false;
        AudioSource.PlayClipAtPoint(clip, pos, Volume);
        return true;
    }

    static AudioClip Load(string clipName)
    {
        AudioClip clip;
        if (_cache.TryGetValue(clipName, out clip))
            return clip;
        if (_missing.Contains(clipName))
            return null;
        clip = Resources.Load<AudioClip>(Root + clipName);
        if (clip == null)
            _missing.Add(clipName);
        else
            _cache[clipName] = clip;
        return clip;
    }

    static string KeyFor(int skillId, int level)
    {
        string name = null;
        try
        {
            SkillNameData data = SkillNameTable.Instance != null ? SkillNameTable.Instance.GetName(skillId, Mathf.Max(1, level)) : null;
            if (data == null && SkillNameTable.Instance != null)
                data = SkillNameTable.Instance.GetName(skillId, 1);
            name = data != null ? data._name : null;
        }
        catch { }
        return Normalize(name);
    }

    public static string Normalize(string skillName)
    {
        if (string.IsNullOrEmpty(skillName))
            return null;
        var sb = new StringBuilder(skillName.Length);
        bool us = false;
        foreach (char ch in skillName.ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(ch)) { sb.Append(ch); us = false; }
            else if (ch == '\'') { }
            else if (!us && sb.Length > 0) { sb.Append('_'); us = true; }
        }
        return sb.ToString().TrimEnd('_');
    }
}
