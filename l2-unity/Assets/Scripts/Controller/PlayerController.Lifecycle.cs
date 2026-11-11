using System;

using UnityEngine;

public partial class PlayerController : MonoBehaviour
{
    public void UpdateRunSpeed(float defaultRunSpeed)
    {
        if (_calcSpeed == null)
            _calcSpeed = new PlayerMovementSpeedCalculator();
        // Stub/CharSelected may omit UserInfo — never leave planar speed at 0 (run-in-place).
        if (defaultRunSpeed <= 0.001f)
            defaultRunSpeed = 0.7f;
        _calcSpeed.UpdateSpeedRun(defaultRunSpeed);
    }

    public void UpdateWalkSpeed(float defaultWalkSpeed)
    {
        if (_calcSpeed == null)
            _calcSpeed = new PlayerMovementSpeedCalculator();
        if (defaultWalkSpeed <= 0.001f)
            defaultWalkSpeed = 0.45f;
        _calcSpeed.UpdateSpeedWalk(defaultWalkSpeed);
    }

    public void Initialize()
    {
        // Same-instance re-init (domain reload / re-enter world) must not Destroy(this):
        // that wiped Instance in OnDestroy and left move with zero speed.
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }

        _instance = this;
        if (_calcSpeed == null)
            _calcSpeed = new PlayerMovementSpeedCalculator();
        WireControllerPacketServices();
        if (_pp_sender == null)
            _pp_sender = _injectedPositionSender != null ? _injectedPositionSender : new PlayerPositionSender();

        EnsureCharacterController();
    }

    void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }
}
