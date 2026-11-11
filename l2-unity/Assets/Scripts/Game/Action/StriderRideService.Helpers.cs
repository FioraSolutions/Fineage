using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;
using Object = UnityEngine.Object;

public sealed partial class StriderRideService : ILateTickable
{
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
