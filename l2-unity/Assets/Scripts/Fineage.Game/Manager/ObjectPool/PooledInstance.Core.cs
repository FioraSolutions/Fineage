using UnityEngine;

public sealed partial class PooledInstance : MonoBehaviour
{
    public void RecaptureSharedMaterials()
    {
        _sharedMaterials = null;
        CaptureSharedMaterials();
    }
}
