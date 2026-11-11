using System;
using UnityEngine;

public static partial class FineageLaunchArgs
{
    public static void ClearCharacterHint()
    {
        if (!PlayerPrefs.HasKey(CharacterPrefKey))
            return;
        PlayerPrefs.DeleteKey(CharacterPrefKey);
        PlayerPrefs.Save();
    }

    public static bool TryGetAutoLogin(out string account, out string password)
    {
        account = ArgValue("-fineageUser");
        password = ArgValue("-fineagePass");
        if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
        {
            account = null;
            password = null;
            return false;
        }
        return true;
    }
}
