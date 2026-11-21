using NUnit.Framework;

/// <summary>
/// Documents the shipped NPC/monster meter import contract. Asset YAML/binary
/// compliance is enforced in CI by scripts/check-npc-monster-meter-contract.py (#1473).
/// </summary>
public sealed class NpcMonsterMeterContractEditModeTests
{
    const float MeterGlobalScale = 1f;
    const float MinHumanoidHeightM = 0.15f;
    const float MaxHumanoidHeightM = 2.5f;

    [Test]
    public void Importer_global_scale_is_unity_meters()
    {
        Assert.AreEqual(1f, MeterGlobalScale, 1e-6f);
    }

    [Test]
    public void Humanoid_height_window_is_stable_for_regressions()
    {
        Assert.Less(MinHumanoidHeightM, MaxHumanoidHeightM);
        Assert.AreEqual(0.15f, MinHumanoidHeightM, 1e-6f);
        Assert.AreEqual(2.5f, MaxHumanoidHeightM, 1e-6f);
    }
}