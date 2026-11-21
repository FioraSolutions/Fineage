using System;
using System.Collections.Generic;

/// <summary>
/// Maps SpAtkNN_ + weapon suffix → Animator state (CrossFade, not Trigger).
/// Accepts skill-runner tokens such as SpAtk18AnimName_1HS after GetFinalNameAnim.
/// </summary>
public sealed class PlayerSpAtkAnim : PlayerAnimStateMapBase
{
    public static readonly PlayerSpAtkAnim Instance = new PlayerSpAtkAnim();

    public const string Prefix01 = "SpAtk01_";
    public const string Prefix02 = "SpAtk02_";

    protected override IReadOnlyList<string> PrefixesLongestFirst =>
        Array.Empty<string>();

    public static string VariantPrefix(PlayerSpAtkVariant variant)
    {
        switch (variant)
        {
            case PlayerSpAtkVariant.Sp01: return Prefix01;
            case PlayerSpAtkVariant.Sp02: return Prefix02;
            default: return Prefix01;
        }
    }

    public static string ToStateName(PlayerSpAtkVariant variant, PlayerAnimWeapon weapon) =>
        ComposeStateName(VariantPrefix(variant), weapon);

    /// <summary>
    /// Full name after GetFinalNameAnim, e.g. SpAtk01_1HS / SpAtk18AnimName_dual / SpAtk06_hand.
    /// </summary>
    public static bool TryResolve(string animName, out string stateName)
    {
        stateName = null;
        return TryParse(animName, out _, out _, out stateName);
    }

    internal static bool TryParse(
        string animName,
        out int spAtkNumber,
        out PlayerAnimWeapon weapon,
        out string stateName)
    {
        spAtkNumber = 0;
        weapon = PlayerAnimWeapon.Hand;
        stateName = null;
        if (string.IsNullOrEmpty(animName))
            return false;

        int at = animName.IndexOf("SpAtk", StringComparison.OrdinalIgnoreCase);
        if (at < 0)
            return false;

        int digitStart = at + 5;
        int digitEnd = digitStart;
        while (digitEnd < animName.Length && char.IsDigit(animName[digitEnd]))
            digitEnd++;

        if (digitEnd == digitStart ||
            !int.TryParse(animName.Substring(digitStart, digitEnd - digitStart), out spAtkNumber))
            return false;

        string tail = animName.Substring(digitEnd);
        if (tail.StartsWith("AnimName", StringComparison.OrdinalIgnoreCase))
            tail = tail.Substring("AnimName".Length);
        if (tail.StartsWith("_", StringComparison.Ordinal))
            tail = tail.Substring(1);

        if (!TryParseWeaponSuffix(tail, out weapon))
            return false;

        stateName = "SpAtk" + spAtkNumber.ToString("00") + "_" + ToWeaponSuffix(weapon);
        return true;
    }
}