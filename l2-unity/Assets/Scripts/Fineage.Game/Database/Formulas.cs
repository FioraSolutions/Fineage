using System;
using UnityEngine;

public partial class Formulas 
{

    public static readonly int MAX_STAT_VALUE = 100;


    private static readonly double[] STR_COMPUTE = new double[]
      {
        1.036,
        34.845
      };
    private static readonly double[] INT_COMPUTE = new double[]
    {
        1.020,
        31.375
    };
    private static readonly double[] DEX_COMPUTE = new double[]
    {
        1.009,
        19.360
    };
    private static readonly double[] WIT_COMPUTE = new double[]
    {
        1.050,
        20.000
    };
    private static readonly double[] CON_COMPUTE = new double[]
    {
        1.030,
        27.632
    };
    private static readonly double[] MEN_COMPUTE = new double[]
    {
        1.010,
        -0.060
    };

    public static readonly double[] WIT_BONUS = new double[MAX_STAT_VALUE];
    public static readonly double[] MEN_BONUS = new double[MAX_STAT_VALUE];
    public static readonly double[] INT_BONUS = new double[MAX_STAT_VALUE];
    public static readonly double[] STR_BONUS = new double[MAX_STAT_VALUE];
    public static readonly double[] DEX_BONUS = new double[MAX_STAT_VALUE];
    public static readonly double[] CON_BONUS = new double[MAX_STAT_VALUE];

    public static readonly double[] BASE_EVASION_ACCURACY = new double[MAX_STAT_VALUE];

    protected static readonly double[] SQRT_MEN_BONUS = new double[MAX_STAT_VALUE];
    protected static readonly double[] SQRT_CON_BONUS = new double[MAX_STAT_VALUE];


}
