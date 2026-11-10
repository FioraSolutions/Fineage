using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interlude AVE driven from PartySpelled / AbnormalStatusUpdate.
/// Pose follows Mobius <c>abnormalVisualEffect</c>:
/// Sleep/Stun → EntityAction Idle (wait/atkwait), Anchor <c>FLESH_STONE</c> → petrify.
/// </summary>
[DisallowMultipleComponent]
public sealed class EntityAbnormalVisuals : MonoBehaviour
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

    void Awake()
    {
        _entity = GetComponent<Entity>() ?? GetComponentInParent<Entity>();
    }

    void LateUpdate()
    {
        if (_entity == null || _entity.IsDead())
        {
            ClearAll();
            return;
        }

        AbnormalAveFlags flags = AbnormalVisualCatalog.ResolveFlags(_entity.TargetEffects);
        AbnormalPoseKind pose = AbnormalVisualCatalog.ResolvePose(flags);

        if (pose != _pose)
            ApplyPose(pose);
        if (flags != _flags)
            SyncFx(flags);

        _pose = pose;
        _flags = flags;
    }

    void ApplyPose(AbnormalPoseKind pose)
    {
        ClearPetrify();

        switch (pose)
        {
            case AbnormalPoseKind.Idle:
                // PlayStandWait → atkwait in combat so NPC weapons stay in the combat idle pose.
                var actions = EntityActionPacketServices.Actions;
                if (actions != null)
                    actions.Set(_entity, EntityActionKind.Idle, null);
                break;
            case AbnormalPoseKind.Petrify:
                ApplyPetrify();
                break;
            case AbnormalPoseKind.None:
            default:
                break;
        }
    }

    void ApplyPetrify()
    {
        if (_petrified)
            return;

        EntityActionVisual.CancelMove(_entity);

        Animator animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            _petrifyAnimator = animator;
            _speedBeforePetrify = animator.speed > 0.0001f ? animator.speed : 1f;
            // FLESH_STONE: Interlude freezes the skeleton on the current frame (Anchor / Medusa).
            animator.speed = 0f;
        }

        TintStone(true);
        _petrified = true;
    }

    void ClearPetrify()
    {
        if (!_petrified)
            return;
        if (_petrifyAnimator != null)
            _petrifyAnimator.speed = _speedBeforePetrify > 0.0001f ? _speedBeforePetrify : 1f;
        _petrifyAnimator = null;
        _speedBeforePetrify = 1f;
        TintStone(false);
        _petrified = false;
    }

    void TintStone(bool enable)
    {
        if (enable)
        {
            if (_tintedRenderers.Count > 0)
                return;
            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer r = renderers[i];
                if (r == null || r is ParticleSystemRenderer)
                    continue;
                // Skip skill-effect layer FX parented under the pawn.
                if (r.gameObject.layer == L2FxCompositorLayers.SkillEffect)
                    continue;
                _tintedRenderers.Add(r);
                _rendererColors.Add(r.material != null ? r.material.color : Color.white);
                if (r.material != null)
                    r.material.color = new Color(0.55f, 0.55f, 0.58f, 1f);
            }
            return;
        }

        for (int i = 0; i < _tintedRenderers.Count; i++)
        {
            Renderer r = _tintedRenderers[i];
            if (r != null && r.material != null && i < _rendererColors.Count)
                r.material.color = _rendererColors[i];
        }
        _tintedRenderers.Clear();
        _rendererColors.Clear();
    }

    void SyncFx(AbnormalAveFlags flags)
    {
        bool wantSleep = (flags & AbnormalAveFlags.Sleep) != 0;
        bool wantRoot = (flags & AbnormalAveFlags.Root) != 0;
        bool wantPoison = (flags & AbnormalAveFlags.DotPoison) != 0;

        if (wantSleep)
            EnsureFx(ref _sleepFx, SleepPrefabPath, "AVE_Sleep", ResolveHeadBone(), new Vector3(0f, 0.15f, 0f), new Vector3(0f, 2.2f, 0f));
        else
            ClearFx(ref _sleepFx);

        if (wantRoot)
            EnsureFx(ref _rootFx, RootPrefabPath, "AVE_Root", transform, Vector3.zero, Vector3.zero);
        else
            ClearFx(ref _rootFx);

        if (wantPoison)
            EnsureFx(ref _poisonFx, PoisonPrefabPath, "AVE_Poison", transform, new Vector3(0f, 0.4f, 0f), new Vector3(0f, 0.4f, 0f));
        else
            ClearFx(ref _poisonFx);
    }

    void EnsureFx(ref GameObject fx, string path, string name, Transform parent, Vector3 localIfParent, Vector3 localIfRoot)
    {
        if (fx != null)
            return;
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
            return;
        bool hasParent = parent != null && parent != transform;
        fx = Instantiate(prefab, hasParent ? parent : transform);
        fx.name = name;
        fx.transform.localPosition = hasParent ? localIfParent : localIfRoot;
        fx.transform.localRotation = Quaternion.identity;
        L2FxCompositorLayers.ApplySkillEffectLayer(fx);
    }

    static void ClearFx(ref GameObject fx)
    {
        if (fx == null)
            return;
        Destroy(fx);
        fx = null;
    }

    Transform ResolveHeadBone()
    {
        var anim = GetComponentInChildren<Animator>();
        if (anim == null)
            return null;
        // Monsters / broken controllers often have no humanoid Avatar — GetBoneTransform throws.
        if (anim.isHuman && anim.avatar != null)
        {
            Transform head = anim.GetBoneTransform(HumanBodyBones.Head);
            if (head != null)
                return head;
        }

        Transform best = null;
        int bestScore = int.MinValue;
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            string name = t.name;
            if (name.IndexOf("head", System.StringComparison.OrdinalIgnoreCase) < 0)
                continue;
            int score = 0;
            if (name.IndexOf("nub", System.StringComparison.OrdinalIgnoreCase) >= 0)
                score -= 10;
            if (name.Equals("Bip01 head", System.StringComparison.OrdinalIgnoreCase) ||
                name.Equals("Head", System.StringComparison.OrdinalIgnoreCase))
                score += 20;
            if (name.IndexOf("Bip01", System.StringComparison.OrdinalIgnoreCase) >= 0)
                score += 5;
            if (score > bestScore)
            {
                bestScore = score;
                best = t;
            }
        }
        return best;
    }

    void ClearAll()
    {
        ClearPetrify();
        ClearFx(ref _sleepFx);
        ClearFx(ref _rootFx);
        ClearFx(ref _poisonFx);
        _pose = AbnormalPoseKind.None;
        _flags = AbnormalAveFlags.None;
    }

    void OnDestroy() => ClearAll();
}
