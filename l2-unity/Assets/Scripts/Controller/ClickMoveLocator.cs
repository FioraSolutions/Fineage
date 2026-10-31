using UnityEngine;

/// <summary>
/// Hosts LineageEffect.e_u093_a at the click point.
/// Only tune MarkScale / GroundLift — do not edit emitter mats by hand.
/// </summary>
[DisallowMultipleComponent]
public sealed class ClickMoveLocator : MonoBehaviour
{
    /// <summary>Root uniform scale. 1 = generated UC sizes.</summary>
    public const float MarkScale = 1f;

    /// <summary>
    /// Keep 0 so base1 (ground ring) sits on the hit. Icon height comes from
    /// emitter StartLocationOffset Z (~13 UU) on Lock/Ring/Core.
    /// </summary>
    public const float GroundLift = 0f;

    BaseEffect _effect;
    bool _started;

    public void PlaceAt(Vector3 worldPosition)
    {
        transform.position = worldPosition + Vector3.up * GroundLift;
        transform.localScale = Vector3.one * MarkScale;
        gameObject.SetActive(true);

        if (_effect == null)
            _effect = GetComponentInChildren<BaseEffect>(true);

        if (_effect == null)
            return;

        _effect.BindLifetimeToHost();
        _effect.Play();
        _started = true;
    }
}
