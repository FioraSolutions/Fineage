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
        _actions.Add(ActionType.SocialGreeting, new SocialActionUse((int)ActionType.SocialGreeting));
        _actions.Add(ActionType.SocialVictory, new SocialActionUse((int)ActionType.SocialVictory));
        _actions.Add(ActionType.SocialAdvance, new SocialActionUse((int)ActionType.SocialAdvance));
        // Icons-only for now — wire packets/anims in a follow-up.
        RegisterStub(
            ActionType.Trade, ActionType.Pickup, ActionType.Assist,
            ActionType.PartyInvite, ActionType.PartyLeave, ActionType.PartyKick,
            ActionType.PrivateStoreSell, ActionType.PartyMatching,
            ActionType.SocialYes, ActionType.SocialNo, ActionType.SocialBow,
            ActionType.SocialUnaware, ActionType.SocialWaiting, ActionType.SocialLaugh,
            ActionType.SocialApplaud, ActionType.SocialDance, ActionType.SocialSorrow,
            ActionType.SocialCharm, ActionType.SocialShyness,
            ActionType.PrivateStoreBuy, ActionType.DwarvenManufacture, ActionType.Recommend);
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
