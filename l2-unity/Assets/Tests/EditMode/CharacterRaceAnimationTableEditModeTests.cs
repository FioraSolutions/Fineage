using NUnit.Framework;

public sealed class CharacterRaceAnimationTableEditModeTests
{
    [Test]
    public void ParseRaceInterlude_HumanFighter_Male()
    {
        var anim = CharacterRaceAnimationParser.ParseRaceInterlude(CharacterRace.Human, 0, (int)BaseClass.Fighter);
        Assert.AreEqual(CharacterRaceAnimation.MFighter, anim);
    }

    [Test]
    public void ParseRaceInterlude_OrcMystic_Female()
    {
        var anim = CharacterRaceAnimationParser.ParseRaceInterlude(CharacterRace.Orc, 1, (int)BaseClass.MMagic);
        Assert.AreEqual(CharacterRaceAnimation.FShaman, anim);
    }

    [Test]
    public void ParseRace_Elf_IgnoresMageFlag()
    {
        var fighter = CharacterRaceAnimationParser.ParseRace(CharacterRace.Elf, 0, false);
        var mystic = CharacterRaceAnimationParser.ParseRace(CharacterRace.Elf, 0, true);
        Assert.AreEqual(CharacterRaceAnimation.MElf, fighter);
        Assert.AreEqual(CharacterRaceAnimation.MElf, mystic);
    }
}