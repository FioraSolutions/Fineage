
using UnityEngine;

public sealed partial class PlayerWorldApply : EntityWorldApply
{
    private SkillbarWindow Skillbar =>
        SkillbarPacketServices.Skillbar ?? SkillbarPacketServices.Skillbar;

    private EffectManager Effects => GlobalEffectsPacketServices.Effects;

    private PlayerController Player =>
        WorldApplyPacketServices.Player ?? ControllerPacketServices.Player;

    private EventBus Bus =>
        WorldApplyPacketServices.Bus ?? GamePacketServices.Bus;











    /// <summary>
    /// When standing idle, re-fire WAIT_RETURN so NewIdleState CrossFades wait vs atkwait
    /// from the updated <see cref="PlayerEntity.isAutoAttack"/> flag (NPC parity via
    /// EntityActionMachine.RefreshStandWaitIfIdle). Skips cast/swing/locomotion.
    /// </summary>


}
