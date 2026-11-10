using Xunit;

namespace Fineage.Client.Domain.Tests;

/// <summary>
/// A refused zone action used to be invisible: the reject was logged and the player saw nothing,
/// so a clicking player read it as "the game is broken". Every player-facing code must map to words.
/// </summary>
public class NetRejectLinesTests
{
    [Theory]
    [InlineData("range", "Too far away.")]
    [InlineData("not_owned", "You do not have that item.")]
    [InlineData("not_enough_adena", "You do not have enough adena.")]
    [InlineData("skill_mp", "Not enough MP.")]
    [InlineData("skill_target", "That is not a valid target.")]
    [InlineData("skill_reuse", "That skill is not ready yet.")]
    [InlineData("skill_unknown", "Unknown skill.")]
    [InlineData("attackable", "That NPC cannot be talked to.")]
    [InlineData("unknown_teleport", "That teleport is not available.")]
    public void Player_facing_codes_map_to_a_sentence(string code, string expected) =>
        Assert.Equal(expected, NetRejectLines.Resolve(code, "server text"));

    [Fact]
    public void Code_wins_over_the_server_message_so_the_client_owns_the_wording() =>
        Assert.Equal("Too far away.", NetRejectLines.Resolve("range", "range"));

    [Fact]
    public void Unknown_code_falls_back_to_the_server_message()
    {
        Assert.Equal("Some reason.", NetRejectLines.Resolve("something_new", "Some reason."));
    }

    [Theory]
    [InlineData("hello_required")]
    [InlineData("truncated")]
    [InlineData("bad_envelope")]
    public void Protocol_faults_are_not_shown_to_the_player(string code)
    {
        Assert.Equal("", NetRejectLines.Resolve(code, "internal detail"));
    }

    [Fact]
    public void Empty_input_yields_no_line()
    {
        Assert.Equal("", NetRejectLines.Resolve("", ""));
        Assert.Equal("", NetRejectLines.Resolve(null, null));
    }
}
