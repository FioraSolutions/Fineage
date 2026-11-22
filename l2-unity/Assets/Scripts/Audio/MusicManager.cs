using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public partial class MusicManager : MonoBehaviour, IMusicPlayback
{
    public int CurrentEventPriority => Svc.CurrentMusicPriority;
    public EventReference CurrentMusicEvent => Svc.CurrentMusicEvent;

    static MusicManager _instance;

    [System.Obsolete("Use MusicPacketServices.Music or [Inject] MusicManager instead")]
    public static MusicManager Instance => _instance;
}