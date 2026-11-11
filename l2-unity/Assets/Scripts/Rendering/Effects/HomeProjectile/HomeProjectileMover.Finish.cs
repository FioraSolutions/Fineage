using UnityEngine;

public sealed partial class HomeProjectileMover
{
    void HideAndDestroyFlightObject()
    {
        EffectPart[] parts = GetComponentsInChildren<EffectPart>(true);
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i] != null)
            {
                parts[i].StopPart();
            }
        }

        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                renderers[i].enabled = false;
            }
        }

        ClearOwnerWorldPosOverride();
        Destroy(gameObject);
    }

    void Finish()
    {
        _isLaunched = false;
        enabled = false;
        ClearOwnerWorldPosOverride();

        if (!_destroyOnArrive || _arrivalFadeStarted)
        {
            return;
        }

        if (_effect != null)
        {
            _effect.CompleteHomeArrivalAndDestroy(_fadeOutSeconds);
            return;
        }

        Destroy(gameObject);
    }
}
