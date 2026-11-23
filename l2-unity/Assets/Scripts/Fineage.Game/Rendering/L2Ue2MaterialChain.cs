using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// One UE2 material path. Walks the committed <c>*.props.txt</c> chain
/// (Shader slots, Combiner, FinalBlend, TexPanner, TexOscillator, TexEnvMap, TexScaler)
/// and produces the parameters <see cref="L2Ue2MaterialApply"/> writes.
/// A referenced op with no props, or a property this path does not execute, is an error
/// naming the material and the op. No per-mesh rates.
/// </summary>
public static class L2Ue2MaterialChain
{
    public const string ChainShaderName = "L2/World/Ue2Chain";

    public struct UvMod
    {
        public float ScaleU;
        public float ScaleV;
        public float OffsetU;
        public float OffsetV;
        public float PanU;
        public float PanV;
        public bool Oscillate;
        public int OscUType;
        public int OscVType;
        public float OscURate;
        public float OscVRate;
        public float OscUPhase;
        public float OscVPhase;
        public float OscUAmp;
        public float OscVAmp;
        public float OscUOffset;
        public float OscVOffset;
        public float RotationTurns;
        public float RotRate;
        public int RotationType;

        public static UvMod Identity => new UvMod { ScaleU = 1f, ScaleV = 1f };
    }

    public struct UvSample
    {
        public float ScaleU;
        public float ScaleV;
        public float OffsetU;
        public float OffsetV;
    }

    public sealed class Eval
    {
        public string MaterialName;
        public string Op;
        public string DiffuseTexture;
        public string OpacityTexture;
        public string SpecularTexture;
        public string SpecularityMaskTexture;
        public string SelfIlluminationTexture;
        public string SelfIlluminationMaskTexture;
        public string DetailTexture;
        public float DetailScale = 1f;
        public string EnvTexture;
        public int EnvMapType = -1;
        public int CombineOp = -1;
        public int AlphaOp = -1;
        public string CombineTexture;
        public string CombineMaskTexture;
        public UvMod DiffuseUv = UvMod.Identity;
        public UvMod OpacityUv = UvMod.Identity;
        public UvMod SpecularUv = UvMod.Identity;
        public UvMod SelfIllumUv = UvMod.Identity;
        public UvMod CombineUv = UvMod.Identity;
        public bool AlphaTest;
        public bool HasAlphaRef;
        public int AlphaRef;
        public bool TwoSided;
        public bool HasZWrite;
        public bool ZWrite = true;
        public string OutputBlending;
        public string FrameBufferBlending;
        public Color Tint = Color.white;
        public bool ModulateStaticLighting2X = true;
        public bool ModulateSpecular2X;
        public bool PerformLightingOnSpecularPass;
        public int SrcBlend = 1;
        public int DstBlend = 0;
        public readonly List<string> Errors = new List<string>();
    }

    static readonly Regex LineRe = new Regex(
        @"^\s*([A-Za-z_][A-Za-z0-9_]*)\s*=\s*(.*?)\s*$",
        RegexOptions.Compiled);

    static readonly Regex ObjRefRe = new Regex(
        @"^([A-Za-z_][A-Za-z0-9_]*)'([^']+)'$",
        RegexOptions.Compiled);

    static readonly Regex EnumRe = new Regex(
        @"^([A-Za-z_][A-Za-z0-9_]*)\s*\(\s*(-?\d+)\s*\)$",
        RegexOptions.Compiled);

    static readonly HashSet<string> KnownOps = new HashSet<string>(StringComparer.Ordinal)
    {
        "Texture", "Shader", "Combiner", "FinalBlend", "TexPanner", "TexOscillator", "TexEnvMap", "TexScaler",
    };

    static readonly HashSet<string> ShaderProps = new HashSet<string>(StringComparer.Ordinal)
    {
        "Diffuse", "NormalMap", "Opacity", "Specular", "SpecularityMask", "SelfIllumination",
        "SelfIlluminationMask", "Detail", "DetailScale", "OutputBlending", "TwoSided", "Wireframe",
        "ModulateStaticLighting2X", "PerformLightingOnSpecularPass", "ModulateSpecular2X",
        "TreatAsTwoSided", "ZWrite", "AlphaTest", "AlphaRef", "Opacity_Bio", "HeightMap",
        "SpecularMask", "GlossinessMask", "ReflectionMask", "EmissiveMask", "SubsurfaceMask",
        "ClipMask", "FallbackMaterial", "DefaultMaterial", "SurfaceType", "bUseTextureAsHeat",
        "HeatMaterial",
    };

    static readonly HashSet<string> CombinerProps = new HashSet<string>(StringComparer.Ordinal)
    {
        "CombineOperation", "AlphaOperation", "Material1", "Material2", "Mask", "InvertMask", "Modulate2X", "Modulate4X",
    };

    static readonly HashSet<string> FinalBlendProps = new HashSet<string>(StringComparer.Ordinal)
    {
        "FrameBufferBlending", "ZWrite", "ZTest", "AlphaTest", "TwoSided", "AlphaRef", "Material",
    };

    static readonly HashSet<string> PannerProps = new HashSet<string>(StringComparer.Ordinal)
    {
        "Material", "PanDirection", "PanRate",
        "M", "TexCoordSource", "TexCoordCount", "TexCoordProjected",
        "FallbackMaterial", "DefaultMaterial", "SurfaceType", "bUseTextureAsHeat", "HeatMaterial",
    };

    // One allowlist for every Modifier subclass the structural ops do not name
    // (ColorModifier, TexRotator, TexModifier, and the rest). The op is whatever
    // the reference says; the properties that are present decide the effect.
    static readonly HashSet<string> ModifierProps = new HashSet<string>(StringComparer.Ordinal)
    {
        "Material", "Diffuse", "Color", "RenderTwoSided", "AlphaBlend",
        "PanDirection", "PanRate",
        "UScale", "VScale", "UOffset", "VOffset",
        "UOscillationRate", "VOscillationRate", "UOscillationPhase", "VOscillationPhase",
        "UOscillationAmplitude", "VOscillationAmplitude", "UOscillationType", "VOscillationType",
        "Rotation", "TexRotationType", "OscillationRate", "OscillationAmplitude", "OscillationPhase",
        "ConstantRotation",
        "TexCoordSource", "TexCoordCount", "TexCoordProjected",
        "M", "FallbackMaterial", "DefaultMaterial", "SurfaceType", "bUseTextureAsHeat", "HeatMaterial",
    };

    static readonly string[] OutputBlendNames =
    {
        "OB_Normal", "OB_Masked", "OB_Modulate", "OB_Translucent", "OB_Invisible", "OB_Brighten", "OB_Darken",
    };

