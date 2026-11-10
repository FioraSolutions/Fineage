using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static partial class SkillCastSound
{
    static void PlayRelease(int casterId, Entity caster, string shotClip, string throwVoiceKind, string reason)
    {
        PendingCast pending;
        float t = 0f;
        float planned = 0f;
        int skillId = 0;
        if (_pending.TryGetValue(casterId, out pending))
        {
            t = Time.realtimeSinceStartup - pending.StartTime;
            planned = pending.ShotDelaySec;
            skillId = pending.SkillId;
            if (pending.ShotPlayed)
            {
                Debug.Log(
                    "[CastSync] SFX.release SKIP duplicate reason=" + reason +
                    " skill=" + skillId + " t=" + t.ToString("F3"));
                return;
            }
            pending.ShotPlayed = true;
            if (string.IsNullOrEmpty(shotClip))
                shotClip = pending.ShotClip;
            if (string.IsNullOrEmpty(throwVoiceKind))
                throwVoiceKind = pending.ThrowVoiceKind;
            _pending[casterId] = pending;
        }

        bool castWasPlaying = InterludeSfx.IsCastPlaying;
        float castLeft = InterludeSfx.CastRemainingSec;
        InterludeSfx.StopCast();

        Vector3 pos = caster != null ? caster.transform.position : Vector3.zero;
        bool shotOk = false;
        if (!string.IsNullOrEmpty(shotClip))
            shotOk = PlaySfx(shotClip, pos, Volume);

        // 2) Release hero voice — PlayVoice cuts any still-playing cast line.
        PlayVoiceKind(caster, throwVoiceKind);

        float barRem = CastBarOverlay.Remaining01;
        Debug.Log(
            "[CastSync] PIPELINE.hit RELEASE reason=" + reason +
            " skill=" + skillId +
            " t=" + t.ToString("F3") +
            " planned=" + planned.ToString("F3") +
            " drift=" + (t - planned).ToString("F3") +
            " barRemainPct=" + (barRem < 0f ? -1f : barRem * 100f).ToString("F0") +
            " StopCast wasPlaying=" + castWasPlaying +
            " leftSec=" + castLeft.ToString("F2") +
            " shot=" + (shotClip ?? "-") + " ok=" + shotOk +
            " voiceThrow=" + (throwVoiceKind ?? "-"));

        if (caster != null && caster.IsSpiritshotCharged)
            caster.IsSpiritshotCharged = false;
    }

    static void TryPlayExplode(int casterId, Vector3 pos, string reason)
    {
        PendingCast pending;
        if (!_pending.TryGetValue(casterId, out pending) || pending.ExplodePlayed)
            return;
        if (string.IsNullOrEmpty(pending.ExplodeClip))
            return;
        pending.ExplodePlayed = true;
        _pending[casterId] = pending;
        float t = Time.realtimeSinceStartup - pending.StartTime;
        bool ok = PlaySfx(pending.ExplodeClip, pos, Volume);
        float barRem = CastBarOverlay.Remaining01;
        Debug.Log(
            "[CastSync] PIPELINE.hit EXPLODE reason=" + reason +
            " skill=" + pending.SkillId +
            " t=" + t.ToString("F3") +
            " hitMs=" + pending.HitTimeMs.ToString("F0") +
            " barRemainPct=" + (barRem < 0f ? -1f : barRem * 100f).ToString("F0") +
            " clip=" + pending.ExplodeClip +
            " ok=" + ok);
    }

    static float ClipLengthSec(string root, string clipName)
    {
        if (string.IsNullOrEmpty(clipName))
            return 0f;
        AudioClip clip = Load(root, clipName);
        if (clip == null)
            clip = Load(root, clipName.ToLowerInvariant());
        return clip != null ? clip.length : 0f;
    }

    static float VoiceClipLengthSec(Entity caster, string kind)
    {
        string prefix = VoicePrefixFor(caster);
        if (string.IsNullOrEmpty(prefix) || string.IsNullOrEmpty(kind))
            return 0f;
        AudioClip clip = Load(VoiceRoot, prefix + "_" + kind);
        return clip != null ? clip.length : 0f;
    }

    static Transform ResolveTargetTransform(Entity caster, int targetId)
    {
        if (targetId > 0)
        {
            World world = WorldApplyPacketServices.GameWorld ?? GamePacketServices.GameWorld;
            Entity target = world != null ? world.GetEntityNoLockSync(targetId) : null;
            if (target != null)
                return target.transform;
        }
        return caster != null ? caster.Target : null;
    }

    static void PlayCastSfx(string clipName, float volume)
    {
        AudioClip clip = Load(SfxRoot, clipName);
        if (clip == null && !string.IsNullOrEmpty(clipName))
            clip = Load(SfxRoot, clipName.ToLowerInvariant());
        if (clip == null)
            return;
        InterludeSfx.PlayCast(clip, volume);
    }

    static string StemFromClip(string clip)
    {
        if (string.IsNullOrEmpty(clip))
            return null;
        if (clip.EndsWith("_cast", StringComparison.Ordinal))
            return clip.Substring(0, clip.Length - 5);
        if (clip.EndsWith("_shot", StringComparison.Ordinal))
            return clip.Substring(0, clip.Length - 5);
        return clip;
    }

    static void PlayVoiceKind(Entity caster, string kind)
    {
        string prefix = VoicePrefixFor(caster);
        if (string.IsNullOrEmpty(prefix) || string.IsNullOrEmpty(kind))
            return;
        string primary = prefix + "_" + kind;
        if (PlayVoiceClip(primary))
        {
            Debug.Log("[CastSync] VOICE play=" + primary);
            return;
        }
        // Soft fallbacks only within the same phase family.
        if (kind == "Notarget" && PlayVoiceClip(prefix + "_Shot"))
        {
            Debug.Log("[CastSync] VOICE play=" + prefix + "_Shot (fallback from Notarget)");
            return;
        }
        if (kind == "Shot" && PlayVoiceClip(prefix + "_Throw"))
        {
            Debug.Log("[CastSync] VOICE play=" + prefix + "_Throw (fallback from Shot)");
            return;
        }
        Debug.LogWarning("[CastSync] VOICE miss kind=" + kind + " tried=" + primary);
    }

    static string KindForRace(string[] kinds, Entity caster)
    {
        if (kinds == null || kinds.Length == 0 || caster == null)
            return null;

        CharacterRaceAnimation raceId = ResolveRaceForVoice(caster, caster.RaceId);
        for (int i = 0; i < RaceSlots.Length && i < kinds.Length; i++)
        {
            if (RaceSlots[i] == raceId && !string.IsNullOrEmpty(kinds[i]))
                return kinds[i];
        }
        if (kinds.Length > 8 && !string.IsNullOrEmpty(kinds[8]))
            return kinds[8];
        for (int i = 0; i < kinds.Length; i++)
            if (!string.IsNullOrEmpty(kinds[i]))
                return kinds[i];
        return null;
    }

    static string VoicePrefixFor(Entity caster)
    {
        if (caster == null)
            return null;

        CharacterRaceAnimation raceId = ResolveRaceForVoice(caster, caster.RaceId);
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
        CharacterRaceAnimation resolved = CharacterRaceAnimationResolve.ResolveRaceAnimation(
            race, appearance, playerClass);
        bool resolvedMage =
            resolved == CharacterRaceAnimation.MMagic || resolved == CharacterRaceAnimation.FMagic;
        bool looksUnset = raceId == CharacterRaceAnimation.FElf && race != CharacterRace.Elf;
        if (resolvedMage || looksUnset)
        {
            if (raceId != resolved)
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
            case "Black":
            case "Element": return "Type_B";
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
        if (clip == null && !string.IsNullOrEmpty(clipName))
            clip = Load(SfxRoot, clipName.ToLowerInvariant());
        if (clip == null)
            return false;
        InterludeSfx.Play(clip, volume);
        return true;
    }

    static bool PlayVoiceClip(string clipName)
    {
        AudioClip clip = Load(VoiceRoot, clipName);
        if (clip == null)
            return false;
        // Dedicated voice source: throw line replaces/cuts the cast line (Interlude).
        InterludeSfx.PlayVoice(clip, VoiceVolume);
        return true;
    }

    static AudioClip Load(string root, string clipName)
    {
        return InterludeSfx.Load(root + clipName);
    }

    static void EnsureGrpLoaded()
    {
        if (_grpTried)
            return;
        _grpTried = true;
        _bySkill = new Dictionary<int, SkillSoundRow>();
        _bySkillLevel = new Dictionary<long, SkillSoundRow>();

        TextAsset asset = Resources.Load<TextAsset>(GrpResource);
        if (asset == null || string.IsNullOrEmpty(asset.text))
        {
            Debug.LogWarning("[SkillCastSound] missing Resources/" + GrpResource + ".json — using name fallback");
            return;
        }

        try
        {
            SkillSoundGrpDto root = JsonUtility.FromJson<SkillSoundGrpDto>(asset.text);
            if (root == null || root.entries == null)
                return;

            for (int i = 0; i < root.entries.Length; i++)
            {
                SkillSoundEntryDto e = root.entries[i];
                if (e == null)
                    continue;
                var row = new SkillSoundRow
                {
                    Cast = e.cast ?? "",
                    Shot = e.shot ?? "",
                    Explode = e.explode ?? "",
                    Kinds = e.kinds ?? Array.Empty<string>(),
                    ThrowKinds = e.throwKinds ?? Array.Empty<string>(),
                };
                long packed = Pack(e.skillId, e.level);
                _bySkillLevel[packed] = row;
                if (!_bySkill.TryGetValue(e.skillId, out _)
                    || e.level <= LevelOfDefault(e.skillId))
                {
                    _bySkill[e.skillId] = row;
                    _defaultLevel[e.skillId] = e.level;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[SkillCastSound] SkillSoundGrp parse failed: " + ex.Message);
        }
    }

    static int LevelOfDefault(int skillId)
    {
        int lvl;
        return _defaultLevel.TryGetValue(skillId, out lvl) ? lvl : int.MaxValue;
    }

    static bool TryGetRow(int skillId, int level, out SkillSoundRow row)
    {
        row = default;
        if (_bySkillLevel != null && _bySkillLevel.TryGetValue(Pack(skillId, Mathf.Max(1, level)), out row))
            return true;
        if (_bySkill != null && _bySkill.TryGetValue(skillId, out row))
            return true;
        return false;
    }

    static long Pack(int skillId, int level) => ((long)skillId << 16) | (uint)level;
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

        if (normalized == "battle_heal" || normalized == "group_heal"
            || normalized == "elemental_heal" || normalized == "self_heal"
            || normalized == "servitor_heal" || normalized.EndsWith("_heal")
            || normalized.StartsWith("heal_"))
            return "heal";
        if (normalized == "greater_battle_heal" || normalized == "greater_group_heal"
            || normalized.Contains("greater_heal"))
            return "greater_heal";

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
