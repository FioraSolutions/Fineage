using UnityEngine;

public partial class UserInfoDto
{
    private float GetRealSpeed(int baseSpeed , float speedMultiplier)
    {
       return  baseSpeed * speedMultiplier;
    }

    private float GetRealSpeed(double baseSpeed, float speedMultiplier)
    {
        return (float)baseSpeed * speedMultiplier;
    }
}
