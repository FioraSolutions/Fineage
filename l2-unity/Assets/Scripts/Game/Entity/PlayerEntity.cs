using System;
using UnityEngine;
using VContainer;


public partial class PlayerEntity : Entity
{


    private CharacterAnimationAudioHandler _characterAnimationAudioHandler;

    private float _lastServerRunSpeed = 0;

    private static PlayerEntity _instance;
    public Animation RandomName { get; set; }

    public float RemainingTime { get; set; }

    public int CountAtk { get; set; }
    public int CurrentAttackCount { get; set; }

    public bool isAccesNewAtk {get;set;}
    public bool isStop { get; set; }

    public bool IsAttack { get; set; }
    public bool isAutoAttack { get; set; }

    public  Animation LastAtkAnimation { get; set; }
    [Obsolete("Use DomainPacketServices.LocalPlayer instead")]
    public static PlayerEntity Instance { get => _instance; }

    [Inject] PlayerController _playerController;

    private PlayerController LocalPlayer
    {
        get
        {
            PlayerController local = _playerController != null
                ? _playerController
                : GetComponent<PlayerController>();
            return local != null ? local : EntityPacketServices.Player;
        }
    }

    //default combo name
    private readonly Animation[] pAtkList = { AnimationNames.ATK01, AnimationNames.ATK02, AnimationNames.ATK03 };

    //private readonly Animation[] pAtkList = {  AnimationNames.ATK01 };

    private new void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            RandomName = pAtkList[0];
            CountAtk = 2;

        }
        else
        {
            Destroy(this);
        }

        WireEntityPacketServices();
    }

    private void Start()
    {
        WireEntityPacketServices();
    }

    void WireEntityPacketServices()
    {
        PlayerController local = _playerController != null
            ? _playerController
            : GetComponent<PlayerController>();
        EntityPacketServices.Bind(player: local);
        DomainPacketServices.Bind(localPlayer: this);
        GameEventPacketServices.Bind(localPlayer: this);
    }

    public override void Initialize()
    {
        base.Initialize();

        _characterAnimationAudioHandler = GetComponentInChildren<CharacterAnimationAudioHandler>();

        if (_instance == null)
        {
            _instance = this;
        }

        EquipAllArmors();

        EntityLoaded = true;
    }

    public void RefreshVisuals()
    {
        // GearFlowLog.Info("PlayerEntity.RefreshVisuals " + GearFlowLog.Entity(this) +
            // " " + GearFlowLog.Paperdoll(this) +
            // " gear=" + (_gear != null ? _gear.GetType().Name : "null"));
        EquipAllWeapons();
        EquipAllArmors();
    }

    void OnDestroy()
    {
        _instance = null;
    }

    private void EquipAllArmors()
    {
        PlayerAppearance appearance = _appearance as PlayerAppearance;
        UserGear gear = _gear as UserGear;
        if (appearance == null || gear == null)
        {
            // GearFlowLog.Warn("PlayerEntity.EquipAllArmors abort " + GearFlowLog.Entity(this) +
                // " appearance=" + (appearance != null) + " gear=" + (_gear != null ? _gear.GetType().Name : "null"));
            return;
        }

        // GearFlowLog.Info("PlayerEntity.EquipAllArmors " + GearFlowLog.Entity(this) +
            // " " + GearFlowLog.Paperdoll(appearance));
        gear.SyncEquippedArmor(appearance);
    }

    public string GetEquippedWeaponName()
    {
        PlayerGear gear = (PlayerGear)_gear;
        return gear.GetNameWeapon();
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        PlayerStatePacketServices.PlayerState.NotifyEvent(Event.DEAD);
    }

    protected override void OnHit(bool criticalHit)
    {
        base.OnHit(criticalHit);
        _characterAnimationAudioHandler.PlaySound(CharacterSoundEvent.Dmg);
    }



    public string GetCurrentAnimName()
    {
        return _gear.WeaponAnim;
    }

    public GameObject GetGoEtcItem()
    {
        return _gear.GetGoEtcItem();
    }

    public float TargetDistance()
    {
        Vector3 startPos = GetPositionRightHand();
        Transform target = Target;

        return VectorUtils.Distance2D(startPos , target.position);
    }

    public void SetProceduralSpinePose(Vector3 rotation)
    {
        try
        {
            Transform bone = _gear.GetSpineBone();
            SpineProceduralPacketServices.Controller?.SetBoneMod(bone, new BoneModification(rotation, Vector3.zero, 1.0f));
        }
        catch (Exception)
        {
            // Debug.LogError("");
        }


    }

    public void SetProceduralRightUpperArmPose(Vector3 rotation)
    {
        try
        {
            Transform upperArm = _gear.GetRightUpperArm();
            SpineProceduralPacketServices.Controller?.SetBoneMod(upperArm, new BoneModification(rotation, Vector3.zero, 1.0f));
        }
        catch (Exception)
        {
            // Debug.LogError("");
        }

    }

    public void RemoveProceduralPose()
    {
        Transform bone = _gear.GetSpineBone();
        Transform upperArm = _gear.GetRightUpperArm();
        SpineProceduralController procedural = SpineProceduralPacketServices.Controller;
        if (procedural == null)
            return;

        procedural.RemoveBoneMod(bone);
        procedural.RemoveBoneMod(upperArm);
    }
}
