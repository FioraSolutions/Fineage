// Compile gate for the world shaders. A broken HLSL block is otherwise silent until
// something happens to draw with it, and then the mesh goes magenta or black.
using System.Text;
using UnityEditor;
using UnityEngine;

public static class L2ShaderCompileCheck
{
    [MenuItem("L2/Shaders/Check World Shaders")]
    public static void Check()
    {
        int failed = 0;
        var report = new StringBuilder();
        string[] guids = AssetDatabase.FindAssets("t:Shader", new[] { "Assets/Resources/Data/Shaders/World" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(path);
            if (shader == null)
            {
                report.AppendLine("missing " + path);
                failed++;
                continue;
            }

            var messages = ShaderUtil.GetShaderMessages(shader);
            bool bad = false;
            foreach (ShaderMessage message in messages)
            {
                if (message.severity != UnityEditor.Rendering.ShaderCompilerMessageSeverity.Error)
                    continue;
                bad = true;
                report.AppendLine(path + ":" + message.line + " " + message.message);
            }
            report.AppendLine((bad ? "FAIL " : "ok   ") + path);
            if (bad)
                failed++;
        }

        Debug.Log("[L2ShaderCompileCheck]\n" + report);
        if (failed > 0)
            Debug.LogError("[L2ShaderCompileCheck] " + failed + " shader(s) failed");
    }
}
