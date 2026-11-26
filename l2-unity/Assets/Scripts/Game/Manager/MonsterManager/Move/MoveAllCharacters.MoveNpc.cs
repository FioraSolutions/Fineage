using System.Collections.Generic;

public partial class MoveAllCharacters
{
  static readonly List<MoveNpc> RegisteredMoveNpcs = new List<MoveNpc>(64);

  public static void RegisterMoveNpc(MoveNpc npc)
  {
    if (npc == null || RegisteredMoveNpcs.Contains(npc))
      return;
    RegisteredMoveNpcs.Add(npc);
  }

  public static void UnregisterMoveNpc(MoveNpc npc)
  {
    if (npc == null)
      return;
    RegisteredMoveNpcs.Remove(npc);
  }

  void StepRegisteredMoveNpcs()
  {
    if (!FineageMovementCentralization.RouteMoveNpcThroughMoveAllCharacters)
      return;
    if (RegisteredMoveNpcs.Count == 0)
      return;

    float dt = UnityEngine.Time.deltaTime;
    using (FineagePerfProfiling.MoveNpcStep.Auto())
    {
      for (int i = RegisteredMoveNpcs.Count - 1; i >= 0; i--)
      {
        MoveNpc npc = RegisteredMoveNpcs[i];
        if (npc == null)
        {
          RegisteredMoveNpcs.RemoveAt(i);
          continue;
        }

        if (!npc.RequiresCentralizedStep())
        {
          RegisteredMoveNpcs.RemoveAt(i);
          continue;
        }

        npc.StepNpcFrame(dt);
        if (!npc.RequiresCentralizedStep())
          RegisteredMoveNpcs.RemoveAt(i);
      }
    }
  }
}