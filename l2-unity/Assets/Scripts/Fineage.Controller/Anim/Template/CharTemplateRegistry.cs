using System.Collections.Generic;
using UnityEngine;

public static partial class CharTemplateRegistry
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


}
