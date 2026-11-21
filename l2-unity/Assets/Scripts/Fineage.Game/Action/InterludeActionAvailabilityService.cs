using System;
using UnityEngine;

/// <summary>Shared ActionWnd / shortcut availability from catalog requirements + live pet/mount state.</summary>
public sealed class InterludeActionAvailabilityService
{
  static InterludeActionAvailabilityService _instance;
  public static InterludeActionAvailabilityService Instance =>
      _instance ??= new InterludeActionAvailabilityService();

  public event Action Changed;

  InterludeActionAvailabilityState _state = InterludeActionAvailabilityState.Empty;

  public ref readonly InterludeActionAvailabilityState State => ref _state;

  public void Reset()
  {
    _state = InterludeActionAvailabilityState.Empty;
    Changed?.Invoke();
  }

  public void NoteSummon(byte summonKind, int npcId, bool dead, bool mountable = false)
  {
    _state.NoteSummon(summonKind, npcId, dead, mountable);
    Changed?.Invoke();
  }

  public void NoteSummonDismissed(byte summonKind)
  {
    _state.NoteSummonGone(summonKind);
    Changed?.Invoke();
  }

  public void NoteMount(byte mountType, int mountNpcId)
  {
    _state.ApplyMount(mountType, mountNpcId);
    Changed?.Invoke();
  }

  public void NoteClientMountedVisual(bool mounted)
  {
    if (_state.ClientMountedVisual == mounted)
      return;
    _state.ClientMountedVisual = mounted;
    Changed?.Invoke();
  }

  public void ApplyPlayerActionState(
      bool petOut,
      bool mountablePet,
      bool mounted,
      bool canRide,
      byte mountKind,
      int petControlObjectId)
  {
    _state.ApplyPlayerActionState(petOut, mountablePet, mounted, canRide, mountKind, petControlObjectId);
    Changed?.Invoke();
  }

  public bool IsAvailable(int actionId)
  {
    if (!InterludeActionCatalog.TryGet(actionId, out var entry))
    {
      Debug.LogError("[ActionAvailability] No catalog entry for action id=" + actionId + ".");
      return false;
    }

    return IsAvailable(in entry);
  }

  public bool IsAvailable(in InterludeActionCatalog.Entry entry)
  {
    switch (entry.Availability)
    {
      case InterludeActionAvailabilityKind.None:
        return true;
      case InterludeActionAvailabilityKind.PetSummoned:
        return _state.HasPet;
      case InterludeActionAvailabilityKind.ServitorSummoned:
        return _state.HasServitor;
      case InterludeActionAvailabilityKind.MountablePetSummonedOrMounted:
        return _state.RideActionAvailable;
      case InterludeActionAvailabilityKind.SummonTemplateActive:
        return TemplateMatchesActiveSummon(entry.TemplateNpcIds);
      case InterludeActionAvailabilityKind.Unknown:
        Debug.LogError(
            "[ActionAvailability] Unknown requirement on action id=" + entry.Id + " cmd='" + entry.CmdName + "'.");
        return false;
      default:
        Debug.LogError(
            "[ActionAvailability] Unhandled requirement kind=" + entry.Availability + " for action id=" + entry.Id + ".");
        return false;
    }
  }

  bool TemplateMatchesActiveSummon(int[] templates)
  {
    if (templates == null || templates.Length == 0)
    {
      Debug.LogError("[ActionAvailability] Template action with empty npc list.");
      return false;
    }

    if (_state.HasPet)
    {
      for (int i = 0; i < templates.Length; i++)
      {
        if (templates[i] == _state.ActivePetNpcId)
          return true;
      }
    }

    if (_state.HasServitor)
    {
      for (int i = 0; i < templates.Length; i++)
      {
        if (templates[i] == _state.ActiveServitorNpcId)
          return true;
      }
    }

    return false;
  }

  public static void Bind(InterludeActionAvailabilityService service)
  {
    if (service != null)
      _instance = service;
  }
}