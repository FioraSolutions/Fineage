using UnityEngine;

public partial class L2RibbonEmitter
{
    private void EnsureMesh()
    {
        if (_meshFilter == null)
        {
            _meshFilter = GetComponent<MeshFilter>();
        }

        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }

        if (_mesh == null)
        {
            _mesh = new Mesh { name = "L2RibbonStrip" };
            _mesh.MarkDynamic();
        }

        if (_meshFilter != null && _meshFilter.sharedMesh != _mesh)
        {
            _meshFilter.sharedMesh = _mesh;
        }
    }

    private Transform ResolveSearchRoot()
    {
        if (OwnerTarget != null)
        {
            return OwnerTarget;
        }

        if (FollowTarget != null)
        {
            return FollowTarget;
        }

        return null;
    }

    static Entity ResolveOwnerEntity(Transform searchRoot)
    {
        if (searchRoot == null)
        {
            return null;
        }

        return searchRoot.GetComponentInParent<Entity>();
    }
}
