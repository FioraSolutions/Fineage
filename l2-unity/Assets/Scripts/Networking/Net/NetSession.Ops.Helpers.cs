using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;
using System.Collections.Concurrent;

public sealed partial class NetSession : MonoBehaviour
{
    uint NextSeq() => _seq++;

    void Post(Action action, ushort opcode = 0, object coalesceBody = null) =>
        _mainThread.Post(action, opcode, coalesceBody);

    void PostCritical(Action action, ushort opcode = 0) => _mainThread.PostCritical(action, opcode);

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
        BumpZoneEpoch("session-destroy");
        _lobbyFlow?.Dispose();
        _zone?.Dispose();
        if (Instance == this)
            Instance = null;
    }

    public void BindDomainSink(IWorldDomainSink sink) => _sink = sink;

}
