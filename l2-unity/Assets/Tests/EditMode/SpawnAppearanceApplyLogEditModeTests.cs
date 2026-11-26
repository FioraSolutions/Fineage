using NUnit.Framework;

public sealed class SpawnAppearanceApplyLogEditModeTests
{
    [Test]
    public void Log_methods_are_callable()
    {
        SpawnAppearanceApplyLog.LogDefaultMaxHp(1, 0);
        SpawnAppearanceApplyLog.LogDefaultMoveSpeed(1, 0f, 0f);
        SpawnAppearanceApplyLog.LogRaceDerivedFromClass(1, -1, 10);
        Assert.Pass();
    }
}