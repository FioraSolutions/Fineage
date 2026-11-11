using UnityEngine;

public static partial class L2FxQuadSizeDiagnostic
{
    private static float SampleSizeScale(
        float normalizedAge,
        float sizeScaleParam,
        float sizeScaleRepeats,
        int sizeScaleCount,
        float[] times,
        float[] values,
        bool useRegularSizeScale)
    {
        if (sizeScaleCount == 0)
        {
            return 1f;
        }

        float sp = Frac((sizeScaleParam + sizeScaleRepeats) * normalizedAge);

        int idx = 0;
        while (idx < sizeScaleCount && times[idx] < sp)
        {
            idx++;
        }

        float prevS;
        float prevT;
        float nextS;
        float nextT;

        if (idx == 0)
        {
            prevS = 1f;
            prevT = 0f;
            nextS = values[0];
            nextT = times[0];
        }
        else
        {
            prevS = values[idx - 1];
            prevT = times[idx - 1];
            nextS = idx < sizeScaleCount ? values[idx] : prevS;
            nextT = idx < sizeScaleCount ? times[idx] : prevT + 1e-4f;
        }

        if (Mathf.Abs(nextT - prevT) < 1e-4f)
        {
            return prevS;
        }

        float ts = (sp - prevT) / (nextT - prevT);
        return useRegularSizeScale ? Mathf.Lerp(prevS, nextS, ts) : Mathf.Lerp(prevS, nextS, ts);
    }

    private static float RandomRange(Vector2 minMax, float seed, float startTime, float salt)
    {
        float t = Hash11((seed * 17f) + (startTime * 31f) + salt);
        return Mathf.Lerp(minMax.x, minMax.y, t);
    }

    private static float Hash11(float n)
    {
        return Frac(Mathf.Sin(n) * 43758.5453123f);
    }

    private static float Frac(float x)
    {
        return x - Mathf.Floor(x);
    }
}
