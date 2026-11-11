using System;
using UnityEngine;

public partial class PlayerStats : Stats
{
    public double ExpPercent(int level)
    {
        if (Exp == 0)
        {
            return 0;
        }

        long MaxExp = LevelServer.GetExp(level);
        if (MaxExp == 0 & Exp > 0) return 100;
        if (MaxExp != 0)
        {
            double persent = (double)100 / MaxExp;
            double currentPerxent = persent * Exp;
            return Math.Round(currentPerxent, 2);
        }

        return 0;
    }
}
