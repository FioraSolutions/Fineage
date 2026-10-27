using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Editor window for previewing characters, NPCs, and monsters without login/network.
/// Menu: Fineage → Content Preview
/// </summary>
public class ContentPreviewWindow : EditorWindow
{
    // ── Tabs ──
    private enum Tab { Character, Npc }
    private Tab _tab = Tab.Character;

    // ── Character fields ──
    private CharacterRaceAnimation _raceId = CharacterRaceAnimation.MFighter;
    private int _face = 0;
    private int _hairStyle = 0;
    private int _hairColor = 0;
    private int _weaponId = 0;
    private int _shieldId = 0;

    // ── NPC fields ──
    private int _npcId = 20006;
    private string _npcSearch = "";
    private Vector2 _npcSearchScroll;
    private List<NpcSearchResult> _npcSearchResults = new List<NpcSearchResult>();

    // ── Scene state ──
    private bool _previewActive = false;
    private GameObject _spawnedEntity;
    private string _previousScene;

    private struct NpcSearchResult
    {
        public int NpcId;
        public string Name;
        public string Mesh;
    }

    [MenuItem("Fineage/Content Preview %&p")]
    public static void ShowWindow()
    {
        var window = GetWindow<ContentPreviewWindow>("Content Preview");
        window.minSize = new Vector2(340, 480);
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Fineage Content Preview", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Spawn characters, NPCs, and monsters offline — no login or zone-server required.",
            MessageType.Info);
        EditorGUILayout.Space(4);

        // ── Tab bar ──
        _tab = (Tab)GUILayout.Toolbar((int)_tab, new[] { "Character", "NPC / Monster" });
        EditorGUILayout.Space(4);

        switch (_tab)
        {
            case Tab.Character: DrawCharacterTab(); break;
            case Tab.Npc:       DrawNpcTab();       break;
        }

