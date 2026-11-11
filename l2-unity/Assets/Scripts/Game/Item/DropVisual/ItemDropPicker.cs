using UnityEngine;

public sealed partial class ItemDropPicker
{
    public const float CastRadius = 0.02f;
    public const float KeepRadius = 0.06f;
    const float MaxDistance = 80f;
    static readonly Collider[] OverlapBuffer = new Collider[32];

    readonly ItemDropLayerService _layers;
    int _groundPickMask = -1;








}
