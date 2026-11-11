using System;
using UnityEngine;
using VContainer;

public partial class PlayerEntity : Entity
{
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
}
