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


















}
