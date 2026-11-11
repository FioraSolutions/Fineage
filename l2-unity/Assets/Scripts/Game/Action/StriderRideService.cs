using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;
using Object = UnityEngine.Object;
using VContainer;

public sealed partial class StriderRideService : ILateTickable
{
    const string PrefabFolder = "Data/Animations/LineageMonsters/strider_m00";
    const string SeatBone = "Bone15";

    [System.Obsolete("Use [Inject] or AnimationPacketServices.StriderRide instead")]
    public static StriderRideService Instance { get; private set; }

    struct SavedChild
    {
        public Transform Child;
        public Transform Parent;
        public int Sibling;
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;
        public Vector3 LocalScale;
    }

    sealed class Seat
    {
        public Entity Rider;
        public GameObject Mount;
        public Transform Bone;
        public Animator Animator;
        public bool Running;
        public readonly List<SavedChild> Children = new List<SavedChild>();
        public readonly List<GameObject> HiddenPets = new List<GameObject>();
    }

    readonly Dictionary<int, Seat> _seats = new Dictionary<int, Seat>();
    readonly List<int> _gone = new List<int>();


















}
