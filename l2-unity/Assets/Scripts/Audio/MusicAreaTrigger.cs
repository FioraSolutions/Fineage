using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class MusicAreaTrigger : MonoBehaviour
{
    [SerializeField] private int _areaPrority;
    [SerializeField] private EventReference _enterEvent;
    [SerializeField] private float _musicLengthSeconds = 0;
    [SerializeField] private float _loopDelaySeconds = 5;

    private EventDescription _eventDescription;
    private Coroutine _loopCoroutine;

    private static MusicManager Music => MusicPacketServices.Music;

    private void Awake() {
        if (!_eventDescription.isValid()) {
            Lookup();
        }

        int lengthMs = 0;
        _eventDescription.getLength(out lengthMs);
        _musicLengthSeconds = lengthMs / 1000f;
    }

    private void Lookup() {
        _eventDescription = RuntimeManager.GetEventDescription(_enterEvent);
    }

    private void OnTriggerEnter(Collider other) {
        // Debug.Log("Enter area:" + transform.name);
        PlayMusic();
    }

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
