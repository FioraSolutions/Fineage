using Unity.Profiling;

/// <summary>Profiler markers for the zone receive → decode → queue → apply path (audit step 1 / 9).</summary>
public static class NetReceiveProfiling
{
    public static readonly ProfilerMarker Read = new ProfilerMarker("Fineage.Net.Read");
    public static readonly ProfilerMarker Decode = new ProfilerMarker("Fineage.Net.Decode");
    public static readonly ProfilerMarker Queue = new ProfilerMarker("Fineage.Net.Queue");
    public static readonly ProfilerMarker Drain = new ProfilerMarker("Fineage.Net.Drain");
    public static readonly ProfilerMarker Apply = new ProfilerMarker("Fineage.Net.Apply");
}