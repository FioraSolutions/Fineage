using System;

using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class DeadManager : MonoBehaviour, IDead
{
    private void Awake()
    {
        if ((_instance as UnityEngine.Object) == null)
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

        NameplatesManager nameplates = NameplatePacketServices.Nameplates;
        if (nameplates != null)
        {
            nameplates.Remove(data.GetIdEntity());
        }

        data.SetAlphaByte(fade, 255);
        _dict.Add(id, data);
    }
}
