using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public static partial class L2ActorLightCapture
{
    static int DescribeSlot(StringBuilder text, int slot, Material material)
    {
        if (material == null)
        {
            text.AppendLine("slot" + slot + "=null");
            return 0;
        }

        string shader = material.shader != null ? material.shader.name : "";
        Texture baseMap = material.HasProperty("_BaseMap") ? material.GetTexture("_BaseMap") : null;
        Texture spec = material.HasProperty("_SpecMask") ? material.GetTexture("_SpecMask") : null;
        Texture env = material.HasProperty("_EnvMap") ? material.GetTexture("_EnvMap") : null;
        float cutoff = material.HasProperty("_Cutoff") ? material.GetFloat("_Cutoff") : -1f;
        float cull = material.HasProperty("_Cull") ? material.GetFloat("_Cull") : -1f;
        float alphaClip = material.HasProperty("_AlphaClip") ? material.GetFloat("_AlphaClip") : -1f;
        bool keyword = material.IsKeywordEnabled("_ALPHATEST_ON");
        string baseName = baseMap != null ? baseMap.name : "";
        string specName = spec != null ? spec.name : "";
        string envName = env != null ? env.name : "";

        bool envSlot = shader == EnvShader;
        bool textures;
        if (slot == 0)
            textures = Contains(baseName, "t00_ori") && Contains(specName, "t00_sp") && Contains(envName, "n3gold01");
        else if (slot == 1)
            textures = Contains(baseName, "t01_ori") && Contains(specName, "t01_sp") && Contains(envName, "n3gold01");
        else
            textures = true;

        bool flags = envSlot
            && keyword
            && alphaClip > 0.5f
            && Mathf.Abs(cull) < 0.01f
            && Mathf.Abs(cutoff - ExpectedCutoff) < 0.002f;
        bool ok = envSlot && textures && flags;

        text.AppendLine("slot" + slot +
            " material=" + material.name +
            " shader=" + shader +
            " keyword=" + (keyword ? "1" : "0") +
            " cutoff=" + Num(cutoff) +
            " cull=" + Num(cull) +
            " alphaClip=" + Num(alphaClip) +
            " queue=" + material.renderQueue +
            " base=" + baseName +
            " spec=" + specName +
            " env=" + envName +
            " ok=" + (ok ? "1" : "0"));
        if (!envSlot && slot > 1)
            return 1;
        return ok ? 1 : 0;
    }
}
