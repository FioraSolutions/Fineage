using UnityEngine;

public partial class MoveNpc : MonoBehaviour
{
    //setting Rotate
    private Transform _targetObj;

    private float rotationSpeed = 40f; // пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ
    private bool _isRotate = false;
    public bool _isFollow = false;
    private bool _isMove = false;
    private Quaternion _lastRotation; // пїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅ 
    public float rotationThreshold = 0.01f; // пїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ

    //setting Follow
    private ModelMovePawn _targetModelObj;
    //private Vector3 _targetPosFollow;
    private Vector3 _targetMove;
    private float _verticalVelocity = 0;
    private float _gravity = 28;
    // PhysicsVolume.TerminalVelocity (2500 UU/s) scaled to Unity meters (1/52.5).
    const float TerminalVelocity = 47.62f;
    private CharacterController _controller;
    private NpcEntity _entity;

    private Vector3 _flatTransformPos;
    private Vector3 _lastPos;

    private bool _isTeleport = false;
    private bool _detectedIsMove = false;
    private Vector3 _lastPosSpecial = Vector3.zero;
    private DrawLine _debugLine;
    public float stoppingDistance = 0.1f;

}