    static readonly string[] CombineNames =
    {
        "CO_Use_Color_From_Material1", "CO_Use_Color_From_Material2", "CO_Multiply", "CO_Add",
        "CO_Subtract", "CO_AlphaBlend_With_Mask", "CO_Add_With_Mask_Modulation", "CO_Use_Color_From_Mask",
    };

    static readonly string[] AlphaNames =
    {
        "AO_Use_Mask", "AO_Multiply", "AO_Add", "AO_Use_Alpha_From_Material1", "AO_Use_Alpha_From_Material2",
    };

    static readonly string[] RotationNames =
    {
        "TR_FixedRotation", "TR_ConstantlyRotating", "TR_OscillatingRotation",
    };

    static readonly Regex ColorRe = new Regex(
        @"([RGBA])\s*=\s*(-?\d+)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    static readonly HashSet<string> OscProps = new HashSet<string>(StringComparer.Ordinal)
    {
        "Material", "UOscillationRate", "VOscillationRate", "UOscillationPhase", "VOscillationPhase",
        "UOscillationAmplitude", "VOscillationAmplitude", "UOscillationType", "VOscillationType",
        "UOffset", "VOffset",
        "M", "TexCoordSource", "TexCoordCount", "TexCoordProjected",
        "FallbackMaterial", "DefaultMaterial", "SurfaceType", "bUseTextureAsHeat", "HeatMaterial",
    };

    static readonly HashSet<string> EnvProps = new HashSet<string>(StringComparer.Ordinal)
    {
        "Material", "EnvMapType",
    };

    static readonly HashSet<string> ScalerProps = new HashSet<string>(StringComparer.Ordinal)
    {
        "Material", "UScale", "VScale", "UOffset", "VOffset",
        "M", "TexCoordSource", "TexCoordCount", "TexCoordProjected",
        "FallbackMaterial", "DefaultMaterial", "SurfaceType", "bUseTextureAsHeat", "HeatMaterial",
    };

    public static Eval Evaluate(string materialName, IReadOnlyDictionary<string, string> propsByName)
    {
        var eval = new Eval { MaterialName = materialName ?? "" };
        if (string.IsNullOrEmpty(materialName) || propsByName == null ||
            !TryGet(propsByName, materialName, out string stored))
        {
            Error(eval, materialName, "Shader", "missing props");
            return eval;
        }

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var ctx = new Ctx(materialName, propsByName, seen, eval);
        ExecuteRoot(ctx, materialName, Body(stored), eval);
        ApplyBlend(eval);
        return eval;
    }

    /// <summary>
    /// The world chain upgrades imported materials (Standard, SpeedTree, and other
    /// non-pipeline shaders). Sky, UI, effects, terrain splats, and water already
    /// have a client shader and must not be retargeted to another package's diffuse.
    /// </summary>
    public static bool CanUpgradeShader(string shaderName)
    {
        if (string.IsNullOrEmpty(shaderName))
            return true;
        if (shaderName.StartsWith("Hidden/L2/", StringComparison.Ordinal))
            return false;
        if (!shaderName.StartsWith("L2/", StringComparison.Ordinal))
            return true;
        if (shaderName.StartsWith("L2/Sky/", StringComparison.Ordinal)
            || shaderName.StartsWith("L2/UI/", StringComparison.Ordinal)
            || shaderName.StartsWith("L2/Effects/", StringComparison.Ordinal)
            || shaderName.StartsWith("L2/World/Terrain", StringComparison.Ordinal)
            || shaderName.StartsWith("L2/World/Water", StringComparison.Ordinal))
            return false;
        return true;
    }

    public static bool NeedsChainShader(Eval eval)
    {
        if (eval == null)
            return false;
        if (!string.IsNullOrEmpty(eval.OpacityTexture) && eval.OpacityTexture != eval.DiffuseTexture)
            return true;
        if (!string.IsNullOrEmpty(eval.SpecularTexture) || !string.IsNullOrEmpty(eval.SpecularityMaskTexture))
            return true;
        if (!string.IsNullOrEmpty(eval.SelfIlluminationTexture))
            return true;
        if (!string.IsNullOrEmpty(eval.DetailTexture))
            return true;
        if (!string.IsNullOrEmpty(eval.EnvTexture) || eval.CombineOp >= 0)
            return true;
        if (Animated(eval.DiffuseUv) || Animated(eval.OpacityUv) || Animated(eval.SpecularUv) ||
            Animated(eval.SelfIllumUv) || Animated(eval.CombineUv))
            return true;
        if (!string.IsNullOrEmpty(eval.OutputBlending) && eval.OutputBlending != "OB_Normal" && eval.OutputBlending != "OB_Masked")
            return true;
        if (!string.IsNullOrEmpty(eval.FrameBufferBlending) &&
            eval.FrameBufferBlending != "FB_Overwrite" && eval.FrameBufferBlending != "FB_AlphaBlend")
            return true;
        return false;
    }

    /// <summary>UE2 rotator yaw: 65536 units = one turn. Yaw 0 pans +U.</summary>
    public static Vector2 PanVelocity(float panRate, int yaw)
    {
        float turns = yaw / 65536f;
        float ang = turns * Mathf.PI * 2f;
        return new Vector2(Mathf.Cos(ang) * panRate, Mathf.Sin(ang) * panRate);
    }

    public static UvSample SampleUv(UvMod mod, float time)
    {
        var sample = new UvSample
        {
            ScaleU = mod.ScaleU == 0f ? 1f : mod.ScaleU,
            ScaleV = mod.ScaleV == 0f ? 1f : mod.ScaleV,
            OffsetU = mod.OffsetU + mod.PanU * time,
            OffsetV = mod.OffsetV + mod.PanV * time,
        };
        if (!mod.Oscillate)
            return sample;
        ApplyOsc(ref sample.ScaleU, ref sample.OffsetU, mod.OscUType, mod.OscURate, mod.OscUPhase, mod.OscUAmp, mod.OscUOffset, time);
        ApplyOsc(ref sample.ScaleV, ref sample.OffsetV, mod.OscVType, mod.OscVRate, mod.OscVPhase, mod.OscVAmp, mod.OscVOffset, time);
        return sample;
    }

    public static string ShortName(string reference)
    {
        if (string.IsNullOrEmpty(reference))
            return reference;
        int dot = reference.LastIndexOf('.');
        return dot >= 0 ? reference.Substring(dot + 1) : reference;
    }

    public static string PackageName(string reference)
    {
        if (string.IsNullOrEmpty(reference))
            return null;
        int dot = reference.LastIndexOf('.');
        return dot > 0 ? reference.Substring(0, dot) : null;
    }

    sealed class Ctx
    {
        public readonly string Root;
        public readonly IReadOnlyDictionary<string, string> Props;
        public readonly HashSet<string> Seen;
        public readonly Eval Eval;
        public int Depth;

        public Ctx(string root, IReadOnlyDictionary<string, string> props, HashSet<string> seen, Eval eval)
        {
            Root = root;
            Props = props;
            Seen = seen;
            Eval = eval;
        }
    }

    struct Slot
    {
        public string Texture;
        public UvMod Uv;
        public int CombineOp;
        public int AlphaOp;
        public string CombineTexture;
        public string CombineMask;
        public UvMod CombineUv;
        public int EnvType;
        public bool IsEnv;
        public Color Tint;
    }

    static Slot NewSlot()
    {
        return new Slot { Uv = UvMod.Identity, CombineOp = -1, AlphaOp = -1, EnvType = -1, Tint = Color.white };
    }

    static void ExecuteRoot(Ctx ctx, string name, string body, Eval eval)
    {
        Dictionary<string, string> props = ParseProps(body, eval, name, InferOp(body));
        string op = props.TryGetValue("Class", out string cls) && !string.IsNullOrEmpty(cls) ? cls : InferOp(body);
        eval.Op = op;
        if (op == "Modifier" || (!KnownOps.Contains(op) && op != "Shader"))
        {
            RejectUnknown(eval, op, props);
            Slot modified = ResolveModifier(ctx, props, op);
            if (TryColor(props, "Color", out Color rootTint))
                modified.Tint = Multiply(modified.Tint, rootTint);
            MergeSlot(eval, modified, asDiffuse: true);
            if (Bool(props, "RenderTwoSided"))
                eval.TwoSided = true;
            return;
        }
        RejectUnknown(eval, op, props);
        if (op == "FinalBlend")
        {
            ReadFinalBlendFlags(eval, props);
            if (props.TryGetValue("Material", out string child) && !IsNone(child))
                MergeSlot(eval, ResolveRef(ctx, child, "FinalBlend"), asDiffuse: true);
            return;
        }
        if (op != "Shader")
        {
            MergeSlot(eval, ResolveNode(ctx, name, body, op), asDiffuse: true);
            return;
        }

        ReadShaderFlags(eval, props);
        AssignShaderSlot(ctx, eval, props, "Diffuse", SlotKind.Diffuse);
        AssignShaderSlot(ctx, eval, props, "Opacity", SlotKind.Opacity);
        AssignShaderSlot(ctx, eval, props, "Specular", SlotKind.Specular);
        AssignShaderSlot(ctx, eval, props, "SpecularityMask", SlotKind.SpecMask);
        AssignShaderSlot(ctx, eval, props, "SelfIllumination", SlotKind.SelfIllum);
        AssignShaderSlot(ctx, eval, props, "SelfIlluminationMask", SlotKind.SelfIllumMask);
        AssignShaderSlot(ctx, eval, props, "Detail", SlotKind.Detail);
        if (props.TryGetValue("DetailScale", out string scaleRaw) &&
            float.TryParse(scaleRaw, NumberStyles.Float, CultureInfo.InvariantCulture, out float scale))
            eval.DetailScale = scale;
        else if (!string.IsNullOrEmpty(eval.DetailTexture))
            Error(eval, eval.MaterialName, "Shader", "property DetailScale missing");

        if (string.IsNullOrEmpty(eval.DiffuseTexture) &&
            props.TryGetValue("FallbackMaterial", out string fallback) && !IsNone(fallback))
        {
            Slot fb = ResolveRef(ctx, fallback, "Shader");
            if (!string.IsNullOrEmpty(fb.Texture))
                MergeSlot(eval, fb, asDiffuse: true);
        }
        if (props.TryGetValue("DefaultMaterial", out string def) && !IsNone(def))
            Error(eval, eval.MaterialName, "Shader", "property DefaultMaterial is set and not executed");
        FlagUnexecutedShader(eval, props);
    }

    enum SlotKind { Diffuse, Opacity, Specular, SpecMask, SelfIllum, SelfIllumMask, Detail }

    static void AssignShaderSlot(Ctx ctx, Eval eval, Dictionary<string, string> props, string key, SlotKind kind)
    {
        if (!props.TryGetValue(key, out string raw) || IsNone(raw))
            return;
        Slot slot = ResolveRef(ctx, raw, "Shader");
        switch (kind)
        {
            case SlotKind.Diffuse:
                MergeSlot(eval, slot, asDiffuse: true);
                break;
            case SlotKind.Opacity:
                eval.OpacityTexture = slot.Texture;
                eval.OpacityUv = slot.Uv;
                break;
            case SlotKind.Specular:
                if (slot.IsEnv)
                {
                    eval.EnvTexture = slot.Texture;
                    eval.EnvMapType = slot.EnvType;
                    eval.SpecularUv = slot.Uv;
                }
                else
                {
                    eval.SpecularTexture = slot.Texture;
                    eval.SpecularUv = slot.Uv;
                }
                break;
            case SlotKind.SpecMask:
                eval.SpecularityMaskTexture = slot.Texture;
                break;
            case SlotKind.SelfIllum:
                eval.SelfIlluminationTexture = slot.Texture;
                eval.SelfIllumUv = slot.Uv;
                break;
            case SlotKind.SelfIllumMask:
                eval.SelfIlluminationMaskTexture = slot.Texture;
                break;
            case SlotKind.Detail:
                eval.DetailTexture = slot.Texture;
                break;
        }
    }

    static void MergeSlot(Eval eval, Slot slot, bool asDiffuse)
    {
        if (!asDiffuse)
            return;
        eval.DiffuseTexture = slot.Texture;
        eval.DiffuseUv = slot.Uv;
        eval.Tint = Multiply(eval.Tint, slot.Tint.a == 0f && slot.Tint.maxColorComponent == 0f ? Color.white : slot.Tint);
        if (slot.CombineOp >= 0)
        {
            eval.CombineOp = slot.CombineOp;
            eval.AlphaOp = slot.AlphaOp;
            eval.CombineTexture = slot.CombineTexture;
            eval.CombineMaskTexture = slot.CombineMask;
            eval.CombineUv = slot.CombineUv;
        }
        if (slot.IsEnv)
        {
            eval.EnvTexture = slot.Texture;
            eval.EnvMapType = slot.EnvType;
        }
    }

    static Slot ResolveRef(Ctx ctx, string raw, string viaOp)
    {
        Slot empty = NewSlot();
        if (IsNone(raw))
            return empty;
        Match m = ObjRefRe.Match(raw.Trim());
        if (!m.Success)
        {
            Error(ctx.Eval, ctx.Root, viaOp, "unparsed reference '" + raw + "'");
            return empty;
        }
        string op = m.Groups[1].Value;
        string full = m.Groups[2].Value;
        string shortName = ShortName(full);
        if (op == "Texture")
        {
            empty.Texture = full;
            return empty;
        }
        // Full path first: Shaders.TexPanner0 and moon.TexPanner0 are different
        // exports. A short name is used only when that name is unique.
        bool have = TryGet(ctx.Props, full, out string stored) || TryGet(ctx.Props, shortName, out stored);
        if (!have)
        {
            if (!KnownOps.Contains(op))
                Error(ctx.Eval, ctx.Root, op, "'" + shortName + "' unknown op");
            else
                Error(ctx.Eval, ctx.Root, op, "'" + shortName + "' missing props");
            return empty;
        }
        return ResolveNode(ctx, full, Body(stored), op);
    }

    static Slot ResolveNode(Ctx ctx, string name, string body, string op)
    {
        Slot slot = NewSlot();
        if (ctx.Depth > 8 || !ctx.Seen.Add(name))
        {
            Error(ctx.Eval, ctx.Root, op, "'" + name + "' chain depth exceeded");
            return slot;
        }
        ctx.Depth++;
        Dictionary<string, string> props = ParseProps(body, ctx.Eval, name, op);
        RejectUnknown(ctx.Eval, op, props);
        switch (op)
        {
            case "TexPanner":
                slot = ResolvePanner(ctx, props);
                break;
            case "TexOscillator":
                slot = ResolveOscillator(ctx, props);
                break;
            case "TexScaler":
                slot = ResolveScaler(ctx, props);
                break;
            case "TexEnvMap":
                slot = ResolveEnv(ctx, props);
                break;
            case "Combiner":
                slot = ResolveCombiner(ctx, props);
                break;
            case "FinalBlend":
                ReadFinalBlendFlags(ctx.Eval, props);
                if (props.TryGetValue("Material", out string mat) && !IsNone(mat))
                    slot = ResolveRef(ctx, mat, "FinalBlend");
                break;
            case "Shader":
                if (props.TryGetValue("Diffuse", out string diff) && !IsNone(diff))
                    slot = ResolveRef(ctx, diff, "Shader");
                break;
            default:
                slot = ResolveModifier(ctx, props, op);
                break;
        }
        if (TryColor(props, "Color", out Color tint))
            slot.Tint = Multiply(slot.Tint, tint);
        ctx.Depth--;
        ctx.Seen.Remove(name);
        return slot;
    }

    static Slot ResolveModifier(Ctx ctx, Dictionary<string, string> props, string op)
    {
        Slot slot;
        if (props.TryGetValue("Material", out string mat) && !IsNone(mat))
            slot = ResolveRef(ctx, mat, op);
        else if (props.TryGetValue("Diffuse", out string diff) && !IsNone(diff))
            slot = ResolveRef(ctx, diff, op);
        else
        {
            Error(ctx.Eval, ctx.Root, op, "property Material missing");
            slot = NewSlot();
        }
        if (props.ContainsKey("PanRate"))
            slot = ApplyPan(ctx, slot, props, op);
        if (props.ContainsKey("UScale") || props.ContainsKey("VScale"))
            ApplyScale(ctx, ref slot, props, op);
        if (props.ContainsKey("UOscillationRate") || props.ContainsKey("VOscillationRate"))
            ApplyOscillation(ctx, ref slot, props, op);
        if (props.TryGetValue("Rotation", out string rotation) && TryYaw(rotation, out int yaw))
            slot.Uv.RotationTurns += yaw / 65536f;
        if (props.TryGetValue("TexRotationType", out string rotationType) &&
            TryNamed(rotationType, "TR_", RotationNames, out int rotationIndex, out _))
        {
            slot.Uv.RotationType = rotationIndex;
            if (rotationIndex == 1 &&
                props.TryGetValue("OscillationRate", out string rate) && TryYaw(rate, out int rateYaw))
                slot.Uv.RotRate += rateYaw / 65536f;
        }
        if (Bool(props, "RenderTwoSided"))
            ctx.Eval.TwoSided = true;
        return slot;
    }

    static Slot ApplyPan(Ctx ctx, Slot slot, Dictionary<string, string> props, string op)
    {
        if (!props.TryGetValue("PanRate", out string rateRaw) ||
            !float.TryParse(rateRaw, NumberStyles.Float, CultureInfo.InvariantCulture, out float rate))
        {
            Error(ctx.Eval, ctx.Root, op, "property PanRate missing");
            return slot;
        }
        int yaw = 0;
        if (props.TryGetValue("PanDirection", out string dirRaw) && !TryYaw(dirRaw, out yaw))
        {
            Error(ctx.Eval, ctx.Root, op, "property PanDirection missing");
            return slot;
        }
        Vector2 vel = PanVelocity(rate, yaw);
        slot.Uv.PanU += vel.x;
        slot.Uv.PanV += vel.y;
        return slot;
    }

    static void ApplyScale(Ctx ctx, ref Slot slot, Dictionary<string, string> props, string op)
    {
        if (!TryFloat(props, "UScale", out float us) || !TryFloat(props, "VScale", out float vs))
        {
            Error(ctx.Eval, ctx.Root, op, "property UScale/VScale missing");
            return;
        }
        TryFloat(props, "UOffset", out float uo);
        TryFloat(props, "VOffset", out float vo);
        slot.Uv.ScaleU *= us;
        slot.Uv.ScaleV *= vs;
        slot.Uv.OffsetU += uo;
        slot.Uv.OffsetV += vo;
    }

    static void ApplyOscillation(Ctx ctx, ref Slot slot, Dictionary<string, string> props, string op)
    {
        if (!TryFloat(props, "UOscillationRate", out float ur) || !TryFloat(props, "VOscillationRate", out float vr) ||
            !TryFloat(props, "UOscillationPhase", out float up) || !TryFloat(props, "VOscillationPhase", out float vp) ||
            !TryFloat(props, "UOscillationAmplitude", out float ua) || !TryFloat(props, "VOscillationAmplitude", out float va) ||
            !TryOscType(props, "UOscillationType", out int ut) || !TryOscType(props, "VOscillationType", out int vt))
        {
            Error(ctx.Eval, ctx.Root, op, "oscillation property missing");
            return;
        }
        TryFloat(props, "UOffset", out float uo);
        TryFloat(props, "VOffset", out float vo);
        slot.Uv.Oscillate = true;
        slot.Uv.OscUType = ut;
        slot.Uv.OscVType = vt;
        slot.Uv.OscURate = ur;
        slot.Uv.OscVRate = vr;
        slot.Uv.OscUPhase = up;
        slot.Uv.OscVPhase = vp;
        slot.Uv.OscUAmp = ua;
        slot.Uv.OscVAmp = va;
        slot.Uv.OscUOffset = uo;
        slot.Uv.OscVOffset = vo;
    }

    static Slot ResolvePanner(Ctx ctx, Dictionary<string, string> props)
    {
        Slot slot = Child(ctx, props, "TexPanner");
        if (!props.TryGetValue("PanRate", out string rateRaw) ||
            !float.TryParse(rateRaw, NumberStyles.Float, CultureInfo.InvariantCulture, out float rate))
        {
            Error(ctx.Eval, ctx.Root, "TexPanner", "property PanRate missing");
            return slot;
        }
        // The dump stores only non-default tags. PanDirection's default yaw is 0.
        int yaw = 0;
        if (props.TryGetValue("PanDirection", out string dirRaw) && !TryYaw(dirRaw, out yaw))
        {
            Error(ctx.Eval, ctx.Root, "TexPanner", "property PanDirection missing");
            return slot;
        }
        Vector2 vel = PanVelocity(rate, yaw);
        slot.Uv.PanU += vel.x;
        slot.Uv.PanV += vel.y;
        return slot;
    }

    static Slot ResolveOscillator(Ctx ctx, Dictionary<string, string> props)
    {
        Slot slot = Child(ctx, props, "TexOscillator");
        if (!TryFloat(props, "UOscillationRate", out float ur) || !TryFloat(props, "VOscillationRate", out float vr) ||
            !TryFloat(props, "UOscillationPhase", out float up) || !TryFloat(props, "VOscillationPhase", out float vp) ||
            !TryFloat(props, "UOscillationAmplitude", out float ua) || !TryFloat(props, "VOscillationAmplitude", out float va) ||
            !TryOscType(props, "UOscillationType", out int ut) || !TryOscType(props, "VOscillationType", out int vt))
        {
            Error(ctx.Eval, ctx.Root, "TexOscillator", "oscillation property missing");
            return slot;
        }
        TryFloat(props, "UOffset", out float uo);
        TryFloat(props, "VOffset", out float vo);
        slot.Uv.Oscillate = true;
        slot.Uv.OscUType = ut;
        slot.Uv.OscVType = vt;
        slot.Uv.OscURate = ur;
        slot.Uv.OscVRate = vr;
        slot.Uv.OscUPhase = up;
        slot.Uv.OscVPhase = vp;
        slot.Uv.OscUAmp = ua;
        slot.Uv.OscVAmp = va;
        slot.Uv.OscUOffset = uo;
        slot.Uv.OscVOffset = vo;
        return slot;
    }

    static Slot ResolveScaler(Ctx ctx, Dictionary<string, string> props)
    {
        Slot slot = Child(ctx, props, "TexScaler");
        if (!TryFloat(props, "UScale", out float us) || !TryFloat(props, "VScale", out float vs))
        {
            Error(ctx.Eval, ctx.Root, "TexScaler", "property UScale/VScale missing");
            return slot;
        }
        TryFloat(props, "UOffset", out float uo);
        TryFloat(props, "VOffset", out float vo);
        slot.Uv.ScaleU *= us;
        slot.Uv.ScaleV *= vs;
        slot.Uv.OffsetU += uo;
        slot.Uv.OffsetV += vo;
        return slot;
    }

    static Slot ResolveEnv(Ctx ctx, Dictionary<string, string> props)
    {
        Slot slot = Child(ctx, props, "TexEnvMap");
        if (!props.TryGetValue("EnvMapType", out string raw) || !TryEnum(raw, "EM_", out int envType, out string envName))
        {
            Error(ctx.Eval, ctx.Root, "TexEnvMap", "property EnvMapType missing");
            return slot;
        }
        if (envName != "EM_WorldSpace" && envName != "EM_CameraSpace" && envName != "EM_Sphere")
        {
            Error(ctx.Eval, ctx.Root, "TexEnvMap", "property EnvMapType '" + envName + "' unknown");
            return slot;
        }
        slot.IsEnv = true;
        slot.EnvType = envType;
        return slot;
    }

    static Slot ResolveCombiner(Ctx ctx, Dictionary<string, string> props)
    {
        Slot slot = NewSlot();
        if (!props.TryGetValue("CombineOperation", out string cop) || !TryNamed(cop, "CO_", CombineNames, out int combine, out string combineName))
        {
            Error(ctx.Eval, ctx.Root, "Combiner", "property CombineOperation missing");
            return slot;
        }
        if (!KnownCombine(combineName))
        {
            Error(ctx.Eval, ctx.Root, "Combiner", "property CombineOperation '" + combineName + "' unknown");
            return slot;
        }
        slot.CombineOp = combine;
        if (props.TryGetValue("AlphaOperation", out string aop) && TryNamed(aop, "AO_", AlphaNames, out int alpha, out string alphaName))
        {
            if (!KnownAlpha(alphaName))
                Error(ctx.Eval, ctx.Root, "Combiner", "property AlphaOperation '" + alphaName + "' unknown");
            else
                slot.AlphaOp = alpha;
        }
        Slot a = props.TryGetValue("Material1", out string m1) && !IsNone(m1) ? ResolveRef(ctx, m1, "Combiner") : slot;
        Slot b = props.TryGetValue("Material2", out string m2) && !IsNone(m2) ? ResolveRef(ctx, m2, "Combiner") : default;
        slot.Texture = a.Texture;
        slot.Uv = a.Uv;
        slot.Tint = a.Tint.a == 0f && a.Tint.maxColorComponent == 0f ? Color.white : a.Tint;
        slot.CombineOp = combine;
        slot.CombineTexture = b.Texture;
        slot.CombineUv = b.Uv.ScaleU == 0f ? UvMod.Identity : b.Uv;
        if (props.TryGetValue("Mask", out string mask) && !IsNone(mask))
        {
            Slot ms = ResolveRef(ctx, mask, "Combiner");
            slot.CombineMask = ms.Texture;
        }
        if (props.TryGetValue("InvertMask", out string inv) && inv == "true")
            Error(ctx.Eval, ctx.Root, "Combiner", "property InvertMask is set and not executed");
        if (props.TryGetValue("Modulate2X", out string m2x) && m2x == "true")
            Error(ctx.Eval, ctx.Root, "Combiner", "property Modulate2X is set and not executed");
        if (props.TryGetValue("Modulate4X", out string m4x) && m4x == "true")
            Error(ctx.Eval, ctx.Root, "Combiner", "property Modulate4X is set and not executed");
        return slot;
    }

    static Slot Child(Ctx ctx, Dictionary<string, string> props, string op)
    {
        if (!props.TryGetValue("Material", out string mat) || IsNone(mat))
        {
            Error(ctx.Eval, ctx.Root, op, "property Material missing");
            return NewSlot();
        }
        return ResolveRef(ctx, mat, op);
    }

    static void ReadShaderFlags(Eval eval, Dictionary<string, string> props)
    {
        eval.AlphaTest = Bool(props, "AlphaTest");
        if (props.TryGetValue("AlphaRef", out string ar) &&
            int.TryParse(ar, NumberStyles.Integer, CultureInfo.InvariantCulture, out int alphaRef))
        {
            eval.HasAlphaRef = true;
            eval.AlphaRef = alphaRef;
        }
        else if (eval.AlphaTest)
            Error(eval, eval.MaterialName, "Shader", "property AlphaRef missing");
        eval.TwoSided = Bool(props, "TwoSided") || Bool(props, "TreatAsTwoSided");
        if (props.ContainsKey("ZWrite"))
        {
            eval.HasZWrite = true;
            eval.ZWrite = Bool(props, "ZWrite");
        }
        if (props.TryGetValue("OutputBlending", out string blend) && TryNamed(blend, "OB_", OutputBlendNames, out _, out string blendName))
        {
            eval.OutputBlending = blendName;
            if (!KnownOutput(blendName))
                Error(eval, eval.MaterialName, "Shader", "property OutputBlending '" + blendName + "' unknown");
        }
        if (props.ContainsKey("ModulateStaticLighting2X"))
            eval.ModulateStaticLighting2X = Bool(props, "ModulateStaticLighting2X");
        eval.ModulateSpecular2X = Bool(props, "ModulateSpecular2X");
        eval.PerformLightingOnSpecularPass = Bool(props, "PerformLightingOnSpecularPass");
        if (eval.OutputBlending == "OB_Masked")
            eval.AlphaTest = true;
    }

    static void ReadFinalBlendFlags(Eval eval, Dictionary<string, string> props)
    {
        if (props.TryGetValue("FrameBufferBlending", out string blend) && TryEnum(blend, "FB_", out _, out string name))
        {
            eval.FrameBufferBlending = name;
            if (!KnownFrame(name))
                Error(eval, eval.MaterialName, "FinalBlend", "property FrameBufferBlending '" + name + "' unknown");
        }
        else
            Error(eval, eval.MaterialName, "FinalBlend", "property FrameBufferBlending missing");
        if (Bool(props, "AlphaTest"))
            eval.AlphaTest = true;
        if (props.TryGetValue("AlphaRef", out string ar) &&
            int.TryParse(ar, NumberStyles.Integer, CultureInfo.InvariantCulture, out int alphaRef))
        {
            eval.HasAlphaRef = true;
            eval.AlphaRef = alphaRef;
        }
        if (Bool(props, "TwoSided"))
            eval.TwoSided = true;
        if (props.ContainsKey("ZWrite"))
        {
            eval.HasZWrite = true;
            eval.ZWrite = Bool(props, "ZWrite");
        }
        if (props.TryGetValue("ZTest", out string ztest) && ztest != "true" && ztest != "false")
            Error(eval, eval.MaterialName, "FinalBlend", "property ZTest '" + ztest + "' unknown");
    }

    static void FlagUnexecutedShader(Eval eval, Dictionary<string, string> props)
    {
        FlagIfSet(eval, props, "NormalMap", "Shader");
        if (Bool(props, "Wireframe"))
            Error(eval, eval.MaterialName, "Shader", "property Wireframe is set and not executed");
        if (Bool(props, "bUseTextureAsHeat"))
            Error(eval, eval.MaterialName, "Shader", "property bUseTextureAsHeat is set and not executed");
        FlagStructMaterial(eval, props, "Opacity_Bio");
        FlagStructMaterial(eval, props, "HeightMap");
        FlagStructMaterial(eval, props, "SpecularMask");
        FlagStructMaterial(eval, props, "GlossinessMask");
        FlagStructMaterial(eval, props, "ReflectionMask");
        FlagStructMaterial(eval, props, "EmissiveMask");
        FlagStructMaterial(eval, props, "SubsurfaceMask");
        FlagStructMaterial(eval, props, "ClipMask");
        FlagIfSet(eval, props, "HeatMaterial", "Shader");
        if (props.TryGetValue("SurfaceType", out string surface) && surface != "0")
            Error(eval, eval.MaterialName, "Shader", "property SurfaceType is set and not executed");
    }

    static void FlagIfSet(Eval eval, Dictionary<string, string> props, string key, string op)
    {
        if (props.TryGetValue(key, out string value) && !IsNone(value))
            Error(eval, eval.MaterialName, op, "property " + key + " is set and not executed");
    }

    static void FlagStructMaterial(Eval eval, Dictionary<string, string> props, string key)
    {
        if (!props.TryGetValue(key, out string value) || value.IndexOf("Material=None", StringComparison.OrdinalIgnoreCase) >= 0)
            return;
        if (value.IndexOf("Material=", StringComparison.OrdinalIgnoreCase) >= 0)
            Error(eval, eval.MaterialName, "Shader", "property " + key + " is set and not executed");
    }

    static void ApplyBlend(Eval eval)
    {
        string frame = eval.FrameBufferBlending;
        string output = eval.OutputBlending;
        if (!string.IsNullOrEmpty(frame))
        {
            switch (frame)
            {
                case "FB_Overwrite": eval.SrcBlend = 1; eval.DstBlend = 0; break;
                case "FB_Modulate": eval.SrcBlend = 2; eval.DstBlend = 0; break;
                case "FB_AlphaBlend":
                case "FB_AlphaModulate_MightNotFogCorrectly":
                    eval.SrcBlend = 5; eval.DstBlend = 10; break;
                case "FB_Translucent": eval.SrcBlend = 5; eval.DstBlend = 10; break;
                case "FB_Darken": eval.SrcBlend = 0; eval.DstBlend = 6; break;
                case "FB_Brighten": eval.SrcBlend = 1; eval.DstBlend = 1; break;
                case "FB_Invisible": eval.SrcBlend = 0; eval.DstBlend = 1; break;
            }
            return;
        }
        switch (output)
        {
            case "OB_Modulate": eval.SrcBlend = 2; eval.DstBlend = 0; break;
            case "OB_Translucent": eval.SrcBlend = 5; eval.DstBlend = 10; break;
            case "OB_Brighten": eval.SrcBlend = 1; eval.DstBlend = 1; break;
            case "OB_Darken": eval.SrcBlend = 0; eval.DstBlend = 6; break;
            case "OB_Invisible": eval.SrcBlend = 0; eval.DstBlend = 1; break;
            default: eval.SrcBlend = 1; eval.DstBlend = 0; break;
        }
    }

    static void ApplyOsc(ref float scale, ref float offset, int type, float rate, float phase, float amp, float baseOffset, float time)
    {
        float s = Mathf.Sin(time * rate + phase);
        switch (type)
        {
            case 0: // OT_Pan
                offset += baseOffset + amp * s;
                break;
            case 1: // OT_Stretch
                scale *= baseOffset + amp * s;
                break;
            case 2: // OT_StretchRepeat
                scale *= Mathf.Abs(baseOffset + amp * s);
                break;
            case 3: // OT_Jitter — UE2 uses appFrand. This path is deterministic for the same time.
                offset += baseOffset + amp * (Frac(Mathf.Sin(time * rate + phase) * 43758.5453f) * 2f - 1f);
                break;
            default:
                offset += baseOffset + amp * s;
                break;
        }
    }

    static float Frac(float v) => v - Mathf.Floor(v);

    static bool Animated(UvMod uv)
    {
        return Mathf.Abs(uv.PanU) > 1e-8f || Mathf.Abs(uv.PanV) > 1e-8f || uv.Oscillate ||
               Mathf.Abs(uv.ScaleU - 1f) > 1e-8f || Mathf.Abs(uv.ScaleV - 1f) > 1e-8f ||
               Mathf.Abs(uv.OffsetU) > 1e-8f || Mathf.Abs(uv.OffsetV) > 1e-8f;
    }

    static Dictionary<string, string> ParseProps(string body, Eval eval, string name, string op)
    {
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        if (string.IsNullOrEmpty(body))
            return map;
        string[] lines = body.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim().TrimEnd('\r');
            if (line.Length == 0)
                continue;
            Match m = LineRe.Match(line);
            if (!m.Success)
            {
                Error(eval, eval.MaterialName ?? name, op, "unparsed line '" + line + "'");
                continue;
            }
            map[m.Groups[1].Value] = m.Groups[2].Value.Trim();
        }
        return map;
    }

