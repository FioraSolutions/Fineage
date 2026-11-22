using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SocialPlatforms;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public partial class MoveAllCharacters : MonoBehaviour, IMoveAllCharacters
{
    void Update()
    {
        if (_moveDict == null)
            return;

        StepMoves();
        StepRegisteredMoveNpcs();
        PawnYawDriveRegistry.StepAll(Time.deltaTime);

        IGravity gravity = GravityNpcPacketServices.Gravity;
        gravity?.StepGravity();

        StepRotatesBurst();
        RemoveFinishKeyRotate();
    }
}
