using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class SelectableCharacterEntity : MonoBehaviour
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




    //Movement speed to the target without taking into account animation speed



    public bool IsWalking => _walking;

    /// <summary>Lobby click on a standing selected pawn: Social_victory once. Ignored while walking.</summary>


}
