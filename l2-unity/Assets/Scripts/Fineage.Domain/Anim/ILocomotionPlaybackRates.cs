/// <summary>
/// Walk/run animator float multipliers. Defined in Domain so gait math does not
/// depend on controller-layer animation interfaces.
/// </summary>
public interface ILocomotionPlaybackRates
{
    void SetRunSpeed(float value);

    void SetWalkSpeed(float value);
}