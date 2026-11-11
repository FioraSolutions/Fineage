using UnityEngine;
using System.Collections.Generic;

public static partial class CharTemplateRegistry
{
    static bool TryResolveModel(int classId, out BaseStatModel model)
    {
        if (Classes.TryGetValue(classId, out model))
            return true;

        int fallbackId = CharacterClassParser.IsMage(classId)
            ? (int)ClassId.MAGE
            : (int)ClassId.FIGHTER;
        return Classes.TryGetValue(fallbackId, out model);
    }
}
