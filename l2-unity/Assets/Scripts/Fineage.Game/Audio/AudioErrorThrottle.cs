using System.Collections.Generic;

/// <summary>
/// One stack trace per key. Repeats inside the window are counted and emitted once when the window ends.
/// Keys marked once are never repeated.
/// </summary>
public static class AudioErrorThrottle
{
    public const float AggregateWindowSeconds = 1f;

    sealed class Bucket
    {
        public float LastEmit;
        public int Suppressed;
        public bool Once;
    }

    static readonly Dictionary<string, Bucket> Buckets = new Dictionary<string, Bucket>();

    public static void ClearForTests() => Buckets.Clear();

    public static bool TryEmit(string key, float nowSeconds, bool once, out string repeatSuffix)
    {
        repeatSuffix = null;
        if (string.IsNullOrEmpty(key))
            key = "";

        Bucket bucket;
        if (!Buckets.TryGetValue(key, out bucket))
        {
            Buckets[key] = new Bucket { LastEmit = nowSeconds, Suppressed = 0, Once = once };
            return true;
        }

        if (bucket.Once || once)
        {
            bucket.Once = true;
            return false;
        }

        if (nowSeconds - bucket.LastEmit < AggregateWindowSeconds)
        {
            bucket.Suppressed++;
            return false;
        }

        int suppressed = bucket.Suppressed;
        bucket.Suppressed = 0;
        bucket.LastEmit = nowSeconds;
        if (suppressed > 0)
            repeatSuffix = " (repeated " + suppressed + " times)";
        return true;
    }
}
