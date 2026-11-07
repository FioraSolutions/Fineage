using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Locomotion-only multipliers for Animator <c>run_speed</c> / <c>walk_speed</c>.
/// Do not use for gestures (pickup, social) — see <see cref="AnimPlayback"/>.
/// </summary>
public static class CharTemplateRegistry
{
    private static readonly Dictionary<int, BaseStatModel> Classes = new Dictionary<int, BaseStatModel>()
    {
        {
            (int)ClassId.FIGHTER,
            new BaseStatModel(
                new GenderStats(0.0047f, 0.0047f, 1f), // Male
                new GenderStats(0, 0, 0)  // Female
            )
        },
        {
            (int)ClassId.MAGE,
            new BaseStatModel(
                new GenderStats(0.0033f, 0.0047f, 1.8f), // Male (с бонусом 2H)
                new GenderStats(0.0035f, 0, 0) // Female
            )
        }
    };

    /// <summary>
    /// Only Human Fighter/Mage rows are authored. Every other base class (Elf/DE/Orc/Dwarf)
    /// reuses the matching Human archetype so animator run/walk floats track server speed
    /// instead of the flat 0.5/0.3 fallback that made non-Humans look sluggish.
    /// </summary>
    static bool TryResolveModel(int classId, out BaseStatModel model)
    {
        if (Classes.TryGetValue(classId, out model))
            return true;

        int fallbackId = CharacterClassParser.IsMage((CharacterClass)classId)
            ? (int)ClassId.MAGE
            : (int)ClassId.FIGHTER;
        return Classes.TryGetValue(fallbackId, out model);
    }

    public static float GetRunSpeed(int classId, int sex, float serverValue, bool isTwoHanded)
    {
        if (TryResolveModel(classId, out var model))
        {
            var stats = model.GetData(sex);

            float speed = serverValue * stats.RunK;

            if (isTwoHanded)
                speed *= stats.TwoHandedBonus;

            return Mathf.Clamp(speed, 0.25f, 0.85f);
        }

        return 0.5f;
    }

    public static float GetWalkSpeed(int classId, int sex, float serverValue, bool isTwoHanded)
    {
        if (TryResolveModel(classId, out var model))
        {
            var stats = model.GetData(sex);
            return Mathf.Clamp(serverValue * stats.WalkK, 0.2f, 0.7f);
        }

        return 0.3f;
    }
}