    static void RejectUnknown(Eval eval, string op, Dictionary<string, string> props)
    {
        HashSet<string> known = KnownFor(op);
        if (known == null)
            return;
        foreach (var kv in props)
        {
            if (kv.Key == "Class")
                continue;
            if (!known.Contains(kv.Key))
                Error(eval, eval.MaterialName, op, "property " + kv.Key + " unknown");
        }
    }

    static HashSet<string> KnownFor(string op)
    {
        switch (op)
        {
            case "Shader": return ShaderProps;
            case "Combiner": return CombinerProps;
            case "FinalBlend": return FinalBlendProps;
            case "TexPanner": return PannerProps;
            case "TexOscillator": return OscProps;
            case "TexEnvMap": return EnvProps;
            case "TexScaler": return ScalerProps;
            default: return ModifierProps;
        }
    }

    static string InferOp(string body)
    {
        if (string.IsNullOrEmpty(body))
            return "Shader";
        if (body.IndexOf("CombineOperation", StringComparison.Ordinal) >= 0 ||
            body.IndexOf("Material1", StringComparison.Ordinal) >= 0)
            return "Combiner";
        if (body.IndexOf("FrameBufferBlending", StringComparison.Ordinal) >= 0)
            return "FinalBlend";
        if (body.IndexOf("PanRate", StringComparison.Ordinal) >= 0 ||
            body.IndexOf("PanDirection", StringComparison.Ordinal) >= 0)
            return "TexPanner";
        if (body.IndexOf("UOscillationRate", StringComparison.Ordinal) >= 0 ||
            body.IndexOf("VOscillationRate", StringComparison.Ordinal) >= 0)
            return "TexOscillator";
        if (body.IndexOf("EnvMapType", StringComparison.Ordinal) >= 0)
            return "TexEnvMap";
        if (body.IndexOf("UScale", StringComparison.Ordinal) >= 0 ||
            body.IndexOf("VScale", StringComparison.Ordinal) >= 0)
            return "TexScaler";
        // ColorModifier, TexRotator, TexModifier: a Material child and no Diffuse slot.
        if (body.IndexOf("Material =", StringComparison.Ordinal) >= 0 &&
            body.IndexOf("Diffuse =", StringComparison.Ordinal) < 0)
            return "Modifier";
        return "Shader";
    }

