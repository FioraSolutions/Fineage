
public interface ICalcSpeed
{
    public float GetSpeed(bool _running);
    public float CalculateInitialSpeed();

    void UpdateSpeedWalk(float defaultWalkSpeed);

    void UpdateSpeedRun(float defaultRunSpeed);
}
