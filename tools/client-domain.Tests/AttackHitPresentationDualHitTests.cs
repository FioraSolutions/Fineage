using Xunit;

namespace Fineage.Client.Domain.Tests;

public sealed class AttackHitPresentationDualHitTests
{
    [Fact]
    public void Dual_attack_extra_hits_build_two_presentation_rows()
    {
        var extra = new[]
        {
            new AttackHitNet { Damage = 11, Flags = 0x20 },
        };

        int[] extraDamage = { extra[0].Damage };
        int[] extraFlags = { extra[0].Flags };

        Hit[] rows = AttackHitPresentation.BuildHitList(42, 9, 0x10, extraDamage, extraFlags);

        Assert.Equal(2, rows.Length);
        Assert.Equal(9, rows[0].Damage);
        Assert.True(rows[0].hasSoulshot());
        Assert.Equal(11, rows[1].Damage);
        Assert.True(rows[1].isCrit());
    }

    [Fact]
    public void Dual_attack_extra_hits_yield_two_presentation_indices()
    {
        Hit[] rows = AttackHitPresentation.BuildHitList(1, 5, 0, new[] { 6 }, new[] { 0 });
        Assert.Equal(2, rows.Length);
        Assert.NotNull(rows[0]);
        Assert.NotNull(rows[1]);
    }
}