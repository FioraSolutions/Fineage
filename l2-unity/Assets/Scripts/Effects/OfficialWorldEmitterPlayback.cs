using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Instantiates official world-map Emitter prefabs at dump Location/Rotation/DrawScale
/// for the listener's 3×3 tiles (plus the active env scene).
/// </summary>
public sealed class OfficialWorldEmitterPlayback : MonoBehaviour
{
    const float PollSeconds = 0.5f;

    readonly Dictionary<string, Instance> _alive = new Dictionary<string, Instance>();
    readonly List<OfficialWorldEmitterTable.Emitter> _nearby = new List<OfficialWorldEmitterTable.Emitter>();
    readonly HashSet<string> _logged = new HashSet<string>();
    readonly List<PendingSound> _pendingAudio = new List<PendingSound>();

    sealed class PendingSound
    {
        public AudioSource Source;
        public string Token;
        public string Owner;
        public float RadiusUu;
        public float Volume;
        public string Reason;
    }
    Transform _root;
    float _nextPoll;
    int _lastTileX = int.MinValue;
    int _lastTileY = int.MinValue;
    string _lastScene;

    sealed class Instance
    {
        public GameObject Object;
        public L2Particle Particle;
    }

    void OnEnable()
    {
        OfficialWorldEmitterTable.Load();
        OfficialWaveTable.Load();
        if (_root == null)
        {
            var go = new GameObject("OfficialWorldEmitters");
            _root = go.transform;
            _root.SetParent(transform, false);
        }
        _nextPoll = 0f;
    }

    void OnDisable()
    {
        DespawnAll();
    }

    void LateUpdate()
    {
        FlushPendingAudio();
        if (!OfficialWorldEmitterTable.HasEmitters)
            return;
        if (Time.unscaledTime < _nextPoll)
            return;
        _nextPoll = Time.unscaledTime + PollSeconds;

        Transform ear = ResolveEar();
        if (ear == null)
            return;

        Vector3 ue = VectorUtils.ConvertPosUnityToL2j(ear.position);
        VectorUtils.GetSwMapTile(ue, out int tileX, out int tileY);
        string scene = SceneManager.GetActiveScene().name;
        if (tileX == _lastTileX && tileY == _lastTileY && scene == _lastScene && _alive.Count > 0)
            return;
        _lastTileX = tileX;
        _lastTileY = tileY;
        _lastScene = scene;

        OfficialWorldEmitterTable.CollectNearby(tileX, tileY, scene, _nearby);
        var wanted = new HashSet<string>();
        for (int i = 0; i < _nearby.Count; i++)
        {
            OfficialWorldEmitterTable.Emitter entry = _nearby[i];
            if (!IsAudible(entry, ear.position))
                continue;
            wanted.Add(entry.Id);
            if (!_alive.ContainsKey(entry.Id))
                TrySpawn(entry);
        }

        if (_alive.Count == 0)
            return;
        var dead = new List<string>();
        foreach (var kv in _alive)
        {
            if (!wanted.Contains(kv.Key))
                dead.Add(kv.Key);
        }
        for (int i = 0; i < dead.Count; i++)
            Despawn(dead[i]);
    }

    bool IsAudible(OfficialWorldEmitterTable.Emitter entry, Vector3 earUnity)
    {
        if (!entry.hasVisibleLimit)
            return true;
        float maxUu = entry.visibleLimitMax;
        if (maxUu <= 0f)
            return true;
        float maxMeters = maxUu * VectorUtils.L2UuToUnity;
        return (entry.UnityPosition - earUnity).sqrMagnitude <= maxMeters * maxMeters;
    }

    void TrySpawn(OfficialWorldEmitterTable.Emitter entry)
    {
        if (string.IsNullOrEmpty(entry.prefabClass))
        {
            LogOnce(entry.Id, "OfficialWorldEmitterPlayback: " + entry.map + ":" + entry.name +
                " official Emitters array has no resolved particle streams");
            return;
        }

        string path = entry.ResourcesPath;
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            LogOnce("prefab:" + path, "OfficialWorldEmitterPlayback: missing prefab Resources/" + path +
                " for " + entry.map + ":" + entry.name);
            return;
        }

        GameObject instance = Instantiate(prefab, entry.UnityPosition, entry.UnityRotation, _root);
        instance.name = entry.map + "_" + entry.name;
        instance.transform.localScale = entry.UnityScale;
        instance.SetActive(true);

        L2Particle particle = instance.GetComponent<L2Particle>();
        if (particle != null)
            particle.Play();

