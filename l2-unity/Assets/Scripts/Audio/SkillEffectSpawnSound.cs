using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays LineageEffect SpawnSound when a generated effect prefab is instantiated.
/// Lookup is by effect class from the lossless dump; missing clips log package+name.
/// </summary>
public static class SkillEffectSpawnSound
{
    const string ResourcePath = "Data/SkillEffectSpawnSounds";
    const string SfxRoot = "Data/Sounds/SkillSound/";
    const string ItemRoot = "Data/Sounds/ItemSound/";
    const float InterludeVolumeMax = 255f;

    static bool _tried;
    static Dictionary<string, SpawnCue> _byClass;

    struct SpawnCue
    {
        public string EffectClass;
        public string Token;
        public string Package;
        public string Name;
        public string Clip;
        public float Vol;
        public float Rad;
        public bool Looping;
    }

    [Serializable]
    class RootDto
    {
        public EntryDto[] entries;
    }

    [Serializable]
    class EntryDto
    {
        public string effectClass;
        public string token;
        public string package;
        public string name;
        public string clip;
        public float vol;
        public float rad;
        public bool looping;
    }

    public static void TryPlay(string effectClass, Vector3 worldPosition)
    {
        EnsureLoaded();
        string key = Normalize(effectClass);
        if (string.IsNullOrEmpty(key) || _byClass == null)
            return;
        SpawnCue cue;
        if (!_byClass.TryGetValue(key, out cue))
            return;

        AudioClip clip = InterludeSfx.Load(SfxRoot + cue.Clip);
        if (clip == null)
            clip = InterludeSfx.Load(SfxRoot + cue.Clip.ToLowerInvariant());
        if (clip == null && !string.IsNullOrEmpty(cue.Clip))
            clip = InterludeSfx.Load(ItemRoot + cue.Clip);

        if (clip == null)
        {
            Debug.LogError(
                "[SkillEffectSpawnSound] missing clip package=" +
                (string.IsNullOrEmpty(cue.Package) ? "?" : cue.Package) +
                " name=" + (string.IsNullOrEmpty(cue.Name) ? cue.Clip : cue.Name) +
                " token=" + cue.Token +
                " effectClass=" + cue.EffectClass);
            return;
        }

        float volume = cue.Vol <= 0f ? 0f : Mathf.Clamp01(cue.Vol / InterludeVolumeMax);
        float radius = cue.Rad > 0f ? VectorUtils.ConvertL2UuToMeters(cue.Rad) : 0f;
        InterludeSfx.PlayAt(clip, worldPosition, volume, false, radius);
    }

    static void EnsureLoaded()
    {
        if (_tried)
            return;
        _tried = true;
        _byClass = new Dictionary<string, SpawnCue>(StringComparer.OrdinalIgnoreCase);
        TextAsset asset = Resources.Load<TextAsset>(ResourcePath);
        if (asset == null || string.IsNullOrEmpty(asset.text))
        {
            Debug.LogError("[SkillEffectSpawnSound] missing Resources/" + ResourcePath + ".json");
            return;
        }

        RootDto root = JsonUtility.FromJson<RootDto>(asset.text);
        if (root == null || root.entries == null)
        {
            Debug.LogError("[SkillEffectSpawnSound] SkillEffectSpawnSounds.json has no entries");
            return;
        }

        for (int i = 0; i < root.entries.Length; i++)
        {
            EntryDto e = root.entries[i];
            if (e == null || string.IsNullOrEmpty(e.effectClass) || string.IsNullOrEmpty(e.clip))
                continue;
            string key = Normalize(e.effectClass);
            _byClass[key] = new SpawnCue
            {
                EffectClass = e.effectClass,
                Token = e.token ?? "",
                Package = e.package ?? "",
                Name = e.name ?? "",
                Clip = e.clip,
                Vol = e.vol,
                Rad = e.rad,
                Looping = e.looping
            };
        }
    }

    static string Normalize(string effectClass)
    {
        if (string.IsNullOrEmpty(effectClass))
            return "";
        int slash = effectClass.LastIndexOf('/');
        if (slash >= 0 && slash < effectClass.Length - 1)
            effectClass = effectClass.Substring(slash + 1);
        if (effectClass.EndsWith("(Clone)", StringComparison.Ordinal))
            effectClass = effectClass.Substring(0, effectClass.Length - 7).Trim();
        return effectClass.Trim();
    }
}
