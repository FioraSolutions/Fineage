using System.Linq;
using UnityEngine;
using VContainer;

public partial class DefaultEffectAttachmentResolver
{
    private bool ResolveTargetCenter(Entity entity, Transform fallbackRoot, out Transform resolvedTransform, out Vector3 worldPosition)
    {
        resolvedTransform = null;
        worldPosition = Vector3.zero;

        if (fallbackRoot == null)
        {
            return false;
        }

        resolvedTransform = fallbackRoot;

        CharacterController controller = ResolveCharacterController(entity, fallbackRoot);
        bool hasMeshCenter = TryResolveRendererBoundsCenter(entity, fallbackRoot, out Vector3 meshCenter, out Bounds meshBounds);

        // Player capsules are often stubby (h≈0.85, center≈0.5) while the mesh is ~full height.
        // Trusting CC alone puts magician orbs at the hips/feet ("шар снизу").
        if (hasMeshCenter && meshBounds.size.y > 0.01f)
        {
            if (controller != null)
            {
                Vector3 ccWorld = controller.transform.TransformPoint(controller.center);
                float frac = (ccWorld.y - meshBounds.min.y) / meshBounds.size.y;
                if (frac < 0.45f)
                {
                    worldPosition = meshCenter;
                    return true;
                }

                worldPosition = ccWorld;
                return true;
            }

            worldPosition = meshCenter;
            return true;
        }

        if (controller != null)
        {
            worldPosition = controller.transform.TransformPoint(controller.center);
            return true;
        }

        worldPosition = fallbackRoot.position;
        return true;
    }

    private static bool TryResolveRendererBoundsCenter(
        Entity entity,
        Transform fallbackRoot,
        out Vector3 center,
        out Bounds bounds)
    {
        center = Vector3.zero;
        bounds = default;

        Transform searchRoot = entity != null ? entity.transform : fallbackRoot;
        if (searchRoot == null)
        {
            return false;
        }

        Renderer[] renderers = searchRoot.GetComponentsInChildren<Renderer>(true);
        if (renderers == null || renderers.Length == 0)
        {
            return false;
        }

        bool has = false;
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (renderer == null || !renderer.enabled)
            {
                continue;
            }

            // Skip pure UI/FX renderers that sit at the root origin.
            if (renderer is ParticleSystemRenderer)
            {
                continue;
            }

            if (!has)
            {
                bounds = renderer.bounds;
                has = true;
            }
            else
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }

        if (!has || bounds.size.sqrMagnitude <= 0.00001f)
        {
            return false;
        }

        center = bounds.center;
        return true;
    }

    private bool ResolveTargetOverHead(Entity entity, Transform fallbackRoot, out Transform resolvedTransform, out Vector3 worldPosition)
    {
        resolvedTransform = null;
        worldPosition = Vector3.zero;

        if (fallbackRoot == null)
        {
            return false;
        }

        resolvedTransform = fallbackRoot;

        worldPosition = L2OverheadAnchor.GetWorldPos(entity, fallbackRoot);
        return true;
    }

    private CharacterController ResolveCharacterController(Entity entity, Transform fallbackRoot)
    {
        CharacterController controller = null;
        if (entity != null)
        {
            controller = entity.GetComponent<CharacterController>();
            if (controller == null)
            {
                controller = entity.GetComponentInChildren<CharacterController>(true);
            }
        }

        if (controller == null && fallbackRoot != null)
        {
            controller = fallbackRoot.GetComponent<CharacterController>();
            if (controller == null)
            {
                controller = fallbackRoot.GetComponentInChildren<CharacterController>(true);
            }
        }

        return controller;
    }
}
