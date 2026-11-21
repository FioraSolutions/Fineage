using System;

/// <summary>Shared logging when meta table singletons fall back off DI or missing StreamingAssets files.</summary>
public static class MetaTableBootstrapLog
{
    public static void LogDiResolveFailed(string tableName, Exception ex)
    {
        FineageLog.Error(
            FineageLogCategory.World,
            "MetaTable " + tableName + ": VContainer resolve failed — " + (ex?.Message ?? "unknown"));
    }

    public static void LogEmptyInstanceFallback(string tableName)
    {
        FineageLog.Error(
            FineageLogCategory.World,
            "MetaTable " + tableName + ": using non-DI empty instance; check GameLifetimeScope table registration");
    }

    public static void LogMissingDatFile(string tableName, string path)
    {
        FineageLog.Error(
            FineageLogCategory.World,
            "MetaTable " + tableName + ": missing file at " + path);
    }
}