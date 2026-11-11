using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using VContainer;

public partial class SkillExecutor : MonoBehaviour
{
    [Inject] IAnimationManager _animations;
    [Inject] EffectManager _effects;
    private SkillAnimationRunner _animRunner;

    private IAnimationManager Animations =>
        _animations != null ? _animations : ServicePacketServices.Animations;

    private EffectManager Effects =>
        _effects != null ? _effects : ServicePacketServices.Effects;

    public event Action OnSkillSequenceFinished;
    public event Action<AnimationEventsBase> OnAllAnimationFinished;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static SkillExecutor Instance { get; private set; }









}
