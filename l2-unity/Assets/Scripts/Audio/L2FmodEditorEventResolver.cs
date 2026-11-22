#if UNITY_EDITOR
using System;
using FMOD;
using FMODUnity;
using UnityEngine;

// Editor/batchmode bake must not call RuntimeManager.PathToGUID: that reads
// Instance.studioSystem, which is null until play-mode init. Every builder
// that stamps an EventReference onto a baked actor goes through this lookup
// of the FMOD editor event cache (EventManager.EventFromPath).
public static class L2FmodEditorEventResolver
{
    public static Func<string, GUID?> Lookup = LookupEditorCache;

    public static void ResetLookup()
    {
        Lookup = LookupEditorCache;
    }

    public static EventReference Resolve(string eventPath, string actorName)
    {
        string actor = string.IsNullOrEmpty(actorName) ? "(unnamed)" : actorName;
        if (string.IsNullOrEmpty(eventPath))
        {
            Debug.LogError("[FmodEvent] event path empty actor=" + actor);
            return new EventReference();
        }

        GUID? guid;
        try
        {
            guid = Lookup(eventPath);
        }
        catch (Exception e)
        {
            Debug.LogError(
                "[FmodEvent] editor cache lookup failed path=" + eventPath
                + " actor=" + actor + " (" + e.GetType().Name + ": " + e.Message + ")");
            return new EventReference { Path = eventPath };
        }

        if (guid == null)
        {
            Debug.LogError("[FmodEvent] event missing path=" + eventPath + " actor=" + actor);
            return new EventReference { Path = eventPath };
        }

        return new EventReference { Path = eventPath, Guid = guid.Value };
    }

    static GUID? LookupEditorCache(string eventPath)
    {
        EditorEventRef found = EventManager.EventFromPath(eventPath);
        if (found == null)
        {
            return null;
        }

        return found.Guid;
    }
}
#endif
