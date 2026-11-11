using System;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements.Experimental;
using System.Collections.Generic;
using System.Threading.Tasks;
using VContainer;

public partial class World
{
    void DestroyDisappearedUser(GameObject go)
    {
        if (go == null)
            return;

        SetEntityInteraction(go, true);
        if (_objectPool != null && _objectPool.ReturnToPool(ObjectType.User, go))
            return;

        Destroy(go);
    }

    void FinishDisappearedNpc(GameObject go)
    {
        if (go == null)
            return;

        SetEntityInteraction(go, true);
        if (_objectPool != null && _objectPool.ReturnToPool(ObjectType.Npc, go))
            return;

        Destroy(go);
    }

    static void SetEntityInteraction(GameObject go, bool enabled)
    {
        if (go == null)
            return;

        CharacterController controller = go.GetComponent<CharacterController>();
        if (controller != null)
            controller.enabled = enabled;

        Collider[] colliders = go.GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
                colliders[i].enabled = enabled;
        }
    }
}
