using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;
using Object = UnityEngine.Object;

/// <summary>
/// RiderEnter spawns a new LineageMonster.strider and AttachToBone(player, Bone15).
/// The summoned pet stays in the world on the server; here it is hidden while the
/// player sits, then shown again. RiderEnd destroys only the mount the player sat on.
/// </summary>
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

    public StriderRideService()
    {
        Instance = this;
        AnimationPacketServices.Bind(striderRide: this);
    }






    void Dismount(int id)
    {
        Seat seat;
        if (!_seats.TryGetValue(id, out seat))
            return;
        _seats.Remove(id);
        SnapBodyToGroundPose(seat);
        RestoreChildren(seat);
        for (int i = 0; i < seat.HiddenPets.Count; i++)
        {
            GameObject hidden = seat.HiddenPets[i];
            if (hidden == null)
                continue;
            hidden.SetActive(true);
            StandPet(hidden.GetComponent<Entity>());
        }
        if (seat.Mount != null)
        {
            seat.Mount.SetActive(false);
            Object.Destroy(seat.Mount);
        }
    }











}