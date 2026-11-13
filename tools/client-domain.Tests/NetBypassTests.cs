using Xunit;

namespace Fineage.Client.Domain.Tests;

/// <summary>
/// NPC bypass normalization and classification, plus the talk-range constant.
/// The bypass helpers are live on the Net path (<c>NetSession</c> classifies every
/// <c>bypass</c> command through them) and are engine-free.
/// </summary>
public class NetBypassTests
{
    [Fact]
    public void Normalize_strips_the_bypass_prefix_and_the_npc_id()
    {
        Assert.Equal("Chat 1", NetBypass.Normalize("bypass -h npc_61000_Chat 1"));
        Assert.Equal("Buy 3000100", NetBypass.Normalize("bypass -h npc_11_Buy 3000100"));
        Assert.Equal("Sell", NetBypass.Normalize("  bypass   npc_11_Sell  "));
        Assert.Equal("", NetBypass.Normalize(null));
    }

    [Fact]
    public void Classify_routes_teleport_goto_and_support_magic()
    {
        string list;
        int id;

        Assert.Equal(NetBypass.Teleport, NetBypass.Classify("npc_12_teleport NORMAL 3", out list, out id));
        Assert.Equal("NORMAL", list);
        Assert.Equal(3, id);

        Assert.Equal(NetBypass.Teleport, NetBypass.Classify("goto 4", out list, out id));
        Assert.Equal("NORMAL", list);
        Assert.Equal(4, id);

        Assert.Equal(NetBypass.SupportMagic, NetBypass.Classify("bypass -h SupportMagic", out list, out id));
        Assert.Equal(NetBypass.GiveBlessing, NetBypass.Classify("GiveBlessing", out list, out id));
        Assert.Equal(NetBypass.Talk, NetBypass.Classify("DepositP", out list, out id));
    }

    [Fact]
    public void Talk_distance_matches_the_npc_service_range()
    {
        Assert.Equal(250, NetNpcSession.InteractionDistance);
    }
}
