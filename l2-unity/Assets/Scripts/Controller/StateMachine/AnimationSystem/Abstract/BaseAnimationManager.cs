using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public abstract partial class BaseAnimationManager
{
    protected Dictionary<int, AnimationModel> _animationControllers;
    protected Dictionary<int, string[]> _recentAnimationNames;
    protected Dictionary<int, string[]> _recentMonsterAnimationNames;
    protected Dictionary<int, TaskCompletionSource<bool>> _tcsMap;




}
