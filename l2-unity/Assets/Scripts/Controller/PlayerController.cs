using System;

using UnityEngine;

public partial class PlayerController : MonoBehaviour
{
    /* Components */
    private CharacterController _controller;
    /*Rotate*/
    private float _finalAngle;

    /* Movement */
     private Vector3 _moveDirection;
     private bool _running = true;

    /* Gravity */
    private float _verticalVelocity = 0;
    [SerializeField] private float _jumpForce = 10;
    [SerializeField] private float _gravity = 28;

    // PhysicsVolume.TerminalVelocity (2500 UU/s) scaled to Unity meters (1/52.5).
    // Caps falling speed the same way Interlude physFalling does.
    private const float TerminalVelocity = 47.62f;


    /* Target */
    private MovementTarget _movementTarget;

    private bool _isMove = false;
    private Transform _lookAtTarget;
    private GearEffects _gearEffects;


}