        EditorGUILayout.Space(8);
        DrawActions();
    }

    // ────────────────────── Character Tab ──────────────────────

    private void DrawCharacterTab()
    {
        EditorGUILayout.LabelField("Race & Body", EditorStyles.miniBoldLabel);
        _raceId = (CharacterRaceAnimation)EditorGUILayout.EnumPopup("Race / Sex", _raceId);
        _face = EditorGUILayout.IntSlider("Face", _face, 0, 5);
        _hairStyle = EditorGUILayout.IntSlider("Hair Style", _hairStyle, 0, 5);
        _hairColor = EditorGUILayout.IntSlider("Hair Color", _hairColor, 0, 3);

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Equipment", EditorStyles.miniBoldLabel);
        _weaponId = EditorGUILayout.IntField("Weapon ID (0 = none)", _weaponId);
        _shieldId = EditorGUILayout.IntField("Shield ID (0 = none)", _shieldId);
    }

    // ────────────────────── NPC Tab ──────────────────────

    private void DrawNpcTab()
    {
        _npcId = EditorGUILayout.IntField("NPC ID", _npcId);

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Search (requires tables loaded)", EditorStyles.miniBoldLabel);
        EditorGUI.BeginChangeCheck();
        _npcSearch = EditorGUILayout.TextField("Name filter", _npcSearch);
        if (EditorGUI.EndChangeCheck() && _npcSearch.Length >= 2)
            SearchNpcs(_npcSearch);

        if (_npcSearchResults.Count > 0)
        {
            _npcSearchScroll = EditorGUILayout.BeginScrollView(_npcSearchScroll, GUILayout.MaxHeight(160));
            foreach (var r in _npcSearchResults)
            {
                if (GUILayout.Button($"[{r.NpcId}] {r.Name}", EditorStyles.miniButton))
                {
                    _npcId = r.NpcId;
                    _npcSearch = "";
                    _npcSearchResults.Clear();
                    GUI.FocusControl(null);
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }

    // ────────────────────── Actions ──────────────────────

    private void DrawActions()
    {
        bool isPlaying = Application.isPlaying;

        EditorGUILayout.LabelField("Preview", EditorStyles.miniBoldLabel);

        using (new EditorGUI.DisabledScope(isPlaying))
        {
            if (GUILayout.Button("▶  Open Preview Scene & Play", GUILayout.Height(32)))
                OpenPreviewAndPlay();
        }

        using (new EditorGUI.DisabledScope(!isPlaying || !_previewActive))
        {
            if (GUILayout.Button("Spawn / Respawn", GUILayout.Height(28)))
                SpawnCurrent();

            EditorGUILayout.Space(2);
            if (_weaponId != 0 || _shieldId != 0)
            {
                if (GUILayout.Button("Re-equip Weapon"))
                    ReequipWeapon();
            }
        }

        EditorGUILayout.Space(4);
        using (new EditorGUI.DisabledScope(!isPlaying))
        {
            if (GUILayout.Button("■  Stop Preview"))
            {
                _previewActive = false;
                EditorApplication.isPlaying = false;
            }
        }
    }

    // ────────────────────── Scene Management ──────────────────────

    private void OpenPreviewAndPlay()
    {
        if (EditorApplication.isPlaying)
            return;

        _previousScene = SceneManager.GetActiveScene().path;

        string previewScenePath = "Assets/Resources/Scenes/Preview.unity";
        if (!System.IO.File.Exists(previewScenePath))
            CreatePreviewScene(previewScenePath);

        EditorSceneManager.OpenScene(previewScenePath);
        _previewActive = true;

        // Subscribe to play mode change to spawn after entering play
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        EditorApplication.isPlaying = true;
    }

    private void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode && _previewActive)
        {
            // Small delay to let tables initialize
            EditorApplication.delayCall += () =>
            {
                SpawnCurrent();
            };
        }

        if (state == PlayModeStateChange.EnteredEditMode)
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            _previewActive = false;
        }
    }

    // ────────────────────── Scene Creation ──────────────────────

    private static void CreatePreviewScene(string path)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Camera
        var camGo = new GameObject("Preview Camera");
        var cam = camGo.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.15f, 0.18f, 0.22f);
        camGo.transform.position = new Vector3(0, 2f, -5f);
        camGo.transform.LookAt(Vector3.up);
        camGo.AddComponent<PreviewOrbitCamera>();

        // Light
        var lightGo = new GameObject("Directional Light");
        var light = lightGo.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.2f;
        light.color = new Color(1f, 0.96f, 0.88f);
        lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        // Fill light
        var fillGo = new GameObject("Fill Light");
        var fill = fillGo.AddComponent<Light>();
        fill.type = LightType.Directional;
        fill.intensity = 0.4f;
        fill.color = new Color(0.7f, 0.8f, 1f);
        fillGo.transform.rotation = Quaternion.Euler(30f, 150f, 0f);

        // Ground plane
        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.localScale = new Vector3(5, 1, 5);

        // Bootstrap
        var bootstrapGo = new GameObject("PreviewBootstrap");
        bootstrapGo.AddComponent<PreviewSceneBootstrap>();

        EditorSceneManager.SaveScene(scene, path);
    }

    // ────────────────────── Spawning ──────────────────────

    private void SpawnCurrent()
    {
        if (!Application.isPlaying)
            return;

        var bootstrap = UnityEngine.Object.FindFirstObjectByType<PreviewSceneBootstrap>();
        if (bootstrap == null)
        {
            Debug.LogError("[ContentPreview] PreviewSceneBootstrap not found in scene.");
            return;
        }

        bootstrap.EnsureTablesLoaded();

        if (_spawnedEntity != null)
        {
            UnityEngine.Object.Destroy(_spawnedEntity);
            _spawnedEntity = null;
        }

        switch (_tab)
        {
            case Tab.Character:
                _spawnedEntity = SpawnCharacter(bootstrap);
                break;
            case Tab.Npc:
                _spawnedEntity = SpawnNpc(bootstrap);
                break;
        }

        if (_spawnedEntity != null)
            FocusCamera(_spawnedEntity);
    }

    private GameObject SpawnCharacter(PreviewSceneBootstrap bootstrap)
    {
        bool isFemale = _raceId == CharacterRaceAnimation.FElf ||
                        _raceId == CharacterRaceAnimation.FDarkElf ||
                        _raceId == CharacterRaceAnimation.FDwarf ||
                        _raceId == CharacterRaceAnimation.FShaman ||
                        _raceId == CharacterRaceAnimation.FOrc ||
                        _raceId == CharacterRaceAnimation.FMagic ||
                        _raceId == CharacterRaceAnimation.FFighter;

        bool isMage = _raceId == CharacterRaceAnimation.MMagic ||
                      _raceId == CharacterRaceAnimation.FMagic ||
                      _raceId == CharacterRaceAnimation.MShaman ||
                      _raceId == CharacterRaceAnimation.FShaman;

        PlayerAppearance appearance = new PlayerAppearance
        {
            Race = (int)CharacterRaceParser.ParseRace(_raceId),
            Sex = isFemale ? 1 : 0,
            Face = _face,
            HairStyle = _hairStyle,
            HairColor = _hairColor,
            BaseClass = isMage ? (int)BaseClass.MMagic : (int)BaseClass.Fighter,
            Running = false
        };

        CharacterBuilder builder = CharacterBuilder.Instance;
        if (builder == null)
        {
            Debug.LogError("[ContentPreview] CharacterBuilder.Instance is null. " +
                "Ensure ModelTable is loaded.");
            return null;
        }

        GameObject go = builder.BuildCharacterBaseInterlude(_raceId, appearance, EntityType.Player);
        if (go == null)
        {
            Debug.LogError("[ContentPreview] BuildCharacterBaseInterlude returned null for " + _raceId);
            return null;
        }

        go.transform.position = Vector3.zero;
        go.transform.rotation = Quaternion.identity;
        go.name = "Preview_" + _raceId;

        DisableNetworking(go);
        go.SetActive(true);

        // Animate idle if possible
        Animator animator = go.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.enabled = true;
            animator.speed = 1f;
            int waitHash = Animator.StringToHash("wait");
            if (animator.HasState(0, waitHash))
                animator.Play(waitHash, 0, 0f);
        }

        // Equip weapon
        if (_weaponId != 0 || _shieldId != 0)
        {
            Gear gear = go.GetComponent<Gear>();
            if (gear == null)
                gear = go.GetComponentInChildren<Gear>();
            if (gear != null)
            {
                gear.Initialize(99999, _raceId);
                if (_weaponId != 0) gear.EquipWeapon(_weaponId, false);
                if (_shieldId != 0) gear.EquipShield(_shieldId);
            }
            else
            {
                Debug.LogWarning("[ContentPreview] Gear component not found on character prefab.");
            }
        }

        Debug.Log("[ContentPreview] Spawned character: " + _raceId +
            " face=" + _face + " weapon=" + _weaponId);
        return go;
    }

    private GameObject SpawnNpc(PreviewSceneBootstrap bootstrap)
    {
        EntityIdentity identity = new EntityIdentity();
        identity.Id = 90000;
        identity.NpcId = _npcId;
        identity.Position = Vector3.zero;
        identity.Heading = Quaternion.identity;

        NpcSpawnRequest request;
        if (!EntitySpawnShared.TryResolveNpc(identity, null, null, null, out request))
        {
            Debug.LogError("[ContentPreview] Cannot resolve NPC id=" + _npcId +
                ". Check Npcgrp_Classic.txt and NpcName data.");
            return null;
        }

        GameObject prefab = request.Prefab;
        if (prefab == null)
        {
            Debug.LogError("[ContentPreview] Prefab is null for NPC id=" + _npcId);
            return null;
        }

        bool wasActive = prefab.activeSelf;
        prefab.SetActive(false);
        GameObject npcGo = UnityEngine.Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        prefab.SetActive(wasActive);

        npcGo.name = "Preview_NPC_" + _npcId + "_" + (request.NpcName != null ? request.NpcName.Name : "");
        DisableNetworking(npcGo);
        npcGo.SetActive(true);

        // Init animation
        NetworkAnimationController animCtrl = npcGo.GetComponent<NetworkAnimationController>();
        if (animCtrl == null)
            animCtrl = npcGo.GetComponentInChildren<NetworkAnimationController>(true);
        if (animCtrl != null)
            EntitySpawnShared.InitializeNpcAnimation(animCtrl);

        // Init gear for NPC weapons (rhand/lhand from npcgrp)
        Gear gear = npcGo.GetComponent<Gear>();
        if (gear == null)
            gear = npcGo.GetComponentInChildren<Gear>(true);
        if (gear != null && request.Npcgrp != null)
        {
            gear.Initialize(90000, CharacterRaceAnimation.MFighter);
            if (request.Npcgrp.Rhand != 0)
                gear.EquipWeapon(request.Npcgrp.Rhand, false);
            if (request.Npcgrp.Lhand != 0)
            {
                Weapon lWeapon = ItemTable.Instance.GetWeapon(request.Npcgrp.Lhand);
                if (lWeapon != null && lWeapon.Weapongrp != null &&
                    lWeapon.Weapongrp.WeaponType == WeaponType.shield)
                    gear.EquipShield(request.Npcgrp.Lhand);
                else if (request.Npcgrp.Lhand != 0)
                    gear.EquipWeapon(request.Npcgrp.Lhand, true);
            }
        }

        Debug.Log("[ContentPreview] Spawned NPC: id=" + _npcId +
            " name=" + (request.NpcName != null ? request.NpcName.Name : "?") +
            " mesh=" + request.Npcgrp.Mesh);
        return npcGo;
    }

    // ────────────────────── Helpers ──────────────────────

    private static void DisableNetworking(GameObject go)
    {
        foreach (var net in go.GetComponentsInChildren<NetworkTransformReceive>(true))
            net.enabled = false;
        foreach (var net in go.GetComponentsInChildren<NetworkTransformShare>(true))
            net.enabled = false;

        PlayerController pc = go.GetComponent<PlayerController>();
        if (pc != null) pc.enabled = false;
        CharacterController cc = go.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
    }

    private static void FocusCamera(GameObject target)
    {
        PreviewOrbitCamera orbit = UnityEngine.Object.FindFirstObjectByType<PreviewOrbitCamera>();
        if (orbit != null)
        {
            Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                Bounds b = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                    b.Encapsulate(renderers[i].bounds);
                orbit.SetTarget(b.center, b.size.magnitude * 1.2f);
            }
            else
            {
                orbit.SetTarget(target.transform.position + Vector3.up, 4f);
            }
        }
    }

    private void ReequipWeapon()
    {
        // Respawn is the cleanest way — ClearAllHandEquipment is private
        SpawnCurrent();
    }

    private void SearchNpcs(string filter)
    {
        _npcSearchResults.Clear();
        if (NpcNameTable.Instance == null || NpcNameTable.Instance.NpcNames == null)
            return;

        string lower = filter.ToLowerInvariant();
        int count = 0;
        foreach (var pair in NpcNameTable.Instance.NpcNames)
        {
            if (pair.Value == null || pair.Value.Name == null)
                continue;
            if (pair.Value.Name.ToLowerInvariant().Contains(lower))
            {
                string mesh = "";
                Npcgrp grp = NpcgrpTable.Instance.GetNpcgrp(pair.Key);
                if (grp != null) mesh = grp.Mesh;

                _npcSearchResults.Add(new NpcSearchResult
                {
                    NpcId = pair.Key,
                    Name = pair.Value.Name,
                    Mesh = mesh
                });
                if (++count >= 50) break;
            }
        }
    }
}
