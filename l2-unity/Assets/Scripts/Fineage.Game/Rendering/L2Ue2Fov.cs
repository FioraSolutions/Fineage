using UnityEngine;

/// <summary>
/// UE2 <c>FovAngle</c> is the HORIZONTAL field of view in degrees; the vertical
/// angle follows from the viewport aspect (wider viewports see less vertically).
/// Unity's <see cref="Camera.fieldOfView"/> is VERTICAL. The official Interlude
/// login cameras store no FovAngle and inherit the class default
/// PlayerController.uc FovAngle=50.0 (see scripts/Engine.u dump), so the vista
/// camera must run at vertical = 2*atan(tan(50/2) / aspect), derived at the
/// actual aspect — never a hand-tuned constant.
/// </summary>
public static class L2Ue2Fov
{
    public const float OfficialHfovDeg = 50f;

    public static float VerticalForHorizontal(float hfovDeg, float aspect)
    {
        if (aspect <= 0f)
            aspect = 1f;
        float half = Mathf.Clamp(hfovDeg, 1f, 179f) * 0.5f * Mathf.Deg2Rad;
        return 2f * Mathf.Atan(Mathf.Tan(half) / aspect) * Mathf.Rad2Deg;
    }

    public static void Apply(Camera cam, float hfovDeg)
    {
        if (cam == null)
            return;
        cam.fieldOfView = VerticalForHorizontal(hfovDeg, cam.aspect);
    }
}
