using System;

public partial class MusicManager : MonoBehaviour
{
    [SerializeField] private int _currentEventPriority = -1;
    [SerializeField] private EventReference _currentMusicEvent;
    [SerializeField] private Dictionary<EventReference, EventInstance> _musicInstances;
    public int CurrentEventPriority { get { return _currentEventPriority; } }
    public EventReference CurrentMusicEvent { get { return _currentMusicEvent; } }

    private static MusicManager _instance;

    [System.Obsolete("Use MusicPacketServices.Music or [Inject] MusicManager instead")]
    public static MusicManager Instance { get { return _instance; } }






}
