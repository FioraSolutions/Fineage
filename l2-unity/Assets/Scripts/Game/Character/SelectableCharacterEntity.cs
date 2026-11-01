using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class SelectableCharacterEntity : MonoBehaviour
{
    [Inject] IAnimationManager _animations;

    private IAnimationManager Animations
    {
        get { return _animations != null ? _animations : CharacterPacketServices.Animations; }
    }
    //private float _walkSpeed = 1.5f;
    [SerializeField] private string _weaponAnim;
    [SerializeField] private Vector3 _destination;
    [SerializeField] private Vector3 _destEulerAngles;
    [SerializeField] private CharSelectionInfoPackage _characterInfo;
    [SerializeField] private CharSelectInfoPackage _characterInfoInterlude;
    [SerializeField] private bool _walking = false;

    private CharacterController _characterController;
    private BaseAnimationController _baseAnimationController;

    public CharSelectionInfoPackage CharacterInfo {  get { return _characterInfo; } set {  _characterInfo = value; }  }
    public CharSelectInfoPackage CharacterInfoInterlude { get { return _characterInfoInterlude; } set { _characterInfoInterlude = value; } }

    public string WeaponAnim { get { return _weaponAnim; } set { _weaponAnim = value; } }

    const float LobbyTurnDegPerSec = 360f;
    bool _hasDestYaw;

    private void Awake() {
        _characterController = GetComponent<CharacterController>();
        _baseAnimationController = GetComponent<BaseAnimationController>();
        _destination = transform.position;
        if (_characterController != null)
            _characterController.enabled = false;
        App.InjectGameObject(gameObject);
        WireCharacterPacketServices();
    }

    private void Start()
    {
        WireCharacterPacketServices();
    }

    private void WireCharacterPacketServices()
    {
        CharacterPacketServices.Bind(animations: _animations);
    }

    private void Update() {
        float ditanceToDestination = Vector3.Distance(_destination, transform.position);
        if(ditanceToDestination > 0.05f) {
            if(!_walking) {
                StartWalking();
                SetWalkSpeedModel();
            }
            
            // Turn over time (L2 pawns rotate, they don't snap); walk along the current facing.
            Vector3 to = new Vector3(_destination.x - transform.position.x, 0f, _destination.z - transform.position.z);
            if (to.sqrMagnitude > 1e-6f)
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, Quaternion.LookRotation(to), LobbyTurnDegPerSec * Time.deltaTime);
            // Lobby pawns don't use CharacterController.Move: their capsules shoved each other
            // (selected pawn walking through a neighbour piled them up). Slide straight to the slot.
            transform.position = Vector3.MoveTowards(
                transform.position, _destination, Time.deltaTime * GetWalkSpeedPoint(_characterInfoInterlude));
           
        } else {
            if (_walking) {
                StopWalking();
                if (_celebrateOnArrive) {
                    _celebrateOnArrive = false;
                    PlayVictory();
                }
            }
            if (_hasDestYaw)
                transform.rotation = Quaternion.RotateTowards(
                transform.rotation, Quaternion.Euler(_destEulerAngles), LobbyTurnDegPerSec * Time.deltaTime);
        }
    }
    //Movement speed to the target without taking into account animation speed
    private float GetWalkSpeedPoint(CharSelectInfoPackage info)
    {
        if (info == null)
            return 1.2f;
        var race = info.CharacterRaceAnimation;
        if (race == CharacterRaceAnimation.FFighter ||
            race == CharacterRaceAnimation.MFighter ||
            race == CharacterRaceAnimation.FMagic ||
            race == CharacterRaceAnimation.MMagic)
        {
            return 1.2f;
        }

        return 1.5f;
    }
    //animation speed
    private void SetWalkSpeedModel()
    {
        var race = _characterInfoInterlude.CharacterRaceAnimation;
        if (race == CharacterRaceAnimation.FFighter | race == CharacterRaceAnimation.MFighter)
        {
            _baseAnimationController.SetWalkSpeedLobby(0.5f);
        }else if (race == CharacterRaceAnimation.FMagic | race == CharacterRaceAnimation.MMagic)
        {
            _baseAnimationController.SetWalkSpeedLobby(0.5f);
        }
        else if (race == CharacterRaceAnimation.FElf )
        {
            _baseAnimationController.SetWalkSpeedLobby(0.6f);
        }
        else if (race == CharacterRaceAnimation.MElf | race == CharacterRaceAnimation.MDarkElf)
        {
            _baseAnimationController.SetWalkSpeedLobby(0.5f);
        }
        else if (race == CharacterRaceAnimation.MDwarf)
        {
            _baseAnimationController.SetWalkSpeedLobby(0.35f);
        }
    }

    private void StartWalking() {
        _walking = true;
        IAnimationManager animations = Animations;
        if (animations != null)
            animations.PlayLobbyLocomotion(
                _baseAnimationController, AnimationNames.WALK.ToString() + WeaponAnim);
    }

    private void StopWalking() {
        _walking = false;
        IAnimationManager animations = Animations;
        if (animations != null)
            animations.PlayLobbyLocomotion(
                _baseAnimationController, AnimationNames.WAIT.ToString() + WeaponAnim);
    }

    bool _celebrateOnArrive;

    /// <summary>Selected pawn: Interlude raises an arm (Social_victory) once it reaches the Start slot.</summary>
    public void SetDestination(Logongrp destination, bool celebrateOnArrive) {
        SetDestination(destination);
        _celebrateOnArrive = celebrateOnArrive;
    }

    void PlayVictory() {
        if (_baseAnimationController == null || _characterInfoInterlude == null)
            return;
        CharacterRaceAnimation raceAnim = _characterInfoInterlude.CharacterRaceAnimation;
        string race = raceAnim.ToString();
        string folder = "Data/Animations/" + CharacterRaceParser.ParseRace(raceAnim) + "/" + race + "/Clips/";
        string[] names = {
            race + "_m000_b.ao_Social_victory_" + race,
            race + "_M000_b.ao_Social_victory_" + race,
            race + "_m000_b.ao_Social_Victory_" + race,
            race + "_M000_b.ao_Social_Victory_" + race,
        };
        for (int i = 0; i < names.Length; i++) {
            AnimationClip clip = Resources.Load<AnimationClip>(folder + names[i]);
            if (clip != null) {
                _baseAnimationController.TryPlayOneShot(clip);
                return;
            }
        }
        Debug.Log("[LobbyVictory] no Social_victory clip for " + race + " in " + folder);
    }

    public void SetDestination(Logongrp destination) {
        _celebrateOnArrive = false;
        Vector3 pawnPosition = new Vector3(destination.X, destination.Y, destination.Z);
        _destination = VectorUtils.ConvertPosToUnity(pawnPosition);
        _destEulerAngles = new Vector3(0, 360.00f * destination.Yaw / 65536, 0);
        _hasDestYaw = true;
    }
}
