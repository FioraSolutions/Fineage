using NUnit.Framework;

public sealed class AttackHitPresentationDualEditModeTests
{
    [Test]
    public void Attack_dto_with_extra_hits_counts_two_presentations()
    {
        Hit[] rows = AttackHitPresentation.BuildHitList(
            targetId: 99,
            firstDamage: 4,
            firstFlags: 0,
            extraDamage: new[] { 7 },
            extraFlags: new[] { 0x20 });
        var dto = new AttackDto();
        dto.SetHitList(rows);

        Assert.AreEqual(2, AttackHitPresentation.CountHits(dto));
        Assert.AreEqual(4, AttackHitPresentation.ResolveHit(dto, 0).Damage);
        Assert.AreEqual(7, AttackHitPresentation.ResolveHit(dto, 1).Damage);
        Assert.IsTrue(AttackHitPresentation.ResolveHit(dto, 1).isCrit());
    }
}