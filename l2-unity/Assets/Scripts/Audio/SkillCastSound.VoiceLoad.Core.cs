using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static partial class SkillCastSound
{
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
}
