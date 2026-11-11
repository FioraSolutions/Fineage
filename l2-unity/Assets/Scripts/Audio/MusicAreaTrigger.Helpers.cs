using FMODUnity;
using System.Collections;
using UnityEngine;
using System;
using FMOD.Studio;

public partial class MusicAreaTrigger : MonoBehaviour
{
    private void OnTriggerExit(Collider other) {
        // Debug.Log("Exit area:" + transform.name);
        MusicManager music = Music;
        if (music == null)
            return;
        music.ResetPriority();
        music.StopMusic(_enterEvent);
        if (_loopCoroutine != null) {
            StopCoroutine(_loopCoroutine);
        }
    }

    private void PlayMusic() {
        MusicManager music = Music;
        if (music == null)
            return;
        music.PlayMusic(_enterEvent, _areaPrority);
        if (_loopCoroutine != null) {
            StopCoroutine(_loopCoroutine);
        }
        _loopCoroutine = StartCoroutine(MusicLoop());
    }

    IEnumerator MusicLoop() {
        yield return new WaitForSeconds(_musicLengthSeconds);
        yield return new WaitForSeconds(_loopDelaySeconds);

        PlayMusic();
    }

    private void OnTriggerStay(Collider other) {
        MusicManager music = Music;
        if (music == null)
            return;
        if(music.CurrentEventPriority > _areaPrority) {
            music.StopMusic(_enterEvent);
        } else if(music.CurrentMusicEvent.Guid != _enterEvent.Guid) {
            music.PlayMusic(_enterEvent, _areaPrority);
        }
    }
}
