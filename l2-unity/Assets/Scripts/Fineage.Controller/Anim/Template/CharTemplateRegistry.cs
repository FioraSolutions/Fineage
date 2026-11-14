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
                new GenderStats(0.0079f, 0.0086f, 1f) // Female. Step covers about half the ground the old rate did.
            )
        },
        {
            (int)ClassId.MAGE,
            new BaseStatModel(
                new GenderStats(0.0033f, 0.0047f, 1.8f), // Male. 2H clip is twice the 1H run.
                new GenderStats(0.0069f, 0.0187f, 1f) // Female. Walk clip is 0.96s; 2H clip matches 1H.
            )
        },
        {
            (int)ClassId.ELVEN_FIGHTER,
            new BaseStatModel(
                new GenderStats(0.0043f, 0.0047f, 1f),
                new GenderStats(0.0109f, 0.0090f, 1f) // Run clip 0.75s, shorter step than the human fighter.
            )
        },
        {
            (int)ClassId.ELVEN_MAGE,
            new BaseStatModel(
                new GenderStats(0.0043f, 0.0047f, 1f),
                new GenderStats(0.0109f, 0.0090f, 1f) // Same FElf body as the elven fighter.
            )
        },
        {
            (int)ClassId.DARK_FIGHTER,
            new BaseStatModel(
                new GenderStats(0.0058f, 0.0039f, 1f),
                new GenderStats(0.0106f, 0.0183f, 1f) // Walk clip 0.79s, step shorter than the body travel.
            )
        },
        {
            (int)ClassId.DARK_MAGE,
            new BaseStatModel(
                new GenderStats(0.0058f, 0.0039f, 1f),
                new GenderStats(0.0106f, 0.0183f, 1f) // Same FDarkElf body as the dark fighter.
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
                new GenderStats(0.0090f, 0.0168f, 1f) // Short legs: the old cycle covered ~1.7x her step.
            )
        }
    };

    /// <summary>
    /// RunK/WalkK turn server speed into the animator walk_speed/run_speed multiplier.
    /// Unity move speed is serverSpeed * 0.0189, so K = 0.0189 * clipLength / stride.
    /// Stride is two steps: twice the foot's swing relative to the pelvis. A longer
    /// clip or a shorter step needs a higher K, otherwise the body outruns the feet.
    /// </summary>


}
