using UnityEngine;

/// <summary>
/// Click masks <c>World.UpdateMasks</c> pushes (#131 S38c).
/// </summary>
public interface IClickMasks
{
    void SetMasks(LayerMask entityMask, LayerMask clickThroughMask);
}
