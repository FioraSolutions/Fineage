
using UnityEngine;

public sealed partial class UserWorldApply : EntityWorldApply
{
    private readonly EntityActionMachine _actions;

    private World GameWorld =>
        WorldApplyPacketServices.GameWorld ?? GamePacketServices.GameWorld;

}
