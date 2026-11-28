using System.Collections.Generic;

/// <summary>
/// Central Update-step for active pawn yaw drives (roadmap step 10b).
/// </summary>
public interface IPawnYawDriveStep
{
  bool Active { get; }
  void StepFromMoveAllCharacters(float deltaTime);
}

public static class PawnYawDriveRegistry
{
  static readonly List<IPawnYawDriveStep> Active = new List<IPawnYawDriveStep>(128);

  public static void Register(IPawnYawDriveStep drive)
  {
    if (drive == null || Active.Contains(drive))
      return;
    Active.Add(drive);
  }

  public static void Unregister(IPawnYawDriveStep drive)
  {
    if (drive == null)
      return;
    Active.Remove(drive);
  }

  public static void StepAll(float deltaTime)
  {
    if (!FineageMovementCentralization.RoutePawnYawLateUpdateThroughMoveAllCharacters)
      return;

    using (FineagePerfProfiling.MovePawnYawStep.Auto())
    {
      for (int i = Active.Count - 1; i >= 0; i--)
      {
        // A drive stepped earlier in this loop may unregister itself or others (despawn, state
        // change), shrinking the list below the current index.
        if (i >= Active.Count)
          continue;

        IPawnYawDriveStep drive = Active[i];
        if (drive == null || !drive.Active)
        {
          Active.RemoveAt(i);
          continue;
        }

        drive.StepFromMoveAllCharacters(deltaTime);
        if (!drive.Active)
          Active.Remove(drive);
      }
    }
  }

  public static int ActiveCountForTests => Active.Count;
}