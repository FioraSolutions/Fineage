/// <summary>
/// Pure rules for when the shared Sound session should be created (EditMode-testable).
/// </summary>
public static class AudioSessionRules
{
    public const string SoundPrefabResourcesPath = "Prefab/Sound";

    public static bool ShouldCreatePrefab(bool hasAudioManager, bool hasMusicManager) =>
        !hasAudioManager || !hasMusicManager;
}