using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using System.Collections;

public partial class LoginCameraManager : MonoBehaviour
{
    private Dictionary<string, Camera> cameras = new Dictionary<string, Camera>();

    private Camera _activeCamera;

    private static LoginCameraManager _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static LoginCameraManager Instance { get { return _instance; } }









    /// <summary>
    /// Only login/char-select cameras. Never touch gameplay <see cref="CameraController"/> —
    /// <c>Camera.main</c> can be the world camera while Game is preloaded.
    /// </summary>


    /// <summary>
    /// Menu scene often has no ThirdPersonListener; FMOD still needs a StudioListener
    /// or RuntimeManager spams the console on login.
    /// </summary>


}
