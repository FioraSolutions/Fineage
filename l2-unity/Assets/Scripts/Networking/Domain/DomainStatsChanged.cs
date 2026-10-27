/// <summary>
/// Local (or owned) player stats/equip refresh. UserInfo is the Interlude bridge.
/// </summary>
public sealed class DomainStatsChanged
{
    public int ObjectId;
    public UserInfoDto InterludeUserInfo;
}
