using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

public static partial class L2ActorPointLights
{
    static bool Usable(L2ActorLocalLight light)
    {
        return light != null && light.IsActive;
    }

    static bool AlreadyChosen(int index, int chosen)
    {
        for (int i = 0; i < chosen; i++)
        {
            if (Chosen[i] == index)
                return true;
        }
        return false;
    }

    static void Refresh()
    {
        int previousLights = Lights.Count;
        Lights.Clear();
        L2ActorLocalLight[] found = Object.FindObjectsByType<L2ActorLocalLight>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int i = 0; i < found.Length; i++)
        {
            if (found[i] != null)
                Lights.Add(found[i]);
        }

        if (Lights.Count != previousLights)
            _rewriteActors = true;

        PruneActors();
        AdoptActors();
    }
}
