using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Interlude skill SFX + hero voice from skillsoundgrp / chrsound.
/// <list type="number">
/// <item><description>Start: <c>*_cast</c> SFX + race <c>sub</c> voice (e.g. Element)</description></item>
/// <item><description>Leave (HitTime−flight): <c>*_shot</c> SFX + race <c>throw</c> voice (e.g. Shot)</description></item>
/// <item><description>Impact: <c>*_explotion</c> when projectile hits (if present)</description></item>
/// </list>
/// Table: <c>Resources/Data/SkillSoundGrp.json</c>.
/// </summary>
public static class SkillCastSound
{
    const string SfxRoot = "Data/Sounds/SkillSound/";
    const string VoiceRoot = "Data/Sounds/SkillVoice/";
    const string GrpResource = "Data/SkillSoundGrp";

    public static float Volume = 0.8f;
    public static float VoiceVolume = 1f;

    static readonly CharacterRaceAnimation[] RaceSlots =
    {
        CharacterRaceAnimation.MFighter,
        CharacterRaceAnimation.FFighter,
        CharacterRaceAnimation.MDarkElf,
        CharacterRaceAnimation.FDarkElf,
        CharacterRaceAnimation.MDwarf,
        CharacterRaceAnimation.FDwarf,
        CharacterRaceAnimation.MElf,
        CharacterRaceAnimation.FElf,
        CharacterRaceAnimation.MMagic,
        CharacterRaceAnimation.FMagic,
        CharacterRaceAnimation.MOrc,
        CharacterRaceAnimation.FOrc,
        CharacterRaceAnimation.MShaman,
        CharacterRaceAnimation.FShaman,
    };

    static readonly Dictionary<int, string> SkillIdSfx = new Dictionary<int, string>
    {
        { 1011, "heal" }, { 1015, "heal" }, { 1027, "heal" }, { 1216, "heal" },
        { 1217, "greater_heal" }, { 1218, "greater_heal" }, { 1256, "heal" },
        { 1311, "heal" }, { 6847, "heal" },
        { 1177, "wind_strike" }, { 1184, "ice_bolt" }, { 1148, "death_spike" },
        { 1069, "sleep" }, { 1160, "slow" }, { 1164, "curse_weakness" },
        { 1168, "curse_poison" }, { 1221, "curse_fear" }, { 1206, "wind_shackle" },
        { 1040, "shield" }, { 1068, "might" }, { 1013, "recharge" },
        { 1078, "concentration" }, { 1085, "acumen" }, { 1059, "empower" },
        { 1045, "blessed_body" }, { 1048, "blessed_soul" }, { 1240, "guidance" },
        { 1242, "death_whisper" }, { 1238, "freezing_shackle" }, { 1239, "hurricane" },
        { 1235, "hydro_blast" }, { 1230, "blaze" }, { 1267, "shadow_flare" },
        { 1144, "servitor_heal" }, { 1127, "servitor_heal" }, { 1147, "vampiric_touch" },
    };

    static Dictionary<int, SkillSoundRow> _bySkill;
    static Dictionary<long, SkillSoundRow> _bySkillLevel;
    static readonly Dictionary<int, int> _defaultLevel = new Dictionary<int, int>();
    static readonly Dictionary<int, PendingCast> _pending = new Dictionary<int, PendingCast>();
    static int _localReleaseCasterId;
    static bool _grpTried;

    struct SkillSoundRow
    {
        public string Cast;
        public string Shot;
        public string Explode;
        public string[] Kinds;       // cast-time voice (sub)
        public string[] ThrowKinds;  // release-time voice (throw)
    }

    struct PendingCast
    {
        public string ShotClip;
        public string ExplodeClip;
        public string ThrowVoiceKind;
        public bool ShotPlayed;
        public bool ExplodePlayed;
        public float StartTime;
        public float HitTimeMs;
        public float ShotDelaySec;
        public int SkillId;
    }

