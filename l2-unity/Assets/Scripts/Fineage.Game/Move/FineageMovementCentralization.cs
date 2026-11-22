/// <summary>
/// Roadmap step 10b: route per-pawn move/yaw ticks through <see cref="MoveAllCharacters"/>.
/// </summary>
public static class FineageMovementCentralization
{
  public const bool RouteMoveNpcThroughMoveAllCharacters = true;
  public const bool RoutePawnYawLateUpdateThroughMoveAllCharacters = true;
}