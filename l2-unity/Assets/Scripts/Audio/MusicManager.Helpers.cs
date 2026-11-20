using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public partial class MusicManager : MonoBehaviour
{
    public void PlayMusic(EventReference musicEvent, int priority) =>
        Svc.PlayAreaMusic(musicEvent, priority);

    public void StopMusic(EventReference musicEvent) =>
        Svc.StopAreaMusic(musicEvent);

    public void ResetPriority() => Svc.ResetMusicPriority();
}