using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;
using System.Collections.Concurrent;

public sealed partial class NetSession : MonoBehaviour
{
    static bool IsCriticalZoneOpcode(ushort opcode) =>
        opcode == NetOpcode.EntityDespawn
        || opcode == NetOpcode.InventoryUpdate
        || opcode == NetOpcode.ItemList
        || opcode == NetOpcode.EntityDie
        || opcode == NetOpcode.DropItem
        || opcode == NetOpcode.Revive
        || opcode == NetOpcode.TeleportToLocation;

    uint NextSeq() => _seq++;

    void Post(Action action) => _mainThread.Post(action);

    void PostCritical(Action action) => _mainThread.PostCritical(action);

    static async System.Threading.Tasks.Task WaitUntil(Func<bool> pred, float seconds)
    {
        float start = Time.realtimeSinceStartup;
        while (!pred())
        {
            if (Time.realtimeSinceStartup - start > seconds)
                throw new TimeoutException("net wait");
            await System.Threading.Tasks.Task.Delay(20);
        }
    }

    void OnDestroy()
    {
        _lobbyFlow?.Dispose();
        _zone?.Dispose();
        if (Instance == this)
            Instance = null;
    }

    public void BindDomainSink(IWorldDomainSink sink) => _sink = sink;

}
