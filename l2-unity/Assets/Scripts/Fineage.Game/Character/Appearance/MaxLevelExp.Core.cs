using UnityEngine;

public static partial class LevelServer
{
    public static long GetExp(int level)
    {
        if (!IsValidLevel(level))
            return -1;

        return LEVEL[level];
    }

    public static bool IsValidLevel(int level)
    {
        return level > 0 && level <= MAX_LEVEL;
    }
}
