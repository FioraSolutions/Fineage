using System;

public partial class L2GameUI : L2UI
{
    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    protected override void Update()
    {
        base.Update();

        InputManager input = ControllerPacketServices.Input;
        if (input != null && input.TurnCamera)
        {
            DisableMouse();
        }
        else
        {
            EnableMouse();
        }

        CheckUIInputs();
    }

    static void EnsurePartyMemberListWindow()
    {
        if (PartyPacketServices.MemberList != null || Instance == null)
            return;
        Instance.gameObject.AddComponent<PartyMemberListWindow>();
    }
}
