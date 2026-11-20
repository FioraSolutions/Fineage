using Xunit;

namespace Fineage.Client.Domain.Tests;

public sealed class AttackHitSoulshotFlagTests
{
    const int HitFlagUsess = 0x10;

    [Fact]
    public void Hit_hasSoulshot_reads_HITFLAG_USESS()
    {
        var hit = new Hit(100, 42, HitFlagUsess);
        Assert.True(hit.hasSoulshot());
    }

    [Fact]
    public void Dual_style_second_row_can_carry_soulshot_independently()
    {
        var first = new Hit(100, 20, 0);
        var second = new Hit(100, 25, HitFlagUsess);

        Assert.False(first.hasSoulshot());
        Assert.True(second.hasSoulshot());
    }
}