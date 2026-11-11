using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;
using Object = UnityEngine.Object;

public sealed partial class StriderRideService
{
    static void RestoreChildren(Seat seat)
    {
        for (int i = seat.Children.Count - 1; i >= 0; i--)
        {
            SavedChild saved = seat.Children[i];
            if (saved.Child == null)
                continue;
            saved.Child.SetParent(saved.Parent, false);
            saved.Child.SetSiblingIndex(saved.Sibling);
            saved.Child.localPosition = saved.LocalPosition;
            saved.Child.localRotation = saved.LocalRotation;
            saved.Child.localScale = saved.LocalScale;
        }
    }

    static float DivideScale(float world, float parent)
    {
        if (Mathf.Abs(parent) < 0.0001f)
            return world;
        return world / parent;
    }

    static GameObject LoadStrider()
    {
        GameObject[] loaded = Resources.LoadAll<GameObject>(PrefabFolder);
        GameObject best = null;
        for (int i = 0; i < loaded.Length; i++)
        {
            GameObject go = loaded[i];
            if (go == null || go.name != "strider_m00")
                continue;
            if (go.GetComponent<Animator>() != null)
                return go;
            best = go;
        }
        return best;
    }

    static void DisableGameplay(GameObject mount)
    {
        Behaviour[] behaviours = mount.GetComponentsInChildren<Behaviour>(true);
        for (int i = 0; i < behaviours.Length; i++)
        {
            if (behaviours[i] is Animator)
                continue;
            behaviours[i].enabled = false;
        }

        Collider[] colliders = mount.GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = false;
    }

    static Transform FindChild(Transform root, string bone)
    {
        if (root.name == bone)
            return root;
        for (int i = 0; i < root.childCount; i++)
        {
            Transform found = FindChild(root.GetChild(i), bone);
            if (found != null)
                return found;
        }
        return null;
    }
}
