using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static partial class AutoShotSlotGlow
{
    /// <summary>
    /// Armed shots use the same marker as a running toggle: the <c>l2_skilltime.ToggleEffect</c> shine
    /// (<see cref="ToggleSkillShine"/>). The enchant loading strip used before is a different, unrelated animation
    /// (an item-enchant spinner), so the bar slot looked wrong next to the toggle marker.
    /// </summary>
    const string FrameRoot = "Data/UI/ShortCut/skill_time/ToggleEffect";
    const int FrameCount = 13;
    const long FrameMs = 90;

    static Texture2D[] _frames;
    static bool _triedLoad;



}
