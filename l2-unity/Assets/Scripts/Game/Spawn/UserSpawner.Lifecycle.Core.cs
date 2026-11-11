using System;
using UnityEngine;

public sealed partial class UserSpawner
{
    public UserSpawner(
        CharacterBuilder characterBuilder,
        Func<UserEntity, UserBowArrowEvents> createBowArrowEvents,
        AppearFadeService appearFade,
        L2PawnYaw pawnYaw)
    {
        _characterBuilder = characterBuilder;
        _createBowArrowEvents = createBowArrowEvents;
        _appearFade = appearFade;
        _pawnYaw = pawnYaw;
    }
}
