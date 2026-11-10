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

    public void Dismount(Entity rider)
    {
        if (rider == null || rider.Identity == null)
            return;
        Dismount(rider.Identity.Id);
    }

    public void SetMountGait(int objectId, bool running)
    {
        Seat seat;
        if (!_seats.TryGetValue(objectId, out seat) || seat.Animator == null || seat.Running == running)
            return;
        seat.Running = running;
        seat.Animator.CrossFade(running ? "run" : "wait", 0.12f, 0);
    }

    static void HideOwnedStriders(Seat seat)
    {
        Entity[] entities = Object.FindObjectsByType<Entity>(FindObjectsSortMode.None);
        for (int i = 0; i < entities.Length; i++)
        {
            Entity entity = entities[i];
            if (entity == null || entity.Identity == null || entity == seat.Rider)
                continue;
            if (seat.Mount != null && entity.gameObject == seat.Mount)
                continue;
            if (!entity.Identity.Owned || !entity.gameObject.activeInHierarchy)
                continue;
            int npcId = entity.Identity.NpcId;
            if (npcId != 12526 && npcId != 12527 && npcId != 12528)
                continue;
            if (MoveAllCharactersPacketServices.Moves != null)
                MoveAllCharactersPacketServices.Moves.Forget(entity.Identity.Id);
            entity.gameObject.SetActive(false);
            seat.HiddenPets.Add(entity.gameObject);
        }
    }

    static void StandPet(Entity pet)
    {
        if (pet == null)
            return;
        if (pet.Identity != null && MoveAllCharactersPacketServices.Moves != null)
            MoveAllCharactersPacketServices.Moves.Forget(pet.Identity.Id);
        if (EntityActionPacketServices.Actions != null)
            EntityActionPacketServices.Actions.Set(pet, EntityActionKind.Idle, null);
        Animator animator = pet.GetComponentInChildren<Animator>(true);
        if (animator == null)
            return;
        animator.Play("wait", 0, 0f);
        animator.Update(0f);
    }

}
