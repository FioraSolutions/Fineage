/// <summary>Crowd LOD tier (roadmap §7.2 step 13). Higher number = cheaper simulation/draw path.</summary>
public enum CrowdDetailTier : byte
{
    T0Hero = 0,
    T1Near = 1,
    T2Mid = 2,
    T3Far = 3,
    /// <summary>Beyond client pawn draw distance or over max-characters-on-screen budget.</summary>
    TCulled = 4,
}