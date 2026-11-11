using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static partial class SkillCastSound
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
