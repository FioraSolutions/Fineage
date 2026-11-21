using NUnit.Framework;

public sealed class ClanWindowLayoutEditModeTests
{
    static readonly string[] InterludeFooterRowMajor =
    {
        "Member Info", "Notice", "Authority",
        "Clan Info", "Penalty", "Leave",
        "War Info", "Declare War", "Join",
        "Invite", "Edit Auth", "Edit Crest"
    };

    [Test]
    public void Footer_button_grid_matches_interlude_order()
    {
        Assert.AreEqual(12, InterludeFooterRowMajor.Length);
        Assert.AreEqual("Member Info", InterludeFooterRowMajor[0]);
        Assert.AreEqual("Notice", InterludeFooterRowMajor[1]);
        Assert.AreEqual("Edit Crest", InterludeFooterRowMajor[11]);
    }
}