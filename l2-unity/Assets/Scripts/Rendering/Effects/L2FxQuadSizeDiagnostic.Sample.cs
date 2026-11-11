using UnityEngine;

public static partial class L2FxQuadSizeDiagnostic
{
    private static float SampleSpriteSizeScaleFromUniforms(Material mat, float normalizedAge, out float scaleT)
    {
        const float sizeScaleParam = 1f;
        float repeats = mat.HasProperty("_SizeScaleRepeats") ? mat.GetFloat("_SizeScaleRepeats") : 0f;
        const int sizeScaleCount = 5;
        const bool implicitKeyZero = false;
        const bool useRegularSizeScale = false;

        scaleT = Frac((sizeScaleParam + repeats) * normalizedAge);

        BuildKeysFromUniforms(
            sizeScaleCount,
            implicitKeyZero,
            ReadKey(mat, 0),
            ReadKey(mat, 1),
            ReadKey(mat, 2),
            ReadKey(mat, 3),
            ReadKey(mat, 4),
            out float[] times,
            out float[] values);

        return SampleSizeScale(
            normalizedAge,
            sizeScaleParam,
            repeats,
            sizeScaleCount,
            times,
            values,
            useRegularSizeScale);
    }

    private static Vector2 ReadKey(Material mat, int index)
    {
        string prop = "_SizeKey" + index;
        if (!mat.HasProperty(prop))
        {
            return Vector2.zero;
        }

        Vector4 key = mat.GetVector(prop);
        return new Vector2(key.x, key.y);
    }

    private static float EvaluateDynamicSizeScaleSe0(Material mat, float progress, out float scaleT)
    {
        float repeats = mat.HasProperty("_SizeScaleRepeats") ? mat.GetFloat("_SizeScaleRepeats") : 0f;
        scaleT = Frac(progress * repeats);

        Vector2[] keys = new Vector2[5];
        for (int i = 0; i < 5; i++)
        {
            keys[i] = ReadKey(mat, i);
        }

        if (keys[0].x > 0f && scaleT < keys[0].x)
        {
            return Mathf.Lerp(1f, keys[0].y, scaleT / Mathf.Max(keys[0].x, 1e-6f));
        }

        int idx = 0;
        while (idx < 4 && scaleT > keys[idx + 1].x)
        {
            idx++;
        }

        float t0 = keys[idx].x;
        float s0 = keys[idx].y;
        float t1 = keys[idx + 1].x;
        float s1 = keys[idx + 1].y;

        if (Mathf.Abs(t1 - t0) < 1e-6f)
        {
            return s0;
        }

        float u = (scaleT - t0) / (t1 - t0);
        return Mathf.Lerp(s0, s1, Mathf.Clamp01(u));
    }

    private static void BuildKeysFromUniforms(
        int sizeScaleCount,
        bool implicitKeyZero,
        Vector2 key0,
        Vector2 key1,
        Vector2 key2,
        Vector2 key3,
        Vector2 key4,
        out float[] times,
        out float[] values)
    {
        times = new float[8];
        values = new float[8];
        for (int i = 0; i < 8; i++)
        {
            times[i] = 999f;
            values[i] = 1f;
        }

        Vector2[] keys = { key0, key1, key2, key3, key4 };
        if (implicitKeyZero)
        {
            times[0] = 0f;
            values[0] = 0f;
            for (int i = 1; i < sizeScaleCount && i < keys.Length; i++)
            {
                times[i] = keys[i].x;
                values[i] = keys[i].y;
            }
        }
        else
        {
            for (int i = 0; i < sizeScaleCount && i < keys.Length; i++)
            {
                times[i] = keys[i].x;
                values[i] = keys[i].y;
            }
        }
    }

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
