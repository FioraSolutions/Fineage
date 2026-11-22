using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class L2Ue2MaterialChainEditModeTests
{
    [Test]
    public void Panner_yaw0_moves_positive_u()
    {
        var props = Props(
            "beam", "Diffuse = TexPanner'pan'\n",
            "pan", "PanRate = 0.05\nPanDirection = (Pitch=0,Yaw=0,Roll=0)\nMaterial = Texture'DE_V_pan'\n");
        L2Ue2MaterialChain.Eval eval = L2Ue2MaterialChain.Evaluate("beam", props);
        Assert.AreEqual(0, eval.Errors.Count);
        Assert.AreEqual("DE_V_pan", eval.DiffuseTexture);
        L2Ue2MaterialChain.UvSample uv = L2Ue2MaterialChain.SampleUv(eval.DiffuseUv, 2f);
        Assert.AreEqual(0.1f, uv.OffsetU, 1e-4f);
        Assert.AreEqual(0f, uv.OffsetV, 1e-4f);
    }

    [Test]
    public void Panner_yaw16384_moves_positive_v()
    {
        var props = Props(
            "beam", "Diffuse = TexPanner'pan'\n",
            "pan", "PanRate = 1\nPanDirection = (Pitch=0,Yaw=16384,Roll=0)\nMaterial = Texture'beamtex'\n");
        L2Ue2MaterialChain.Eval eval = L2Ue2MaterialChain.Evaluate("beam", props);
        Assert.AreEqual(0, eval.Errors.Count);
        L2Ue2MaterialChain.UvSample uv = L2Ue2MaterialChain.SampleUv(eval.DiffuseUv, 1f);
        Assert.AreEqual(0f, uv.OffsetU, 1e-4f);
        Assert.AreEqual(1f, uv.OffsetV, 1e-4f);
    }

    [Test]
    public void Specular_panner_does_not_scroll_diffuse()
    {
        var props = Props(
            "DE_statue002_s",
            "Diffuse = Texture'De_Statue002'\nSpecular = TexPanner'DE_panner_01'\n",
            "DE_panner_01",
            "PanRate = 0.05\nPanDirection = (Pitch=0,Yaw=0,Roll=0)\nMaterial = Texture'DE_V_pan'\n");
        L2Ue2MaterialChain.Eval eval = L2Ue2MaterialChain.Evaluate("DE_statue002_s", props);
        Assert.AreEqual(0, eval.Errors.Count);
        Assert.AreEqual("De_Statue002", eval.DiffuseTexture);
        Assert.AreEqual("DE_V_pan", eval.SpecularTexture);
        L2Ue2MaterialChain.UvSample diffuse = L2Ue2MaterialChain.SampleUv(eval.DiffuseUv, 2f);
        L2Ue2MaterialChain.UvSample spec = L2Ue2MaterialChain.SampleUv(eval.SpecularUv, 2f);
        Assert.AreEqual(0f, diffuse.OffsetU, 1e-5f);
        Assert.AreEqual(0.1f, spec.OffsetU, 1e-4f);
        Assert.AreEqual(0f, spec.OffsetV, 1e-4f);
    }

    [Test]
    public void Oscillator_pan_uses_sin_of_rate_times_time()
    {
        var props = Props(
            "fire", "Diffuse = TexOscillator'osc'\n",
            "osc",
            "UOscillationRate = 2\nVOscillationRate = 0\n" +
            "UOscillationPhase = 0\nVOscillationPhase = 0\n" +
            "UOscillationAmplitude = 0.25\nVOscillationAmplitude = 0\n" +
            "UOscillationType = OT_Pan (0)\nVOscillationType = OT_Pan (0)\n" +
            "UOffset = 0\nVOffset = 0\nMaterial = Texture'flame'\n");
        L2Ue2MaterialChain.Eval eval = L2Ue2MaterialChain.Evaluate("fire", props);
        Assert.AreEqual(0, eval.Errors.Count);
        L2Ue2MaterialChain.UvSample uv = L2Ue2MaterialChain.SampleUv(eval.DiffuseUv, Mathf.PI * 0.25f);
        Assert.AreEqual(0.25f, uv.OffsetU, 1e-4f);
        Assert.AreEqual("flame", eval.DiffuseTexture);
    }

    [Test]
    public void Scaler_multiplies_uv_and_adds_offset()
    {
        var props = Props(
            "planks", "Diffuse = TexScaler'scale'\n",
            "scale", "UScale = 2\nVScale = 4\nUOffset = 0.25\nVOffset = -0.5\nMaterial = Texture'plank'\n");
        L2Ue2MaterialChain.Eval eval = L2Ue2MaterialChain.Evaluate("planks", props);
        Assert.AreEqual(0, eval.Errors.Count);
        L2Ue2MaterialChain.UvSample uv = L2Ue2MaterialChain.SampleUv(eval.DiffuseUv, 0f);
        Assert.AreEqual(2f, uv.ScaleU, 1e-5f);
        Assert.AreEqual(4f, uv.ScaleV, 1e-5f);
        Assert.AreEqual(0.25f, uv.OffsetU, 1e-5f);
        Assert.AreEqual(-0.5f, uv.OffsetV, 1e-5f);
        Assert.IsTrue(L2Ue2MaterialChain.NeedsChainShader(eval));
    }

    [Test]
    public void Shader_slots_resolve_opacity_specular_and_self_illumination()
    {
        var props = Props(
            "statue",
            "Diffuse = Texture'body'\nOpacity = Texture'op'\nSpecular = Texture'spec'\n" +
            "SpecularityMask = Texture'mask'\nSelfIllumination = Texture'glow'\n" +
            "SelfIlluminationMask = Texture'glowmask'\nDetail = Texture'detail'\nDetailScale = 8\n" +
            "OutputBlending = OB_Normal (0)\nAlphaTest = false\nAlphaRef = 0\n" +
            "TwoSided = false\nZWrite = true\nModulateStaticLighting2X = true\n");
        L2Ue2MaterialChain.Eval eval = L2Ue2MaterialChain.Evaluate("statue", props);
        Assert.AreEqual(0, eval.Errors.Count);
        Assert.AreEqual("body", eval.DiffuseTexture);
        Assert.AreEqual("op", eval.OpacityTexture);
        Assert.AreEqual("spec", eval.SpecularTexture);
        Assert.AreEqual("mask", eval.SpecularityMaskTexture);
        Assert.AreEqual("glow", eval.SelfIlluminationTexture);
        Assert.AreEqual("glowmask", eval.SelfIlluminationMaskTexture);
        Assert.AreEqual("detail", eval.DetailTexture);
        Assert.AreEqual(8f, eval.DetailScale, 1e-5f);
        Assert.IsTrue(L2Ue2MaterialChain.NeedsChainShader(eval));
    }

    [Test]
    public void Combiner_multiply_reads_both_materials_and_mask()
    {
        var props = Props(
            "water", "Diffuse = Combiner'mix'\nOutputBlending = OB_Translucent (3)\n",
            "mix",
            "CombineOperation = CO_Multiply (2)\nAlphaOperation = AO_Multiply (1)\n" +
            "Material1 = Texture'water_a'\nMaterial2 = Texture'water_b'\nMask = Texture'water_m'\n");
        L2Ue2MaterialChain.Eval eval = L2Ue2MaterialChain.Evaluate("water", props);
        Assert.AreEqual(0, eval.Errors.Count);
        Assert.AreEqual(2, eval.CombineOp);
        Assert.AreEqual(1, eval.AlphaOp);
        Assert.AreEqual("water_a", eval.DiffuseTexture);
        Assert.AreEqual("water_b", eval.CombineTexture);
        Assert.AreEqual("water_m", eval.CombineMaskTexture);
        Assert.AreEqual(5, eval.SrcBlend);
        Assert.AreEqual(10, eval.DstBlend);
    }

    [Test]
    public void FinalBlend_brighten_sets_additive_and_alpha_test()
    {
        var props = Props(
            "leaf",
            "FrameBufferBlending = FB_Brighten (6)\nZWrite = false\nAlphaTest = true\nAlphaRef = 128\n" +
            "TwoSided = true\nMaterial = Shader'leaf_sh'\n",
            "leaf_sh", "Diffuse = Texture'leaftex'\nOutputBlending = OB_Normal (0)\n");
        L2Ue2MaterialChain.Eval eval = L2Ue2MaterialChain.Evaluate("leaf", props);
        Assert.AreEqual(0, eval.Errors.Count);
        Assert.AreEqual("leaftex", eval.DiffuseTexture);
        Assert.AreEqual("FB_Brighten", eval.FrameBufferBlending);
        Assert.AreEqual(1, eval.SrcBlend);
        Assert.AreEqual(1, eval.DstBlend);
        Assert.IsTrue(eval.AlphaTest);
        Assert.AreEqual(128, eval.AlphaRef);
        Assert.IsTrue(eval.TwoSided);
        Assert.IsFalse(eval.ZWrite);
    }

    [Test]
    public void EnvMap_camera_space_is_recorded()
    {
        var props = Props(
            "metal", "Specular = TexEnvMap'env'\nDiffuse = Texture'metaltex'\n",
            "env", "EnvMapType = EM_CameraSpace (1)\nMaterial = Texture'cube'\n");
        L2Ue2MaterialChain.Eval eval = L2Ue2MaterialChain.Evaluate("metal", props);
        Assert.AreEqual(0, eval.Errors.Count);
        Assert.AreEqual("cube", eval.EnvTexture);
        Assert.AreEqual(1, eval.EnvMapType);
        Assert.AreEqual("metaltex", eval.DiffuseTexture);
    }

    [Test]
    public void Unknown_op_logs_material_and_op()
    {
        var props = Props("spinmat", "Diffuse = TexRotator'spin'\n");
        LogAssert.Expect(LogType.Error, "[Ue2Material] material spinmat op TexRotator 'spin' unknown op");
        L2Ue2MaterialChain.Eval eval = L2Ue2MaterialChain.Evaluate("spinmat", props);
        Assert.AreEqual(1, eval.Errors.Count);
        Assert.IsTrue(eval.Errors[0].Contains("TexRotator"));
        Assert.IsTrue(eval.Errors[0].Contains("spinmat"));
    }

    [Test]
    public void Unknown_property_logs_material_and_op()
    {
        var props = Props("brickmat", "Diffuse = Texture'brick'\nNotARealProp = 1\n");
        LogAssert.Expect(LogType.Error, "[Ue2Material] material brickmat op Shader property NotARealProp unknown");
        L2Ue2MaterialChain.Eval eval = L2Ue2MaterialChain.Evaluate("brickmat", props);
        Assert.AreEqual(1, eval.Errors.Count);
    }

    [Test]
    public void Missing_panner_props_logs_material_and_op()
    {
        var props = Props("statue", "Specular = TexPanner'DE_panner_01'\nDiffuse = Texture'De_Statue002'\n");
        LogAssert.Expect(LogType.Error, "[Ue2Material] material statue op TexPanner 'DE_panner_01' missing props");
        L2Ue2MaterialChain.Eval eval = L2Ue2MaterialChain.Evaluate("statue", props);
        Assert.AreEqual("De_Statue002", eval.DiffuseTexture);
        Assert.IsNull(eval.SpecularTexture);
        Assert.AreEqual(1, eval.Errors.Count);
    }

    [Test]
    public void Committed_statue_shader_names_the_missing_panner()
    {
        string path = Path.Combine(Application.dataPath, "Resources/Data/Textures/oren_dev_t/Shader/DE_statue002_s.props.txt");
        Assert.IsTrue(File.Exists(path), path);
        var props = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "DE_statue002_s", File.ReadAllText(path) },
        };
        LogAssert.Expect(LogType.Error, "[Ue2Material] material DE_statue002_s op TexPanner 'DE_panner_01' missing props");
        L2Ue2MaterialChain.Eval eval = L2Ue2MaterialChain.Evaluate("DE_statue002_s", props);
        Assert.AreEqual("De_Statue002", eval.DiffuseTexture);
        Assert.AreEqual("DE_V_cube_mask01", eval.SpecularityMaskTexture);
        Assert.IsTrue(L2Ue2MaterialChain.NeedsChainShader(eval));
        Assert.AreEqual(1, eval.Errors.Count);
    }

    [Test]
    public void Committed_masked_shader_has_no_errors()
    {
        string path = Path.Combine(Application.dataPath, "Resources/Data/Textures/V_Obj_T/Materials/O_rope_L01.props.txt");
        Assert.IsTrue(File.Exists(path), path);
        var props = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "O_rope_L01", File.ReadAllText(path) },
        };
        L2Ue2MaterialChain.Eval eval = L2Ue2MaterialChain.Evaluate("O_rope_L01", props);
        Assert.AreEqual(0, eval.Errors.Count);
        Assert.AreEqual("Speaking_Town_T.O_rope_L", eval.DiffuseTexture);
        Assert.AreEqual("OB_Masked", eval.OutputBlending);
        Assert.IsTrue(eval.AlphaTest);
        Assert.AreEqual(0, eval.AlphaRef);
        Assert.IsTrue(eval.TwoSided);
        Assert.IsFalse(eval.ZWrite);
    }

    [Test]
    public void Texture_index_uses_the_only_match_and_errors_when_ambiguous()
    {
        var names = new List<string> { "O_rope_L", "shared", "shared" };
        var packages = new List<string> { "V_Obj_T", "A", "B" };
        var paths = new List<string> { "Data/Textures/V_Obj_T/O_rope_L", "Data/Textures/A/shared", "Data/Textures/B/shared" };
        var errors = new List<string>();
        string unique = L2Ue2MaterialApply.PickTexturePath(names, packages, paths, "Speaking_Town_T.O_rope_L", errors, "O_rope_L01");
        Assert.AreEqual("Data/Textures/V_Obj_T/O_rope_L", unique);
        Assert.AreEqual(0, errors.Count);

        string picked = L2Ue2MaterialApply.PickTexturePath(names, packages, paths, "A.shared", errors, "mat");
        Assert.AreEqual("Data/Textures/A/shared", picked);
        Assert.AreEqual(0, errors.Count);

        string ambiguous = L2Ue2MaterialApply.PickTexturePath(names, packages, paths, "shared", errors, "mat");
        Assert.IsNull(ambiguous);
        Assert.AreEqual(1, errors.Count);
        Assert.IsTrue(errors[0].Contains("ambiguous"));
        Assert.IsTrue(errors[0].Contains("mat"));

        errors.Clear();
        string missing = L2Ue2MaterialApply.PickTexturePath(names, packages, paths, "nope", errors, "mat");
        Assert.IsNull(missing);
        Assert.IsTrue(errors[0].Contains("missing"));
    }

    [Test]
    public void Chain_does_not_upgrade_sky_or_terrain_shaders()
    {
        Assert.IsFalse(L2Ue2MaterialChain.CanUpgradeShader("L2/Sky/Cloud"));
        Assert.IsFalse(L2Ue2MaterialChain.CanUpgradeShader("L2/World/TerrainBaseModulate2X"));
        Assert.IsFalse(L2Ue2MaterialChain.CanUpgradeShader("L2/World/WaterModulate2X"));
        Assert.IsFalse(L2Ue2MaterialChain.CanUpgradeShader("L2/UI/BitmapFontScreenSpace"));
        Assert.IsTrue(L2Ue2MaterialChain.CanUpgradeShader("Standard"));
        Assert.IsTrue(L2Ue2MaterialChain.CanUpgradeShader("L2/World/StaticMeshModulate2X"));
    }

    [Test]
    public void Chain_refuses_a_diffuse_from_another_package()
    {
        var names = new List<string> { "Cloud_Final01", "grass010" };
        var packages = new List<string> { "L2_Skies", "field_deco_T" };
        Assert.IsTrue(L2Ue2MaterialApply.TexturePackageAgrees(
            "Cloud_Final01", "L2_Skies.Cloud_Final01", names, packages, out _));
        Assert.IsFalse(L2Ue2MaterialApply.TexturePackageAgrees(
            "Cloud_Final01", "field_deco_T.grass010", names, packages, out string detail));
        Assert.IsTrue(detail.Contains("L2_Skies"));
        Assert.IsTrue(detail.Contains("field_deco_T"));
        Assert.IsTrue(L2Ue2MaterialApply.TexturePackageAgrees(
            null, "field_deco_T.grass010", names, packages, out _));
    }

    static Dictionary<string, string> Props(params string[] pairs)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i + 1 < pairs.Length; i += 2)
            map[pairs[i]] = pairs[i + 1];
        return map;
    }
}
