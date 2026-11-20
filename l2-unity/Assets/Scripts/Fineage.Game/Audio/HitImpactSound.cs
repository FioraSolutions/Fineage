/// <summary>Armor / crit hit SFX via unified AudioService (bridge).</summary>
public static class HitImpactSound
{
    public static bool Play(bool criticalHit) =>
        InterludeSfxBridge.PlayHitImpact != null && InterludeSfxBridge.PlayHitImpact(criticalHit);
}