using System;

using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class DeadManager : MonoBehaviour, IDead
{
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

    void OnDestroy()
    {
        if (ReferenceEquals(_instance, this))
            _instance = null;
    }
}
