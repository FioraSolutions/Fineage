using UnityEngine;

public static partial class CompositePartSpawnHelper
{
    public static void ApplyHomeFlightOverrides(CompositePrefabPart part, Transform instanceTransform)
    {
        if (part == null || instanceTransform == null ||
            part.homeProjectile == null || !part.homeProjectile.IsEnabled)
        {
            return;
        }

        ParticleGroup[] groups = instanceTransform.GetComponentsInChildren<ParticleGroup>(true);
        bool hasAnchor = false;
        int anchorCount = 0;
        for (int i = 0; i < groups.Length; i++)
        {
            if (groups[i] != null && groups[i].IsHomeFlightAnchor)
            {
                hasAnchor = true;
                anchorCount++;
            }
        }

        if (!hasAnchor && groups.Length > 0 && groups[0] != null)
        {
            groups[0].ApplyRuntimeHomeFlightProfile(ParticleGroupHomeFlightProfile.DefaultAnchor);
            hasAnchor = true;
        }

        if (!hasAnchor || !part.homeProjectile.mirrorDualFlight || anchorCount != 1)
        {
            return;
        }

        groups = instanceTransform.GetComponentsInChildren<ParticleGroup>(true);
        for (int i = 0; i < groups.Length; i++)
        {
            ParticleGroup source = groups[i];
            if (source == null || !source.IsHomeFlightAnchor || source.name.EndsWith("_Mirror"))
            {
                continue;
            }

            GameObject mirrorObject = Object.Instantiate(source.gameObject, source.transform.parent);
            mirrorObject.name = source.gameObject.name + "_Mirror";
            Transform mirrorTransform = mirrorObject.transform;
            mirrorTransform.SetPositionAndRotation(source.transform.position, source.transform.rotation);
            mirrorTransform.localScale = source.transform.localScale;

            ParticleGroup mirrorGroup = mirrorObject.GetComponent<ParticleGroup>();
            if (mirrorGroup != null)
            {
                mirrorGroup.ApplyRuntimeHomeFlightProfile(ParticleGroupHomeFlightProfile.MirroredAnchor);
            }

            break;
        }
    }

    public static void AttachIfFollow(
        bool follow,
        Transform resolvedTransform,
        Transform instanceTransform,
        Vector3 resolvedWorldPosition)
    {
        if (!follow || instanceTransform == null || resolvedTransform == null)
        {
            return;
        }

        instanceTransform.SetParent(resolvedTransform, true);
        Vector3 localAnchor = resolvedTransform.InverseTransformPoint(resolvedWorldPosition);
        instanceTransform.localPosition = localAnchor;
    }

    public static void AttachIfFollow(
        CompositePrefabPart part,
        Transform resolvedTransform,
        Transform instanceTransform,
        Vector3 adjustedOffset,
        Vector3 resolvedWorldPosition)
    {
        if (part == null || instanceTransform == null || !part.followResolvedTransform || resolvedTransform == null)
        {
            return;
        }

        instanceTransform.SetParent(resolvedTransform, true);
        Vector3 localAnchor = resolvedTransform.InverseTransformPoint(resolvedWorldPosition);
        instanceTransform.localPosition = localAnchor + adjustedOffset;
    }

    public static float ResolveAttachmentHeight(Transform resolvedTransform, Transform owner)
    {
        Transform basis = resolvedTransform != null ? resolvedTransform : owner;
        if (basis == null)
        {
            return 1.8f;
        }

        CharacterController controller = basis.GetComponentInParent<CharacterController>();
        if (controller != null && controller.height > 0f)
        {
            return controller.height;
        }

        Renderer[] renderers = basis.GetComponentsInParent<Renderer>(true);
        if (renderers != null && renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            if (bounds.size.y > 0.01f)
            {
                return bounds.size.y;
            }
        }

        return 1.8f;
    }
}
