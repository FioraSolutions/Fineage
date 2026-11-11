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
}
