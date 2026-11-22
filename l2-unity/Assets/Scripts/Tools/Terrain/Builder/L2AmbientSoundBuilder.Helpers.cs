#if UNITY_EDITOR
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public partial class L2AmbientSoundBuilder : MonoBehaviour
{
    public static GameObject BuildAmbientSoundsFromT3d(string t3dAbsolutePath) {
        AmbientSound[] sounds = L2T3DInfoParser.ParseAmbientSounds(t3dAbsolutePath).ToArray();
        return BuildAmbientSounds(sounds);
    }

    private static GameObject BuildAmbientSounds(AmbientSound[] sounds) {
        GameObject container = new GameObject("AmbientSounds");

        foreach (AmbientSound ambientSound in sounds) {
            // Debug.Log(ambientSound);

            GameObject soundObject = BuildAmbientSoundGameObject(ambientSound);
            soundObject.transform.parent = container.transform;
        }

        return container;
    }

    private static GameObject BuildAmbientSoundGameObject(AmbientSound ambientSound) {

        GameObject go = new GameObject(ambientSound.name);

        // collider
        SphereCollider collider = go.AddComponent<SphereCollider>();
        float soundRadius = ambientSound.soundRadius / 52.5f * 14f;
        collider.radius = soundRadius;
        collider.isTrigger = true;

        go.transform.position = VectorUtils.ConvertPosToUnity(ambientSound.position);
        go.layer = LayerMask.NameToLayer("AmbientSound");

        // event — editor cache only. RuntimeManager.PathToGUID needs a live
        // studio system, which batchmode bake does not initialize.
        AmbientSoundEmitter emitter = go.AddComponent<AmbientSoundEmitter>();
        string actor = string.IsNullOrEmpty(ambientSound.name) ? go.name : ambientSound.name;
        string raw = ambientSound.ambientSoundName;
        if (string.IsNullOrEmpty(raw)) {
            Debug.LogError("[FmodEvent] ambient sound name empty actor=" + actor);
        } else {
            string[] parts = raw.Split('.');
            if (parts.Length < 3 || string.IsNullOrEmpty(parts[0]) || string.IsNullOrEmpty(parts[2])) {
                Debug.LogError(
                    "[FmodEvent] ambient sound name is not Package.Group.Name name=\""
                    + raw + "\" actor=" + actor);
            } else {
                string eventName = $"event:/AmbSound/{parts[0]}/{parts[2]}";
                emitter.EventReference = L2FmodEditorEventResolver.Resolve(eventName, actor);
            }
        }
        emitter.OverrideAttenuation = true;
        emitter.OverrideMinDistance = soundRadius * 0.05f;
        emitter.OverrideMaxDistance = soundRadius;
        emitter.Loop = true;
        emitter.PlayEvent = EmitterGameEvent.TriggerEnter;
        emitter.StopEvent = EmitterGameEvent.TriggerExit;
        emitter.CollisionTag = "Player";
        emitter.AllowFadeout = true;
        if (ambientSound.ambientRandom == 0) {
            emitter.PlayChancePercent = 100;
            emitter.LoopDelaySeconds = 0;
        } else {
            emitter.PlayChancePercent = ambientSound.ambientRandom;
            emitter.LoopDelaySeconds = 3;
        }

        // from UE pitch to FMOD
        float uePitch = ambientSound.soundPitch;
        float fmodPitch = uePitch / 100f + 1f;
        emitter.SoundPitch = fmodPitch;

        AmbientSoundType ambientSoundType = (AmbientSoundType)Enum.Parse(typeof(AmbientSoundType), ambientSound.ambientSoundType);
        emitter.AmbientSoundType = ambientSoundType;
        emitter.enabled = true;

        return go;
    }
}
#endif
