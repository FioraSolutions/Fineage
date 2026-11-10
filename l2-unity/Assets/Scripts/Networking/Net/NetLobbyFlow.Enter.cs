using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetLobbyFlow
{
    public async Task EnterZoneWithHandoff(HandoffMsg handoff)
    {
        _handoff = handoff;
        _setCharacterId(handoff != null ? handoff.CharacterId : 0);
        _resetSessionObjectId();
        try
        {
            _lobby?.Dispose();
            _lobby = null;
            await _connectZoneForHandoff(handoff);
        }
        catch (Exception ex)
        {
            Debug.LogError("[Net] EnterZone failed: " + ex.Message);
        }
    }

    static async Task WaitUntil(Func<bool> pred, float seconds)
    {
        float start = Time.realtimeSinceStartup;
        while (!pred())
        {
            if (Time.realtimeSinceStartup - start > seconds)
                throw new TimeoutException("net wait");
            await Task.Delay(20);
        }
    }

}
