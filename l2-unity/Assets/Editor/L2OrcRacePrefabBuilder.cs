using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Builds Orc / Shaman create-and-spawn prefabs from the FBX and PNG drop in .work.
/// Menu: Fineage / Build Orc Race Prefabs. Batch: -executeMethod L2OrcRacePrefabBuilder.Build
/// </summary>
public static class L2OrcRacePrefabBuilder
{
    const string ShaderPath = "Assets/Resources/Data/Shaders/World/L2ActorModulate2X.shader";
    const string BaseControllerPath = "Assets/Resources/Data/Animations/Animator/_Template/Player_Basic.controller";
    const string PawnTemplate = "Assets/Resources/Data/Animations/Human/MFighter/Pawn_MFighter.prefab";
    const string UserTemplate = "Assets/Resources/Data/Animations/Human/MFighter/User_MFighter.prefab";
    const string PlayerTemplate = "Assets/Resources/Data/Animations/Human/MFighter/Player_MFighter.prefab";
    const float ModelScale = 0.019f;

    static readonly string[] Races = { "MOrc", "FOrc", "MShaman", "FShaman" };
    static readonly string[] Parts = { "u", "l", "g", "b" };

    [MenuItem("Fineage/Build Orc Race Prefabs")]
    public static void Build()
    {
        CopyDropIntoAssets();
        AssetDatabase.Refresh();
        PrepareModels();
        Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(ShaderPath);
        if (shader == null)
            throw new System.InvalidOperationException("Missing shader " + ShaderPath);

        var bodyMats = new Dictionary<string, Material>();
        var faceMats = new Dictionary<string, Material>();
        var hairMats = new Dictionary<string, Material>();
        BuildMaterials(shader, bodyMats, faceMats, hairMats);
        BuildArmorPrefabs(bodyMats);
        BuildFaceAndHairPrefabs(faceMats, hairMats);
        BuildSkeletonsAndContainers();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("L2OrcRacePrefabBuilder finished");
    }

    static string PackageOf(string race)
    {
        return race.Contains("Shaman") ? "Shaman" : "Orc";
    }

    static void CopyDropIntoAssets()
    {
        // Application.dataPath is l2-unity/Assets. Two parents up is the Fineage repo.
        string fineage = Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName;
        string work = Path.Combine(fineage, ".work");
        string texRoot = Path.Combine(work, "umodel-orc");
        string fbxRoot = Path.Combine(work, "orc-fbx");
        if (!Directory.Exists(fbxRoot))
            throw new System.InvalidOperationException("Missing FBX drop " + fbxRoot);

        foreach (string race in Races)
        {
            string package = PackageOf(race);
            string texSrc = Path.Combine(texRoot, race, "Texture");
            string bodyDir = "Assets/Resources/Data/SysTextures/" + race;
            string faceDir = "Assets/Resources/Data/SysTextures/LineageFaceTex";
            Directory.CreateDirectory(Path.Combine(Directory.GetParent(Application.dataPath).FullName, bodyDir));
            if (Directory.Exists(texSrc))
            {
                foreach (string png in Directory.GetFiles(texSrc, "*.png"))
                {
                    string name = Path.GetFileName(png);
                    bool face = name.Contains("_f.png") || name.Contains("_f1.png");
                    string destDir = face ? faceDir : bodyDir;
                    string absDestDir = Path.Combine(Directory.GetParent(Application.dataPath).FullName, destDir);
                    Directory.CreateDirectory(absDestDir);
                    File.Copy(png, Path.Combine(absDestDir, name), true);
                }
            }

            string fbxSrc = Path.Combine(fbxRoot, package);
            string modelDir = Path.Combine(
                Directory.GetParent(Application.dataPath).FullName,
                "Assets/Resources/Data/Animations/" + package + "/Models");
            Directory.CreateDirectory(modelDir);
            if (!Directory.Exists(fbxSrc))
                continue;
            foreach (string fbx in Directory.GetFiles(fbxSrc, "*.fbx"))
            {
                string name = Path.GetFileName(fbx);
                if (name.EndsWith("_anim.fbx") && !name.StartsWith(race))
                    continue;
                if (!name.StartsWith(race) && !name.StartsWith(race + "_anim"))
                    continue;
                File.Copy(fbx, Path.Combine(modelDir, name), true);
            }
        }
    }

