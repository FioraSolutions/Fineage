using System.Collections.Generic;
using UnityEngine;

public sealed partial class NpcDecoCatalog
{
    public static string ShortName(string decoEffectName)
    {
        if (string.IsNullOrWhiteSpace(decoEffectName))
            return string.Empty;

        int dot = decoEffectName.LastIndexOf('.');
        return dot >= 0 && dot < decoEffectName.Length - 1
            ? decoEffectName.Substring(dot + 1)
            : decoEffectName;
    }

    static BaseEffect LoadAt(string resourcesPath)
    {
        BaseEffect typed = Resources.Load<BaseEffect>(resourcesPath);
        if (typed != null)
            return typed;

        GameObject go = Resources.Load<GameObject>(resourcesPath);
        return go != null ? go.GetComponent<BaseEffect>() : null;
    }
}
