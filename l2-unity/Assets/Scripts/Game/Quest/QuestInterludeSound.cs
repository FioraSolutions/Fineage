using UnityEngine;

public static class QuestInterludeSound
{
    public static void Play(string file, int type)
    {
        string stem = (file ?? "").Trim();
        if (stem.Length == 0)
        {
            Debug.LogError("[PlaySound] empty file name.");
            return;
        }

        string path = stem.Contains("/")
            ? "Data/Sounds/" + stem.Replace('.', '/')
            : "Data/Sounds/ItemSound/" + stem;

        if (type == 1)
            path = "Data/Sounds/Music/" + stem;

        AudioClip clip = InterludeSfx.Load(path);
        if (clip == null)
        {
            Debug.LogError($"[PlaySound] missing sound asset path={path} wire={stem}.");
            return;
        }

        if (stem.StartsWith("tutorial_voice", System.StringComparison.OrdinalIgnoreCase))
            InterludeSfx.PlayVoice(clip, 1f);
        else
            InterludeSfx.Play(clip, 0.85f);
    }
}