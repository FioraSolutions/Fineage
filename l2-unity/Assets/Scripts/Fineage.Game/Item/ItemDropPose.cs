using UnityEngine;

public static partial class ItemDropPose
{
    const int SlotYawStep = 1310720;

    public struct ThrowPose
    {
        public Vector3 LandEuler;
        public Vector3 TumbleAxis;
        public float TumbleDegPerSec;
        public bool StickOnLand;
        public bool TumbleInFlight;
        public float ArcHeightMeters;
    }



    /// <summary>
    /// Cartwheel in the throw's vertical plane (Unreal Pitch). DropType 2 rolls
    /// around the throw. Do not use actor-local X: Unity blades lie on local X,
    /// so Pitch there is a drill spin and looks like "no tumble".
    /// </summary>

}
