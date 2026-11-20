using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace Fineage.TerrainBsp
{
  /// <summary>Parse Interlude BrushDump.java T3D brush blocks (file order preserved).</summary>
  public static class L2T3DBrushParser
  {
    public static List<L2BspBrush> ParseFile(string t3dPath)
    {
      var list = new List<L2BspBrush>();
      using var reader = new StreamReader(t3dPath);
      string line;
      while ((line = reader.ReadLine()) != null)
      {
        line = line.Trim();
        if (!line.StartsWith("Begin Actor Class=Brush Name=", StringComparison.Ordinal))
          continue;
        var brush = new L2BspBrush
        {
          name = line.Replace("Begin Actor Class=Brush Name=", ""),
          polyFlags = new[] { "PF_Default" }
        };
        var polys = new List<L2BspPoly>();
        while ((line = reader.ReadLine()) != null && !line.Contains("End Actor"))
        {
          line = line.Trim();
          if (line.StartsWith("CsgOper=", StringComparison.Ordinal))
            brush.csgOper = line.Replace("CsgOper=", "");
          else if (line.StartsWith("PolyFlags=", StringComparison.Ordinal))
          {
            int pf = int.Parse(line.Replace("PolyFlags=", ""), CultureInfo.InvariantCulture);
            brush.polyFlags = L2BspPolyFlags.Decode(pf);
          }
          else if (line.StartsWith("Location=", StringComparison.Ordinal))
            brush.position = ParseVector3Line(line);
          else if (line.StartsWith("PrePivot=", StringComparison.Ordinal))
            brush.prePivot = ParseVector3Line(line);
          else if (line.StartsWith("Begin Polygon", StringComparison.Ordinal))
          {
            var poly = ParsePolygonLine(line);
            var verts = new List<Vector3>();
            while ((line = reader.ReadLine()) != null && !line.Contains("End Polygon"))
            {
              line = line.Trim();
              if (line.Contains("Origin"))
                poly.origin = ParseVector3Triplet(line.Replace("Origin", "").Trim());
              else if (line.Contains("TextureU"))
                poly.textureU = ParseVector3Triplet(line.Replace("TextureU", "").Trim());
              else if (line.Contains("TextureV"))
                poly.textureV = ParseVector3Triplet(line.Replace("TextureV", "").Trim());
              else if (line.Contains("Normal"))
                poly.normal = ParseVector3Triplet(line.Replace("Normal", "").Trim());
              else if (line.Contains("Vertex"))
                verts.Add(ParseVector3Triplet(line.Replace("Vertex", "").Trim()));
            }
            poly.vertices = verts.ToArray();
            if (poly.polyFlags == null || poly.polyFlags.Length == 0)
              poly.polyFlags = new[] { "PF_Default" };
            polys.Add(poly);
          }
        }
        brush.polygons = polys.ToArray();
        list.Add(brush);
      }
      return list;
    }

    static L2BspPoly ParsePolygonLine(string line)
    {
      var poly = new L2BspPoly();
      string rest = line.Replace("Begin Polygon", "").Trim();
      foreach (string part in rest.Split(' '))
      {
        if (part.StartsWith("Texture=", StringComparison.Ordinal))
          poly.texture = part.Replace("Texture=", "");
        if (part.StartsWith("Flags=", StringComparison.Ordinal))
        {
          int flags = int.Parse(part.Replace("Flags=", ""), CultureInfo.InvariantCulture);
          poly.polyFlags = L2BspPolyFlags.Decode(flags);
        }
      }
      if (poly.polyFlags == null)
        poly.polyFlags = new[] { "PF_Default" };
      return poly;
    }

    static Vector3 ParseVector3Line(string line)
    {
      int eq = line.IndexOf('=');
      string body = line.Substring(eq + 1).TrimEnd(')');
      if (body.StartsWith("(")) body = body.Substring(1);
      float x = 0, y = 0, z = 0;
      foreach (string part in body.Split(','))
      {
        if (part.IndexOf('X') >= 0) x = ParseFloat(part);
        else if (part.IndexOf('Y') >= 0) y = ParseFloat(part);
        else if (part.IndexOf('Z') >= 0) z = ParseFloat(part);
      }
      return new Vector3(x, y, z);
    }

    static Vector3 ParseVector3Triplet(string line)
    {
      string[] p = line.Split(',');
      return new Vector3(ParseFloat(p[0]), ParseFloat(p[1]), ParseFloat(p[2]));
    }

    static float ParseFloat(string s)
    {
      int eq = s.IndexOf('=');
      if (eq >= 0) s = s.Substring(eq + 1);
      return float.Parse(s.Trim(), CultureInfo.InvariantCulture);
    }
  }
}