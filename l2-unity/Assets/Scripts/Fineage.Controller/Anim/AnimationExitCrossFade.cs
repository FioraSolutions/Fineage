using System.Collections.Generic;
using UnityEngine;

public static partial class AnimationExitCrossFade
{
    static Animation[] _exitOverrides;

    static Animation[] ExitOverrides
    {
        get
        {
            if (_exitOverrides != null)
            {
                return _exitOverrides;
            }

            var list = new List<Animation>(8);
            CollectExitOverrides(SpecialAnimationNames.arrayAtkSpecials, list);
            CollectExitOverrides(SpecialAnimationNames.arrayPhisicalAtkSpecials, list);
            CollectExitOverrides(SpecialAnimationNames.arrayMagicAtkSpecials, list);
            _exitOverrides = list.ToArray();
            return _exitOverrides;
        }
    }


}
