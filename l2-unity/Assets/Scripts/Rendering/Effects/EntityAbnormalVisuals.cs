using System.Collections.Generic;
using UnityEngine;

public sealed partial class EntityAbnormalVisuals : MonoBehaviour
{
    const string SleepPrefabPath = "Data/Effects/su_empower_v1069/su_sleep_ta/su_sleep_ta";
    const string RootPrefabPath = "Data/Effects/m_u013_a_v1201/sp_dryad_root_ta/sp_dryad_root_ta";
    // Mobius abnormalVisualEffect=DOT_POISON — looping green poison aura while POISON slot is live.
    const string PoisonPrefabPath = "Data/Effects/curse_poison/bl_curse_poison_ta/bl_curse_poison_ta";

    Entity _entity;
    AbnormalPoseKind _pose;
    AbnormalAveFlags _flags;

    GameObject _sleepFx;
    GameObject _rootFx;
    GameObject _poisonFx;

    bool _petrified;
    Animator _petrifyAnimator;
    float _speedBeforePetrify = 1f;
    readonly List<Color> _rendererColors = new();
    readonly List<Renderer> _tintedRenderers = new();


    void OnDestroy() => ClearAll();
}
