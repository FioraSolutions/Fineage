using System;
using UnityEngine;


public class PlayerEntity : Entity
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
    public static PlayerEntity Instance { get => _instance; }

    private PlayerController LocalPlayer
    {
        get
        {
            PlayerController local = GetComponent<PlayerController>();
            if (local != null)
                return local;
            return IncomingPacketActions.Player;
        }
    }

    //default combo name
    private readonly Animation[] pAtkList = { AnimationNames.ATK01, AnimationNames.ATK02, AnimationNames.ATK03 };

    //private readonly Animation[] pAtkList = {  AnimationNames.ATK01 };

    private void Awake()
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
        PlayerStateMachine.Instance.NotifyEvent(Event.DEAD);
    }

    protected override void OnHit(bool criticalHit)
    {
        base.OnHit(criticalHit);
        _characterAnimationAudioHandler.PlaySound(CharacterSoundEvent.Dmg);
    }



    public override float UpdateMAtkSpeed(int mAtkSpd)
    {
        float converted = base.UpdateMAtkSpeed(mAtkSpd);
        PlayerAnimationController.Instance.SetMAtkSpd(converted);

        return converted;
    }

    public override float UpdatePAtkSpeed(int pAtkSpd)
    {
        float converted = base.UpdatePAtkSpeed(pAtkSpd);
        PlayerAnimationController.Instance.SetPAtkSpd(converted);

        return converted;
    }

   
    public void RefreshRandomPAttack()
    {
        int randomIndex = UnityEngine.Random.Range(0, pAtkList.Length);
        RandomName = pAtkList[randomIndex];
    }


    public void UpdatePAtkSpeedPlayer(int pAtkSpd)
    {
        if (pAtkSpd == 0) return;
         float timeAtck = CalcBaseParam.CalculateTimeL2j(pAtkSpd);
         float speedAnim = CalcBaseParam.GetAnimatedSpeed(pAtkSpd, timeAtck);
         PlayerAnimationController.Instance.SetPAtkSpeed(speedAnim);
    }



    public override float UpdateRunSpeed(float serverValue)
    {
        if (serverValue <= 0.001f)
            serverValue = 120f;
        float converted = base.UpdateRunSpeed(serverValue);
        // Apply planar speed first — anim NRE must not leave PlayerController at 0.
        PlayerController player = LocalPlayer;
        if (player != null)
            player.UpdateRunSpeed(converted);

        PlayerAppearance playerApperance = _appearance as PlayerAppearance;
        bool twoHanded = _gear != null && _gear.IsTwoHandedEquipped();
        int baseClass = playerApperance != null ? playerApperance.BaseClass : 0;
        int sex = playerApperance != null ? playerApperance.Sex : 0;
        float anim_converted = CharTemplateRegistry.GetRunSpeed(baseClass, sex, serverValue, twoHanded);
        if (PlayerAnimationController.Instance != null)
            PlayerAnimationController.Instance.SetRunSpeed(anim_converted);

        _lastServerRunSpeed = serverValue;
        return converted;
    }

    public void RefreshRunSpeed()
    {
        PlayerAppearance playerApperance = _appearance as PlayerAppearance;
        bool twoHanded = _gear != null && _gear.IsTwoHandedEquipped();
        int baseClass = playerApperance != null ? playerApperance.BaseClass : 0;
        int sex = playerApperance != null ? playerApperance.Sex : 0;
        float anim_converted = CharTemplateRegistry.GetRunSpeed(baseClass, sex, _lastServerRunSpeed, twoHanded);
        if (PlayerAnimationController.Instance != null)
            PlayerAnimationController.Instance.SetRunSpeed(anim_converted);
    }

    public override float UpdateWalkSpeed(float serverValue)
    {
        if (serverValue <= 0.001f)
            serverValue = 80f;
        float converted = base.UpdateWalkSpeed(serverValue);
        PlayerController player = LocalPlayer;
        if (player != null)
            player.UpdateWalkSpeed(converted);

        PlayerAppearance playerApperance = _appearance as PlayerAppearance;
        bool twoHanded = _gear != null && _gear.IsTwoHandedEquipped();
        int baseClass = playerApperance != null ? playerApperance.BaseClass : 0;
        int sex = playerApperance != null ? playerApperance.Sex : 0;
        float anim_converted = CharTemplateRegistry.GetWalkSpeed(baseClass, sex, serverValue, twoHanded);
        if (PlayerAnimationController.Instance != null)
            PlayerAnimationController.Instance.SetWalkSpeed(anim_converted);

        return converted;
    }

  



    public override void UpdateWaitType(WaitType moveType)
    {
        base.UpdateWaitType(moveType);
    }

    public override void UpdateMoveType(bool running)
    {
        base.UpdateMoveType(running);

        PlayerController player = LocalPlayer;
        if (player != null)
            player.Running = running;

        if (PlayerStateMachine.Instance != null)
        {
           // PlayerStateMachine.Instance.NotifyEvent(Event.MOVE_TYPE_UPDATED);
        }

        if (CharacterInfoWindow.Instance != null)
        {
            CharacterInfoWindow.Instance.UpdateValues();
        }
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
        Transform target = PlayerEntity.Instance.Target;

        return VectorUtils.Distance2D(startPos , target.position);
    }

    public void SetProceduralSpinePose(Vector3 rotation)
    {
        try
        {
            Transform bone = _gear.GetSpineBone();
            SpineProceduralController.Instance.SetBoneMod(bone, new BoneModification(rotation, Vector3.zero, 1.0f));
        }
        catch(Exception ex)
        {
            // Debug.LogError("");
        }


    }

    public void SetProceduralRightUpperArmPose(Vector3 rotation)
    {
        try
        {
            Transform upperArm = _gear.GetRightUpperArm();
            SpineProceduralController.Instance.SetBoneMod(upperArm, new BoneModification(rotation, Vector3.zero, 1.0f));
        }
        catch (Exception ex)
        {
            // Debug.LogError("");
        }

    }

    public void RemoveProceduralPose()
    {
        Transform bone = _gear.GetSpineBone();
        Transform upperArm = _gear.GetRightUpperArm();
        SpineProceduralController.Instance.RemoveBoneMod(bone);
        SpineProceduralController.Instance.RemoveBoneMod(upperArm);
    }
}