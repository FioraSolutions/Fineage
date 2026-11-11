using System;
using UnityEngine;

public abstract partial class AnimationEventsBase : MonoBehaviour
{
    public event Action<string> OnAnimationFinished;
    public event Action<string> OnAnimationStartShoot;
    public event Action<string> OnAnimationStartHit;
    /// <summary>L2 AnimNotify_AttackShot — melee Hit/SoulShot only.</summary>
    public event Action<string> OnAnimationAttackShot;
    public event Action<string> OnAnimationFinishedHit;
    public event Action<string> OnAnimationStartLoadArrow;

    // Клипы иногда содержат несколько одинаковых Animation Events на одном кадре.
    private int _lastDuplicateCompleteFrame = int.MinValue;
    private string _lastDuplicateCompleteAnim;

    /// <summary>
    /// Unity Animation Event on clip end. Raw notify — no priority lock / queue.
    /// Phase await is owned by SMB (<see cref="AnimationManager.NotifyMagicPhaseFinished"/>);
    /// this remains a fallback for subscribers.
    /// </summary>

    /// <summary>
    /// Вызывается из Unity Animation Event на клипе. Базовая реализация только диспатчит подписчикам;
    /// у игрока/монстров переопределено в <see cref="BaseAnimationController"/> (лог + дедуп дублей).
    /// </summary>


    /// <summary>
    /// Unity Animation Event on melee clips (L2 AnimNotify_AttackShot). Function name must be AttackShot.
    /// </summary>

    /// <summary>Legacy clip event — not used for melee Hit/SoulShot.</summary>


}
