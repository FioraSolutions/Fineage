using System;
using UnityEngine;

public partial class PlayerStats : Stats
{
    public double WeightPercent()
    {
        if (CurrWeight == 0)
        {
            return 0;
        }

        if (MaxWeight == 0 & CurrWeight > 0) return 100;

        if (MaxWeight != 0)
        {
            double persent = (double)100 / MaxWeight;
            double currentPerxent = persent * CurrWeight;
            return Math.Round(currentPerxent, 2);
        }

        return 0;
    }
}
