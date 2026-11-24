using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Fineage.TerrainBsp
{
    /// <summary>
    /// Per-tile list of BSP faces the carver drops and static-mesh materials whose original
    /// TwoSided flag is still backface-culled. Mesh paths match <c>L2MetaDataUtils.GetFolderAndFileFromInfo</c>.
    /// </summary>
    public static class L2TileFaceAudit
    {
        public sealed class Record
        {
            public string tile;
            public string kind;
            public string subject;
            public string detail;
        }

        static readonly Regex TwoSidedLine = new Regex(
            @"^\s*(TwoSided|TreatAsTwoSided)\s*=\s*true",
            RegexOptions.Multiline);
        static readonly Regex CullLine = new Regex(@"- _Cull:\s*([0-9.]+)");

        public static List<Record> AuditTile(string dataRoot, string tileId)
        {
            var list = new List<Record>();
            if (string.IsNullOrEmpty(dataRoot) || string.IsNullOrEmpty(tileId))
            {
                list.Add(Row(tileId, "missing-tile", tileId, "data root or tile id missing"));
                return list;
            }

            string t3d = Path.Combine(dataRoot, "Maps", tileId, "Meta", tileId + ".t3d");
            if (!File.Exists(t3d))
                list.Add(Row(tileId, "missing-t3d", t3d, "tile t3d missing"));
            else
                AuditBrushes(tileId, L2T3DBrushParser.ParseFile(t3d), list);

            string individuals = Path.Combine(dataRoot, "Maps", tileId, "Meta", "StaticMeshActorIndividuals.txt");
            if (!File.Exists(individuals))
            {
                list.Add(Row(tileId, "missing-individuals", individuals, "StaticMeshActorIndividuals.txt missing"));
                return list;
            }

            var packages = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string raw in File.ReadAllLines(individuals))
            {
                string meshRef = raw.Trim();
                if (meshRef.Length == 0 || meshRef.StartsWith("#", StringComparison.Ordinal))
                    continue;
                if (!TryMeshFile(dataRoot, meshRef, out string meshPath, out string error))
                {
                    list.Add(Row(tileId, "missing-mesh", meshRef, error));
                    continue;
                }
                if (!File.Exists(meshPath))
                    list.Add(Row(tileId, "missing-mesh", meshRef, "file missing " + meshPath));
                else if (TryPackage(meshRef, out string package))
                    packages.Add(package);
            }

            Dictionary<string, bool> twoSided = LoadTwoSided(dataRoot);
            foreach (string package in packages)
                AuditPackageMaterials(tileId, dataRoot, package, twoSided, list);
            return list;
        }

        public static void AuditBrushes(string tileId, IReadOnlyList<L2BspBrush> brushes, List<Record> list)
        {
            if (brushes == null)
                return;
            var portals = L2BspCsgCarver.CollectPortalCaps(brushes);
            for (int i = 0; i < brushes.Count; i++)
            {
                L2BspBrush brush = brushes[i];
                string skip = L2BspCsgCarver.UnusableReason(brush);
                if (skip != null)
                {
                    list.Add(Row(tileId, "skipped-brush", brush != null ? brush.name : "null", skip));
                    continue;
                }
                if (string.Equals(brush.csgOper, L2BspCsgCarver.CsgSubtract, StringComparison.Ordinal))
                    continue;
                if (!string.Equals(brush.csgOper, L2BspCsgCarver.CsgAdd, StringComparison.Ordinal))
                {
                    list.Add(Row(tileId, "unknown-csg", brush.name, brush.csgOper ?? "null"));
                    continue;
                }

                Vector3 basePos = brush.position - brush.prePivot;
                var subtracts = new List<L2BspAabb>();
                for (int later = i + 1; later < brushes.Count; later++)
                    L2BspCsgCarver.AccumulateSubtract(brushes[later], subtracts);
                L2BspPoly[] polys = brush.polygons ?? Array.Empty<L2BspPoly>();
                for (int p = 0; p < polys.Length; p++)
                {
                    L2BspPoly poly = polys[p];
                    if (poly == null || poly.vertices == null || poly.vertices.Length < 3)
                        continue;
                    if (!L2BspPolyFlags.ShouldRenderPolygon(poly.polyFlags, poly.texture))
                        continue;
                    if (!L2BspPolyFlags.IsBrushTextureRenderable(poly.texture))
                        continue;
                    List<Vector3[]> pieces = L2BspCsgCarver.CarveFacePieces(brush, poly, basePos, portals, subtracts);
                    if (pieces != null && pieces.Count == 0)
                    {
                        list.Add(Row(tileId, "carved-away", brush.name,
                            (poly.texture ?? "no-texture") + " face " + p));
                    }
                }
            }
        }

        static void AuditPackageMaterials(
            string tileId,
            string dataRoot,
            string package,
            Dictionary<string, bool> twoSided,
            List<Record> list)
        {
            string[] dirs =
            {
                Path.Combine(dataRoot, "StaticMeshes", package, "Materials"),
                Path.Combine(dataRoot, "Textures", package, "Materials"),
                Path.Combine(dataRoot, "Textures", package, "Shader"),
            };
            for (int d = 0; d < dirs.Length; d++)
            {
                if (!Directory.Exists(dirs[d]))
                    continue;
                string[] mats = Directory.GetFiles(dirs[d], "*.mat");
                for (int i = 0; i < mats.Length; i++)
                {
                    string name = Path.GetFileNameWithoutExtension(mats[i]);
                    if (!twoSided.TryGetValue(name, out bool sided) || !sided)
                        continue;
                    string body = File.ReadAllText(mats[i]);
                    Match cull = CullLine.Match(body);
                    if (!cull.Success)
                    {
                        list.Add(Row(tileId, "culled-two-sided", name, mats[i] + " has no _Cull"));
                        continue;
                    }
                    if (!float.TryParse(cull.Groups[1].Value, System.Globalization.NumberStyles.Float,
                            System.Globalization.CultureInfo.InvariantCulture, out float value))
                        continue;
                    if (value > 0.5f)
                        list.Add(Row(tileId, "culled-two-sided", name, mats[i] + " cull=" + value.ToString("0.###")));
                }
            }
        }

        static Dictionary<string, bool> LoadTwoSided(string dataRoot)
        {
            var map = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
            var clash = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string[] roots =
            {
                Path.Combine(dataRoot, "Textures"),
                Path.Combine(dataRoot, "SysTextures"),
                Path.Combine(dataRoot, "StaticMeshes"),
            };
            for (int r = 0; r < roots.Length; r++)
            {
                if (!Directory.Exists(roots[r]))
                    continue;
                foreach (string file in Directory.EnumerateFiles(roots[r], "*.props.txt", SearchOption.AllDirectories))
                {
                    string name = Path.GetFileName(file);
                    if (name.EndsWith(".props.txt", StringComparison.OrdinalIgnoreCase))
                        name = name.Substring(0, name.Length - ".props.txt".Length);
                    if (clash.Contains(name))
                        continue;
                    bool sided = TwoSidedLine.IsMatch(File.ReadAllText(file));
                    if (map.TryGetValue(name, out bool existing) && existing != sided)
                    {
                        map.Remove(name);
                        clash.Add(name);
                        continue;
                    }
                    map[name] = sided;
                }
            }
            return map;
        }

        /// <summary>Same split as the static-mesh importer: package is segment 0, file is segment 2 when the ref has a group.</summary>
        public static bool TryMeshFile(string dataRoot, string meshRef, out string fullPath, out string error)
        {
            fullPath = null;
            error = null;
            if (!TryPackageAndFile(meshRef, out string package, out string file))
            {
                error = "mesh ref \"" + meshRef + "\" is not package.mesh";
                return false;
            }
            fullPath = Path.Combine(dataRoot, "StaticMeshes", package, file + ".fbx");
            return true;
        }

        public static bool TryPackage(string meshRef, out string package)
        {
            package = null;
            return TryPackageAndFile(meshRef, out package, out _);
        }

        static bool TryPackageAndFile(string meshRef, out string package, out string file)
        {
            package = null;
            file = null;
            if (string.IsNullOrEmpty(meshRef))
                return false;
            string[] parts = meshRef.Split('.');
            if (parts.Length == 2)
            {
                package = parts[0];
                file = parts[1];
                return package.Length > 0 && file.Length > 0;
            }
            if (parts.Length > 2)
            {
                package = parts[0];
                file = parts[2];
                return package.Length > 0 && file.Length > 0;
            }
            return false;
        }

        static Record Row(string tile, string kind, string subject, string detail)
        {
            return new Record { tile = tile ?? "", kind = kind, subject = subject ?? "", detail = detail ?? "" };
        }
    }
}
