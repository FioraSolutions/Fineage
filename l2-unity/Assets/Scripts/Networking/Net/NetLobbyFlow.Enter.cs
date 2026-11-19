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
            // Keep the login socket until the world spawn lands so a zone refusal
            // or restart can re-enable Start and let the player retry.
            await _connectZoneForHandoff(handoff);
        }
        catch (Exception ex)
        {
            Debug.LogError("[Net] EnterZone failed: " + ex.Message);
            string line = string.IsNullOrWhiteSpace(ex.Message)
                ? "Could not enter the world. Try again."
                : ex.Message;
            _post(() => CharSelectEnterWorld.Failed(line));
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
