using NUnit.Framework;

public sealed class LocomotionProfileEditModeTests
{
    [Test]
    public void LobbyWalk_FighterRaces_UseStandardSpeed()
    {
        Assert.AreEqual(1.2f, LocomotionProfileLobby.LobbyWalkSpeed(CharacterRaceAnimation.FFighter));
        Assert.AreEqual(1.2f, LocomotionProfileLobby.LobbyWalkSpeed(CharacterRaceAnimation.MMagic));
    }

    [Test]
    public void LobbyWalk_OtherRaces_UseAltSpeed()
    {
        Assert.AreEqual(1.5f, LocomotionProfileLobby.LobbyWalkSpeed(CharacterRaceAnimation.FDarkElf));
    }
}