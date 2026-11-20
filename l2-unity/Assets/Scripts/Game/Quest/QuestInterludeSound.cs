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

        string path;
        if (type == 1)
            path = "Data/Sounds/Music/" + stem;
        else if (stem.StartsWith("tutorial_voice", System.StringComparison.OrdinalIgnoreCase))
            path = "Data/Sounds/Voice/" + stem;
        else if (stem.Contains("/"))
            path = "Data/Sounds/" + stem.Replace('.', '/');
        else
            path = "Data/Sounds/ItemSound/" + stem;

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
