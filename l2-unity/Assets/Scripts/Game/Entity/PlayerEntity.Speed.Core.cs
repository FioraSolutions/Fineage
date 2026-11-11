using UnityEngine;
using VContainer;
using System;

public partial class PlayerEntity
{
    public override float UpdateMAtkSpeed(int mAtkSpd)
    {
        float converted = base.UpdateMAtkSpeed(mAtkSpd);
        AnimationPacketServices.PlayerAnimation?.SetMAtkSpd(converted);

        return converted;
    }

    public override float UpdatePAtkSpeed(int pAtkSpd)
    {
        float converted = base.UpdatePAtkSpeed(pAtkSpd);
        AnimationPacketServices.PlayerAnimation?.SetPAtkSpd(converted);

        return converted;
    }

    public void RefreshRandomPAttack()
    {
        int randomIndex = UnityEngine.Random.Range(0, pAtkList.Length);
        RandomName = pAtkList[randomIndex];
    }

    public void UpdatePAtkSpeedPlayer(int pAtkSpd)
    {
        if (pAtkSpd == 0) return;
         float timeAtck = CalcBaseParam.CalculateTimeL2j(pAtkSpd);
         float speedAnim = CalcBaseParam.GetAnimatedSpeed(pAtkSpd, timeAtck);
        AnimationPacketServices.PlayerAnimation?.SetPAtkSpeed(speedAnim);
    }
}
