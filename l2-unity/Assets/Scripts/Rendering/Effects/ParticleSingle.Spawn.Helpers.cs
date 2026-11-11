using UnityEngine;
using UnityEngine.Rendering;

public partial class ParticleSingle
{
    private Renderer ResolveRenderer()
    {
        if (_particles == null || _particles.Length == 0)
        {
            return null;
        }

        return _particles[0];
    }

    private void SetActive(bool value, string reason = null)
    {
        Renderer renderer = ResolveRenderer();
        if (renderer == null)
        {
            // ParticleSingleLifetimeDebug.LogSetActive(BuildDebugSnapshot(Now()), value, reason, null, false);
            return;
        }

        bool wasActive = renderer.gameObject.activeSelf;
        renderer.gameObject.SetActive(value);
        // ParticleSingleLifetimeDebug.LogSetActive(BuildDebugSnapshot(Now()), value, reason, renderer, wasActive);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (!value)
        {
            // DropAdenaDestroyLog.Event(
                // "SET_ACTIVE_FALSE",
                // this,
                // $"reason={reason} slot='{renderer.name}' wasActive={wasActive}",
                // includeStack: true);
        }
#endif
    }
}