        AttachSpawnSounds(instance, entry);
        _alive[entry.Id] = new Instance { Object = instance, Particle = particle };
    }

    void AttachSpawnSounds(GameObject instance, OfficialWorldEmitterTable.Emitter entry)
    {
        if (entry.spawnSounds == null || entry.spawnSounds.Length == 0)
            return;
        AudioService service = AudioPacketServices.Service;
        for (int i = 0; i < entry.spawnSounds.Length; i++)
        {
            string token = entry.spawnSounds[i];
            if (string.IsNullOrEmpty(token))
                continue;
            string resourcesPath;
            string error;
            if (OfficialWaveTable.TryResolveDatToken(token, out resourcesPath, out error))
            {
                // official package.name token
            }
            else if (OfficialWaveTable.TryResolve(SoundStem(token), out OfficialWaveTable.Entry wave, out error, packageHint: PackageHint(token)))
            {
                resourcesPath = wave.ResourcesPath;
            }
            else
            {
                LogOnce("wave:" + token, error ?? OfficialWaveTable.FormatMissingWaveError(PackageHint(token), SoundStem(token),
                    "SpawnSound for " + entry.map + ":" + entry.name));
                continue;
            }
            if (service == null)
                continue;
            if (!service.TryLoadClip(resourcesPath, out AudioClip clip, out string loadError))
            {
                LogOnce("clip:" + resourcesPath, loadError ?? ("OfficialWorldEmitterPlayback: missing clip \"" + resourcesPath +
                    "\" for " + entry.map + ":" + entry.name + " SpawnSound=" + token));
                continue;
            }

            AudioSource source = instance.AddComponent<AudioSource>();
            source.clip = clip;
            source.spatialBlend = 1f;
            source.dopplerLevel = 0f;
            source.loop = true;
            source.volume = OfficialAmbientSoundUnits.VolumeToScale(entry.soundVolume);
            float radiusUu = Ue2SoundAudibility.ResolveActorSoundRadiusUu(entry.soundRadius);
            AudioService.ApplyUe2LoopFalloff(source, radiusUu);
            string reason = Ue2SoundAudibility.HasRadius(entry.soundRadius) ? "audible" : "actor-default";
            if (!AudioListenerLifecycle.TryPrepare())
            {
                AudioPlayTrace.Write(
                    "refuse", Time.unscaledTime, token, "WorldEmitter", entry.name, 0, 0f, radiusUu,
                    entry.soundVolume, Ue2SoundAudibility.SlotAmbient, "no-listener");
                _pendingAudio.Add(new PendingSound
                {
                    Source = source,
                    Token = token,
                    Owner = entry.name,
                    RadiusUu = radiusUu,
                    Volume = entry.soundVolume,
                    Reason = reason
                });
                continue;
            }

            source.Play();
            AudioPlayTrace.Write(
                "play", Time.unscaledTime, token, "WorldEmitter", entry.name, 0, 0f, radiusUu,
                entry.soundVolume, Ue2SoundAudibility.SlotAmbient, reason);
        }
    }

    void FlushPendingAudio()
    {
        if (_pendingAudio.Count == 0 || !AudioListenerLifecycle.TryPrepare())
            return;
        for (int i = 0; i < _pendingAudio.Count; i++)
        {
            PendingSound pending = _pendingAudio[i];
            if (pending.Source == null)
                continue;
            pending.Source.Play();
            AudioPlayTrace.Write(
                "play", Time.unscaledTime, pending.Token, "WorldEmitter", pending.Owner, 0, 0f,
                pending.RadiusUu, pending.Volume, Ue2SoundAudibility.SlotAmbient, pending.Reason);
        }

        _pendingAudio.Clear();
    }

    static string SoundStem(string token)
    {
        int dot = token.LastIndexOf('.');
        return dot >= 0 ? token.Substring(dot + 1) : token;
    }

    static string PackageHint(string token)
    {
        int dot = token.IndexOf('.');
        return dot > 0 ? token.Substring(0, dot) : null;
    }

    Transform ResolveEar()
    {
        if (Camera.main != null)
            return Camera.main.transform;
        return transform;
    }

    void Despawn(string id)
    {
        if (!_alive.TryGetValue(id, out Instance inst))
            return;
        _alive.Remove(id);
        if (inst?.Object != null)
            Destroy(inst.Object);
    }

    void DespawnAll()
    {
        foreach (var kv in _alive)
        {
            if (kv.Value?.Object != null)
                Destroy(kv.Value.Object);
        }
        _alive.Clear();
    }

    void LogOnce(string key, string message)
    {
        if (!_logged.Add(key))
            return;
        OfficialWorldEmitterTable.ErrorLogged?.Invoke(message);
        Debug.LogError(message);
    }
}
