using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using VContainer;

public class SkillExecutor : MonoBehaviour
{
    [Inject] IAnimationManager _animations;
    [Inject] EffectManager _effects;
    private SkillAnimationRunner _animRunner;

    private IAnimationManager Animations
    {
        get { return _animations != null ? _animations : IncomingPacketActions.Animations; }
    }

    private EffectManager Effects
    {
        get { return _effects != null ? _effects : IncomingPacketActions.Effects; }
    }

    public event Action OnSkillSequenceFinished;
    public event Action<AnimationEventsBase> OnAllAnimationFinished;
    public static SkillExecutor Instance { get; private set; }


    public SkillExecutor()
    {
        _animRunner = new SkillAnimationRunner();

    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public async Task ExecuteSkill(Entity entity , AnimationCombo animationCombo , AnimationEventsBase actions)
    {
        if (entity == null || animationCombo == null) return;
        int objectId = entity.Identity.Id;


        // _emitter.SetupActions(actions);

        string[] cycle = animationCombo.GetAnimCycle();
        _animRunner.StartRun(cycle, objectId, Animations, () => OnAllAnimationFinish(actions));
    }

    public async Task ExecuteSkillOverride(Skillgrp skill, Entity entity, AnimationCombo animationCombo, AnimationEventsBase actions, bool isLong = false)
    {
        if (entity == null || animationCombo == null) return;
        int objectId = entity.Identity.Id;

        EffectManager effects = Effects;
        if (effects != null)
            effects.PlayEffect(skill.Id, entity.transform, entity.GetMagicCastData());

        string[] cycle = animationCombo.GetAnimCycle();
        SkillAnimationRunner.Trace = skill != null && skill.Id == 2046;
        if (SkillAnimationRunner.Trace)
        {
            Debug.Log(
                "[SUMMON_ANIM] ExecuteOverride objectId=" + objectId +
                " anim=" + skill.Animation +
                " isMagic=" + skill.IsMagic +
                " hitGrp=" + skill.HitTime +
                " cycle=" + (cycle == null ? "null" : string.Join(",", cycle)) +
                " long=" + isLong +
                " runner=" + (_animRunner != null) +
                " animations=" + (Animations != null));
        }
        if (isLong)
        {
            _animRunner.StartRunLongOverride(cycle, objectId, Animations, () => OnAllAnimationFinish(actions));
        }
        else
        {
            _animRunner.StartRunOverride(cycle, objectId, Animations, () => OnAllAnimationFinish(actions));
        }
    }


    private void OnAllAnimationFinish(AnimationEventsBase actions)
    {
        OnAllAnimationFinished?.Invoke(actions);
    }


}