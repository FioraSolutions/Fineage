/// <summary>
/// Pure helper: drop work captured under an older zone epoch (post-disconnect stale applies).
/// </summary>
public static class ZoneEpochGate
{
    public static bool IsCurrent(int capturedEpoch, int liveEpoch) =>
        capturedEpoch == liveEpoch;
}
