using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using static UnityEngine.GraphicsBuffer;

public partial class PlayerActions : MonoBehaviour
{
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

        WireActionPacketServices();
        ShortcutPacketServices.Bind(actions: this);

        _actions = new Dictionary<ActionType, L2Action>();
        AttackAction attack = new AttackAction();
        attack.Bind(_targets);
        _actions.Add(ActionType.Attack, attack);
        _actions.Add(ActionType.Sit, new SitStandAction());
        _actions.Add(ActionType.WalkRun, new WalkRunAction());
        NextTargetAction nextTarget = new NextTargetAction();
        nextTarget.Bind(_targets);
        _actions.Add(ActionType.NextTarget, nextTarget);
        _actions.Add(ActionType.Mount, new RideStriderAction());
        RegisterSocial(
            ActionType.SocialGreeting, ActionType.SocialVictory, ActionType.SocialAdvance,
            ActionType.SocialYes, ActionType.SocialNo, ActionType.SocialBow,
            ActionType.SocialUnaware, ActionType.SocialWaiting, ActionType.SocialLaugh,
            ActionType.SocialApplaud, ActionType.SocialDance, ActionType.SocialSorrow,
            ActionType.SocialCharm, ActionType.SocialShyness);
        RegisterStub(
            ActionType.Trade, ActionType.Pickup, ActionType.Assist,
            ActionType.PartyInvite, ActionType.PartyLeave, ActionType.PartyKick,
            ActionType.PartyMatching, ActionType.PartyLeader,
            ActionType.PrivateStoreSell, ActionType.PrivateStoreBuy,
            ActionType.DwarvenManufacture, ActionType.Recommend);
    }

    void RegisterSocial(params ActionType[] types)
    {
        for (int i = 0; i < types.Length; i++)
        {
            ActionType t = types[i];
            if (!_actions.ContainsKey(t))
                _actions.Add(t, new SocialActionUse((int)t));
        }
    }

    void RegisterStub(params ActionType[] types)
    {
        for (int i = 0; i < types.Length; i++)
        {
            ActionType t = types[i];
            if (!_actions.ContainsKey(t))
                _actions.Add(t, new StubAction((int)t));
        }
    }

    private void Start()
    {
        WireActionPacketServices();
    }

    private void WireActionPacketServices()
    {
        ActionPacketServices.Bind(input: _input, targets: _targets);
    }
}
