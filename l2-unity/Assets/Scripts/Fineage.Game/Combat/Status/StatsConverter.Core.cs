using System;

public partial class StatsConverter
{
    public float ConvertStat(Stat statType, float value) {
        switch (statType) {
            case Stat.SPEED:
                return ConvertSpeed(value);
            case Stat.PHYS_ATTACK_SPEED:
                return ConvertPAtkSpd(value);
            case Stat.MAGIC_ATTACK_SPEED:
                return ConvetMAtkSpd(value);
            case Stat.ANIM_RUN_SPEED:
                return ConvertAnimSpeed(value);
            case Stat.ANIM_MAGIC_RUN_SPEED:
                return ConvertAnimMagicSpeedL2j(value);
            case Stat.ANIM_ATTACK_SPEED:
                return ConvertAnimSpeedL2j(value);
            case Stat.ANIM_WALK_SPEED:
                return ConvertAnimWalkSpeedL2j(value);
            case Stat.RUN_SPEED_MONSTER:
                return ConvertSpeedPlus(value);
        }

        return value;
    }

    private float ConvertSpeed(float value) {
        return NumberUtils.ScaleToUnity(value);
    }

    private float ConvertSpeedPlus(float value)
    {
        return NumberUtils.ScaleToUnityPlus(value);
    }

    private float ConvertAnimSpeed(float value)
    {
        return NumberUtils.ScaleAnimSpeedToUnity(value);
    }
}
