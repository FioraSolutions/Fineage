using System.Collections.Generic;
using UnityEngine;
using VContainer;
using System.Collections;

public partial class SelectableCharacterEntity : MonoBehaviour
{
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
            }
            if (_hasDestYaw)
                transform.rotation = Quaternion.RotateTowards(
                transform.rotation, Quaternion.Euler(_destEulerAngles), LobbyTurnDegPerSec * Time.deltaTime);
        }
    }

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
}
