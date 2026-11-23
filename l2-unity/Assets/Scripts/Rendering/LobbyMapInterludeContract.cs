/// <summary>Source-of-truth identifiers for the Interlude account lobby map (not later chronicle lobby swaps).</summary>
public static class LobbyMapInterludeContract
{
    public const string AdditiveLobbySceneName = "l2_lobby";
    public const string RetailMapName = "Lobby.unr";
    /// <summary>zone-render.tsv map id for Lobby.unr (the package name, not the scene).</summary>
    public const string ZoneCatalogMap = "Lobby";
    public const string LobbyEnvironmentProfileResource = LobbyAccountEnvironmentProfile.DefaultResourcePath;

    /// <summary>
    /// The lobby renders its retail skybox BSP box (Brush279 CSG_Add with
    /// L2_Skies faces around SkyZoneInfo0) — that box IS the vista's sky.
    /// World tiles use the Unity-skybox stand-in (zone-render sky pan) instead,
    /// so their own L2_Skies skybox brushes stay excluded from the brush bake.
    /// </summary>
    public static bool BakesSkyboxBsp(string tileId)
    {
        return string.Equals(tileId, AdditiveLobbySceneName, System.StringComparison.OrdinalIgnoreCase);
    }
}