    public static void OnMagicSkillUse(Entity caster, MagicSkillUseDto dto)
    {
        if (caster == null || dto == null)
            return;
        if (WeaponChargeSkill.IsCharge(dto.SkillId))
            return;

        EnsureGrpLoaded();
        Vector3 pos = caster.transform.position;

        SkillSoundRow row;
        bool fromGrp = TryGetRow(dto.SkillId, dto.SkillLvl, out row);

        string castClip = fromGrp ? row.Cast : null;
        string shotClip = fromGrp ? row.Shot : null;
        string explodeClip = fromGrp ? row.Explode : null;
        // skillsoundgrp voice slots. Scrolls/potions/items have empty kinds — no hero voice.
        // Only invent VoiceKindFor / "Shot" when the skill is missing from the table entirely.
        string castVoice = fromGrp ? KindForRace(row.Kinds, caster) : null;
        string throwVoice = fromGrp ? KindForRace(row.ThrowKinds, caster) : null;

        string key = KeyFor(dto.SkillId, dto.SkillLvl);
        if (string.IsNullOrEmpty(castClip) && !string.IsNullOrEmpty(key))
            castClip = key + "_cast";
        if (string.IsNullOrEmpty(shotClip) && !string.IsNullOrEmpty(key))
            shotClip = key + "_shot";
        if (!fromGrp)
        {
            if (string.IsNullOrEmpty(castVoice))
                castVoice = VoiceKindFor(key ?? StemFromClip(castClip), caster);
            if (string.IsNullOrEmpty(throwVoice))
                throwVoice = "Shot";
        }

        // Packet HitTime ≈ impact; projectile leaves at HitTime − flight.
        // Same ResolveMagicFlightTimeMs as READY_TO_ACT / MagicCastData (not explode-gated).
        Transform targetTf = ResolveTargetTransform(caster, dto.TargetId);
        float flightMs = SetupDurationHelper.ResolveMagicFlightTimeMs(caster, dto.SkillId, targetTf);
        float shotDelay = Mathf.Max(0f, (dto.HitTime - flightMs) / 1000f);

        float castLen = ClipLengthSec(SfxRoot, castClip);
        float shotLen = ClipLengthSec(SfxRoot, shotClip);
        float voiceLen = VoiceClipLengthSec(caster, castVoice);
        float explodeDelay = 0f;
        if (!string.IsNullOrEmpty(explodeClip))
            explodeDelay = Mathf.Max(shotDelay + 0.05f, dto.HitTime / 1000f);
        float hitSec = dto.HitTime / 1000f;

        Debug.Log(
            "[CastSync] PIPELINE.start skill=" + dto.SkillId +
            " lv=" + dto.SkillLvl +
            " hitMs=" + dto.HitTime.ToString("F0") +
            " flightMs=" + flightMs.ToString("F0") +
            " fromGrp=" + fromGrp +
            " key=" + (key ?? "-") +
            "\n  t=0.000  CAST_SFX+VOICE  cast=" + (castClip ?? "-") +
            " len=" + castLen.ToString("F2") +
            " voice=" + (castVoice ?? "-") + " vLen=" + voiceLen.ToString("F2") +
            "\n  t=" + shotDelay.ToString("F3") +
            "  RELEASE  StopCast + SHOT + THROW  shot=" + (shotClip ?? "-") +
            " len=" + shotLen.ToString("F2") +
            " throw=" + (throwVoice ?? "-") +
            "\n  t=" + explodeDelay.ToString("F3") +
            "  EXPLODE  clip=" + (explodeClip ?? "-") +
            "\n  t=" + hitSec.ToString("F3") +
            "  BAR_EMPTY (gauge HitTime)" +
            "\n  note=anim speeds/walls come later in ANIM.setup; compare BAR.tick remainPct vs these t=");

        // 1) Cast SFX + cast-time hero voice (sub / Element / White / …).
        if (!string.IsNullOrEmpty(castClip))
            PlayCastSfx(castClip, Volume);
        PlayVoiceKind(caster, castVoice);

        int casterId = caster.Identity != null ? caster.Identity.Id : 0;
        _pending[casterId] = new PendingCast
        {
            ShotClip = shotClip ?? "",
            ExplodeClip = explodeClip ?? "",
            ThrowVoiceKind = throwVoice ?? "",
            ShotPlayed = false,
            ExplodePlayed = false,
            StartTime = Time.realtimeSinceStartup,
            HitTimeMs = dto.HitTime,
            ShotDelaySec = shotDelay,
            SkillId = dto.SkillId,
        };
        if (caster == DomainPacketServices.LocalPlayer)
            _localReleaseCasterId = casterId;

        string shotCapture = shotClip ?? "";
        string throwCapture = throwVoice ?? "";
        Entity casterCapture = caster;
        int idCapture = casterId;
        InterludeSfx.PlayAfter(shotDelay, () =>
            PlayRelease(idCapture, casterCapture, shotCapture, throwCapture, "timer"));

        // Explode backup if projectile callback missed (impact ≈ HitTime in Fineage timing).
        if (!string.IsNullOrEmpty(explodeClip))
        {
            Vector3 hitPos = targetTf != null ? targetTf.position : pos;
            InterludeSfx.PlayAfter(explodeDelay, () => TryPlayExplode(casterId, hitPos, "timer"));
        }
    }

    public static void NotifyLocalCastBarFinished()
    {
        if (_localReleaseCasterId <= 0)
            return;
        PendingCast pending;
        string shot = "";
        string throwKind = "";
        if (_pending.TryGetValue(_localReleaseCasterId, out pending))
        {
            shot = pending.ShotClip;
            throwKind = pending.ThrowVoiceKind;
            Debug.Log(
                "[CastSync] SFX.castBarDone skill=" + pending.SkillId +
                " t=" + (Time.realtimeSinceStartup - pending.StartTime).ToString("F3") +
                " plannedShot=" + pending.ShotDelaySec.ToString("F3") +
                " alreadyShot=" + pending.ShotPlayed);
        }
        PlayRelease(_localReleaseCasterId, DomainPacketServices.LocalPlayer, shot, throwKind, "castBar");
    }

    public static void OnMagicSkillLaunched(Entity caster, MagicSkillLaunchedDto dto)
    {
        // Launched is often cast-release on L2 packs — explode is projectile impact.
    }

    public static void OnProjectileImpact(int attackerEntityId, Vector3 hitPoint)
    {
        if (attackerEntityId <= 0)
            return;
        TryPlayExplode(attackerEntityId, hitPoint, "projectile");
    }

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
        CharacterRaceAnimation resolved = CharacterRaceAnimationParser.ResolveRaceAnimation(
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

    [Serializable]
    class SkillSoundGrpDto
    {
        public SkillSoundEntryDto[] entries;
    }

    [Serializable]
    class SkillSoundEntryDto
    {
        public int skillId;
        public int level;
        public string cast;
        public string shot;
        public string explode;
        public string[] kinds;
        public string[] throwKinds;
    }
}