    static bool KnownOutput(string name)
    {
        switch (name)
        {
            case "OB_Normal":
            case "OB_Masked":
            case "OB_Modulate":
            case "OB_Translucent":
            case "OB_Invisible":
            case "OB_Brighten":
            case "OB_Darken":
                return true;
            default:
                return false;
        }
    }

    static bool KnownFrame(string name)
    {
        switch (name)
        {
            case "FB_Overwrite":
            case "FB_Modulate":
            case "FB_AlphaBlend":
            case "FB_AlphaModulate_MightNotFogCorrectly":
            case "FB_Translucent":
            case "FB_Darken":
            case "FB_Brighten":
            case "FB_Invisible":
                return true;
            default:
                return false;
        }
    }

    static bool KnownCombine(string name)
    {
        switch (name)
        {
            case "CO_Use_Color_From_Material1":
            case "CO_Use_Color_From_Material2":
            case "CO_Multiply":
            case "CO_Add":
            case "CO_Subtract":
            case "CO_AlphaBlend_With_Mask":
            case "CO_Add_With_Mask_Modulation":
            case "CO_Use_Color_From_Mask":
                return true;
            default:
                return false;
        }
    }

    static bool KnownAlpha(string name)
    {
        switch (name)
        {
            case "AO_Use_Mask":
            case "AO_Multiply":
            case "AO_Add":
            case "AO_Use_Alpha_From_Material1":
            case "AO_Use_Alpha_From_Material2":
                return true;
            default:
                return false;
        }
    }

