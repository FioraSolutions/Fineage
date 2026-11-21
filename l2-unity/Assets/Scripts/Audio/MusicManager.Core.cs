using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public partial class MusicManager : MonoBehaviour
{
    AudioService _service;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            EnsureService();
            MusicPacketServices.Bind(music: this);
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(this);
            return;
        }
    }

    internal void BindService(AudioService service) => _service = service;

    void EnsureService()
    {
        if (_service == null)
            _service = GetComponent<AudioService>();
        if (_service == null)
            _service = gameObject.AddComponent<AudioService>();
    }

    AudioService Svc
    {
        get
        {
            if (_service == null)
                EnsureService();
            return _service;
        }
    }

    void OnDestroy()
    {
        if (_instance == this)
            Clear();
    }

    public void Clear() => Svc.ClearAllMusic();
}