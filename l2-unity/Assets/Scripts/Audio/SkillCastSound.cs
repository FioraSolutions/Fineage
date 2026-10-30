using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Interlude skill cast/shot SFX + hero skill-voice from chrsound.uax.
/// Clips: Resources/Data/Sounds/SkillSound (*_cast/_shot) and SkillVoice (M_HMagician_Shot, …).
/// Voice plays 2D so camera distance never mutes short chrsound clips.
/// </summary>
public static class SkillCastSound
{
    const string SfxRoot = "Data/Sounds/SkillSound/";
    const string VoiceRoot = "Data/Sounds/SkillVoice/";
    static readonly Dictionary<string, AudioClip> _cache = new Dictionary<string, AudioClip>();
    // Misses are NOT cached forever — Editor play-without-domain-reload used to poison this
    // when SkillVoice was added mid-session, leaving mage voice silent until restart.
    static readonly Dictionary<string, float> _missUntil = new Dictionary<string, float>();
    const float MissRetrySeconds = 2f;

    static AudioSource _voice2D;

    public static float Volume = 0.8f;
    public static float VoiceVolume = 1f;

    /// <summary>Known skillId → SFX stem when name table is empty or alias needed.</summary>
    static readonly Dictionary<int, string> SkillIdSfx = new Dictionary<int, string>
    {
        { 1011, "heal" },           // Heal
        { 1015, "heal" },           // Battle Heal (no battle_heal clip)
        { 1027, "heal" },           // Group Heal
        { 1216, "heal" },           // Self Heal (mystic starter) — no self_heal_* clip
        { 1217, "greater_heal" },   // Greater Heal
        { 1218, "greater_heal" },   // Greater Battle Heal
        { 1256, "heal" },           // The End / related
        { 1311, "heal" },           // Body of Avatar etc. heal-adjacent fallback
        { 1147, "heal" },           // Elemental Heal
        { 6847, "heal" },           // alt Self Heal id in Classic names
        { 1177, "wind_strike" },
        { 1184, "ice_bolt" },
        { 1148, "death_spike" },
        { 1069, "sleep" },
        { 1160, "slow" },
        { 1164, "curse_weakness" },
        { 1168, "curse_poison" },
        { 1221, "curse_fear" },
        { 1206, "wind_shackle" },
        { 1040, "shield" },
        { 1068, "might" },
        { 1013, "recharge" },
        { 1078, "concentration" },
        { 1085, "acumen" },
        { 1059, "empower" },
        { 1045, "blessed_body" },
        { 1048, "blessed_soul" },
        { 1240, "guidance" },
        { 1242, "death_whisper" },
        { 1238, "freezing_shackle" },
        { 1239, "hurricane" },
        { 1235, "hydro_blast" },
        { 1230, "prominence" },
        { 1267, "shadow_flare" },
        { 1144, "servitor_heal" },
        { 1127, "servitor_heal" },
    };

    public static void OnMagicSkillUse(Entity caster, MagicSkillUseDto dto)
    {
        if (caster == null || dto == null)
            return;
        string key = KeyFor(dto.SkillId, dto.SkillLvl);
        Vector3 pos = caster.transform.position;

        if (!string.IsNullOrEmpty(key))
        {
            PlaySfx(key + "_cast", pos, Volume);
            AudioManager host = AudioManager.Instance;
            if (host != null)
            {
                float delay = Mathf.Max(0f, dto.HitTime / 1000f);
                host.StartCoroutine(PlayLater(key, caster, delay));
            }
            else
                PlaySfx(key + "_shot", pos, Volume);
        }

        PlayVoice(caster, key, pos);
    }

    static IEnumerator PlayLater(string key, Entity caster, float delay)
    {
        yield return new WaitForSeconds(delay);
        Vector3 pos = caster != null ? caster.transform.position : Vector3.zero;
        if (!PlaySfx(key + "_shot", pos, Volume))
            PlaySfx(key + "_explotion", pos, Volume);
    }

    static void PlayVoice(Entity caster, string skillKey, Vector3 pos)
    {
        string prefix = VoicePrefixFor(caster);
        if (string.IsNullOrEmpty(prefix))
            return;
        string kind = VoiceKindFor(skillKey, caster);
        if (PlayVoiceClip(prefix + "_" + kind))
            return;
        if (kind != "Shot" && PlayVoiceClip(prefix + "_Shot"))
            return;
        PlayVoiceClip(prefix + "_Throw");
    }

