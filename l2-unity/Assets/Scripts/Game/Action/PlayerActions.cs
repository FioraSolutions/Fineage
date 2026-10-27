using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using static UnityEngine.GraphicsBuffer;

public class PlayerActions : MonoBehaviour
{
    [Inject] GameClient _gameClient;
    [Inject] InputManager _input;
    [Inject] TargetManager _targets;
    private Dictionary<ActionType, L2Action> _actions;
    
    private static PlayerActions _instance;
    public static PlayerActions Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }

        _actions = new Dictionary<ActionType, L2Action>();
        AttackAction attack = new AttackAction();
        attack.Bind(_targets, _gameClient);
        _actions.Add(ActionType.Attack, attack);
        _actions.Add(ActionType.Sit, new SitStandAction());
        _actions.Add(ActionType.WalkRun, new WalkRunAction());
        NextTargetAction nextTarget = new NextTargetAction();
        nextTarget.Bind(_targets);
        _actions.Add(ActionType.NextTarget, nextTarget);
        _actions.Add(ActionType.RideStrider, new RideStriderAction());
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    private void Update()
    {
        ListenToKeybindedActions();
    }

    private void ListenToKeybindedActions()
    {
        InputManager input = _input != null ? _input : IncomingPacketActions.Input;
        if (input == null)
            return;

        if (input.Attack)
            UseAction(ActionType.Attack);

        if (input.NextTarget)
            UseAction(ActionType.NextTarget);
    }

    public void UseAction(ActionType actionType)
    {
        if (_actions.TryGetValue(actionType, out L2Action action))
        {
            action.UseAction();
        }
        else
        {
            // Debug.LogWarning("Action not found.");
        }
    }

    public void UseSkill(int skillId)
    {
        GameClient game = _gameClient != null ? _gameClient : IncomingPacketActions.Game;
        if (game == null)
            return;
        game.Send(new RequestMagicSkillUseCommand(skillId, 0, 0));
    }
}