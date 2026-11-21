using Unity.Profiling;

/// <summary>Profiler markers for the zone receive → decode → queue → apply path (audit step 1 / 9).</summary>
public static class NetReceiveProfiling
{
    public const string ReadName = "Fineage.Net.Read";
    public const string DecodeName = "Fineage.Net.Decode";
    public const string QueueName = "Fineage.Net.Queue";
    public const string DrainName = "Fineage.Net.Drain";
    public const string ApplyName = "Fineage.Net.Apply";

    public static readonly ProfilerMarker Read = new ProfilerMarker(ReadName);
    public static readonly ProfilerMarker Decode = new ProfilerMarker(DecodeName);
    public static readonly ProfilerMarker Queue = new ProfilerMarker(QueueName);
    public static readonly ProfilerMarker Drain = new ProfilerMarker(DrainName);
    public static readonly ProfilerMarker Apply = new ProfilerMarker(ApplyName);
}