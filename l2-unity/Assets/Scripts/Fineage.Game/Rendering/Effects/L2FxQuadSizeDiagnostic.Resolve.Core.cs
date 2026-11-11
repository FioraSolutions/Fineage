using UnityEngine;

public static partial class L2FxQuadSizeDiagnostic
{
    private static void ResolveFlipbookFrames(
        Material mat,
        float normalizedAge,
        out int frameA,
        out int frameB,
        out float blend)
    {
        int start = mat.HasProperty("_SubdivisionStart") ? Mathf.RoundToInt(mat.GetFloat("_SubdivisionStart")) : 0;
        int end = mat.HasProperty("_SubdivisionEnd") ? Mathf.RoundToInt(mat.GetFloat("_SubdivisionEnd")) : start;
        int span = Mathf.Max(end - start, 1);
        float frame = start + Mathf.Clamp01(normalizedAge) * span;
        frameA = Mathf.Clamp(Mathf.FloorToInt(frame), start, end);
        frameB = Mathf.Clamp(frameA + 1, start, end);
        blend = Mathf.Clamp01(frame - Mathf.Floor(frame));
    }

    private static float ResolveInitialDelay(Material mat)
    {
        if (!mat.HasProperty("_InitialDelayRange"))
        {
            return 0f;
        }

        Vector4 delayRange = mat.GetVector("_InitialDelayRange");
        float seed = mat.HasProperty("_Seed") ? mat.GetFloat("_Seed") : 0f;
        float startTime = mat.HasProperty("_StartTime") ? mat.GetFloat("_StartTime") : 0f;
        return Mathf.Max(0f, RandomRange(new Vector2(delayRange.x, delayRange.y), seed, startTime, 3f));
    }
}
