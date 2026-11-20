#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Applies shared import rules from <c>ImportPolicy/asset-import-rules.json</c>.
/// </summary>
public sealed class FineageAssetImportPostprocessor : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        string path = assetPath.Replace('\\', '/');
        var rule = FineageAssetImportRules.ResolveTextureRule(path);
        if (rule == null)
        {
            Debug.LogError("Fineage import policy: no texture rule matched path=" + path);
            return;
        }

        var expectation = FineageAssetImportRules.BuildTextureExpectation(rule);
        if (expectation == null)
            return;

        var importer = (TextureImporter)assetImporter;
        FineageAssetImportRules.ApplyTextureRule(importer, expectation);
    }

    void OnPreprocessAudio()
    {
        string path = assetPath.Replace('\\', '/');
        var rule = FineageAssetImportRules.ResolveAudioRule(path);
        if (rule == null)
        {
            Debug.LogError("Fineage import policy: no audio rule matched path=" + path);
            return;
        }

        var expectation = FineageAssetImportRules.BuildAudioExpectation(rule, path);
        FineageAssetImportRules.ApplyAudioRule((AudioImporter)assetImporter, expectation);
    }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        var rules = FineageAssetImportRules.Load();
        if (rules?.requiredReadableAssets == null)
            return;

        foreach (var required in rules.requiredReadableAssets)
        {
            if (string.IsNullOrEmpty(required.assetPath))
                continue;
            string full = System.IO.Path.GetFullPath(
                System.IO.Path.Combine(Application.dataPath, "..", required.assetPath));
            string text = System.IO.File.Exists(full) ? System.IO.File.ReadAllText(full) : null;
            if (text == null)
            {
                Debug.LogError(
                    "Fineage import policy: required readable asset missing path="
                    + required.assetPath
                    + " reason="
                    + required.reason);
                continue;
            }

            string needle = "  " + required.yamlField + ": " + required.expectedValue;
            if (!text.Contains(needle))
            {
                Debug.LogError(
                    "Fineage import policy: required "
                    + required.yamlField
                    + "="
                    + required.expectedValue
                    + " not set on "
                    + required.assetPath
                    + " ("
                    + required.reason
                    + ")");
            }
        }

        foreach (string path in importedAssets)
        {
            string normalized = path.Replace('\\', '/');
            if (!normalized.EndsWith(".png") && !normalized.EndsWith(".jpg") && !normalized.EndsWith(".jpeg")
                && !normalized.EndsWith(".tga") && !normalized.EndsWith(".psd"))
                continue;
            var rule = FineageAssetImportRules.ResolveTextureRule(normalized);
            if (rule == null || !rule.readable)
                continue;
            var tex = AssetImporter.GetAtPath(normalized) as TextureImporter;
            if (tex != null && !tex.isReadable)
            {
                Debug.LogError(
                    "Fineage import policy: texture rule "
                    + rule.id
                    + " expects readable but isReadable=false path="
                    + normalized);
            }
        }
    }
}
#endif