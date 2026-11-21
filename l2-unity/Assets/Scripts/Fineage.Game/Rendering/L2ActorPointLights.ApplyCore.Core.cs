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
        PruneActorGroups();
        BootstrapSceneActorsIfNeeded();
    }
}
