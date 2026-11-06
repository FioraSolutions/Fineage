using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EntitySpawnShared
{
    /// <summary>npcIds already reported as unresolvable, so a repeated NpcInfo does not spam logs.</summary>
    static readonly HashSet<int> ReportedUnresolvedNpcIds = new HashSet<int>();
    const int MaxUnresolvedReports = 20;
    static bool _unresolvedReportsSuppressed;

    public static bool TryResolveNpc(
        EntityIdentity identity,
        NpcgrpTable npcGrps,
        NpcNameTable npcNames,
        ModelTable models,
        out NpcSpawnRequest request)
    {
        request = null;
        if (identity == null)
            return false;

        npcGrps = npcGrps != null ? npcGrps : NpcgrpTable.Instance;
        npcNames = npcNames != null ? npcNames : NpcNameTable.Instance;
        models = models != null ? models : ModelTable.Instance;

        int visualNpcId = identity.NpcId;
        if (npcGrps.GetNpcgrp(visualNpcId) == null)
            visualNpcId = PetVisuals.VisualNpcId(identity.NpcId);

        Npcgrp npcgrp = npcGrps.GetNpcgrp(visualNpcId);
        NpcName npcName = npcNames.GetNpcName(identity.NpcId);
        if (npcName == null)
            npcName = npcNames.GetNpcName(visualNpcId);
        if (npcName == null || npcgrp == null)
        {
            ReportUnresolved(
                identity.NpcId,
                "no entry in Npcgrp_Classic.txt / NpcName_Classic-eu.txt");
            return false;
        }

        GameObject prefab = models.GetNpc(npcgrp.Mesh);
        string usedMesh = npcgrp.Mesh;
        if (prefab == null)
        {
            bool resolved = IsMonsterMesh(npcgrp.Mesh)
                ? TryResolveFallbackMonsterPrefab(models, npcgrp.Mesh, out prefab, out usedMesh)
                : TryResolveFallbackNpcPrefab(models, npcgrp.Mesh, out prefab, out usedMesh);
            if (!resolved)
            {
                ReportUnresolved(
                    identity.NpcId,
                    "mesh '" + npcgrp.Mesh + "' has no prefab at Resources/Data/Animations/"
                    + npcgrp.Mesh.Replace('.', '/')
                    + " and no fallback mesh resolved");
                return false;
            }
            ReportSubstituted(identity.NpcId, npcgrp.Mesh, usedMesh);
        }

        identity.EntityType = ResolveEntityType(prefab, npcgrp.ClassName);
        if (identity.NpcId == 31760)
        {
            // Debug.Log("SpawnNpc>>> Spawn 31760 p5");
            identity.EntityType = EntityType.NPC;
        }

        request = new NpcSpawnRequest
        {
            Identity = identity,
            Npcgrp = npcgrp,
            NpcName = npcName,
            Prefab = prefab
        };
        return true;
    }

    /// <summary>
    /// One line per npcId when the client cannot build a spawn request. Without this the NPC
    /// silently disappears, which looks identical to the server never sending NpcInfo.
    /// </summary>
    static void ReportUnresolved(int npcId, string reason)
    {
        if (!ReportedUnresolvedNpcIds.Add(npcId))
            return;

        if (_unresolvedReportsSuppressed)
            return;
        if (ReportedUnresolvedNpcIds.Count > MaxUnresolvedReports)
        {
            _unresolvedReportsSuppressed = true;
            FineageLog.Warn(
                FineageLog.Category.World,
                "NpcInfo unresolved reports capped at " + MaxUnresolvedReports
                + " distinct npcIds; further ids are not logged.");
            return;
        }

        FineageLog.Warn(
            FineageLog.Category.World,
            "NpcInfo npcId=" + npcId + " not rendered: " + reason);
    }

    /// <summary>
    /// Reported once per substituted mesh so a missing asset pack variant is visible in the
    /// log instead of being silently papered over.
    /// </summary>
    static void ReportSubstituted(int npcId, string wanted, string used)
    {
        string key = wanted + " -> " + used;
        if (!ReportedSubstitutedMeshes.Add(key))
            return;
        FineageLog.Warn(
            FineageLog.Category.World,
            "NpcInfo npcId=" + npcId + " mesh '" + wanted + "' missing; rendering '"
            + used + "' instead");
    }

    /// <summary>
    /// The client ships only part of the Interlude NPC art. Elven Village alone asks for
    /// <c>a_teleporter_FElf_m00</c> and <c>a_guard_MElf_m00</c>, which have no prefab, so the
    /// gatekeeper and the newbie guide were dropped on spawn while the server kept them alive.
    /// Rather than hide the NPC, substitute an existing rig in this order: same outfit on a
    /// shipped race, then the same race's generic townsfolk body.
    /// </summary>
    static bool TryResolveFallbackNpcPrefab(
        ModelTable models,
        string wantedMesh,
        out GameObject prefab,
        out string usedMesh)
    {
        prefab = null;
        usedMesh = null;
        if (models == null || string.IsNullOrEmpty(wantedMesh))
            return false;

        int dot = wantedMesh.IndexOf('.');
        if (dot <= 0 || dot == wantedMesh.Length - 1)
            return false;

        string package = wantedMesh.Substring(0, dot);
        string asset = wantedMesh.Substring(dot + 1);

        for (int i = 0; i < FallbackRaceNames.Length; i++)
        {
            string candidate = asset;
            for (int r = 0; r < MissingRaceNames.Length; r++)
                candidate = candidate.Replace(MissingRaceNames[r], FallbackRaceNames[i]);
            if (candidate == asset)
                continue;
            prefab = models.GetNpc(package + "." + candidate);
            if (prefab != null)
            {
                usedMesh = package + "." + candidate;
                return true;
            }
        }

        // Last resort: the race's generic townsfolk body keeps the NPC visible and clickable.
        bool male = asset.IndexOf('M') >= 0 && asset.Contains("_M");
        string[] generic =
        {
            package + ".a_common_people_" + (male ? "MElf" : "FElf") + "_m00",
            package + ".a_common_people_" + (male ? "MHuman" : "FHuman") + "_m00",
            package + ".a_common_peopleA_" + (male ? "MHuman" : "FHuman") + "_m00"
        };
        for (int i = 0; i < generic.Length; i++)
        {
            prefab = models.GetNpc(generic[i]);
            if (prefab != null)
            {
                usedMesh = generic[i];
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Monster rigs live in <c>LineageMonsters*</c>; townsfolk rigs live in <c>LineageNPCs*</c>. The
    /// package prefix is what decides which stand-in table applies, so this stays a pure string test
    /// and works even when no prefab resolved at all.
    /// </summary>
    static bool IsMonsterMesh(string mesh) =>
        !string.IsNullOrEmpty(mesh) &&
        mesh.StartsWith("LineageMonster", System.StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Monster rigs the art pack does not ship. Elven Village is the loud case: Dryads, Lireins and
    /// Spore Fungus have no prefab, so the server kept them alive while the client dropped every row
    /// from the knownlist ("not all the mobs around Elven Village"). Substituting a shipped rig by
    /// silhouette keeps the mob visible, targetable and killable; the alternative — hiding it — is
    /// what made the area look empty.
    /// </summary>
    static bool TryResolveFallbackMonsterPrefab(
        ModelTable models,
        string wantedMesh,
        out GameObject prefab,
        out string usedMesh)
    {
        prefab = null;
        usedMesh = null;
        if (models == null || string.IsNullOrEmpty(wantedMesh))
            return false;

        int dot = wantedMesh.IndexOf('.');
        if (dot <= 0 || dot == wantedMesh.Length - 1)
            return false;

        string package = wantedMesh.Substring(0, dot);
        string asset = wantedMesh.Substring(dot + 1);

        // Exact rig first, so a mesh added to the art pack later wins over the stand-in.
        prefab = models.GetNpc(package + "." + asset);
        if (prefab != null)
        {
            usedMesh = package + "." + asset;
            return true;
        }

        string[] standIns = MonsterStandInMeshes(asset);
        for (int i = 0; i < standIns.Length; i++)
        {
            prefab = models.GetNpc("LineageMonsters." + standIns[i]);
            if (prefab != null)
            {
                usedMesh = "LineageMonsters." + standIns[i];
                return true;
            }
            prefab = models.GetNpc("LineageMonsters2." + standIns[i]);
            if (prefab != null)
            {
                usedMesh = "LineageMonsters2." + standIns[i];
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Shipped rigs per silhouette family, most specific first. Both arrays are ordered, so the
    /// stand-in stays stable across runs and shows up once in the substitution log.
    /// </summary>
    static string[] MonsterStandInMeshes(string asset)
    {
        // Tree spirit / dryad family: humanoid trunk with a hollow torso.
        if (Has(asset, "dryad") || Has(asset, "ti_mi_tran") || Has(asset, "ti_mi_riran") || Has(asset, "treant"))
            return new[] { "stone_golem_m00", "orc_fighter_m00", "goblin_m00" };

        // Fungus / slime family: short, wide, no weapon.
        if (Has(asset, "fungus") || Has(asset, "slime") || Has(asset, "mushroom"))
            return new[] { "poison_toad_m00", "elpy_m00", "goblin_m00" };

        // Winged spirit family (Lirein / Pixy / Sylph): tall, hovering, thin.
        if (Has(asset, "sylph") || Has(asset, "pixy") || Has(asset, "fairy") || Has(asset, "spirit"))
            return new[] { "hatchling_m00", "wyvern_m00", "elpy_m00" };

        // Biped orc / goblinoid brute.
        if (Has(asset, "orc") || Has(asset, "goblin") || Has(asset, "bugbear") || Has(asset, "ogre"))
            return new[] { "orc_m00", "orc_fighter_m00", "goblin_m00" };

        // Arachnid.
        if (Has(asset, "spider") || Has(asset, "aracne"))
            return new[] { "giant_spider_m00" };

        // Canine / feline quadruped.
        if (Has(asset, "wolf") || Has(asset, "keltir") || Has(asset, "fox") || Has(asset, "cat"))
            return new[] { "wolf_m00", "fox_m00", "keltir_m00" };

        // Undead.
        if (Has(asset, "zombie") || Has(asset, "skeleton") || Has(asset, "ghoul") || Has(asset, "vampire"))
            return new[] { "werewolf_bi_m00", "werewolf_m00" };

        // Anything else still needs a body; a generic humanoid brute is the least surprising.
        return new[] { "goblin_m00", "orc_m00" };
    }

    static bool Has(string text, string needle) =>
        text.IndexOf(needle, System.StringComparison.OrdinalIgnoreCase) >= 0;

    /// <summary>Race tags used inside LineageNPCs mesh names that the client art pack omits.</summary>
    static readonly string[] MissingRaceNames = { "_FElf_", "_MElf_", "_FDarkElf_", "_MDarkElf_" };

    /// <summary>Race tags the client art pack actually ships for NPC outfits, in preference order.</summary>
    static readonly string[] FallbackRaceNames =
    {
        "_FHuman_", "_MHuman_", "_FDwarf_", "_MDwarf_", "_FFighter_", "_MFighter_"
    };

    static readonly HashSet<string> ReportedSubstitutedMeshes = new HashSet<string>();

    public static EntityType ResolveEntityType(GameObject prefab, string className)
    {
        bool hasMonster = FindOnPrefab<MonsterEntity>(prefab) != null;
        bool hasNpc = FindOnPrefab<NpcEntity>(prefab) != null;
        if (hasMonster && !hasNpc)
            return EntityType.Monster;
        if (hasNpc && !hasMonster)
            return EntityType.NPC;
        return EntityTypeParser.ParseEntityType(className);
    }

    public static T FindOnPrefab<T>(GameObject go) where T : Component
    {
        if (go == null)
            return null;
        T component = go.GetComponent<T>();
        return component != null ? component : go.GetComponentInChildren<T>(true);
    }

    public static GameObject AcquireNpcGameObject(
        ObjectPoolManager pool,
        GameObject prefab,
        EntityIdentity identity)
    {
        if (prefab == null)
            return null;

        ObjectType? poolType = null;
        if (identity.EntityType == EntityType.NPC)
            poolType = ObjectType.Npc;
        else if (identity.EntityType == EntityType.Monster)
            poolType = ObjectType.Monster;

        Vector3 serverPos = identity.Position;
        Vector3 grounded = GroundSnapHelper.SnapToGroundOrKeep(serverPos);
        identity.Position = grounded;
        if (poolType.HasValue && pool != null)
        {
            ObjectType tag = poolType.Value;
            pool.AddPrefabToPool(tag, prefab, 1);
            GameObject pooled = pool.SpawnFromPool(tag, prefab);
            if (pooled != null)
            {
                ApplyGroundedTransform(pooled, grounded, identity.Heading);
                return pooled;
            }
        }

        bool prefabWasActive = prefab.activeSelf;
        if (prefabWasActive)
            prefab.SetActive(false);

        GameObject npcGo = Object.Instantiate(prefab, grounded, identity.Heading);
        ApplyGroundedTransform(npcGo, grounded, identity.Heading);

        if (prefabWasActive)
            prefab.SetActive(true);

        return npcGo;
    }

    public static void Place(GameObject go, Transform parent, Vector3 position, Quaternion rotation, string name)
    {
        if (go == null)
            return;
        if (parent != null)
            go.transform.SetParent(parent);
        Vector3 grounded = GroundSnapHelper.SnapToGroundOrKeep(position);
        ApplyGroundedTransform(go, grounded, rotation);
        if (!string.IsNullOrEmpty(name))
            go.transform.name = name;
    }

    public static void ReapplyGroundAfterActivate(GameObject go, EntityIdentity identity)
    {
        if (go == null || identity == null)
            return;

        TryApplyGroundSnap(go, identity);

        MonoBehaviour host = go.GetComponent<Entity>();
        if (host == null)
            host = WorldApplyPacketServices.GameWorld;
        if (host != null)
            host.StartCoroutine(RetryGroundSnap(go, identity));
    }

    static IEnumerator RetryGroundSnap(GameObject go, EntityIdentity identity)
    {
        float[] at = { 0.25f, 1f, 2f };
        float elapsed = 0f;
        for (int i = 0; i < at.Length; i++)
        {
            yield return new WaitForSeconds(at[i] - elapsed);
            elapsed = at[i];
            if (go == null || identity == null)
                yield break;

            if (TryApplyGroundSnap(go, identity))
                yield break;
        }
    }

    static bool TryApplyGroundSnap(GameObject go, EntityIdentity identity)
    {
        if (PetSummonRise.IsRising(go))
            return false;

        Vector3 from = go.transform.position;
        if (!GroundSnapHelper.TrySnapToGround(from, out Vector3 snapped))
            return false;

        identity.Position = snapped;
        ApplyGroundedTransform(go, snapped, identity.Heading);
        return true;
    }

    public static void ApplyGroundedTransform(GameObject go, Vector3 position, Quaternion rotation)
    {
        if (go == null)
            return;

        CharacterController[] controllers = go.GetComponentsInChildren<CharacterController>(true);
        bool[] wasEnabled = new bool[controllers.Length];
        for (int i = 0; i < controllers.Length; i++)
        {
            if (controllers[i] == null)
                continue;
            wasEnabled[i] = controllers[i].enabled;
            controllers[i].enabled = false;
        }

        go.transform.SetPositionAndRotation(position, rotation);

        for (int i = 0; i < controllers.Length; i++)
        {
            if (controllers[i] == null)
                continue;
            controllers[i].enabled = wasEnabled[i];
        }
    }

    public static void SanitizeCharacterControllerStepOffset(GameObject root)
    {
        if (root == null)
            return;

        CharacterController[] controllers = root.GetComponentsInChildren<CharacterController>(true);
        for (int i = 0; i < controllers.Length; i++)
        {
            CharacterController cc = controllers[i];
            if (cc == null)
                continue;
            if (cc.radius <= 0f)
                cc.radius = 0.1f;
            if (cc.height < cc.radius * 2f)
                cc.height = cc.radius * 2f;
            // Unity: stepOffset <= height + 2*radius; keep well inside to avoid the warning on scaled rigs.
            float maxStep = Mathf.Max(0f, Mathf.Min(cc.height * 0.5f, cc.height + cc.radius * 2f - 0.01f));
            if (cc.stepOffset > maxStep)
                cc.stepOffset = maxStep;
        }
    }

    /// <summary>
    /// Leftover client-prediction: SetNewPosition is never called, so it lerps back to spawn.
    /// </summary>
    public static void DisableLegacyPositionSync(GameObject root)
    {
        if (root == null)
            return;

        NetworkTransformReceive receive = root.GetComponent<NetworkTransformReceive>();
        if (receive != null)
            receive.enabled = false;
    }

    public static void ApplyNpcIdentity(Entity npc, NpcSpawnRequest request)
    {
        EntityIdentity identity = request.Identity;
        Npcgrp npcgrp = request.Npcgrp;
        NpcName npcName = request.NpcName;

        Appearance appearance = new Appearance();
        appearance.RHand = npcgrp.Rhand;
        appearance.LHand = npcgrp.Lhand;
        appearance.CollisionRadius = npcgrp.CollisionRadius;
        appearance.CollisionHeight = npcgrp.CollisionHeight;

        string packetName = identity.Name;
        string packetTitle = identity.Title;
        npc.Status = request.Status;
        npc.Stats = request.Stats;
        npc.Identity = identity;
        npc.Identity.NpcClass = npcgrp.ClassName;
        bool pet = PetVisuals.IsPet(identity);
        if (!pet)
        {
            npc.Identity.Name = npcName.Name;
            npc.Identity.Title = npcName.Title;
        }
        else
        {
            npc.Identity.Name = string.IsNullOrEmpty(packetName) ? npcName.Name : packetName;
            npc.Identity.Title = string.IsNullOrEmpty(packetTitle) ? npcName.Title : packetTitle;
        }
        if ((npc.Identity.Title == null || npc.Identity.Title.Length == 0) &&
            identity.EntityType == EntityType.Monster)
        {
            npc.Identity.Title = " Lvl: " + npc.Stats.Level;
        }

        npc.Identity.TitleColor = npcName.TitleColor;
        npc.Appearance = appearance;
        npc.SetDead(false);
        SeedNpcHpFromGrp(npc, npcgrp);
    }

    public static void ResetNpcRuntimeForSpawn(Entity npc)
    {
        if (npc == null)
            return;

        npc.Running = npc.Identity != null && npc.Identity.IsRunning;
        npc.InCombat = false;
        npc.IsSoulshotCharged = false;
        npc.TargetId = 0;
        npc.Target = null;
        npc.LastTarget = null;
        npc.AttackTarget = null;
        npc.ActionSlot.Clear();
    }

    public static void InitializeNpcAnimation(NetworkAnimationController controller)
    {
        if (controller == null)
            return;

        Animator animator = controller.GetAnimator();
        if (animator != null)
        {
            animator.enabled = true;
            animator.speed = 1f;
        }

        controller.Initialize();
        controller.SetAnimatorSpeed(1f);
        controller.SetPAtkSpeed(1f);

        animator = controller.GetAnimator();
        if (animator == null)
            return;

        int waitHash = Animator.StringToHash("wait");
        if (animator.HasState(0, waitHash))
        {
            animator.Play(waitHash, 0, 0f);
            animator.Update(0f);
        }

        CrowdAnimTest.Apply(controller);
    }

    public static void LogStriderAppear(string when, Entity entity, string detail = null)
    {
        if (entity == null || entity.Identity == null)
            return;
        int npcId = entity.Identity.NpcId;
        if (npcId != 12526 && npcId != 12527 && npcId != 12528)
            return;

        Animator animator = entity.GetComponentInChildren<Animator>(true);
        int waitHash = Animator.StringToHash("wait");
        bool hasWait = animator != null && animator.HasState(0, waitHash);
        bool isWait = animator != null && animator.GetCurrentAnimatorStateInfo(0).shortNameHash == waitHash;
        string clip = "(none)";
        string controller = "(none)";
        string speed = "-";
        string enabled = "-";
        if (animator != null)
        {
            controller = animator.runtimeAnimatorController != null
                ? animator.runtimeAnimatorController.name
                : "(no controller)";
            speed = animator.speed.ToString("F3");
            enabled = animator.enabled.ToString() + " patkspd=" + animator.GetFloat("patkspd").ToString("F3");
            AnimatorClipInfo[] infos = animator.GetCurrentAnimatorClipInfo(0);
            if (infos.Length > 0 && infos[0].clip != null)
                clip = infos[0].clip.name + " w=" + infos[0].weight.ToString("F2");
        }

        Debug.Log(
            "[STRIDER_WAIT] when=" + when +
            " npc=" + npcId +
            " controller=" + controller +
            " enabled=" + enabled +
            " speed=" + speed +
            " hasWait=" + hasWait +
            " stateIsWait=" + isWait +
            " clip=" + clip +
            (string.IsNullOrEmpty(detail) ? "" : " " + detail));
    }

    public static bool IsStriderNpc(Entity entity)
    {
        if (entity == null || entity.Identity == null)
            return false;
        int npcId = entity.Identity.NpcId;
        return npcId == 12526 || npcId == 12527 || npcId == 12528;
    }

    public static void LogStriderGait(string when, Entity entity, string detail = null)
    {
        if (!IsStriderNpc(entity))
            return;

        Animator animator = entity.GetComponentInChildren<Animator>(true);
        string state = "-";
        string clip = "(none)";
        if (animator != null)
        {
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName("run"))
                state = "run";
            else if (info.IsName("walk"))
                state = "walk";
            else if (info.IsName("wait"))
                state = "wait";
            else if (info.IsName("atk01"))
                state = "atk01";
            else if (info.IsName("atkwait"))
                state = "atkwait";
            else
                state = "other";
            AnimatorClipInfo[] infos = animator.GetCurrentAnimatorClipInfo(0);
            if (infos.Length > 0 && infos[0].clip != null)
                clip = infos[0].clip.name;
        }

        bool moving = MoveAllCharacters.Instance != null &&
            MoveAllCharacters.Instance.IsMoving(entity.Identity.Id);
        string action = entity.ActionSlot != null ? entity.ActionSlot.Action.ToString() : "-";
        Debug.Log(
            "[STRIDER_GAIT] when=" + when +
            " npc=" + entity.Identity.NpcId +
            " id=" + entity.Identity.Id +
            " active=" + entity.gameObject.activeInHierarchy +
            " action=" + action +
            " moving=" + moving +
            " running=" + entity.Running +
            " inCombat=" + entity.InCombat +
            " state=" + state +
            " clip=" + clip +
            (string.IsNullOrEmpty(detail) ? "" : " " + detail));
    }

    static void SeedNpcHpFromGrp(Entity npc, Npcgrp npcgrp)
    {
        if (npc == null || npcgrp == null || npc.Stats == null || npc.Status == null)
            return;

        if (npc.Identity != null && npc.Identity.Owned && npc.Stats.MaxHp > 1)
            return;

        int grpHp = Mathf.Max(1, Mathf.RoundToInt(npcgrp.MaxHp));
        if (grpHp <= 1)
            return;

        npc.Stats.MaxHp = grpHp;
        npc.Status.SetHp(grpHp);
    }

    public static void BindGearAndAnimation(Entity entity, GameObject go, IAnimationController controller)
    {
        if (go != null)
        {
            Gear gear = go.GetComponent<Gear>();
            if (gear != null)
                gear.Initialize(entity.Identity.Id, entity.RaceId);
        }

        entity.Initialize();
    }

    public static void RegisterAnimation(IWorldSpawnContext world, int objectId, IAnimationController controller, Entity entity)
    {
        if (world != null && world.Animations != null && controller != null)
            world.Animations.RegisterController(objectId, controller, entity);
    }

    public static void ApplyNpcMoveSpeeds(Entity npc, bool useRealPAtk)
    {
        if (useRealPAtk)
            npc.UpdateNpcPAtkSpd((int)npc.Stats.PAtkRealSpeed);
        else
            npc.UpdateNpcPAtkSpd((int)npc.Stats.PAtkSpd);
        npc.UpdateNpcRunningSpd(npc.Stats.RunRealSpeed);
        npc.UpdateNpcWalkSpd(npc.Stats.WalkRealSpeed);
        npc.Running = npc.Identity.IsRunning;
    }
}
