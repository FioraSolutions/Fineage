using System;

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

public partial class AnimationManager : BaseAnimationManager , IAnimationManager
{
   
    private static AnimationManager _instance;
    private readonly HashSet<int> _awaitSubscribedObjectIds = new HashSet<int>();
    private readonly Dictionary<int, string> _expectedFinishNameByObjectId = new Dictionary<int, string>();
    private const string SP_TIME_ATK = "sptimeatk";
    private const string CAST_TRIGGER_MID = "CastMid";
    private const string CAST_TRIGGER_END = "CastEnd";
    private const string CAST_TRIGGER_SHOT = "MagicShot";
    private const string CAST_TRIGGER_END_2P = "CastEnd2P";
    private const string CAST_TRIGGER_SHOT_2P = "MagicShot2P";
    private const string SHOT_SOURCE_RUNNER = "Runner";
    private const int OVERRIDE_AWAIT_WARN_TIMEOUT_MS = 5000;

    [System.Obsolete("Use [Inject] IAnimationManager or AnimationPacketServices.Animations instead")]
    public static IAnimationManager Instance
    {
        get
        {
            if (_instance == null && App.HasContainer)
            {
                try
                {
                    _instance = App.Resolve<AnimationManager>();
                }
                catch
                {
                }
            }

            if (_instance == null)
                _instance = new AnimationManager();
            return _instance;
        }
    }

    //Async Wait End Event
}
