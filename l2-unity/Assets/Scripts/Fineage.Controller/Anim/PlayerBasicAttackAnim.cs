using System;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class PlayerBasicAttackAnim : PlayerAnimStateMapBase
{
    public static readonly PlayerBasicAttackAnim Instance = new PlayerBasicAttackAnim();

    public const string Prefix01 = "jatk01_";
    public const string Prefix02 = "jatk02_";
    public const string Prefix03 = "jatk03_";

    static readonly string[] Prefixes =
    {
        Prefix01,
        Prefix02,
        Prefix03
    };

    protected override IReadOnlyList<string> PrefixesLongestFirst => Prefixes;




    /// <summary>
    /// Full name after GetFinalNameAnim, e.g. jatk01_1HS / jatk03_pole / jatk02_bow.
    /// </summary>




}