    static string VoicePrefixFor(Entity caster)
    {
        if (caster == null)
            return null;

        CharacterRaceAnimation raceId = caster.RaceId;
        // Prefab default is FElf=0; if appearance says Human mage male, prefer Magician bank.
        raceId = ResolveRaceForVoice(caster, raceId);

        switch (raceId)
        {
            case CharacterRaceAnimation.MMagic: return "M_HMagician";
            case CharacterRaceAnimation.FMagic: return "F_HMagician";
            case CharacterRaceAnimation.MFighter: return "M_HFighter";
            case CharacterRaceAnimation.FFighter: return "F_HFighter";
            case CharacterRaceAnimation.MElf: return "M_Elf";
            case CharacterRaceAnimation.FElf: return "F_Elf";
            case CharacterRaceAnimation.MDarkElf: return "M_DarkElf";
            case CharacterRaceAnimation.FDarkElf: return "F_DarkElf";
            case CharacterRaceAnimation.MDwarf: return "M_Dwarf";
            case CharacterRaceAnimation.FDwarf: return "F_Dwarf";
            case CharacterRaceAnimation.MOrc: return "M_Orc_Fighter";
            case CharacterRaceAnimation.FOrc: return "F_Orc_Fighter";
            case CharacterRaceAnimation.MShaman: return "M_Orc_Magician";
            case CharacterRaceAnimation.FShaman: return "F_Orc_Magician";
            default: return null;
        }
    }

    static CharacterRaceAnimation ResolveRaceForVoice(Entity caster, CharacterRaceAnimation raceId)
    {
        PlayerAppearance appearance = caster.Appearance as PlayerAppearance;
        if (appearance == null)
            return raceId;

        int playerClass = caster.Identity != null ? caster.Identity.PlayerClass : 0;
        CharacterRace race = (CharacterRace)appearance.Race;
        CharacterRaceAnimation resolved = CharacterRaceAnimationParser.ResolveRaceAnimation(
            race, appearance, playerClass);
        // Prefab default is FElf=0. Also fix Human fighter mesh wrongly used for a mystic class.
        bool looksUnset = raceId == CharacterRaceAnimation.FElf && race != CharacterRace.Elf;
        bool mysticAsFighter =
            (resolved == CharacterRaceAnimation.MMagic || resolved == CharacterRaceAnimation.FMagic) &&
            (raceId == CharacterRaceAnimation.MFighter || raceId == CharacterRaceAnimation.FFighter);
        if (looksUnset || mysticAsFighter)
        {
            caster.RaceId = resolved;
            return resolved;
        }
        return raceId;
    }

    static string VoiceKindFor(string skillKey, Entity caster)
    {
        bool orcOrDwarf =
            caster != null && (
                caster.RaceId == CharacterRaceAnimation.MDwarf ||
                caster.RaceId == CharacterRaceAnimation.FDwarf ||
                caster.RaceId == CharacterRaceAnimation.MOrc ||
                caster.RaceId == CharacterRaceAnimation.FOrc ||
                caster.RaceId == CharacterRaceAnimation.MShaman ||
                caster.RaceId == CharacterRaceAnimation.FShaman);

        string k = skillKey ?? "";
        string kind;
        if (ContainsAny(k, "heal", "cure", "bless", "holy", "recharge", "vitalize", "restore",
                "purify", "peace", "greater_heal", "group_heal", "battle_heal"))
            kind = "White";
        else if (ContainsAny(k, "poison", "curse", "death", "dark", "sleep", "fear", "silence",
                "bleed", "anchor", "hex", "drain", "vampiric", "corpse", "soul"))
            kind = "Black";
        else if (ContainsAny(k, "summon", "servitor", "cubic"))
            kind = "Call";
        else if (ContainsAny(k, "fire", "flame", "ice", "frost", "wind", "storm", "water", "aqua",
                "bolt", "blaze", "flare", "burn", "hydro", "tempest", "twister", "prominence",
                "element", "aura_flare", "aura_burn", "hurricane"))
            kind = "Element";
        else if (ContainsAny(k, "might", "shield", "acumen", "focus", "haste", "guidance",
                "empower", "wild_magic", "concentration", "decrease", "slow", "buff", "song",
                "dance", "chant", "blessed"))
            kind = "Sub";
        else if (ContainsAny(k, "throw", "spear", "bow"))
            kind = "Throw";
        else
            kind = "Shot";

        if (!orcOrDwarf)
            return kind;

        switch (kind)
        {
            case "White": return "Type_A";
            case "Black": return "Type_B";
            case "Element":
            case "Call":
            case "Sub": return "Type_C";
            default: return kind;
        }
    }

