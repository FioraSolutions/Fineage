using UnityEngine;

public partial class NewIdleState : StateBase
{
    private const float CombatSwingNormGate = 0.95f;

    /// <summary>
    /// Payload from JAtk/SpAtk/Magic SMB SwitchToIdle. Wall clock may end before animNorm hits
    /// CombatSwingNormGate — still apply atkwait/wait (do not treat as early interrupt).
    /// </summary>
    public const string WaitReturnFromCombatSmb = "smb_finished";

}
