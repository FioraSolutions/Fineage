using UnityEngine;

/// <summary>
/// Reproduces calib sprite vertex-shader quad size in C# for debugging.
/// Matches: sizeM = sizeUU / 52.5 * K in object space.
/// </summary>
public static partial class L2FxQuadSizeDiagnostic
{
    private const float UuToMeters = 1f / 52.5f;
    private const float DefaultWorldCalibration = 1.8f;
    public const float LogIntervalSec = 0.25f;

    public struct QuadSizeSnapshot
    {
        public float startSizeMidUU;
        public float sizeMul;
        public float sizeUU;
        public float sizeMetersVertex;
        public float finalWidthM;
        public float finalHeightM;
        public float ageNorm;
        public float ageSec;
        public float lifetimeSec;
        public float initialDelaySec;
        public float scaleT;
        public float worldCalibrationK;
        public Vector3 lossyScale;
        public Vector3 quadSpan;
        public string sizeScalePath;
        public bool loopPreview;
        public int flipbookFrameA;
        public int flipbookFrameB;
        public float flipbookBlend;
    }

}
