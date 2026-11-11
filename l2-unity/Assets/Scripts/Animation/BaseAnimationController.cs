using System;
using System.Collections.Generic;
using UnityEngine;

public partial class BaseAnimationController : AnimationEventsBase, IAnimationController
{
    [SerializeField] protected Animator _animator;
    [SerializeField] protected bool _resetStateOnReceive = false;
    [SerializeField] protected float _spAtk01ClipLength = 1000;
    [SerializeField] protected Dictionary<string, float> _atkClipLengths;
    private string _lastAnimationVariableName;
    private float _lastAtkClipLength;
    private float _pAtkSpd;

    private readonly Dictionary<string, string> _base_motion = new Dictionary<string, string>(3);

    private const string BASE_MOTION_CAST_MID = "FDarkElf_m001_b.ao_CastMid_FDarkElf";
    private const string BASE_MOTION_END = "FDarkElf_m001_b.ao_CastEnd_FDarkElf";
    private const string BASE_MOTION_MAGIC_SHOOT = "FDarkElf_m001_b.ao_MagicShot_A_FDarkElf";

    private AnimatorOverrideController _overrideController;
    private CastClipPlayable _castPlayable;
    private bool _castIdleSkip;

    // Дубли OnAnimationShoot на одном кадре (два ивента на клипе / два слоя) — в подписчики уходит один раз.
    private int _lastDuplicateShootFrame = int.MinValue;
    private string _lastDuplicateShootAnim;



}
