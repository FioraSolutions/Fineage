using Xunit;

namespace Fineage.Client.Domain.Tests;

/// <summary>
/// Clan pledge book and notice mapping. Salvaged from the retired <c>client-zone-v2.Tests</c>
/// MessagePack codec host; only the assertions that touch surviving domain code remain.
/// </summary>
public class ZoneV2ClanTests
{
    [Fact]
    public void Pledge_state_tracks_leader_members_and_clear()
    {
        var book = new ZoneV2ClanBook();
        book.NoteName(9, "Strangers");
        book.ApplyMembers(new ZoneV2PledgeMemberList
        {
            ClanId = 4,
            LeaderObjectId = 1001,
            Members =
            [
                new ZoneV2PledgeMember { ObjectId = 1001, Name = "Leader", Level = 10, Online = true },
                new ZoneV2PledgeMember { ObjectId = 2001, Name = "Ally", Level = 8, Online = false }
            ]
        });
        Assert.True(book.HasClan);
        Assert.Equal("", book.Name);
        Assert.True(book.IsLeader(1001));
        Assert.False(book.IsLeader(2001));
        Assert.Equal(1, book.OnlineCount());
        Assert.Equal("Leader", book.LeaderName());

        book.ApplyStatus(new ZoneV2PledgeStatus
        {
            ClanId = 4,
            Name = "Wolves",
            Level = 0,
            LeaderObjectId = 1001,
            MemberCount = 2,
            DissolvingExpiryUnixMs = 50
        });
        Assert.Equal("Wolves", book.Name);
        Assert.Equal("Wolves", book.NameFor(4));
        Assert.True(book.Dissolving);
        Assert.Equal(2, book.Members.Length);
        Assert.Equal("Strangers", book.NameFor(9));

        book.ApplyStatus(new ZoneV2PledgeStatus { ClanId = 0, Name = "Wolves" });
        Assert.False(book.HasClan);
        Assert.Empty(book.Members);
        Assert.False(book.Dissolving);
        Assert.Equal("Wolves", book.NameFor(4));
        Assert.Equal("Strangers", book.NameFor(9));

        book.ApplyStatus(new ZoneV2PledgeStatus { ClanId = 4, Name = "Wolves", LeaderObjectId = 1001 });
        book.ApplyMembers(new ZoneV2PledgeMemberList { ClanId = 0, Members = [] });
        Assert.False(book.HasClan);
        Assert.Empty(book.Members);
    }

    [Fact]
    public void Clan_error_codes_become_readable_lines()
    {
        Assert.Equal("The clan leader cannot leave. Dissolve the clan instead.", ZoneV2ClanNotices.Map("clan_leader_leave"));
        Assert.Equal("That clan invitation has expired.", ZoneV2ClanNotices.LineOrEquip("clan_invite", "clan_invite"));
        Assert.Equal("The clan is full.", ZoneV2ClanNotices.LineOrEquip("clan_full", ""));
        Assert.Equal("You must wait a day.", ZoneV2ClanNotices.LineOrEquip("clan_join_penalty", "You must wait a day."));
        Assert.Equal("You do not have that item.", ZoneV2EquipNotices.Line("not_owned", ""));
        Assert.Equal("You do not have that item.", ZoneV2ClanNotices.LineOrEquip("not_owned", ""));
        Assert.Equal("The clan request was refused.", ZoneV2ClanNotices.Map("clan_unknown"));
    }
}
