using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;
using Object = UnityEngine.Object;

public sealed partial class StriderRideService
{
    public bool IsRiding(Entity rider)
    {
        return rider != null && rider.Identity != null && _seats.ContainsKey(rider.Identity.Id);
    }

    public void LateTick()
    {
        if (_seats.Count == 0)
            return;

        _gone.Clear();
        foreach (KeyValuePair<int, Seat> pair in _seats)
        {
            Seat seat = pair.Value;
            if (seat.Rider == null || seat.Mount == null)
            {
                _gone.Add(pair.Key);
                continue;
            }
            Place(seat);
        }

        for (int i = 0; i < _gone.Count; i++)
            Dismount(_gone[i]);
    }

    public bool TryMount(Entity rider)
    {
        if (rider == null || rider.Identity == null)
            return false;
        int id = rider.Identity.Id;
        if (_seats.ContainsKey(id))
            return true;

        GameObject prefab = LoadStrider();
        if (prefab == null)
            return false;

        GameObject mount = Object.Instantiate(prefab);
        mount.name = "strider_ride";
        DisableGameplay(mount);
        Transform bone = FindChild(mount.transform, SeatBone);
        if (bone == null)
        {
            Object.Destroy(mount);
            return false;
        }

        Seat seat = new Seat
        {
            Rider = rider,
            Mount = mount,
            Bone = bone,
            Animator = mount.GetComponentInChildren<Animator>()
        };
        if (seat.Animator != null)
        {
            seat.Animator.Play("wait", 0, 0f);
            seat.Animator.Update(0f);
        }
        Place(seat);
        AttachChildren(seat);
        _seats.Add(id, seat);
        HideOwnedStriders(seat);
        return true;
    }
}
