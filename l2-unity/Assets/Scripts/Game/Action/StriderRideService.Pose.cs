using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;
using Object = UnityEngine.Object;

public sealed partial class StriderRideService
{
    static void Place(Seat seat)
    {
        Transform rider = seat.Rider.transform;
        Quaternion yaw = Quaternion.Euler(0f, rider.eulerAngles.y, 0f);
        seat.Mount.transform.SetPositionAndRotation(rider.position, yaw);
    }

    static void AttachChildren(Seat seat)
    {
        Transform rider = seat.Rider.transform;
        Quaternion yaw = Quaternion.Euler(0f, rider.eulerAngles.y, 0f);
        for (int i = rider.childCount - 1; i >= 0; i--)
        {
            Transform child = rider.GetChild(i);
            if (child == seat.Mount.transform)
                continue;
            Vector3 worldScale = child.lossyScale;
            seat.Children.Add(new SavedChild
            {
                Child = child,
                Parent = rider,
                Sibling = child.GetSiblingIndex(),
                LocalPosition = child.localPosition,
                LocalRotation = child.localRotation,
                LocalScale = child.localScale
            });
            child.SetParent(seat.Bone, false);
            child.localPosition = Vector3.zero;
            child.localRotation = Quaternion.Inverse(seat.Bone.rotation) * yaw;
            Vector3 seatScale = seat.Bone.lossyScale;
            child.localScale = new Vector3(
                DivideScale(worldScale.x, seatScale.x),
                DivideScale(worldScale.y, seatScale.y),
                DivideScale(worldScale.z, seatScale.z));
        }
    }

    static void SnapBodyToGroundPose(Seat seat)
    {
        // RiderEnd clears the ride flag and plays GetCurWaitAnimName on the body.
        // The sit clip keeps the hips on Bone15. The player root is the feet, so the
        // standing wait is sampled before the body is parented back.
        Animator body = FindBodyAnimator(seat);
        if (body == null)
            return;

        string state = "wait_1HS";
        AnimationManager animations = GamePacketServices.Animations as AnimationManager;
        if (animations != null && seat.Rider != null && seat.Rider.Identity != null)
        {
            string final = animations.GetFinalNameAnim(seat.Rider.Identity.Id, AnimationNames.WAIT.Value);
            if (PlayerLocomotionAnim.TryResolve(final, out string resolved, out _))
                state = resolved;
        }

        body.Play(state, 0, 0f);
        body.Update(0f);
    }

    static Animator FindBodyAnimator(Seat seat)
    {
        for (int i = 0; i < seat.Children.Count; i++)
        {
            Transform child = seat.Children[i].Child;
            if (child == null)
                continue;
            Animator body = child.GetComponentInChildren<Animator>();
            if (body != null)
                return body;
        }

        return seat.Rider != null ? seat.Rider.GetComponentInChildren<Animator>() : null;
    }

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
