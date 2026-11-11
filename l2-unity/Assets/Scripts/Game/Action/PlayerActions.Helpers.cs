using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using static UnityEngine.GraphicsBuffer;

public partial class PlayerActions : MonoBehaviour
{
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
        InputManager input = _input != null ? _input : ActionPacketServices.Input;
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
        if (!ZoneCommands.IsConnected)
            return;
        ZoneCommands.Send(new RequestMagicSkillUseCommand(skillId, GameCtrl.AsInt, 0));
    }
}
