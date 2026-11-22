using System;
using System.Collections.Generic;

namespace Fineage.TerrainBsp
{
    /// <summary>UE2 BSP PolyFlags bitmask decode (matches L2MetaDataUtils).</summary>
    public static class L2BspPolyFlags
    {
        public const int Invisible = 1;
        public const int Masked = 2;
        public const int Translucent = 4;
        public const int NotSolid = 8;
        public const int Semisolid = 32;
        public const int TwoSided = 256;
        public const int Unlit = 4194304;
        public const int Portal = 67108864;

        static readonly (string name, int bit)[] Table =
        {
            ("PF_Invisible", Invisible),
            ("PF_Masked", Masked),
            ("PF_Translucent", Translucent),
            ("PF_NotSolid", NotSolid),
            ("PF_Semisolid", Semisolid),
            ("PF_TwoSided", TwoSided),
            ("PF_Unlit", Unlit),
            ("PF_Portal", Portal),
        };

        public static string[] Decode(int value)
        {
            if (value == 0)
                return new[] { "PF_Default" };
            var list = new List<string>();
            foreach (var (name, bit) in Table)
            {
                if ((value & bit) != 0)
                    list.Add(name);
            }
            return list.ToArray();
        }

        public static bool Contains(string[] flags, string name)
        {
            if (flags == null)
                return false;
            for (int i = 0; i < flags.Length; i++)
            {
                if (string.Equals(flags[i], name, StringComparison.Ordinal))
                    return true;
            }
            return false;
        }

        public static bool ShouldRenderPolygon(string[] polyFlags, string texture)
        {
            if (Contains(polyFlags, "PF_Invisible"))
                return false;
            if (Contains(polyFlags, "PF_NotSolid"))
                return false;
            if (Contains(polyFlags, "PF_Portal"))
                return false;
            if (IsEtcPortalCap(texture))
                return false;
            return true;
        }

        public static bool ShouldRenderBrush(string[] brushPolyFlags)
        {
            if (Contains(brushPolyFlags, "PF_Invisible"))
                return false;
            if (Contains(brushPolyFlags, "PF_NotSolid"))
                return false;
            return true;
        }

        public static bool ShouldCollide(string[] polyFlags)
        {
            if (Contains(polyFlags, "PF_NotSolid"))
                return false;
            if (Contains(polyFlags, "PF_Semisolid"))
                return false;
            return true;
        }

        public static bool IsPortalCapTexture(string texture)
        {
            if (string.IsNullOrEmpty(texture))
                return false;
            if (texture.IndexOf("AntiPortal", StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            if (texture.IndexOf("ZonePortal", StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            return false;
        }

        static bool IsEtcPortalCap(string texture) => IsPortalCapTexture(texture);

        public static bool IsBrushTextureRenderable(string texture)
        {
            return IsBrushTextureRenderable(texture, includeSky: false);
        }

        // World tiles exclude L2_Skies brushes (their sky comes from the sky path,
        // not BSP). The lobby's skybox IS a BSP box with L2_Skies faces, so the
        // lobby bake passes includeSky: true; every other exclusion stays.
        public static bool IsBrushTextureRenderable(string texture, bool includeSky)
        {
            if (string.IsNullOrEmpty(texture))
                return false;
            if (!includeSky && texture.StartsWith("L2_Skies", StringComparison.OrdinalIgnoreCase))
                return false;
            if (texture.StartsWith("L2_ETC_Textures", StringComparison.OrdinalIgnoreCase))
                return false;
            if (texture.StartsWith("Engine.", StringComparison.OrdinalIgnoreCase))
                return false;
            if (texture.IndexOf("Water", StringComparison.OrdinalIgnoreCase) >= 0)
                return false;
            return true;
        }
    }
}