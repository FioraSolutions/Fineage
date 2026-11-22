using UnityEngine;

/// <summary>
/// Click masks <c>World</c> injects (#131 S38c, S38o).
/// <c>GameLifetimeScope</c> registers the scene <c>ClickManager</c> as this interface.
/// </summary>
public interface IClickMasks
{
    void SetMasks(LayerMask entityMask, LayerMask clickThroughMask);
}
