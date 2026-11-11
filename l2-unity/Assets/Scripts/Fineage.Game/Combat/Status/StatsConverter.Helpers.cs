using System;

public partial class StatsConverter
{
    private float ConvertAnimMagicSpeedL2j(float value)
    {
        return NumberUtils.ScaleAnimSpeedMagicToUnity(value);
    }

    private float ConvertAnimSpeedL2j(float value)
    {
        return NumberUtils.ScaleAnimSpeedL2jToUnity(value);
    }

    private float ConvertAnimWalkSpeedL2j(float value)
    {
        return NumberUtils.ScaleAnimWalkSpeedL2jToUnity(value);
    }

    private float ConvertPAtkSpd(float value) {
        if (value < 2) {
            return 2700;
        }
        //Debug.Log("Current atack speed convert " + (480000 / value));
        return (480000 / value);
    }

    private float ConvetMAtkSpd(float value) {
        //        if (skill.isMagic()) {
        //            return (int) ((skillTime * 333) / attacker.getMAtkSpd());
        //        }
        //        return (int) ((skillTime * 333) / attacker.getPAtkSpd());
        return value;
    }
}
