using Xunit;

namespace Fineage.Client.Domain.Tests;

public sealed class AttackHitPresentationDualHitTests
{
    [Fact]
    public void Dual_attack_extra_hits_build_two_presentation_rows()
    {
        int[] extraDamage = { 11 };
        int[] extraFlags = { 0x20 };

        Hit[] rows = AttackHitWireRows.BuildHitList(42, 9, 0x10, extraDamage, extraFlags);

        Assert.Equal(2, rows.Length);
        Assert.Equal(9, rows[0].Damage);
        Assert.True(rows[0].hasSoulshot());
        Assert.Equal(11, rows[1].Damage);
        Assert.True(rows[1].isCrit());
    }

    [Fact]
    public void Dual_attack_extra_hits_plan_two_presentations()
    {
        AttackHitCue[] cues = AttackHitWireRows.PlanCues(
            42,
            9,
            0x10,
            new[] { 0 },
            new[] { 0x80 });

        Assert.Equal(2, cues.Length);
        Assert.Equal(0.25, cues[0].CycleFraction, 3);
        Assert.Equal(0.50, cues[1].CycleFraction, 3);
        Assert.Equal("9", cues[0].Label);
        Assert.Equal(AttackHitLabelKind.Damage, cues[0].Kind);
        Assert.True(cues[0].Soulshot);
        Assert.Equal("Miss", cues[1].Label);
        Assert.Equal(AttackHitLabelKind.Miss, cues[1].Kind);
        Assert.NotEqual(cues[0].CycleFraction, cues[1].CycleFraction);
    }

    [Fact]
    public void Dual_attack_second_hit_crit_is_its_own_label()
    {
        AttackHitCue[] cues = AttackHitWireRows.PlanCues(1, 4, 0, new[] { 7 }, new[] { 0x20 });

        Assert.Equal(2, cues.Length);
        Assert.Equal("4", cues[0].Label);
        Assert.Equal("7", cues[1].Label);
        Assert.Equal(AttackHitLabelKind.Critical, cues[1].Kind);
        Assert.Equal(0.25, cues[0].CycleFraction, 3);
        Assert.Equal(0.50, cues[1].CycleFraction, 3);
    }

    [Fact]
    public void Dual_attack_extra_hits_yield_two_presentation_indices()
    {
        Hit[] rows = AttackHitWireRows.BuildHitList(1, 5, 0, new[] { 6 }, new[] { 0 });
        Assert.Equal(2, rows.Length);
        Assert.NotNull(rows[0]);
        Assert.NotNull(rows[1]);
    }
}