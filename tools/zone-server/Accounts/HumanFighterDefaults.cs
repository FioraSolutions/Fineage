namespace Fineage.ZoneServer.Accounts;

/// <summary>Interlude Human Fighter (race 0, class 0) base stats for char create/template.</summary>
public static class HumanFighterDefaults
{
    public const int RaceHuman = 0;
    public const int ClassFighter = 0;

    public const int BaseStr = 40;
    public const int BaseDex = 30;
    public const int BaseCon = 43;
    public const int BaseInt = 21;
    public const int BaseWit = 11;
    public const int BaseMen = 25;

    public static bool IsHumanFighter(int race, int classId) =>
        race == RaceHuman && classId == ClassFighter;
}
