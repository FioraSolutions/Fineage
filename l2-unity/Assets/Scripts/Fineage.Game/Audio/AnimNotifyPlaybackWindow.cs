/// <summary>
/// Decides which slice of a normalized anim sequence may fire notifies this step.
/// A jump larger than the elapsed time at the authored rate is a discontinuity:
/// the cursor snaps and the skipped range is not replayed.
/// A span wider than one authored frame keeps only the latest sound notify.
/// </summary>
public static class AnimNotifyPlaybackWindow
{
    public struct Result
    {
        public float Cursor;
        public bool Emit;
        public bool Wrapped;
        public float FromExclusive;
        public float ToInclusive;
        public float Span;
        public float OneFrame;
        public string Error;
    }

    public static Result Step(
        bool hasLast,
        float lastNormalized,
        float cycle,
        float deltaSeconds,
        float rate,
        int frameCount)
    {
        Result result = new Result
        {
            Cursor = cycle,
            OneFrame = frameCount > 0 ? 1f / frameCount : float.NaN
        };

        if (rate <= 0f || frameCount <= 0)
        {
            result.Error = "AnimNotifyPlayback: sequence is missing rate or frameCount, notify catch-up skipped rate="
                + rate.ToString(System.Globalization.CultureInfo.InvariantCulture)
                + " frames=" + frameCount;
            return result;
        }

        if (deltaSeconds < 0f)
            deltaSeconds = 0f;

        if (!hasLast)
        {
            // The first sample is rarely exactly 0. A start-of-clip notify (the death
            // cry sits a fraction of a frame in) still belongs to this observation when
            // it lands inside the first authored frame. Joining mid-cycle does not
            // replay notifies that already passed.
            if (cycle <= result.OneFrame + 0.0001f)
            {
                result.Emit = true;
                result.FromExclusive = -0.0001f;
                result.ToInclusive = cycle;
                result.Span = cycle + 0.0001f;
            }
            return result;
        }

        bool wrapped = cycle + 0.0001f < lastNormalized;
        float span = wrapped ? (1f - lastNormalized) + cycle : cycle - lastNormalized;
        if (span < 0f)
            span = 0f;
        float budget = deltaSeconds * rate / frameCount + result.OneFrame;
        result.Span = span;
        result.Wrapped = wrapped;
        result.FromExclusive = lastNormalized;
        result.ToInclusive = cycle;
        result.Emit = span <= budget;
        return result;
    }

    public static bool ShouldPlayOnlyLatest(float span, float oneFrame) =>
        oneFrame > 0f && span > oneFrame;
}
