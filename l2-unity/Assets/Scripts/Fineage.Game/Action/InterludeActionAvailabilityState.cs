/// <summary>Owner summon + mount snapshot for action availability (from PetInfo / UserInfo / CharInfo).</summary>
public struct InterludeActionAvailabilityState
{
  public const byte SummonKindPet = 2;
  public const byte SummonKindServitor = 1;

  public bool HasPet;
  public bool HasServitor;
  public int ActivePetNpcId;
  public int ActiveServitorNpcId;
  /// <summary>Server: mountable pet out (PlayerActionState / SummonInfo.mountable).</summary>
  public bool MountablePet;
  /// <summary>Server: Ride action 38 allowed (PlayerActionState.canRide).</summary>
  public bool CanRide;
  /// <summary>Mobius / Interlude mount type byte: 0 none, 1 ground mount, 2 wyvern (fly).</summary>
  public byte MountType;
  public int MountNpcId;
  public bool ClientMountedVisual;

  public bool PlayerMounted => MountType != 0 || MountNpcId != 0 || ClientMountedVisual;

  public bool RideActionAvailable => CanRide || PlayerMounted;

  public bool WyvernMounted => MountType == 2;

  public static InterludeActionAvailabilityState Empty => default;

  public void NoteSummon(byte summonKind, int npcId, bool dead, bool mountable = false)
  {
    if (summonKind == SummonKindPet)
    {
      HasPet = !dead;
      ActivePetNpcId = npcId;
      if (!dead)
        MountablePet = mountable;
      return;
    }

    if (summonKind == SummonKindServitor)
    {
      HasServitor = !dead;
      ActiveServitorNpcId = npcId;
    }
  }

  public void NoteSummonGone(byte summonKind)
  {
    if (summonKind == SummonKindPet)
    {
      HasPet = false;
      ActivePetNpcId = 0;
      MountablePet = false;
      CanRide = false;
      return;
    }

    if (summonKind == SummonKindServitor)
    {
      HasServitor = false;
      ActiveServitorNpcId = 0;
    }
  }

  public void ApplyMount(byte mountType, int mountNpcId)
  {
    MountType = mountType;
    MountNpcId = mountNpcId;
  }

  public void ApplyPlayerActionState(
      bool petOut,
      bool mountablePet,
      bool mounted,
      bool canRide,
      byte mountKind,
      int petControlObjectId)
  {
    HasPet = petOut;
    if (!petOut)
    {
      ActivePetNpcId = 0;
      MountablePet = false;
    }
    else
      MountablePet = mountablePet;

    CanRide = canRide;

    if (mounted)
      ApplyMount(mountKind, petControlObjectId);
    else
      ApplyMount(0, 0);

    ClientMountedVisual = mounted;
  }
}