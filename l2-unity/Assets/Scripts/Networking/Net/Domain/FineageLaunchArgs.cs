using System;
using UnityEngine;

public static partial class FineageLaunchArgs
{
    public const string CharacterPrefKey = "Fineage.LocalCharacter";

    public static string CharacterHint
    {
        get { return FirstNonEmpty(ArgValue("-fineageChar"), PlayerPrefs.GetString(CharacterPrefKey, "")); }
    }

    /// <summary>Drop a leftover character name so the next login opens the lobby. Main thread only.</summary>

    public static bool QuitAfterSpawn
    {
        get { return HasArg("-fineageQuitAfterSpawn"); }
    }




}
