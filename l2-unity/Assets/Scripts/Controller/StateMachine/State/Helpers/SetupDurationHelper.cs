using System;
using System.Collections.Generic;
using UnityEngine;

public partial class SetupDurationHelper
{
    public static readonly HashSet<int> PotionSkillIds = new HashSet<int>
    {
        2031, 2011,
        2244, 2245, 2246, 2247,
        2278, 2279, 2280, 2281, 2282, 2283, 2284, 2285
    };


    /// <summary>
    /// Mix01–Mix09 stretch the first clip. Letters A–I and MS01 stretch the middle one.
    /// </summary>


    /// <summary>
    /// Skillgrp animation tag with at least one real clip. Wolf Collar is is_magic=0
    /// but animation [f] (CastMid / CastEnd / MagicThrow).
    /// </summary>



    /// <summary>
    /// Instant potion/herb: keep current stance. Do not enter MAGIC_SKILLS / PHYSICAL_SKILLS
    /// or pickup cannot WAIT_RETURN (graph has no exit from pickup).
    /// If we already entered a skill state, leave it without cutting pickup.
    /// </summary>




}