    static bool ContainsAny(string hay, params string[] needles)
    {
        for (int i = 0; i < needles.Length; i++)
            if (hay.IndexOf(needles[i]) >= 0)
                return true;
        return false;
    }

    static bool PlaySfx(string clipName, Vector3 pos, float volume)
    {
        AudioClip clip = Load(SfxRoot, clipName);
        if (clip == null)
            return false;
        // 2D for cast SFX too — L2 camera is often far enough that 3D PlayClipAtPoint is inaudible.
        Play2D(clip, volume);
        return true;
    }

    static bool PlayVoiceClip(string clipName)
    {
        AudioClip clip = Load(VoiceRoot, clipName);
        if (clip == null)
            return false;
        Play2D(clip, VoiceVolume);
        return true;
    }

    static void Play2D(AudioClip clip, float volume)
    {
        if (clip == null)
            return;
        EnsureVoiceSource();
        if (_voice2D == null)
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main != null
                ? Camera.main.transform.position
                : Vector3.zero, volume);
            return;
        }
        _voice2D.PlayOneShot(clip, volume);
    }

    static void EnsureVoiceSource()
    {
        if (_voice2D != null)
            return;
        GameObject go = GameObject.Find("FineageSkillAudio2D");
        if (go == null)
        {
            go = new GameObject("FineageSkillAudio2D");
            Object.DontDestroyOnLoad(go);
        }
        _voice2D = go.GetComponent<AudioSource>();
        if (_voice2D == null)
            _voice2D = go.AddComponent<AudioSource>();
        _voice2D.playOnAwake = false;
        _voice2D.spatialBlend = 0f;
        _voice2D.loop = false;
        _voice2D.volume = 1f;
    }

    static AudioClip Load(string root, string clipName)
    {
        string path = root + clipName;
        AudioClip clip;
        if (_cache.TryGetValue(path, out clip) && clip != null)
            return clip;

        float until;
        if (_missUntil.TryGetValue(path, out until) && Time.realtimeSinceStartup < until)
            return null;

        clip = Resources.Load<AudioClip>(path);
        if (clip == null)
        {
            _missUntil[path] = Time.realtimeSinceStartup + MissRetrySeconds;
            return null;
        }
        _missUntil.Remove(path);
        _cache[path] = clip;
        return clip;
    }

    static string KeyFor(int skillId, int level)
    {
        string fromId;
        if (SkillIdSfx.TryGetValue(skillId, out fromId))
            return fromId;

        string name = null;
        try
        {
            SkillNameTable table = TablePacketServices.SkillNames;
            SkillNameData data = table != null ? table.GetName(skillId, Mathf.Max(1, level)) : null;
            if (data == null && table != null)
                data = table.GetName(skillId, 1);
            name = data != null ? data.Name : null;
        }
        catch { }

        string normalized = Normalize(name);
        if (string.IsNullOrEmpty(normalized))
            return null;

        // Alias missing stems onto existing SkillSound files.
        if (normalized == "battle_heal" || normalized == "group_heal"
            || normalized == "elemental_heal" || normalized == "self_heal"
            || normalized == "servitor_heal" || normalized.EndsWith("_heal")
            || normalized.StartsWith("heal_"))
            return "heal";
        if (normalized == "greater_battle_heal" || normalized == "greater_group_heal"
            || normalized.Contains("greater_heal"))
            return "greater_heal";

        // If stem has no cast clip, fall back for any *heal* name.
        if (normalized.Contains("heal") && Load(SfxRoot, normalized + "_cast") == null)
            return "heal";

        return normalized;
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
