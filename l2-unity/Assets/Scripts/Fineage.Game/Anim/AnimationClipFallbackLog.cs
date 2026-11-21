/// <summary>Logs silent animation duration substitutions (audit batch 6).</summary>
public static class AnimationClipFallbackLog
{
    static readonly System.Collections.Generic.HashSet<string> LoggedKeys =
        new System.Collections.Generic.HashSet<string>();

    public static void LogDefaultAttackLength(int objectId, string stateName)
    {
        string key = objectId + ":" + stateName;
        lock (LoggedKeys)
        {
            if (!LoggedKeys.Add(key))
                return;
        }

        FineageLog.Error(
            FineageLogCategory.World,
            "Animation: default attack clip length 1s for objectId=" + objectId +
            " state='" + stateName + "' — check animator clips / import");
    }
}
