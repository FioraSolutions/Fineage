public static partial class ClanPacketServices
{
    private static ClanWindow _clan;
    private static ClanAskJoinWindow _ask;


    public static ClanWindow Clan =>
        _clan != null ? _clan : ClanWindow.Instance;

    public static ClanAskJoinWindow Ask =>
        _ask != null ? _ask : ClanAskJoinWindow.Instance;



    /// <summary>
    /// Mobius <c>JoinPledge</c>. The window is only refreshed, never forced open: an accept
    /// arrives while the player is already looking at the invite popup, and a decline just
    /// needs to stop the wait.
    /// </summary>



}
