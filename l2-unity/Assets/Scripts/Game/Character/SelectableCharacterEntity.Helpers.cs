using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class SelectableCharacterEntity : MonoBehaviour
{
    void SetWalkSpeedModel()
    {
        if (_baseAnimationController == null)
            return;
        _baseAnimationController.SetWalkSpeedLobby(CharacterAnimPaths.LobbyWalkSpeed);
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

    public bool TryPlayVictory() {
        if (_walking)
            return false;
        PlayVictory();
        return true;
    }

    void PlayVictory() {
        if (_baseAnimationController == null || _characterInfoInterlude == null)
            return;
        CharacterRaceAnimation raceAnim = _characterInfoInterlude.CharacterRaceAnimation;
        string race = raceAnim.ToString();
        string folder = CharacterAnimPaths.ClipsFolder(raceAnim);
        string[] names = {
            race + "_m000_b.ao_Social_victory_" + race,
            race + "_M000_b.ao_Social_victory_" + race,
            race + "_m000_b.ao_Social_Victory_" + race,
            race + "_M000_b.ao_Social_Victory_" + race,
        };
        for (int i = 0; i < names.Length; i++) {
            AnimationClip clip = Resources.Load<AnimationClip>(folder + names[i]);
            if (clip != null) {
                // Gesture channel: CastClipPlayable one-shot at AnimPlayback.GestureRate.
                _baseAnimationController.TryPlayOneShot(clip);
                return;
            }
        }
        Debug.Log("[LobbyVictory] no Social_victory clip for " + race + " in " + folder);
    }

    public void SetDestination(Logongrp destination) {
        Vector3 pawnPosition = new Vector3(destination.X, destination.Y, destination.Z);
        _destination = VectorUtils.ConvertPosToUnity(pawnPosition);
        _destEulerAngles = new Vector3(0, 360.00f * destination.Yaw / 65536, 0);
        _hasDestYaw = true;
    }
}
