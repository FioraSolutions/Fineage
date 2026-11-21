using System.Collections.Generic;
using UnityEngine;
using System;

public sealed partial class EntityAbnormalVisuals : MonoBehaviour
{
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
        bool wantStun = (flags & AbnormalAveFlags.Stun) != 0;

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

        // Interlude AVE_STUN — three stars circling the head (Bluff, Shield Stun, Hammer Crush …).
        if (wantStun)
            EnsureStunStars();
        else
            ClearStunStars();

    }

    void SyncStealth(AbnormalAveFlags flags)
    {
        if ((flags & AbnormalAveFlags.Stealth) != 0)
            ApplyStealth();
        else
            ClearStealth();
    }

    /// <summary>Official Interlude AVE_STEALTH: the pawn becomes semi-invisible, including the local hero.</summary>
    void ApplyStealth()
    {
        if (_stealthed)
            return;
        if (_entity == null)
        {
            Debug.LogError("[AVE_STEALTH] Cannot apply STEALTH visual: Entity is null on this pawn.");
            return;
        }

        if (!_stealthFade.TryBegin(_entity, StealthAlphaByte, out _stealthRenderers, out _stealthInstances, out _stealthShared))
            return;

        _stealthFade.SetAlphaByte(_stealthInstances, StealthAlphaByte);
        _stealthed = true;
    }

    void ClearStealth()
    {
        if (!_stealthed)
            return;
        _stealthFade.Restore(_stealthRenderers, _stealthShared, _stealthInstances);
        _stealthRenderers = null;
        _stealthInstances = null;
        _stealthShared = null;
        _stealthed = false;
    }

    void EnsureStunStars()
    {
        if (_stunFx != null)
            return;

        Transform head = ResolveHeadBone();
        // Head bone sits at the skull's base; lift the ring above the crown so the stars read over the head.
        _stunFx = EntityStunStars.Attach(head != null ? head : transform, new Vector3(0f, 0.25f, 0f));
    }

    void ClearStunStars()
    {
        if (_stunFx == null)
            return;
        Destroy(_stunFx.gameObject);
        _stunFx = null;
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
        ClearStunStars();
        ClearStealth();
        _pose = AbnormalPoseKind.None;
        _flags = AbnormalAveFlags.None;
    }
}
