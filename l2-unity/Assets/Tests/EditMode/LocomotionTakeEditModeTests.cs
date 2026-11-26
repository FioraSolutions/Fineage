using NUnit.Framework;

public sealed class LocomotionTakeEditModeTests
{
    [Test]
    public void Creature_take_matches_the_sequence_token()
    {
        Assert.IsTrue(LocomotionGait.SameLocomotionTake("dryad_m00.ao_Wait", "wait"));
        Assert.IsTrue(LocomotionGait.SameLocomotionTake("dryad_m00.ao_Walk", "walk"));
        Assert.IsTrue(LocomotionGait.SameLocomotionTake("orc_fighter_m00.ao_run", "run"));
        Assert.IsTrue(LocomotionGait.SameLocomotionTake("giant_spider_m00.ao_Walk.001", "walk"));
        Assert.IsTrue(LocomotionGait.SameLocomotionTake("giant_spider_m00.ao_run.001", "run"));
        Assert.IsTrue(LocomotionGait.SameLocomotionTake("wolf_m00.ao_Wait", "wait"));
    }

    [Test]
    public void Weapon_and_combat_takes_are_not_the_base_slot()
    {
        Assert.IsFalse(LocomotionGait.SameLocomotionTake("walk_1hs", "walk"));
        Assert.IsFalse(LocomotionGait.SameLocomotionTake("run_bow", "run"));
        Assert.IsFalse(LocomotionGait.SameLocomotionTake("wolf_m00.ao_atkwait", "wait"));
        Assert.IsFalse(LocomotionGait.SameLocomotionTake("dryad_m00.ao_deathwait", "wait"));
        Assert.IsFalse(LocomotionGait.SameLocomotionTake("dryad_m00.ao_SpWait01", "wait"));
        Assert.IsTrue(LocomotionGait.SameLocomotionTake("walk_1hs", "walk_1hs"));
    }
}
