using System;
using UnityEngine;
using System.Collections.Generic;

public partial class PlayerStateMachine : MonoBehaviour
{
    private void Update()
    {
        _stateInstance?.Update();
        _intentionInstance?.Update();
    }

    public void ChangeState(PlayerState newState)
    {
        PlayerState prev = _currentState;
        //if (_enableLogs) Debug.Log($"[StateMachine][STATE] {prev} → {newState}");
        if (CombatTrace.Enabled && prev != newState)
            CombatTrace.Write("anim.state", "who=local from=" + prev + " to=" + newState + " intention=" + _currentIntention);

        _stateInstance?.Exit();
        _currentState = newState;
        InitializeState();
        _stateInstance?.Enter();
    }

    public void ChangeIntention(Intention intention)
    {
        ChangeIntention(intention, null);
    }

    public void ChangeIntention(Intention newIntention, object arg0)
    {
        Intention prev = _currentIntention;
        //if (_enableLogs) Debug.Log($"[Player->StateMachine][INTENTION] {prev} → {newIntention}");
        if (CombatTrace.Enabled)
            CombatTrace.Write("intent", "who=local from=" + prev + " to=" + newIntention + " state=" + _currentState);
        _intentionInstance?.Exit();
        _currentIntention = newIntention;
        InitializeIntention();
        _intentionInstance?.Enter(arg0);
    }
}
