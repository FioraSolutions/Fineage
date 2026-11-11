using NUnit.Framework;
using UnityEngine;

/// <summary>
/// EntitySpawn mapper SoT (#930): Attackable→EntityType and heading degrees.
/// </summary>
public sealed class EntitySpawnApplyEditModeTests
{
    [Test]
    public void Folk_AttackableFalse_IsNpc_AndHeadingMatches()
    {
        const int heading = 65536; // ~360° L2 units → 0 mod 360 after convert
        Assert.AreEqual(EntityType.NPC, EntitySpawnApply.ResolveEntityType(false));
        float degrees = EntitySpawnApply.ResolveHeadingDegrees(heading);
        Assert.AreEqual(0f, degrees % 360f, 0.01f);
        Quaternion q = Quaternion.Euler(0f, degrees, 0f);
        Assert.AreEqual(0f, q.eulerAngles.y, 0.5f);
    }

    [Test]
    public void Monster_AttackableTrue_IsMonster()
    {
        Assert.AreEqual(EntityType.Monster, EntitySpawnApply.ResolveEntityType(true));
    }

    [Test]
    public void Heading_QuarterTurn_MapsNearNinety()
    {
        // 65536 / 4 = 16384 → ~90°
        float degrees = EntitySpawnApply.ResolveHeadingDegrees(16384);
        Assert.AreEqual(90f, degrees, 0.5f);
    }
}
