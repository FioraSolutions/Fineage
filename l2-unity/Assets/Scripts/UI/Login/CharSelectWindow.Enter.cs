using System.Collections;
using UnityEngine;

public partial class CharSelectWindow
{
    public bool IsEnteringWorld => _enterWorld.IsEntering;

    public void NotifyEnterWorldSucceeded()
    {
        _enterWorld.Succeed();
    }

    public void NotifyEnterWorldFailed(string message)
    {
        if (!_enterWorld.IsEntering)
            return;
        _enterWorld.Fail();
        NetSession.TryGetBound()?.AbortZoneEnter();
        RefreshLobbyActions();
        string line = string.IsNullOrWhiteSpace(message)
            ? "Could not enter the world. Try again."
            : message.Trim();
        ShowLobbyStatus(line);
    }

    void BeginEnterWorldAttempt()
    {
        StopEnterWorldTimeout();
        _enterTimeout = StartCoroutine(EnterWorldTimeout());
    }

    void StopEnterWorldTimeout()
    {
        if (_enterTimeout == null)
            return;
        StopCoroutine(_enterTimeout);
        _enterTimeout = null;
    }

    IEnumerator EnterWorldTimeout()
    {
        float start = Time.realtimeSinceStartup;
        while (_enterWorld.IsEntering)
        {
            if (_enterWorld.HasTimedOut(Time.realtimeSinceStartup - start))
            {
                NotifyEnterWorldFailed("Could not enter the world. Try again.");
                _enterTimeout = null;
                yield break;
            }
            yield return null;
        }
        _enterTimeout = null;
    }
}
