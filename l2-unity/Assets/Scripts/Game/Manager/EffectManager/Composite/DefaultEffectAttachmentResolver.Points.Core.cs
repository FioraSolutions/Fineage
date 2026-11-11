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

        if (controller != null)
        {
            worldPosition = controller.transform.TransformPoint(controller.center);
            return true;
        }

        Transform searchRoot = entity != null ? entity.transform : fallbackRoot;
        Renderer[] renderers = searchRoot.GetComponentsInChildren<Renderer>(true);
        if (renderers != null && renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                {
                    bounds.Encapsulate(renderers[i].bounds);
                }
            }

            if (bounds.size.sqrMagnitude > 0.00001f)
            {
                worldPosition = bounds.center;
                return true;
            }
        }

        worldPosition = fallbackRoot.position;
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
