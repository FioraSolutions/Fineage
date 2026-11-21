using NUnit.Framework;

public sealed class InterludeActionAvailabilityEditModeTests
{
    InterludeActionAvailabilityService _service;

    [SetUp]
    public void SetUp()
    {
        InterludeActionCatalog.EnsureLoaded();
        InterludeMountablePetCatalog.EnsureLoaded();
        _service = new InterludeActionAvailabilityService();
        _service.Reset();
    }

    [Test]
    public void Catalog_mount_action_uses_mountable_requirement()
    {
        Assert.IsTrue(InterludeActionCatalog.TryGet(38, out var mount));
        Assert.AreEqual(InterludeActionAvailabilityKind.MountablePetSummonedOrMounted, mount.Availability);
        Assert.AreEqual("Mount/Dismount", mount.CmdName);
        Assert.AreEqual("action036", mount.IconStem);
    }

    [Test]
    public void Catalog_pet_commands_require_pet_summoned()
    {
        Assert.IsTrue(InterludeActionCatalog.TryGet(15, out var hold));
        Assert.AreEqual(InterludeActionAvailabilityKind.PetSummoned, hold.Availability);
        Assert.IsTrue(InterludeActionCatalog.TryGet(21, out var sumHold));
        Assert.AreEqual(InterludeActionAvailabilityKind.ServitorSummoned, sumHold.Availability);
    }

    [Test]
    public void Mount_hidden_without_pet_or_mount()
    {
        Assert.IsFalse(_service.IsAvailable(38));
        Assert.IsTrue(_service.IsAvailable(0));
    }

    [Test]
    public void Mount_visible_with_mountable_pet()
    {
        _service.NoteSummon(InterludeActionAvailabilityState.SummonKindPet, 12526, dead: false);
        Assert.IsTrue(_service.IsAvailable(38));
    }

    [Test]
    public void Mount_visible_when_player_mounted_wyvern()
    {
        _service.NoteMount(mountType: 2, mountNpcId: 12621);
        Assert.IsTrue(_service.IsAvailable(38));
        Assert.IsTrue(_service.State.WyvernMounted);
    }

  [Test]
  public void Mount_visible_when_client_ride_visual_active()
  {
    _service.NoteClientMountedVisual(true);
    Assert.IsTrue(_service.IsAvailable(38));
  }

    [Test]
    public void Pet_attack_hidden_without_pet()
    {
        Assert.IsFalse(_service.IsAvailable(16));
        _service.NoteSummon(InterludeActionAvailabilityState.SummonKindPet, 12077, dead: false);
        Assert.IsTrue(_service.IsAvailable(16));
    }

    [Test]
    public void Template_pet_skill_requires_matching_npc()
    {
        Assert.IsTrue(InterludeActionCatalog.TryGet(1003, out var wildStun));
        Assert.AreEqual(InterludeActionAvailabilityKind.SummonTemplateActive, wildStun.Availability);
        Assert.IsFalse(_service.IsAvailable(1003));
        _service.NoteSummon(InterludeActionAvailabilityState.SummonKindPet, 12311, dead: false);
        Assert.IsTrue(_service.IsAvailable(1003));
    }

    [Test]
    public void Summon_gone_clears_pet_actions()
    {
        _service.NoteSummon(InterludeActionAvailabilityState.SummonKindPet, 12526, dead: false);
        Assert.IsTrue(_service.IsAvailable(15));
        _service.NoteSummonDismissed(InterludeActionAvailabilityState.SummonKindPet);
        Assert.IsFalse(_service.IsAvailable(15));
        Assert.IsFalse(_service.IsAvailable(38));
    }
}