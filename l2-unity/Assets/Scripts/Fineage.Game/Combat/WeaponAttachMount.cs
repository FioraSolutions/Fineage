using UnityEngine;

public readonly struct WeaponAttachMount
{
    public WeaponAttachSocket Socket { get; }
    public string ModelKey { get; }
    public bool MirrorScaleX { get; }
    public bool DoubleSidedMaterials { get; }
    public bool SnapIdentityAtSocket { get; }

    public WeaponAttachMount(
        WeaponAttachSocket socket,
        string modelKey,
        bool mirrorScaleX = false,
        bool doubleSidedMaterials = false,
        bool snapIdentityAtSocket = true)
    {
        Socket = socket;
        ModelKey = modelKey;
        MirrorScaleX = mirrorScaleX;
        DoubleSidedMaterials = doubleSidedMaterials;
        SnapIdentityAtSocket = snapIdentityAtSocket;
    }
}