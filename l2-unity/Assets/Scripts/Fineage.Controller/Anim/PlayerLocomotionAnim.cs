using System;
using System.Collections.Generic;

public sealed partial class PlayerLocomotionAnim : PlayerAnimStateMapBase
{
    public static readonly PlayerLocomotionAnim Instance = new PlayerLocomotionAnim();

    static readonly string[] Prefixes =
    {
        "atkwait_", // before wait_
        "wait_",
        "walk_",
        "run_"
    };

    protected override IReadOnlyList<string> PrefixesLongestFirst => Prefixes;






    /// <summary>
    /// Full name from PlayAnimation (e.g. wait_1HS, walk_dual, run_bow, atkwait_1HS).
    /// </summary>

}