    static bool TryOscType(Dictionary<string, string> props, string key, out int type)
    {
        type = 0;
        if (!props.TryGetValue(key, out string raw))
            return false;
        if (!TryEnum(raw, "OT_", out type, out string name))
            return false;
        return name == "OT_Pan" || name == "OT_Stretch" || name == "OT_StretchRepeat" || name == "OT_Jitter";
    }

    public static bool TryOutputBlending(string raw, out string name)
    {
        if (!TryNamed(raw, "OB_", OutputBlendNames, out _, out name))
            return false;
        return KnownOutput(name);
    }

    static bool TryNamed(string raw, string prefix, string[] names, out int index, out string name)
    {
        if (TryEnum(raw, prefix, out index, out name))
            return true;
        if (int.TryParse((raw ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out index) &&
            index >= 0 && index < names.Length)
        {
            name = names[index];
            return true;
        }
        name = null;
        index = 0;
        return false;
    }

    static bool TryColor(Dictionary<string, string> props, string key, out Color color)
    {
        color = Color.white;
        if (!props.TryGetValue(key, out string raw) || string.IsNullOrEmpty(raw))
            return false;
        int r = 255, g = 255, b = 255, a = 255;
        bool any = false;
        foreach (Match match in ColorRe.Matches(raw))
        {
            any = true;
            int value = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture) & 255;
            switch (char.ToUpperInvariant(match.Groups[1].Value[0]))
            {
                case 'R': r = value; break;
                case 'G': g = value; break;
                case 'B': b = value; break;
                case 'A': a = value; break;
            }
        }
        if (!any)
            return false;
        color = new Color(r / 255f, g / 255f, b / 255f, a / 255f);
        return true;
    }