    static void PrepareModels()
    {
        foreach (string race in Races)
        {
            string package = PackageOf(race);
            string dir = "Assets/Resources/Data/Animations/" + package + "/Models";
            if (!AssetDatabase.IsValidFolder(dir))
                continue;
            foreach (string guid in AssetDatabase.FindAssets("t:Model", new[] { dir }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.EndsWith(".fbx"))
                    continue;
                var importer = (ModelImporter)UnityEditor.AssetImporter.GetAtPath(path);
                importer.globalScale = ModelScale;
                importer.useFileScale = false;
                importer.animationType = ModelImporterAnimationType.Generic;
                importer.importAnimation = path.EndsWith("_anim.fbx");
                importer.materialImportMode = ModelImporterMaterialImportMode.None;
                importer.SaveAndReimport();
            }
        }
    }

    static void BuildMaterials(
        Shader shader,
        Dictionary<string, Material> bodyMats,
        Dictionary<string, Material> faceMats,
        Dictionary<string, Material> hairMats)
    {
        foreach (string race in Races)
        {
            string bodyDir = "Assets/Resources/Data/SysTextures/" + race;
            foreach (string guid in AssetDatabase.FindAssets("t:Texture2D", new[] { bodyDir }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string name = Path.GetFileNameWithoutExtension(path);
                bool hair = name.Contains("_m00_");
                Material mat = MakeMaterial(shader, path, hair);
                string matPath = bodyDir + "/Materials/" + name + ".mat";
                SaveMaterial(mat, matPath);
                if (hair)
                    hairMats[name] = mat;
                else
                    bodyMats[name] = mat;
            }
        }

        foreach (string guid in AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Resources/Data/SysTextures/LineageFaceTex" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string name = Path.GetFileNameWithoutExtension(path);
            if (!name.StartsWith("MOrc") && !name.StartsWith("FOrc") && !name.StartsWith("MShaman") && !name.StartsWith("FShaman"))
                continue;
            Material mat = MakeMaterial(shader, path, false);
            string matPath = "Assets/Resources/Data/SysTextures/LineageFaceTex/Materials/" + name + ".mat";
            SaveMaterial(mat, matPath);
            faceMats[name] = mat;
        }
    }

    static Material MakeMaterial(Shader shader, string texturePath, bool cutout)
    {
        var mat = new Material(shader);
        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
        mat.SetTexture("_BaseMap", tex);
        mat.SetTexture("_MainTex", tex);
        mat.SetFloat("_Cull", 0f);
        mat.doubleSidedGI = true;
        if (cutout)
        {
            mat.SetFloat("_AlphaClip", 1f);
            mat.SetFloat("_Cutoff", 0.498039216f);
            mat.EnableKeyword("_ALPHATEST_ON");
            mat.SetOverrideTag("RenderType", "TransparentCutout");
            mat.renderQueue = 2450;
        }
        else
        {
            mat.SetFloat("_AlphaClip", 0f);
            mat.SetOverrideTag("RenderType", "Opaque");
            mat.renderQueue = 2000;
        }
        return mat;
    }

    static void SaveMaterial(Material mat, string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path) ?? ".");
        var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null)
        {
            EditorUtility.CopySerialized(mat, existing);
            Object.DestroyImmediate(mat);
            return;
        }
        AssetDatabase.CreateAsset(mat, path);
    }

    static void BuildArmorPrefabs(Dictionary<string, Material> bodyMats)
    {
        foreach (string race in Races)
        {
            string package = PackageOf(race);
            foreach (string part in Parts)
            {
                string mesh = race + "_m001_" + part;
                string fbx = "Assets/Resources/Data/Animations/" + package + "/Models/" + mesh + ".fbx";
                string prefab = "Assets/Resources/Data/Animations/" + package + "/" + mesh + ".prefab";
                bodyMats.TryGetValue(race + "_m001_t02_" + part, out Material mat);
                WrapFbx(fbx, prefab, mesh, mat == null ? null : new[] { mat });
            }
        }
    }

    static void BuildFaceAndHairPrefabs(Dictionary<string, Material> faceMats, Dictionary<string, Material> hairMats)
    {
        foreach (string race in Races)
        {
            string package = PackageOf(race);
            string faceFbx = "Assets/Resources/Data/Animations/" + package + "/Models/" + race + "_m000_f.fbx";
            for (int face = 0; face < 3; face++)
            {
                string key = race + "_m000_t0" + face + "_f";
                string key1 = key + "1";
                faceMats.TryGetValue(key, out Material faceMat);
                faceMats.TryGetValue(key1, out Material faceMat1);
                Material[] mats = faceMat1 != null ? new[] { faceMat, faceMat1 } : new[] { faceMat };
                string prefab = "Assets/Resources/Data/Animations/Orc/" + race + "/Faces/" + race + "_f_" + face + ".prefab";
                WrapFbx(faceFbx, prefab, race + "_f_" + face, mats);
            }

            string modelDir = "Assets/Resources/Data/Animations/" + package + "/Models";
            foreach (string guid in AssetDatabase.FindAssets("t:Model", new[] { modelDir }))
            {
                string fbx = AssetDatabase.GUIDToAssetPath(guid);
                string file = Path.GetFileNameWithoutExtension(fbx);
                // Alpha hair meshes omit bones, so they cannot share the body skeleton index order.
                if (!file.EndsWith("_bh") || !file.StartsWith(race + "_m"))
                    continue;
                string part = file.EndsWith("_ah") ? "ah" : "bh";
                int style = StyleIndex(file);
                if (style < 0)
                    continue;
                for (int color = 0; color < 4; color++)
                {
                    string texName = race + "_m" + style.ToString("000") + "_t0" + color + "_m00_" + part;
                    if (!hairMats.TryGetValue(texName, out Material mat))
                    {
                        string fallback = race + "_m" + style.ToString("000") + "_t0" + color + "_m00_bh";
                        hairMats.TryGetValue(fallback, out mat);
                    }
                    if (mat == null)
                        continue;
                    string prefab = "Assets/Resources/Data/Animations/Orc/" + race + "/Hair/" + race + "_h_" + style + "_" + color + "_" + part + ".prefab";
                    WrapFbx(fbx, prefab, race + "_h_" + style + "_" + color + "_" + part, new[] { mat });
                }
            }
        }
    }

    static int StyleIndex(string file)
    {
        int marker = file.IndexOf("_m", System.StringComparison.OrdinalIgnoreCase);
        if (marker < 0 || marker + 5 > file.Length)
            return -1;
        string digits = file.Substring(marker + 2, 3);
        return int.TryParse(digits, out int style) ? style : -1;
    }

    static void WrapFbx(string fbxPath, string prefabPath, string name, Material[] materials)
    {
        var source = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);
        if (source == null)
        {
            Debug.LogWarning("Missing fbx " + fbxPath);
            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(prefabPath) ?? ".");
        var root = new GameObject(name);
        root.layer = 6;
        try
        {
            root.tag = "User";
        }
        catch (UnityException)
        {
        }

        var instance = (GameObject)PrefabUtility.InstantiatePrefab(source);
        instance.transform.SetParent(root.transform, false);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;
        if (materials != null && materials.Length > 0 && materials[0] != null)
        {
            foreach (var renderer in instance.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                var slots = renderer.sharedMaterials;
                int count = Mathf.Min(slots.Length == 0 ? materials.Length : slots.Length, materials.Length);
                if (slots.Length == 0)
                    slots = new Material[count];
                for (int i = 0; i < count; i++)
                {
                    if (materials[i] != null)
                        slots[i] = materials[i];
                }
                renderer.sharedMaterials = slots;
            }
        }

        PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        Object.DestroyImmediate(root);
    }

    static void BuildSkeletonsAndContainers()
    {
        var basic = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(BaseControllerPath);
        foreach (string race in Races)
        {
            string package = PackageOf(race);
            string animFbx = "Assets/Resources/Data/Animations/" + package + "/Models/" + race + "_anim.fbx";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(animFbx) == null)
                animFbx = "Assets/Resources/Data/Animations/" + package + "/Models/" + race + "_m001_u.fbx";
            var clips = AssetDatabase.LoadAllAssetsAtPath(animFbx).OfType<AnimationClip>()
                .Where(clip => clip != null && !clip.name.StartsWith("__preview"))
                .ToArray();
            var controller = BuildOverride(basic, race, clips);
            string userSkel = "Assets/Resources/Data/Animations/Orc/" + race + "/" + race + "_anim_user.prefab";
            string playerSkel = "Assets/Resources/Data/Animations/Orc/" + race + "/" + race + "_anim_player.prefab";
            BuildSkeleton(animFbx, userSkel, controller, false);
            BuildSkeleton(animFbx, playerSkel, controller, true);
            CloneContainer(PawnTemplate, "Assets/Resources/Data/Animations/Orc/" + race + "/Pawn_" + race + ".prefab", userSkel, "Pawn_" + race);
            CloneContainer(UserTemplate, "Assets/Resources/Data/Animations/Orc/" + race + "/User_" + race + ".prefab", userSkel, "User_" + race);
            CloneContainer(PlayerTemplate, "Assets/Resources/Data/Animations/Orc/" + race + "/Player_" + race + ".prefab", playerSkel, "Player_" + race);
        }
    }

    static AnimatorOverrideController BuildOverride(RuntimeAnimatorController basic, string race, AnimationClip[] clips)
    {
        string path = "Assets/Resources/Data/Animations/Animator/Player/" + race + ".overrideController";
        var over = new AnimatorOverrideController(basic) { name = race };
        var pairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        over.GetOverrides(pairs);
        var next = new List<KeyValuePair<AnimationClip, AnimationClip>>(pairs.Count);
        foreach (var pair in pairs)
        {
            AnimationClip match = MatchClip(pair.Key, clips);
            next.Add(new KeyValuePair<AnimationClip, AnimationClip>(pair.Key, match != null ? match : pair.Value));
        }
        over.ApplyOverrides(next);
        var existing = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(path);
        if (existing != null)
            AssetDatabase.DeleteAsset(path);
        AssetDatabase.CreateAsset(over, path);
        return over;
    }

    static AnimationClip MatchClip(AnimationClip original, AnimationClip[] clips)
    {
        if (original == null || clips == null)
            return null;
        string name = original.name.ToLowerInvariant();
        string[] keys = { "wait_1hs", "wait_hand", "walk_1hs", "walk_hand", "run_1hs", "run_hand" };
        foreach (string key in keys)
        {
            if (!name.Contains(key))
                continue;
            foreach (var clip in clips)
            {
                if (clip.name.ToLowerInvariant().Contains(key))
                    return clip;
            }
            if (key.StartsWith("wait"))
            {
                foreach (var clip in clips)
                {
                    if (clip.name.ToLowerInvariant().Contains("wait_hand"))
                        return clip;
                }
            }
        }
        return null;
    }

    static void BuildSkeleton(string animFbx, string prefabPath, RuntimeAnimatorController controller, bool player)
    {
        var source = AssetDatabase.LoadAssetAtPath<GameObject>(animFbx);
        if (source == null)
        {
            Debug.LogWarning("Missing anim fbx " + animFbx);
            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(prefabPath) ?? ".");
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(source);
        instance.name = Path.GetFileNameWithoutExtension(prefabPath);
        foreach (var renderer in instance.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            renderer.enabled = false;
        if (instance.GetComponent<SkinnedMeshSync>() == null)
            instance.AddComponent<SkinnedMeshSync>();
        var animator = instance.GetComponent<Animator>();
        if (animator == null)
            animator = instance.AddComponent<Animator>();
        animator.runtimeAnimatorController = controller;
        animator.applyRootMotion = false;
        if (instance.GetComponent<SurfaceDetector>() == null)
            instance.AddComponent<SurfaceDetector>();
        if (player)
        {
            if (instance.GetComponent<PlayerAnimationController>() == null)
                instance.AddComponent<PlayerAnimationController>();
            if (instance.GetComponent<CharacterAnimationAudioHandler>() == null)
                instance.AddComponent<CharacterAnimationAudioHandler>();
        }
        else if (instance.GetComponent<BaseAnimationAudioHandler>() == null)
        {
            instance.AddComponent<BaseAnimationAudioHandler>();
        }

        PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
        Object.DestroyImmediate(instance);
    }

    static void CloneContainer(string templatePath, string destPath, string skeletonPath, string name)
    {
        var skeleton = AssetDatabase.LoadAssetAtPath<GameObject>(skeletonPath);
        if (skeleton == null)
        {
            Debug.LogWarning("Missing skeleton " + skeletonPath);
            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(destPath) ?? ".");
        var root = PrefabUtility.LoadPrefabContents(templatePath);
        root.name = name;
        var remove = new List<GameObject>();
        foreach (var transform in root.GetComponentsInChildren<Transform>(true))
        {
            if (transform == null || transform.gameObject == root)
                continue;
            if (PrefabUtility.IsAnyPrefabInstanceRoot(transform.gameObject))
                remove.Add(transform.gameObject);
        }
        foreach (var go in remove)
        {
            if (go != null)
                Object.DestroyImmediate(go);
        }

        Transform parent = root.transform.Find("model");
        if (parent == null)
            parent = root.transform;
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(skeleton, parent);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;
        PrefabUtility.SaveAsPrefabAsset(root, destPath);
        PrefabUtility.UnloadPrefabContents(root);
    }
}
