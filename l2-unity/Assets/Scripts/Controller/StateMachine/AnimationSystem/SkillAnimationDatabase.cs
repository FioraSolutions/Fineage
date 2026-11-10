using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SkillAnimationDatabase
{
    private static readonly Dictionary<string, SkillClips> _raceCacheMMagic = new Dictionary<string, SkillClips>();
    private static readonly Dictionary<string, SkillClips> _raceCacheFMagic = new Dictionary<string, SkillClips>();
    private static readonly Dictionary<string, SkillClips> _raceCacheMFighter = new Dictionary<string, SkillClips>();
    private static readonly Dictionary<string, SkillClips> _raceCacheMElf = new Dictionary<string, SkillClips>();
    private const string  FOLDER_MElf = "Data/Animations/Elf/MElf/Clips/";
    private const string MElf = "MElf";
    private static readonly Dictionary<string, SkillClips> _raceCacheFDarkElf = new Dictionary<string, SkillClips>();
    private const string  FOLDER_MMagic = "Data/Animations/Magic/MMagic/Clips/";
    private const string  FOLDER_FMagic = "Data/Animations/Magic/FMagic/Clips/";
    private const string  FOLDER_MFighter = "Data/Animations/Fighter/MFighter/Clips/";
    private const string  FOLDER_FDarkElf = "Data/Animations/DarkElf/FDarkElf/Clips/";
    private const string MMagic = "MMagic";
    private const string FMagic = "FMagic";
    private const string MFighter = "MFighter";
    private const string FDarkElf = "FDarkElf";
    public class SkillClips
    {
        private AnimationClip _clip;
        private string _name;
        private string _path;
        
        public SkillClips(string folder , string name)
        {
            string path = folder + name;

            _name = name;
            _path = path;
        }

        public void Load()
        {
            if(_clip == null)
            {
                _clip = Resources.Load<AnimationClip>(_path);
                if (_clip != null)
                {
                    // Debug.Log($"SkillAnimationDatabase> Загрузился ролик анимации  в кэш имя {_name} его путь {_path}");
                }
                else
                {
                    // Debug.LogWarning($"SkillAnimationDatabase> Не смогли загрузить в кэщ {_name} его путь {_path}");
                }

            }
        }

        public AnimationClip GetClip()
        {
            return _clip;
        }

        public string GetName()
        {
            return _name;
        }
    }


    public static void Initialize()
    {
        if (_raceCacheMMagic.Count > 0)
            return;

        //MMagic Эти ролики берет для переиспользования (этот override аниматор)
        _raceCacheMMagic.Add("CastLong_" + MMagic, new SkillClips(FOLDER_MMagic, "MMagic_M000_b.ao_CastLong_MMagic"));
        _raceCacheMMagic.Add("CastShort_" + MMagic, new SkillClips(FOLDER_MMagic, "MMagic_M000_b.ao_CastShort_MMagic"));
        _raceCacheMMagic.Add("CastMid_"+ MMagic, new SkillClips(FOLDER_MMagic, "MMagic_M000_b.ao_CastMid_MMagic"));
        _raceCacheMMagic.Add("CastEnd_"+ MMagic, new SkillClips (FOLDER_MMagic, "MMagic_M000_b.ao_CastEnd_MMagic"));
        _raceCacheMMagic.Add("MagicShot_" + MMagic, new SkillClips (FOLDER_MMagic, "MMagic_M000_b.ao_MagicShot_MMagic"));
        _raceCacheMMagic.Add("MagicNoTarget_" + MMagic, new SkillClips(FOLDER_MMagic, "MMagic_M000_b.ao_MagicNoTarget_MMagic"));
        _raceCacheMMagic.Add("MagicNotarget_" + MMagic, new SkillClips(FOLDER_MMagic, "MMagic_M000_b.ao_MagicNoTarget_MMagic"));
        _raceCacheMMagic.Add("MagicThrow_" + MMagic, new SkillClips(FOLDER_MMagic, "MMagic_M000_b.ao_MagicThrow_MMagic"));

        // FMagic.overrideController: Cast* body clips are not imported yet — reuse MMagic cast
        // poses so female mage still plays CastMid/End/Shot after MagicSkillUse (plan #mesh FX).
        _raceCacheFMagic.Add("CastLong_" + FMagic, new SkillClips(FOLDER_MMagic, "MMagic_M000_b.ao_CastLong_MMagic"));
        _raceCacheFMagic.Add("CastShort_" + FMagic, new SkillClips(FOLDER_MMagic, "MMagic_M000_b.ao_CastShort_MMagic"));
        _raceCacheFMagic.Add("CastMid_" + FMagic, new SkillClips(FOLDER_MMagic, "MMagic_M000_b.ao_CastMid_MMagic"));
        _raceCacheFMagic.Add("CastEnd_" + FMagic, new SkillClips(FOLDER_MMagic, "MMagic_M000_b.ao_CastEnd_MMagic"));
        _raceCacheFMagic.Add("MagicShot_" + FMagic, new SkillClips(FOLDER_MMagic, "MMagic_M000_b.ao_MagicShot_MMagic"));
        _raceCacheFMagic.Add("MagicThrow_" + FMagic, new SkillClips(FOLDER_MMagic, "MMagic_M000_b.ao_MagicThrow_MMagic"));
        _raceCacheFMagic.Add("MagicNoTarget_" + FMagic, new SkillClips(FOLDER_FMagic, "FMagic_m000_b.ao_MagicNoTarget_FMagic"));
        _raceCacheFMagic.Add("MagicNotarget_" + FMagic, new SkillClips(FOLDER_FMagic, "FMagic_m000_b.ao_MagicNoTarget_FMagic"));

        _raceCacheMFighter.Add("CastShort_" + MFighter, new SkillClips(FOLDER_MMagic, "MMagic_M000_b.ao_CastShort_MMagic"));
        _raceCacheMFighter.Add("CastMid_" + MFighter, new SkillClips(FOLDER_MFighter, "MFighter_m000_b.ao_CastMid_MFighter"));
        _raceCacheMFighter.Add("CastEnd_" + MFighter, new SkillClips(FOLDER_MFighter, "MFighter_m000_b.ao_CastEnd_MFighter"));
        _raceCacheMFighter.Add("MagicShot_" + MFighter, new SkillClips(FOLDER_MFighter, "MFighter_m000_b.ao_MagicShot_MFighter"));
        _raceCacheMFighter.Add("MagicThrow_" + MFighter, new SkillClips(FOLDER_MFighter, "MFighter_m000_b.ao_MagicThrow_MFighter"));

        // MElf (light elf male) — clips from Elf.ukx via L2MElfAnimImporter, same keys as MMagic
        _raceCacheMElf.Add("CastLong_" + MElf, new SkillClips(FOLDER_MElf, "MElf_m000_b.ao_CastLong_MElf"));
        _raceCacheMElf.Add("CastShort_" + MElf, new SkillClips(FOLDER_MElf, "MElf_m000_b.ao_CastShort_MElf"));
        _raceCacheMElf.Add("CastMid_" + MElf, new SkillClips(FOLDER_MElf, "MElf_m000_b.ao_CastMid_MElf"));
        _raceCacheMElf.Add("CastEnd_" + MElf, new SkillClips(FOLDER_MElf, "MElf_m000_b.ao_CastEnd_MElf"));
        _raceCacheMElf.Add("MagicShot_" + MElf, new SkillClips(FOLDER_MElf, "MElf_m000_b.ao_MagicShot_MElf"));
        _raceCacheMElf.Add("MagicNoTarget_" + MElf, new SkillClips(FOLDER_MElf, "MElf_m000_b.ao_MagicNotarget_MElf"));
        _raceCacheMElf.Add("MagicNotarget_" + MElf, new SkillClips(FOLDER_MElf, "MElf_m000_b.ao_MagicNotarget_MElf"));
        _raceCacheMElf.Add("MagicThrow_" + MElf, new SkillClips(FOLDER_MElf, "MElf_m000_b.ao_MagicThrow_MElf"));

        //эти ролики берет для замены на свои (т.е это оригинальный аниматор)
        //FDarkElf
        _raceCacheFDarkElf.Add("CastLong_" + FDarkElf, new SkillClips(FOLDER_FDarkElf, "FDarkElf_m001_b.ao_CastLong_FDarkElf"));
        _raceCacheFDarkElf.Add("CastShort_" + FDarkElf, new SkillClips(FOLDER_FDarkElf, "FDarkElf_m001_b.ao_CastShort_FDarkElf"));
        _raceCacheFDarkElf.Add("CastMid_" + FDarkElf, new SkillClips(FOLDER_FDarkElf, "FDarkElf_m001_b.ao_CastMid_FDarkElf"));
        _raceCacheFDarkElf.Add("CastEnd_" + FDarkElf, new SkillClips(FOLDER_FDarkElf, "FDarkElf_m001_b.ao_CastEnd_FDarkElf"));
        _raceCacheFDarkElf.Add("MagicShot_" + FDarkElf, new SkillClips(FOLDER_FDarkElf, "FDarkElf_m001_b.ao_MagicShot_A_FDarkElf"));
        _raceCacheFDarkElf.Add("MagicThrow_" + FDarkElf, new SkillClips(FOLDER_FDarkElf, "FDarkElf_m001_b.ao_MagicShot_A_FDarkElf"));


    }

    public static void LoadRaceAnimations(string raceName)
    {
        Dictionary<string, SkillClips> cache = CacheFor(raceName);
        if (cache == null)
            return;

        foreach (var item in cache)
            item.Value.Load();
    }

    public static AnimationClip GetOverrideClip(string overrideAnimName, string raceName)
    {
        Dictionary<string, SkillClips> cache = CacheFor(raceName);
        if (cache == null || string.IsNullOrEmpty(overrideAnimName))
            return null;

        string key = overrideAnimName + "_" + raceName;
        if (cache.TryGetValue(key, out SkillClips clips))
        {
            clips.Load();
            return clips.GetClip();
        }
        return null;
    }

    static Dictionary<string, SkillClips> CacheFor(string raceName)
    {
        Initialize();
        if (string.IsNullOrEmpty(raceName))
            return null;
        // Exact match first; then contains — overrideController names are usually "MMagic"
        // but some prefabs report a longer controller name.
        if (raceName == MMagic || ContainsRaceToken(raceName, MMagic))
            return _raceCacheMMagic;
        if (raceName == FMagic || ContainsRaceToken(raceName, FMagic))
            return _raceCacheFMagic;
        if (raceName == MFighter || ContainsRaceToken(raceName, MFighter))
            return _raceCacheMFighter;
        if (raceName == MElf || ContainsRaceToken(raceName, MElf))
            return _raceCacheMElf;
        return null;
    }

    static bool ContainsRaceToken(string raceName, string token) =>
        raceName.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0;

    public static string GetAnimationClipName(string trigerName, string raceName)
    {
        string slotTriggerName = ResolveOverrideSlotTriggerName(trigerName);
        if (FDarkElf == raceName)
        {
            string key = slotTriggerName + "_" + raceName;

            if (_raceCacheFDarkElf.ContainsKey(key))
            {

                return _raceCacheFDarkElf[key].GetName();
            }
        }
        return "";
    }

    /// <summary>
    /// Maps dedicated 2-phase animator triggers to base override slots (CastEnd / MagicShot).
    /// </summary>
    public static string ResolveOverrideSlotTriggerName(string triggerName)
    {
        if (string.IsNullOrEmpty(triggerName))
        {
            return triggerName;
        }

        switch (triggerName)
        {
            case "CastEnd2P":
                return "CastEnd";
            case "MagicShot2P":
                return "MagicShot";
            default:
                return triggerName;
        }
    }

    /// <summary>
    /// Имя из OnAnimationComplete на клипе (CastMid/CastEnd/MagicShot), не ключ override (CastLong/MagicNoTarget).
    /// </summary>
    public static string ResolveOverrideCompletionEventName(string triggerOrOverrideKey)
    {
        if (string.IsNullOrEmpty(triggerOrOverrideKey))
        {
            return triggerOrOverrideKey;
        }

        string slotName = ResolveOverrideSlotTriggerName(triggerOrOverrideKey);
        if (!string.Equals(slotName, triggerOrOverrideKey, StringComparison.Ordinal))
        {
            return slotName;
        }

        switch (triggerOrOverrideKey)
        {
            case "CastLong":
            case "CastShort":
                return "CastMid";
            case "MagicNoTarget":
            case "MagicNotarget":
            case "MagicThrow":
                return "MagicShot";
            default:
                return triggerOrOverrideKey;
        }
    }

}
