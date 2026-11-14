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
                new GenderStats(0.0047f, 0.0047f, 1f) // Female. Zeros clamped run/walk to the floor, so she slid.
            )
        },
        {
            (int)ClassId.MAGE,
            new BaseStatModel(
                new GenderStats(0.0033f, 0.0047f, 1.8f), // Male. 2H clip is twice the 1H run.
                new GenderStats(0.0033f, 0.0090f, 1f) // Female. Walk clip is 0.96s; 2H clip matches 1H.
            )
        },
        {
            (int)ClassId.ELVEN_FIGHTER,
            new BaseStatModel(
                new GenderStats(0.0043f, 0.0047f, 1f),
                new GenderStats(0.0065f, 0.0043f, 1f) // Run clip 0.75s vs human 0.54s.
            )
        },
        {
            (int)ClassId.ELVEN_MAGE,
            new BaseStatModel(
                new GenderStats(0.0043f, 0.0047f, 1f),
                new GenderStats(0.0065f, 0.0043f, 1f)
            )
        },
        {
            (int)ClassId.DARK_FIGHTER,
            new BaseStatModel(
                new GenderStats(0.0058f, 0.0039f, 1f),
                new GenderStats(0.0058f, 0.0074f, 1f) // Walk clip 0.79s.
            )
        },
        {
            (int)ClassId.DARK_MAGE,
            new BaseStatModel(
                new GenderStats(0.0058f, 0.0039f, 1f),
                new GenderStats(0.0058f, 0.0074f, 1f)
            )
        },
        {
            (int)ClassId.ORC_FIGHTER,
            new BaseStatModel(
                new GenderStats(0.0072f, 0.0051f, 1f),
                new GenderStats(0.0072f, 0.0051f, 1f)
            )
        },
        {
            (int)ClassId.ORC_MAGE,
            new BaseStatModel(
                new GenderStats(0.0087f, 0.0051f, 1f), // Shaman run clip is 1.0s.
                new GenderStats(0.0072f, 0.0051f, 1f)
            )
        },
        {
            (int)ClassId.DWARVEN_FIGHTER,
            new BaseStatModel(
                new GenderStats(0.0040f, 0.0027f, 1f),
                new GenderStats(0.0040f, 0.0055f, 1f)
            )
        }
    };

    /// <summary>
    /// RunK/WalkK are scaled from the human fighter clips (run 0.54s, walk 0.50s at 0.0047)
    /// by each race's own clip length, so a longer cycle is not played at the human rate
    /// and the feet do not slide. Human mage keeps its tuned run; her walk clip is 0.96s.
    /// </summary>


}
