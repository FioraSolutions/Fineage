using UnityEngine;
using UnityEngine.Rendering;

public sealed partial class L2ActorFade
{
    public byte AlphaByte(float elapsed)
    {
        return ClampAlphaByte(255f - elapsed * AlphaPerSecond);
    }

    public byte AppearAlphaByte(float elapsed)
    {
        return ClampAlphaByte(AppearStartAlpha + elapsed * AlphaPerSecond);
    }

    public bool TryBegin(Entity entity, out Renderer[] renderers, out Material[][] instances)
    {
        return TryBegin(entity, 255, false, out renderers, out instances, out _);
    }

    public bool TryBegin(
        Entity entity,
        byte startAlphaByte,
        out Renderer[] renderers,
        out Material[][] instances,
        out Material[][] sharedBackup)
    {
        return TryBegin(entity, startAlphaByte, true, out renderers, out instances, out sharedBackup);
    }

    public void SetAlphaByte(Material[][] instances, byte alphaByte)
    {
        SetAlpha(instances, alphaByte / 255f);
    }
}
