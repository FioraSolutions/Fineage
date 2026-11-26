using NUnit.Framework;

public sealed class MoveCentralizationEditModeTests
{
  [Test]
  public void Step10b_flags_route_through_MoveAllCharacters()
  {
    Assert.IsTrue(FineageMovementCentralization.RouteMoveNpcThroughMoveAllCharacters);
    Assert.IsTrue(FineageMovementCentralization.RoutePawnYawLateUpdateThroughMoveAllCharacters);
  }

  [Test]
  public void Pawn_yaw_registry_starts_empty()
  {
    Assert.AreEqual(0, PawnYawDriveRegistry.ActiveCountForTests);
  }
}