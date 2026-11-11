using System;
using UnityEngine;

public partial class Formulas
{
    public static void Init()
	{
		for (int i = 0; i<STR_BONUS.Length; i++)
			STR_BONUS[i] = Math.Floor(Math.Pow(STR_COMPUTE[0], i - STR_COMPUTE[1]) * 100 + .5d) / 100;
		for (int i = 0; i<INT_BONUS.Length; i++)
			INT_BONUS[i] = Math.Floor(Math.Pow(INT_COMPUTE[0], i - INT_COMPUTE[1]) * 100 + .5d) / 100;
		for (int i = 0; i<DEX_BONUS.Length; i++)
			DEX_BONUS[i] = Math.Floor(Math.Pow(DEX_COMPUTE[0], i - DEX_COMPUTE[1]) * 100 + .5d) / 100;
		for (int i = 0; i<WIT_BONUS.Length; i++)
			WIT_BONUS[i] = Math.Floor(Math.Pow(WIT_COMPUTE[0], i - WIT_COMPUTE[1]) * 100 + .5d) / 100;
		for (int i = 0; i<CON_BONUS.Length; i++)
			CON_BONUS[i] = Math.Floor(Math.Pow(CON_COMPUTE[0], i - CON_COMPUTE[1]) * 100 + .5d) / 100;
		for (int i = 0; i<MEN_BONUS.Length; i++)
			MEN_BONUS[i] = Math.Floor(Math.Pow(MEN_COMPUTE[0], i - MEN_COMPUTE[1]) * 100 + .5d) / 100;
		
		for (int i = 0; i<BASE_EVASION_ACCURACY.Length; i++)
			BASE_EVASION_ACCURACY[i] = Math.Sqrt(i)* 6;
		
		// Precompute square root values
		for (int i = 0; i<SQRT_CON_BONUS.Length; i++)
			SQRT_CON_BONUS[i] = Math.Sqrt(CON_BONUS[i]);
		for (int i = 0; i<SQRT_MEN_BONUS.Length; i++)
			SQRT_MEN_BONUS[i] = Math.Sqrt(MEN_BONUS[i]);
	}
}