    static Color Multiply(Color a, Color b)
    {
        return new Color(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a);
    }

    /// <summary>
    /// Evaluate the props file and every other props file in its package.
    /// Keys are group.name when the file sits in a group directory, and the
    /// bare name when that name occurs once.
    /// </summary>
    public static Eval EvaluateAt(string propsPath)
    {
        if (string.IsNullOrEmpty(propsPath) || !File.Exists(propsPath))
        {
            var missing = new Eval { MaterialName = propsPath ?? "" };
            Error(missing, propsPath, "Shader", "missing props");
            return missing;
        }
        string packageDir = PackageDirectory(propsPath);
        Dictionary<string, string> index = IndexProps(packageDir);
        return Evaluate(PropsKey(propsPath, packageDir, index), index);
    }

    static string PackageDirectory(string propsPath)
    {
        var dir = new DirectoryInfo(Path.GetDirectoryName(propsPath));
        while (dir != null && dir.Parent != null)
        {
            string parent = dir.Parent.Name;
            if (parent.Equals("Textures", StringComparison.OrdinalIgnoreCase) ||
                parent.Equals("SysTextures", StringComparison.OrdinalIgnoreCase))
                return dir.FullName;
            dir = dir.Parent;
        }
        return Path.GetDirectoryName(propsPath);
    }

