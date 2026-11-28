public class NetworkEntity : Entity
{
    public override float UpdateMAtkSpeed(int mAtkSpd)
    {
        float converted = base.UpdateMAtkSpeed(mAtkSpd);
        // Doors spawn as MonsterEntity without a NetworkAnimationReceive; there is no animation to speed up.
        if (_networkAnimationReceive != null)
            _networkAnimationReceive.SetMAtkSpd(converted);

        return converted;
    }

    public override float UpdatePAtkSpeed(int pAtkSpd)
    {
        float converted = base.UpdatePAtkSpeed(pAtkSpd);
        if (_networkAnimationReceive != null)
            _networkAnimationReceive.SetPAtkSpd(converted);

        return converted;
    }
}