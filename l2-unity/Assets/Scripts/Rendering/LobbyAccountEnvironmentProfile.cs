using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Data-driven account-lobby render settings. Values baked from Interlude
/// <c>l2_lobby.unity</c> RenderSettings (retail <c>Lobby.unr</c> island) and forest-login fog reference.
/// </summary>
[CreateAssetMenu(fileName = "LobbyAccountEnvironmentProfile", menuName = "Fineage/Lobby/Account Environment Profile")]
public sealed class LobbyAccountEnvironmentProfile : ScriptableObject
{
    public const string DefaultResourcePath = "Settings/LobbyAccountEnvironmentProfile";

    [TextArea(2, 6)]
    public string sourceNotes =
        "Baked from l2-lobby/Assets/Resources/Scenes/l2_lobby.unity RenderSettings "
        + "(Interlude Lobby.unr Talking Island hall). Sky: Data/Effects/skybox/L2Sky.mat. "
        + "Retail env tuning: system/env.int StaticLightingAdjust (see MaxCheaters Interlude login pack notes).";

    [Header("Ambient (matches l2_lobby.unity)")]
    public AmbientMode ambientMode = AmbientMode.Skybox;
    [Range(0f, 2f)] public float ambientIntensity = 0.6f;
    public Color ambientSky = new Color(0.5f, 0.5f, 0.5f, 1f);
    public Color ambientEquator = new Color(0.114f, 0.125f, 0.133f, 1f);
    public Color ambientGround = new Color(0.047f, 0.043f, 0.035f, 1f);

    [Header("Sky / reflection")]
    public string skyboxResourcePath = "Data/Effects/skybox/L2Sky";
    [Range(0f, 2f)] public float reflectionIntensity = 1f;

    [Header("Fog (account roster uses hall — keep off like retail Lobby.unr char select)")]
    public bool fogEnabled;
    public Color fogColor = new Color(130f / 255f, 160f / 255f, 165f / 255f, 1f);
    public float fogStartDistance = 19.05f;
    public float fogEndDistance = 123.8f;
}