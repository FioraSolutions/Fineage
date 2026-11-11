using System;
using UnityEngine;

public sealed partial class UserSpawner
{
    private readonly CharacterBuilder _characterBuilder;
    private readonly Func<UserEntity, UserBowArrowEvents> _createBowArrowEvents;
    private readonly AppearFadeService _appearFade;
    private readonly L2PawnYaw _pawnYaw;

}
