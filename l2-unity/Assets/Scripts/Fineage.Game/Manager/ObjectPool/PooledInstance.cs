using UnityEngine;

public sealed partial class PooledInstance : MonoBehaviour
{
    public ObjectType Type;
    public GameObject SourcePrefab;
    public Vector3 OriginalLocalScale = Vector3.one;

    private Renderer[] _renderers;
    private Material[][] _sharedMaterials;



}
