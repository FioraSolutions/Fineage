using System;

public partial class MusicManager : MonoBehaviour
{
    public void PlayMusic(EventReference musicEvent, int priority) {
        if(priority < _currentEventPriority) {
            return;
        }

        _currentEventPriority = priority;
        _currentMusicEvent = musicEvent;

        if (_musicInstances.ContainsKey(musicEvent)) {
            _musicInstances[musicEvent].start();
        } else {
            EventInstance musicInstance = RuntimeManager.CreateInstance(musicEvent);
            _musicInstances.Add(musicEvent, musicInstance);
            musicInstance.start();
        }

        //Debug.Log("Playing music " + musicEvent + " with priority " + priority);
    }

    public void StopMusic(EventReference musicEvent) {
        if(_musicInstances != null && _musicInstances.ContainsKey(musicEvent)) {
            _musicInstances[musicEvent].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }             
    }

    public void ResetPriority() {
        _currentEventPriority = -1;
    }
}
