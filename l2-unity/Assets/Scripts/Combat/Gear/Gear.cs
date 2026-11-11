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
}
