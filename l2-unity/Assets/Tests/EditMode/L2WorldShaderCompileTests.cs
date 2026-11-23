using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

public class L2WorldShaderCompileTests
{
    static readonly (ShaderCompilerPlatform Platform, BuildTarget Target)[] Targets =
    {
        (ShaderCompilerPlatform.Metal, BuildTarget.StandaloneOSX),
        (ShaderCompilerPlatform.Vulkan, BuildTarget.StandaloneLinux64),
        (ShaderCompilerPlatform.D3D, BuildTarget.StandaloneWindows64),
    };

    [Test]
    public void World_shaders_compile_for_Metal_Vulkan_and_D3D()
    {
        // Every shader under Data/Shaders, including Hidden/L2/* (ZoneFog lives here).
        // ShaderHasError / GetShaderMessages are the importer errors the editor console
        // shows. CompileVariant is per pass, so a pass that only fails when Metal
        // concatenates the shader is still reported by ShaderHasError.
        string[] guids = AssetDatabase.FindAssets("t:Shader", new[] { "Assets/Resources/Data/Shaders" });
        Assert.Greater(guids.Length, 0, "no shaders under Assets/Resources/Data/Shaders");
        var failures = new StringBuilder();
        int compiled = 0;
        bool sawZoneFog = false;
        bool sawTerrainLayer = false;
        bool sawTerrainBase = false;
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(path);
            if (shader == null)
            {
                failures.AppendLine(path + " did not load");
                continue;
            }
            if (shader.name == "Hidden/L2/ZoneFog")
                sawZoneFog = true;
            if (shader.name == "L2/World/TerrainLayerModulate2X")
                sawTerrainLayer = true;
            if (shader.name == "L2/World/TerrainBaseModulate2X")
                sawTerrainBase = true;

            if (ShaderUtil.ShaderHasError(shader))
                failures.AppendLine(path + " ShaderUtil.ShaderHasError");

            foreach (ShaderMessage message in ShaderUtil.GetShaderMessages(shader))
            {
                if (message.severity != ShaderCompilerMessageSeverity.Error)
                    continue;
                failures.AppendLine(path + ":" + message.line + " " + message.message);
            }

            ShaderData data = ShaderUtil.GetShaderData(shader);
            if (data == null)
            {
                failures.AppendLine(path + " ShaderUtil.GetShaderData returned null");
                continue;
            }
            for (int s = 0; s < data.SubshaderCount; s++)
            {
                ShaderData.Subshader sub = data.GetSubshader(s);
                for (int p = 0; p < sub.PassCount; p++)
                {
                    ShaderData.Pass pass = sub.GetPass(p);
                    for (int t = 0; t < Targets.Length; t++)
                    {
                        compiled += Compile(path, shader.name, pass, ShaderType.Vertex, Targets[t].Platform, Targets[t].Target, failures);
                        compiled += Compile(path, shader.name, pass, ShaderType.Fragment, Targets[t].Platform, Targets[t].Target, failures);
                    }
                }
            }
        }
        if (!sawZoneFog)
            failures.AppendLine("Hidden/L2/ZoneFog was not in Assets/Resources/Data/Shaders");
        if (!sawTerrainLayer || !sawTerrainBase)
            failures.AppendLine("L2/World/TerrainLayerModulate2X and TerrainBaseModulate2X were not both compiled");
        Assert.Greater(compiled, 0, "no shader variant compiled");
        if (failures.Length > 0)
            Assert.Fail(failures.ToString());
    }

    static int Compile(
        string path,
        string shaderName,
        ShaderData.Pass pass,
        ShaderType stage,
        ShaderCompilerPlatform platform,
        BuildTarget target,
        StringBuilder failures)
    {
        ShaderData.VariantCompileInfo info = pass.CompileVariant(stage, System.Array.Empty<string>(), platform, target);
        if (info.Success && (info.Messages == null || info.Messages.Length == 0))
            return 1;
        var real = new List<string>();
        bool environment = false;
        if (info.Messages != null)
        {
            for (int i = 0; i < info.Messages.Length; i++)
            {
                string text = info.Messages[i].message;
                if (CompilerMissing(text))
                    environment = true;
                else if (info.Messages[i].severity == ShaderCompilerMessageSeverity.Error)
                    real.Add(text);
            }
        }
        if (real.Count == 0 && (info.Success || environment))
            return info.Success ? 1 : 0;
        if (real.Count == 0 && !info.Success)
            real.Add("compile failed");
        for (int i = 0; i < real.Count; i++)
            failures.AppendLine(path + " " + shaderName + " " + platform + " " + stage + ": " + real[i]);
        return 0;
    }

    static bool CompilerMissing(string message)
    {
        if (string.IsNullOrEmpty(message))
            return false;
        string lower = message.ToLowerInvariant();
        return lower.Contains("shader compiler")
            || lower.Contains("not supported")
            || lower.Contains("no available")
            || lower.Contains("failed to find")
            || lower.Contains("toolchain")
            || lower.Contains("does not support");
    }
}
