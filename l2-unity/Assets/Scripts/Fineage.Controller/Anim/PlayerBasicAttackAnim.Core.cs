using System;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class PlayerBasicAttackAnim : PlayerAnimStateMapBase
{
    public static string VariantPrefix(PlayerBasicAttackVariant variant)
    {
        switch (variant)
        {
            case PlayerBasicAttackVariant.Atk01: return Prefix01;
            case PlayerBasicAttackVariant.Atk02: return Prefix02;
            case PlayerBasicAttackVariant.Atk03: return Prefix03;
            default: return Prefix01;
        }
    }

    public static string ToStateName(PlayerBasicAttackVariant variant, PlayerAnimWeapon weapon) =>
        ComposeStateName(VariantPrefix(variant), weapon);

    public static bool TryParseVariantPrefix(string animName, out PlayerBasicAttackVariant variant, out string suffix)
    {
        variant = default;
        suffix = null;
        if (!Instance.TryResolveStateName(animName, out _, out string matchedPrefix))
        {
            return false;
        }

        if (!TryVariantFromPrefix(matchedPrefix, out variant))
        {
            return false;
        }

        suffix = animName.Substring(matchedPrefix.Length);
        return true;
    }

    public static bool TryResolve(string animName, out string stateName)
    {
        stateName = null;
        return Instance.TryResolveStateName(animName, out stateName, out _);
    }
}
