using System;
using System.Collections.Generic;

public abstract partial class PlayerAnimStateMapBase
{
    public static string ComposeStateName(string prefix, PlayerAnimWeapon weapon) =>
        prefix + ToWeaponSuffix(weapon);

    protected bool TryResolveStateName(string animName, out string stateName, out string matchedPrefix)
    {
        stateName = null;
        matchedPrefix = null;
        if (string.IsNullOrEmpty(animName))
        {
            return false;
        }

        IReadOnlyList<string> prefixes = PrefixesLongestFirst;
        for (int i = 0; i < prefixes.Count; i++)
        {
            string prefix = prefixes[i];
            if (!animName.StartsWith(prefix, StringComparison.Ordinal))
            {
                continue;
            }

            string suffix = animName.Substring(prefix.Length);
            if (!TryParseWeaponSuffix(suffix, out PlayerAnimWeapon weapon))
            {
                return false;
            }

            matchedPrefix = prefix;
            stateName = ComposeStateName(prefix, weapon);
            return true;
        }

        return false;
    }
}
