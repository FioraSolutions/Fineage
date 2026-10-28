namespace UnityEngine;

/// <summary>
/// Stand-in so <c>FineageWireConfig</c> can compile in the tools test host.
/// Enable/override paths do not read stored prefs.
/// </summary>
public static class PlayerPrefs
{
    public static int GetInt(string key, int defaultValue) => defaultValue;

    public static void SetInt(string key, int value)
    {
    }

    public static void Save()
    {
    }
}
