using System;
using UnityEngine;

public partial class NumberUtils
{


    //private static float defaultMetrUnit = 0.0190f;
    private static float defaultMetrUnit = 0.0189f;







    // Interlude sequence rate is 30 fps (AnimTween). Imported clips keep the same
    // keys on a 24 fps timeline: stone golem run is 91 frames, Unity stop time
    // (91-1)/24 = 3.75s. L2 plays those frames at 30 * PlayAnim rate.
    // Live Crasher PlayAnim rate is 1.10 (NonAttackSpeedRate; the AnimRate slot is 0).
    // Animator speed = 1.10 * 30/24. 24/30 ≈ 0.8 is the raw fps ratio, not the
    // parameter: the longer Unity timeline has to be played faster to finish
    // the cycle in the same 91/30/1.10 seconds.
    const float L2SequenceFps = 30f;
    const float UnityClipFps = 24f;
    const float L2NpcMovePlayRate = 1.10f;


    /// <summary>
    /// Same as <see cref="NpcLocomotionAnimRate()"/> but with a source rate read
    /// from AnimTween col "rate" (monsters are 30/24/15 fps). rate/24 corrects the
    /// imported 24 fps Unity clip to the L2 wall-clock cycle length.
    /// </summary>



}
