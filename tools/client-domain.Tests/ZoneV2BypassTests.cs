using Xunit;

namespace Fineage.Client.Domain.Tests;

/// <summary>
/// NPC bypass normalization and classification, plus the talk-range constant.
/// The bypass helpers are live on the Net path (<c>NetSession</c> classifies every
/// <c>bypass</c> command through them) and are engine-free.
/// </summary>
public class ZoneV2BypassTests
{
    [Fact]
    public void Normalize_strips_the_bypass_prefix_and_the_npc_id()
    {
        Assert.Equal("Chat 1", ZoneV2Bypass.Normalize("bypass -h npc_61000_Chat 1"));
        Assert.Equal("Buy 3000100", ZoneV2Bypass.Normalize("bypass -h npc_11_Buy 3000100"));
        Assert.Equal("Sell", ZoneV2Bypass.Normalize("  bypass   npc_11_Sell  "));
        Assert.Equal("", ZoneV2Bypass.Normalize(null));
    }

    [Fact]
    public void Classify_routes_teleport_goto_and_support_magic()
    {
        string list;
        int id;

        Assert.Equal(ZoneV2Bypass.Teleport, ZoneV2Bypass.Classify("npc_12_teleport NORMAL 3", out list, out id));
        Assert.Equal("NORMAL", list);
        Assert.Equal(3, id);

        Assert.Equal(ZoneV2Bypass.Teleport, ZoneV2Bypass.Classify("goto 4", out list, out id));
        Assert.Equal("NORMAL", list);
        Assert.Equal(4, id);

        Assert.Equal(ZoneV2Bypass.SupportMagic, ZoneV2Bypass.Classify("bypass -h SupportMagic", out list, out id));
        Assert.Equal(ZoneV2Bypass.GiveBlessing, ZoneV2Bypass.Classify("GiveBlessing", out list, out id));
        Assert.Equal(ZoneV2Bypass.Talk, ZoneV2Bypass.Classify("DepositP", out list, out id));
    }

    [Fact]
    public void Talk_distance_matches_the_npc_service_range()
    {
        Assert.Equal(150, ZoneV2NpcSession.InteractionDistance);
    }
}
