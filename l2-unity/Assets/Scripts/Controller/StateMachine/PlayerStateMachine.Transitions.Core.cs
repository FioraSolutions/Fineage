using System;

public partial class PlayerStateMachine : MonoBehaviour
{
    private void InitializeState()
    {
        _stateInstance = GetOrAddState(_dictState, _currentState);
    }

    private void InitializeIntention()
    {
        _intentionInstance = GetOrAddIntention(_dictIntention, _currentIntention);
    }

    private StateBase GetOrAddState(Dictionary<PlayerState, StateBase> dict, PlayerState key)
    {

        if (!dict.ContainsKey(key))
        {
            var state = CreatorStatePlayer.GetState(key, this);
            dict.Add(key, state);
        }


        return dict[key];
    }

    private IntentionBase GetOrAddIntention(Dictionary<Intention, IntentionBase> dict, Intention key)
    {

        if (!dict.ContainsKey(key))
        {
            var state = CreateIntentionPlayer.GetIntention(key, this);
            dict.Add(key, state);
        }


        return dict[key];
    }
}
