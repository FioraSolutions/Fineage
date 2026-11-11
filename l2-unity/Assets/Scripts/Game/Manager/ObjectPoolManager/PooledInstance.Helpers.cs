using UnityEngine;

public sealed partial class PooledInstance : MonoBehaviour
{
    public void CaptureSharedMaterials()
    {
        if (_sharedMaterials != null)
        {
            return;
        }

        _renderers = GetComponentsInChildren<Renderer>(true);
        _sharedMaterials = new Material[_renderers.Length][];
        for (int i = 0; i < _renderers.Length; i++)
        {
            Renderer r = _renderers[i];
            _sharedMaterials[i] = r != null ? r.sharedMaterials : null;
        }
    }

    public void RestoreSharedMaterials()
    {
        if (_renderers == null || _sharedMaterials == null)
        {
            return;
        }

        for (int i = 0; i < _renderers.Length; i++)
        {
            Renderer r = _renderers[i];
            if (r == null || _sharedMaterials[i] == null)
            {
                continue;
            }

            // Drop runtime material instances created by death-fade (.material / .materials).
            Material[] instances = r.materials;
            r.sharedMaterials = _sharedMaterials[i];
            for (int m = 0; m < instances.Length; m++)
            {
                Material inst = instances[m];
                if (inst == null)
                {
                    continue;
                }

                bool isShared = false;
                Material[] shared = _sharedMaterials[i];
                for (int s = 0; s < shared.Length; s++)
                {
                    if (inst == shared[s])
                    {
                        isShared = true;
                        break;
                    }
                }

                if (!isShared)
                {
                    Destroy(inst);
                }
            }
        }
    }
}
