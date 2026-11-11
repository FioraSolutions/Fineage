using UnityEngine;

public static partial class LevelServer
{
    public static int ClampLevel(int level)
    {
        if (level < 1) return 1;
        if (level > MAX_LEVEL) return MAX_LEVEL;
        return level;
    }

    public static int GetLevelFromExp(long totalExp)
    {
        if (totalExp < 0)
            return 1;

        for (int level = 1; level <= MAX_LEVEL; level++)
        {
            if (totalExp < LEVEL[level])
                return level - 1;
        }

        return MAX_LEVEL;
    }

    public static long GetExpToNextLevel(int level, long currentExp)
    {
        level = ClampLevel(level);

        if (level >= MAX_LEVEL)
            return 0;

        long nextLevelExp = LEVEL[level + 1];

        long remaining = nextLevelExp - currentExp;
        return remaining > 0 ? remaining : 0;
    }
}
