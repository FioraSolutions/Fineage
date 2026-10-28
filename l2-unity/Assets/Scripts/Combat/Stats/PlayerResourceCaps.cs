/// <summary>
/// Keeps HP/MP max in sync with current values for UI bars (CharSelected / UserInfo may disagree).
/// </summary>
public static class PlayerResourceCaps
{
    public static void EnsureMaxAtLeastCurrent(PlayerStats stats, PlayerStatus status)
    {
        if (stats == null || status == null)
            return;

        int curHp = CeilingPositive(status.GetHp());
        int curMp = CeilingPositive(status.GetMp());

        if (stats.MaxHp < curHp)
            stats.MaxHp = curHp;
        if (stats.MaxMp < curMp)
            stats.MaxMp = curMp;
    }

    public static int DisplayCurrent(double current, int max)
    {
        int cur = CeilingPositive(current);
        if (max > 0 && cur > max)
            return max;
        return cur;
    }

    public static int EffectiveMax(int max, double current)
    {
        int cur = CeilingPositive(current);
        return max >= cur ? max : cur;
    }

    static int CeilingPositive(double value)
    {
        if (value <= 0)
            return 0;
        return (int)System.Math.Ceiling(value);
    }
}
