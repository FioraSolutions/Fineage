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
}
