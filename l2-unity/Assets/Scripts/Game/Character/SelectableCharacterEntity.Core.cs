using System;
using UnityEngine;
using VContainer;

public partial class SelectableCharacterEntity : MonoBehaviour
{
    private void Awake() {
        _characterController = GetComponent<CharacterController>();
        _baseAnimationController = GetComponent<BaseAnimationController>();
        _destination = transform.position;
        if (_characterController != null)
            _characterController.enabled = false;
        InjectSharedMotion();
        WireCharacterPacketServices();
    }

    void InjectSharedMotion()
    {
        if (_animations != null)
            return;

        IObjectResolver lobby = HierarchyScopeInjection.Lobby;
        if (lobby == null)
        {
            throw new InvalidOperationException(
                "[LobbyMotion] lobby container missing — LoginLifetimeScope did not build, so the lobby pawn was not injected.");
        }

        HierarchyScopeInjection.Inject(lobby, gameObject);
        if (_animations == null)
        {
            throw new InvalidOperationException(
                "[LobbyMotion] IAnimationManager missing — LoginLifetimeScope did not register AnimationManager, so the lobby pawn has no motion pipeline.");
        }
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
            }
            if (_hasDestYaw)
                transform.rotation = Quaternion.RotateTowards(
                transform.rotation, Quaternion.Euler(_destEulerAngles), LobbyTurnDegPerSec * Time.deltaTime);
        }
    }

    private float GetWalkSpeedPoint(CharSelectInfoPackage info)
    {
        if (info == null)
            return LocomotionProfileResolver.LobbyWalkSpeed(CharacterRaceAnimation.FFighter);
        return LocomotionProfileResolver.LobbyWalkSpeed(info.CharacterRaceAnimation);
    }
}
