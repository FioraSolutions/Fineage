using UnityEngine;
using System.Collections;

public partial class BoatSway : MonoBehaviour
{
    public float swayRangeDegreesX = 1f;
    public float swaySpeedX = 0.3f;
    public float swayAngleX = 0;
    public float pauseSwayXDuration = 0.3f;
    private bool swayDirectionRightX = true;
    private bool pauseSwayX = false;

    public float swayRangeDegreesZ = 2f;
    public float swaySpeedZ = 0.7f;
    public float swayAngleZ = 0;
    public float pauseSwayZDuration = 0.5f;
    private bool swayDirectionRightZ = true;
    private bool pauseSwayZ = false;




}
