using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ModelTable;
using static Unity.Burst.Intrinsics.Arm;
using static UnityEngine.Rendering.DebugUI;

public partial class Gear : AbstractMeshManager 
{
    protected NetworkAnimationController _networkAnimationReceive;
    protected int _ownerId;
    protected CharacterRaceAnimation _raceId;
    public const string etcName = "etc_";
    public const string weaponName = "weapon_";
    public const string shieldName = "shield_";
    public Transform[] allBone = new Transform[4];
    private GameObject _goCurrentEtcItem;
    [Header("Weapons")]
    [Header("Meta")]
    private Weapon _rightHandWeapon;
    private Weapon _leftHandWeapon;
    private Weapon _leftHandShield;
    private string _origShieldPrefabName = "";
 
    public Action<int, Weapon>  OnEquipAnimationRefresh;
    public Action<string> OnUnequipAnimationRefresh;

    [Header("Models")]
    [Header("Right hand")]
    private WeaponType _lastRightHandType;
    private WeaponType _lastShieldHandType;
    [SerializeField] private WeaponType _rightHandType;
    [SerializeField] protected Transform _rightHandBone;
    [SerializeField] protected Transform _rightHand;
    [Header("LeftHand")]
    [SerializeField] private WeaponType _leftHandType;
    [SerializeField] protected Transform _leftHandBone;
    protected Transform _spineBone;
    protected Transform _rightUpperArm;
    [SerializeField] protected Transform _shieldBone;
    [SerializeField] protected Transform _leftHand;
    [SerializeField] protected string _weaponAnim;
    protected string _lastWeaponAnim = "";
    private int _weaponRange;

    public WeaponType WeaponType { get { return _leftHandType != WeaponType.none ? _leftHandType : _rightHandType; } }
    public WeaponType RightHandWeaponType { get { return _rightHandType; } }
    public WeaponType LeftHandWeaponType { get { return _leftHandType; } }
    public string WeaponAnim { get { return _weaponAnim; } }
    public string LastWeaponAnim { get { return _lastWeaponAnim; } }

    public virtual void Initialize(int ownderId, CharacterRaceAnimation raceId) {
        TryGetComponent(out _networkAnimationReceive);
        _ownerId = ownderId;
        _raceId = raceId;
        _weaponAnim = "hand";
        _weaponRange = 40; //default
    }

    void LateUpdate()
    {
        FaceShieldWithBody();
    }

    // Shield meshes are flat on local Y. In the lowered 1HS idle the bone turns that
    // normal out to the side, so from behind the shield is a thin edge along the back.
    // Combat idle already points the same normal forward and is left alone.
    void FaceShieldWithBody()
    {
        Transform bone = GetShieldBone();
        if (bone == null)
            return;

        Transform shield = null;
        for (int i = 0; i < bone.childCount; i++)
        {
            Transform child = bone.GetChild(i);
            if (child.name.StartsWith(shieldName))
            {
                shield = child;
                break;
            }
        }
        if (shield == null)
            return;

        shield.localRotation = Quaternion.identity;

        Vector3 forward = transform.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 1e-6f)
            return;
        forward.Normalize();

        Vector3 normal = shield.TransformDirection(Vector3.up);
        Vector3 normalFlat = new Vector3(normal.x, 0f, normal.z);
        if (normalFlat.sqrMagnitude < 1e-4f)
            return;
        normalFlat.Normalize();

        float align = Vector3.Dot(normalFlat, forward);
        float edge = 1f - Mathf.Abs(align);
        float weight = Mathf.Clamp01((edge - 0.35f) / 0.45f);
        if (weight <= 0f)
            return;

        float yaw = Vector3.SignedAngle(normalFlat, forward, Vector3.up);
        shield.Rotate(Vector3.up, yaw * weight, Space.World);
    }
}
