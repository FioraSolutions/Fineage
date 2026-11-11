using UnityEngine;

public static partial class L2FxAlphaBlendDrawOrder
{
    public static void Apply(Component effectRoot)
    {
        if (effectRoot == null)
        {
            return;
        }

        ParticleGroupV2[] groups = effectRoot.GetComponentsInChildren<ParticleGroupV2>(true);
        if (groups == null || groups.Length < 2)
        {
            return;
        }

        AddLocationFromOtherEmitterProvider[] trails =
            effectRoot.GetComponentsInChildren<AddLocationFromOtherEmitterProvider>(true);

        bool anyPuff = false;
        bool anyCore = false;
        var isPuff = new bool[groups.Length];
        for (int i = 0; i < groups.Length; i++)
        {
            isPuff[i] = IsPuffGroup(groups[i], trails);
            if (isPuff[i])
            {
                anyPuff = true;
            }
            else
            {
                anyCore = true;
            }
        }

        if (!anyPuff || !anyCore)
        {
            return;
        }

        for (int i = 0; i < groups.Length; i++)
        {
            SetGroupSorting(
                groups[i],
                isPuff[i] ? PuffSortingOrder : CoreSortingOrder);
        }
    }

    public static bool IsPuffGroup(ParticleGroupV2 group, AddLocationFromOtherEmitterProvider[] trails)
    {
        if (group == null)
        {
            return false;
        }

        if (trails != null)
        {
            for (int i = 0; i < trails.Length; i++)
            {
                if (trails[i] != null && trails[i].ContainsTail(group))
                {
                    return true;
                }
            }
        }

        return HasBlendBetweenFlipbook(group);
    }
}
