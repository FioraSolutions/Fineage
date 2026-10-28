using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class DeadManager : MonoBehaviour, IDead
{
    public event Action<int> OnReadyToRemove;

    private static IDead _instance;
    public static IDead Instance { get { return _instance; } }

    [Inject] private L2ActorFade _actorFade;

    private L2ActorFade ActorFade
    {
        get { return _actorFade != null ? _actorFade : ManagerInteractionPacketServices.ActorFade; }
    }

    private Dictionary<int, DeadData> _dict;
    private List<int> _remove = new List<int>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _dict = new Dictionary<int, DeadData>();
            _remove = new List<int>();
            App.InjectGameObject(gameObject);
            WireManagerInteractionPacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireManagerInteractionPacketServices();
    }

    void Update()
    {
        if (_dict == null || _dict.Count == 0)
        {
            return;
        }

        foreach (KeyValuePair<int, DeadData> kvp in _dict)
        {
            DeadData data = kvp.Value;
            if (data == null || !data.IsValid())
            {
                _remove.Add(kvp.Key);
                continue;
            }

            L2ActorFade fade = ActorFade;
            if (fade == null)
            {
                _remove.Add(kvp.Key);
                continue;
            }

            data.AddElapsed(Time.deltaTime);
            if (data.Elapsed > L2ActorFade.DurationSeconds)
            {
                Finish(data);
                _remove.Add(kvp.Key);
                continue;
            }

            data.SetAlphaByte(fade, fade.AlphaByte(data.Elapsed));
        }

        Remove(_remove);
    }

    public void AddDeadAndRemove(int id, DeadData data)
    {
        if (data == null || _dict.ContainsKey(id))
        {
            return;
        }

        L2ActorFade fade = ActorFade;
        if (fade == null || !data.TryBeginFade(fade))
        {
            OnReadyToRemove?.Invoke(id);
            return;
        }

        if (NameplatesManager.Instance != null)
        {
            NameplatesManager.Instance.Remove(data.GetIdEntity());
        }

        data.SetAlphaByte(fade, 255);
        _dict.Add(id, data);
    }

    private void Finish(DeadData data)
    {
        int id = data.GetIdEntity();
        if (id != 0)
        {
            OnReadyToRemove?.Invoke(id);
        }
    }

    private void Remove(List<int> remove)
    {
        for (int i = 0; i < remove.Count; i++)
        {
            _dict.Remove(remove[i]);
        }

        remove.Clear();
    }

    private void WireManagerInteractionPacketServices()
    {
        ManagerInteractionPacketServices.Bind(actorFade: _actorFade);
    }
}