    static string PropsObjectName(string file)
    {
        string fileName = Path.GetFileName(file);
        const string suffix = ".props.txt";
        if (fileName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            return fileName.Substring(0, fileName.Length - suffix.Length);
        return Path.GetFileNameWithoutExtension(fileName);
    }

    static string GroupDirectory(string file, string packageDir)
    {
        string classDir = Path.GetDirectoryName(file);
        string aboveClass = classDir == null ? null : Path.GetDirectoryName(classDir);
        if (aboveClass != null && string.Equals(Path.GetDirectoryName(aboveClass), packageDir, StringComparison.OrdinalIgnoreCase))
            return Path.GetFileName(classDir);
        return "";
    }

    static Dictionary<string, string> IndexProps(string packageDir)
    {
        var index = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        string[] files = Directory.GetFiles(packageDir, "*.props.txt", SearchOption.AllDirectories);
        var texts = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        foreach (string file in files)
        {
            string name = PropsObjectName(file);
            string text = File.ReadAllText(file);
            if (!texts.TryGetValue(name, out List<string> list))
            {
                list = new List<string>();
                texts[name] = list;
            }
            list.Add(text);
            string group = GroupDirectory(file, packageDir);
            if (!string.IsNullOrEmpty(group))
                index[group + "." + name] = text;
        }
        foreach (var kv in texts)
        {
            bool same = true;
            for (int i = 1; i < kv.Value.Count; i++)
            {
                if (kv.Value[i] != kv.Value[0])
                {
                    same = false;
                    break;
                }
            }
            // Identical nested copies keep the short name. Two different
            // TexPanner0 exports do not.
            if (same)
                index[kv.Key] = kv.Value[0];
        }
        return index;
    }

    static string PropsKey(string propsPath, string packageDir, Dictionary<string, string> index)
    {
        string name = PropsObjectName(propsPath);
        string group = GroupDirectory(propsPath, packageDir);
        if (!string.IsNullOrEmpty(group) && index.ContainsKey(group + "." + name))
            return group + "." + name;
        return name;
    }

    static bool TryEnum(string raw, string prefix, out int index, out string name)
    {
        index = 0;
        name = null;
        if (string.IsNullOrEmpty(raw))
            return false;
        Match m = EnumRe.Match(raw.Trim());
        if (!m.Success)
            return false;
        name = m.Groups[1].Value;
        index = int.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture);
        return name.StartsWith(prefix, StringComparison.Ordinal);
    }

