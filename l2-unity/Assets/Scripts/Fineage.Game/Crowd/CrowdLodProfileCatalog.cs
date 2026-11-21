using UnityEngine;

public static class CrowdLodProfileCatalog
{
    public const string ResourcesPath = "Settings/CrowdLodProfile";

    static CrowdLodProfile _builtin;

    public static CrowdLodProfile LoadActive()
    {
        CrowdLodProfile loaded = Resources.Load<CrowdLodProfile>(ResourcesPath);
        if (loaded != null)
            return loaded;

        FineageLog.Error(
            FineageLogCategory.World,
            "CrowdLodProfile missing at Resources/" + ResourcesPath
                + "; using built-in tier distances (25/60/120 m). Add the asset to override.");

        if (_builtin == null)
        {
            _builtin = ScriptableObject.CreateInstance<CrowdLodProfile>();
            _builtin.hideNameplateFromTier = CrowdDetailTier.T3Far;
        }

        return _builtin;
    }
}