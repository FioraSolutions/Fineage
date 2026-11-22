using System;
using NUnit.Framework;

/// <summary>
/// Duelist physical skills — expected Mix tags from Interlude skillgrp + AnimationCombo_Interlude.
/// </summary>
public sealed class SkillAnimationResolverEditModeTests
{
    static SkillgrpTable _table;

    [OneTimeSetUp]
    public void LoadTables()
    {
        _table = new SkillgrpTable();
        _table.Initialize();
        SkillgrpTable.Bind(_table);
    }

    [OneTimeTearDown]
    public void UnbindTables() => SkillgrpTable.Unbind();

    static void AssertMixStems(int skillId, params string[] expectedStems)
    {
        string[] stems = SkillAnimationResolver.GetSpAtkPhaseStems(skillId, 1);
        Assert.IsNotNull(stems, "skillId=" + skillId);
        Assert.AreEqual(expectedStems.Length, stems.Length, "skillId=" + skillId);
        for (int i = 0; i < expectedStems.Length; i++)
            Assert.AreEqual(expectedStems[i], stems[i], "skillId=" + skillId + " phase=" + i);
    }

    [Test]
    public void Duelist_TripleSlash_Mix05_dualPhases()
    {
        AssertMixStems(1, "SpAtk08", "SpAtk13", "SpAtk22");
    }

    [Test]
    public void Duelist_DoubleSonicSlash_Mix04_dualPhases()
    {
        AssertMixStems(5, "SpAtk07", "SpAtk12", "SpAtk21");
    }

    [Test]
    public void Duelist_SonicBlaster_Mix06_singlePhase()
    {
        AssertMixStems(6, "SpAtk18");
    }

    [Test]
    public void Duelist_SonicStorm_Mix07_singlePhase()
    {
        AssertMixStems(7, "SpAtk19");
    }

    [Test]
    public void Duelist_TripleSonicSlash_Mix03_dualPhases()
    {
        AssertMixStems(261, "SpAtk07", "SpAtk13", "SpAtk20");
    }

    [Test]
    public void SonicBuster_U_singlePhase()
    {
        AssertMixStems(9, "SpAtk03");
    }

    [Test]
    public void ForceCharge_traySkillIsMobius4271()
    {
        Assert.AreEqual(4271, SkillAnimationResolver.ForceChargeTraySkillId);
    }

    [Test]
    public void ToTriggerPrefix_appendsUnderscore()
    {
        Assert.AreEqual("SpAtk08_", SkillAnimationResolver.ToTriggerPrefix("SpAtk08"));
        Assert.AreEqual("SpAtk08_", SkillAnimationResolver.ToTriggerPrefix("SpAtk08_"));
    }
}