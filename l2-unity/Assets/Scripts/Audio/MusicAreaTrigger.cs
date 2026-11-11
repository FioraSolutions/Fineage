using FMODUnity;
using System.Collections;
using UnityEngine;
using FMOD.Studio;

public partial class MusicAreaTrigger : MonoBehaviour
{
    [SerializeField] private int _areaPrority;
    [SerializeField] private EventReference _enterEvent;
    [SerializeField] private float _musicLengthSeconds = 0;
    [SerializeField] private float _loopDelaySeconds = 5;

    private EventDescription _eventDescription;
    private Coroutine _loopCoroutine;

    private static MusicManager Music => MusicPacketServices.Music;







}