    static bool TryYaw(string raw, out int yaw)
    {
        yaw = 0;
        if (string.IsNullOrEmpty(raw))
            return false;
        Match m = Regex.Match(raw, @"Yaw\s*=\s*(-?\d+)", RegexOptions.IgnoreCase);
        if (!m.Success)
            return false;
        yaw = int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
        return true;
    }

    static bool TryFloat(Dictionary<string, string> props, string key, out float value)
    {
        value = 0f;
        return props.TryGetValue(key, out string raw) &&
               float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    static bool Bool(Dictionary<string, string> props, string key)
    {
        return props.TryGetValue(key, out string raw) &&
               string.Equals(raw, "true", StringComparison.OrdinalIgnoreCase);
    }

    static bool IsNone(string raw)
    {
        if (string.IsNullOrEmpty(raw))
            return true;
        string t = raw.Trim();
        return t.Equals("None", StringComparison.OrdinalIgnoreCase) || t == "none";
    }

    static bool TryGet(IReadOnlyDictionary<string, string> props, string name, out string stored)
    {
        if (props is Dictionary<string, string> dict && dict.Comparer == StringComparer.OrdinalIgnoreCase)
            return dict.TryGetValue(name, out stored);
        foreach (var kv in props)
        {
            if (string.Equals(kv.Key, name, StringComparison.OrdinalIgnoreCase))
            {
                stored = kv.Value;
                return true;
            }
        }
        stored = null;
        return false;
    }

    static string Body(string stored)
    {
        if (string.IsNullOrEmpty(stored))
            return "";
        if (stored.IndexOf('=') >= 0)
            return stored;
        if (File.Exists(stored))
            return File.ReadAllText(stored);
        return stored;
    }

    static void Error(Eval eval, string material, string op, string detail)
    {
        string message = "[Ue2Material] material " + (material ?? "") + " op " + op + " " + detail;
        eval.Errors.Add(message);
        Debug.LogError(message);
    }
}
