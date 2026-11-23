using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fineage.TerrainBsp
{
  /// <summary>
  /// UE2 BSP brush order is preserved in the dump. CSG_Subtract brushes carve CSG_Add geometry;
  /// subtract brush primitives are never rendered. AntiPortal caps on add brushes define doorway holes.
  /// </summary>
  public static class L2BspCsgCarver
  {
    public const string CsgAdd = "CSG_Add";
    public const string CsgSubtract = "CSG_Subtract";

    public sealed class Analysis
    {
      public int brushCount;
      public int subtractBrushCount;
      public int subtractSolidPolygonsWouldRender;
      public int portalCapCount;
      public int invisibleRenderedPolygons;
      public int portalFlagRenderedPolygons;
    }

    public static Analysis Analyze(IReadOnlyList<L2BspBrush> brushes)
    {
      var a = new Analysis { brushCount = brushes?.Count ?? 0 };
      if (brushes == null) return a;
      foreach (var b in brushes)
      {
        if (b == null) continue;
        bool subtract = string.Equals(b.csgOper, CsgSubtract, StringComparison.Ordinal);
        if (subtract) a.subtractBrushCount++;
        bool geometryBrush = BrushUsable(b);
        bool portalBrush = BrushUsableForPortalCollection(b);
        var basePos = b.position - b.prePivot;
        foreach (var pd in b.polygons ?? Array.Empty<L2BspPoly>())
        {
          if (pd?.vertices == null || pd.vertices.Length < 3) continue;
          if (!subtract && portalBrush && L2BspPolyFlags.IsPortalCapTexture(pd.texture))
          {
            var world = WorldVerts(basePos, pd.vertices);
            if (TryAxisRect(world, out _, out _, out _, out _, out _, out _, out _, out _))
              a.portalCapCount++;
          }
          if (!geometryBrush) continue;
          if (!L2BspPolyFlags.ShouldRenderPolygon(pd.polyFlags, pd.texture))
          {
            if (L2BspPolyFlags.Contains(pd.polyFlags, "PF_Invisible"))
              a.invisibleRenderedPolygons++;
            if (L2BspPolyFlags.Contains(pd.polyFlags, "PF_Portal"))
              a.portalFlagRenderedPolygons++;
            continue;
          }
          if (subtract && L2BspPolyFlags.IsBrushTextureRenderable(pd.texture))
            a.subtractSolidPolygonsWouldRender++;
        }
      }
      return a;
    }

    public static List<L2BspAabb> CollectSubtractVolumes(IReadOnlyList<L2BspBrush> brushes)
    {
      return CollectSubtractVolumesInner(brushes);
    }

    /// <summary>Append this brush when it is a usable CSG_Subtract. Order of calls is the carve order.</summary>
    public static void AccumulateSubtract(L2BspBrush brush, List<L2BspAabb> into)
    {
      if (into == null || brush == null || !string.Equals(brush.csgOper, CsgSubtract, StringComparison.Ordinal))
        return;
      if (!BrushUsable(brush))
        return;
      L2BspAabb? box = BrushBounds(brush);
      if (box.HasValue)
        into.Add(box.Value);
    }

    /// <summary>Null when the brush can be CSG'd. Otherwise a concrete reason it is dropped.</summary>
    public static string UnusableReason(L2BspBrush b)
    {
      if (b == null)
        return "null brush";
      if (b.position == Vector3.zero)
        return "position is zero";
      if (b.polygons == null)
        return "no polygons";
      if (!L2BspPolyFlags.ShouldRenderBrush(b.polyFlags))
        return "brush flags not rendered";
      if (BrushHasHugeVertices(b))
        return "vertex exceeds 60000";
      return null;
    }

    static List<L2BspAabb> CollectSubtractVolumesInner(IReadOnlyList<L2BspBrush> brushes)
    {
      var list = new List<L2BspAabb>();
      foreach (var b in brushes)
      {
        if (b == null || !string.Equals(b.csgOper, CsgSubtract, StringComparison.Ordinal))
          continue;
        if (!BrushUsable(b)) continue;
        var box = BrushBounds(b);
        if (box.HasValue) list.Add(box.Value);
      }
      return list;
    }

    public static List<L2BspPortalRect> CollectPortalCaps(IReadOnlyList<L2BspBrush> brushes)
    {
      var list = new List<L2BspPortalRect>();
      if (brushes == null) return list;
      foreach (var b in brushes)
      {
        if (b == null || !string.Equals(b.csgOper, CsgAdd, StringComparison.Ordinal) || !BrushUsableForPortalCollection(b))
          continue;
        Vector3 basePos = b.position - b.prePivot;
        float[] bmin = { float.MaxValue, float.MaxValue, float.MaxValue };
        float[] bmax = { float.MinValue, float.MinValue, float.MinValue };
        var worlds = new List<Vector3[]>();
        foreach (var pd in b.polygons ?? Array.Empty<L2BspPoly>())
        {
          if (pd?.vertices == null || pd.vertices.Length < 3)
          {
            worlds.Add(null);
            continue;
          }
          var w = WorldVerts(basePos, pd.vertices);
          worlds.Add(w);
          AccumBounds(w, bmin, bmax);
        }
        for (int p = 0; p < (b.polygons?.Length ?? 0); p++)
        {
          var pd = b.polygons[p];
          if (pd == null || worlds[p] == null) continue;
          if (!L2BspPolyFlags.IsPortalCapTexture(pd.texture)) continue;
          if (!TryAxisRect(worlds[p], out int axis, out float plane, out int av, out int bv, out float a0, out float a1, out float b0, out float b1))
            continue;
          if (Mathf.Abs(a0 - bmin[av]) > 8f || Mathf.Abs(a1 - bmax[av]) > 8f
              || Mathf.Abs(b0 - bmin[bv]) > 8f || Mathf.Abs(b1 - bmax[bv]) > 8f)
            continue;
          list.Add(new L2BspPortalRect
          {
            brushName = b.name,
            axis = axis,
            plane = plane,
            a = av,
            b = bv,
            a0 = a0,
            a1 = a1,
            b0 = b0,
            b1 = b1
          });
        }
      }
      return list;
    }

    /// <summary>
    /// Local-space vertex pieces after portal + subtract CSG. Null = this poly is the portal cap (drop).
    /// Empty list = fully carved away.
    /// </summary>
    public static List<Vector3[]> CarveFacePieces(
      L2BspBrush brush,
      L2BspPoly pd,
      Vector3 basePos,
      IReadOnlyList<L2BspPortalRect> portals,
      IReadOnlyList<L2BspAabb> subtracts)
    {
      var single = new List<Vector3[]> { pd.vertices };
      if (pd?.vertices == null || pd.vertices.Length < 3)
        return single;

      var world = WorldVerts(basePos, pd.vertices);
      if (!TryAxisRect(world, out int axis, out float plane, out int a, out int bv, out float a0, out float a1, out float b0, out float b1))
        return single;

      bool portalCap = string.Equals(brush.csgOper, CsgSubtract, StringComparison.Ordinal)
                         && L2BspPolyFlags.IsPortalCapTexture(pd.texture);
      L2BspPortalRect hit = default;
      bool portalHit = false;
      for (int i = 0; i < portals.Count; i++)
      {
        var portal = portals[i];
        if (portal.axis != axis || Mathf.Abs(portal.plane - plane) > 4f || portal.a != a)
          continue;
        bool inside = portal.a0 >= a0 - 4f && portal.a1 <= a1 + 4f
                      && portal.b0 >= b0 - 4f && portal.b1 <= b1 + 4f;
        if (!inside) continue;
        if (portalCap && portal.brushName == brush.name
            && Mathf.Abs(portal.a0 - a0) < 8f && Mathf.Abs(portal.a1 - a1) < 8f
            && Mathf.Abs(portal.b0 - b0) < 8f && Mathf.Abs(portal.b1 - b1) < 8f)
          return null;
        hit = portal;
        portalHit = true;
        break;
      }

      var holes = new List<(float a0, float a1, float b0, float b1)>();
      if (portalHit)
      {
        bool same = Mathf.Abs(hit.a0 - a0) < 8f && Mathf.Abs(hit.a1 - a1) < 8f
                    && Mathf.Abs(hit.b0 - b0) < 8f && Mathf.Abs(hit.b1 - b1) < 8f;
        if (same)
          return new List<Vector3[]>();
        holes.Add((Mathf.Max(hit.a0, a0), Mathf.Min(hit.a1, a1), Mathf.Max(hit.b0, b0), Mathf.Min(hit.b1, b1)));
      }

      foreach (var sub in subtracts)
      {
        if (!sub.IntersectsPlane(axis, plane)) continue;
        float ha0 = Mathf.Max(a0, sub.Min(a));
        float ha1 = Mathf.Min(a1, sub.Max(a));
        float hb0 = Mathf.Max(b0, sub.Min(bv));
        float hb1 = Mathf.Min(b1, sub.Max(bv));
        if (ha1 - ha0 < 2f || hb1 - hb0 < 2f) continue;
        holes.Add((ha0, ha1, hb0, hb1));
      }

      if (holes.Count == 0)
        return single;

      return FrameSubtract(basePos, axis, plane, a, bv, a0, a1, b0, b1, holes);
    }

    static List<Vector3[]> FrameSubtract(Vector3 basePos, int axis, float plane, int a, int b,
      float a0, float a1, float b0, float b1, List<(float pa0, float pa1, float pb0, float pb1)> holes)
    {
      // Merge holes naïvely: punch each hole from the full frame (multiple holes may overlap).
      var pieces = new List<Vector3[]> { MakeQuadLocal(basePos, axis, plane, a, b, a0, a1, b0, b1) };
      foreach (var hole in holes)
      {
        var next = new List<Vector3[]>();
        foreach (var piece in pieces)
        {
          if (piece == null || piece.Length < 4) { next.Add(piece); continue; }
          var pw = WorldVerts(basePos, piece);
          if (!TryAxisRect(pw, out int ax, out float pl, out int aa, out int bb, out float fa0, out float fa1, out float fb0, out float fb1))
          {
            next.Add(piece);
            continue;
          }
          if (ax != axis || aa != a || bb != b || Mathf.Abs(pl - plane) > 4f)
          {
            next.Add(piece);
            continue;
          }
          float pa0 = Mathf.Max(fa0, hole.pa0), pa1 = Mathf.Min(fa1, hole.pa1);
          float pb0 = Mathf.Max(fb0, hole.pb0), pb1 = Mathf.Min(fb1, hole.pb1);
          if (pa1 - pa0 < 2f || pb1 - pb0 < 2f || pa0 >= fa1 || pa1 <= fa0 || pb0 >= fb1 || pb1 <= fb0)
          {
            next.Add(piece);
            continue;
          }
          AddFrame(next, basePos, axis, plane, a, b, fa0, fa1, fb0, fb1, pa0, pa1, pb0, pb1);
        }
        pieces = next;
      }
      return pieces;
    }

    static void AddFrame(List<Vector3[]> frame, Vector3 basePos, int axis, float plane, int a, int b,
      float a0, float a1, float b0, float b1, float pa0, float pa1, float pb0, float pb1)
    {
      void Add(float ua0, float ua1, float ub0, float ub1)
      {
        if (ua1 - ua0 < 2f || ub1 - ub0 < 2f) return;
        frame.Add(MakeQuadLocal(basePos, axis, plane, a, b, ua0, ua1, ub0, ub1));
      }
      Add(a0, a1, b0, pb0);
      Add(a0, a1, pb1, b1);
      Add(a0, pa0, pb0, pb1);
      Add(pa1, a1, pb0, pb1);
    }

    static Vector3[] MakeQuadLocal(Vector3 basePos, int axis, float plane, int a, int b, float ua0, float ua1, float ub0, float ub1)
    {
      Vector3 c0 = WithAxis(axis, plane, a, ua0, b, ub0) - basePos;
      Vector3 c1 = WithAxis(axis, plane, a, ua1, b, ub0) - basePos;
      Vector3 c2 = WithAxis(axis, plane, a, ua1, b, ub1) - basePos;
      Vector3 c3 = WithAxis(axis, plane, a, ua0, b, ub1) - basePos;
      return new[] { c0, c1, c2, c3 };
    }

    public static bool BrushUsable(L2BspBrush b) => UnusableReason(b) == null;

    /// <summary>AntiPortal doorway brushes are brush-level NotSolid/Invisible in UE2 but still define holes.</summary>
    public static bool BrushUsableForPortalCollection(L2BspBrush b)
    {
      if (b == null || b.position == Vector3.zero || b.polygons == null) return false;
      return !BrushHasHugeVertices(b);
    }

    static bool BrushHasHugeVertices(L2BspBrush b)
    {
      foreach (var pd in b.polygons)
      {
        if (pd?.vertices == null) continue;
        foreach (var v in pd.vertices)
          if (Mathf.Abs(v.x) > 60000f || Mathf.Abs(v.y) > 60000f || Mathf.Abs(v.z) > 60000f)
            return true;
      }
      return false;
    }

    static L2BspAabb? BrushBounds(L2BspBrush b)
    {
      Vector3 basePos = b.position - b.prePivot;
      float[] min = { float.MaxValue, float.MaxValue, float.MaxValue };
      float[] max = { float.MinValue, float.MinValue, float.MinValue };
      bool any = false;
      foreach (var pd in b.polygons)
      {
        if (pd?.vertices == null) continue;
        foreach (var v in pd.vertices)
        {
          Vector3 w = basePos + v;
          any = true;
          for (int ax = 0; ax < 3; ax++)
          {
            float c = Axis(w, ax);
            if (c < min[ax]) min[ax] = c;
            if (c > max[ax]) max[ax] = c;
          }
        }
      }
      if (!any) return null;
      return new L2BspAabb { X0 = min[0], X1 = max[0], Y0 = min[1], Y1 = max[1], Z0 = min[2], Z1 = max[2] };
    }

    static Vector3[] WorldVerts(Vector3 basePos, Vector3[] local) {
      var w = new Vector3[local.Length];
      for (int i = 0; i < local.Length; i++) w[i] = basePos + local[i];
      return w;
    }

    static void AccumBounds(Vector3[] world, float[] bmin, float[] bmax)
    {
      for (int i = 0; i < world.Length; i++)
        for (int ax = 0; ax < 3; ax++)
        {
          float c = Axis(world[i], ax);
          if (c < bmin[ax]) bmin[ax] = c;
          if (c > bmax[ax]) bmax[ax] = c;
        }
    }

    public static bool TryAxisRect(Vector3[] world, out int axis, out float plane, out int a, out int b,
      out float a0, out float a1, out float b0, out float b1)
    {
      axis = -1; plane = 0; a = b = 0; a0 = a1 = b0 = b1 = 0;
      if (world == null || world.Length < 3) return false;
      float[] min = { float.MaxValue, float.MaxValue, float.MaxValue };
      float[] max = { float.MinValue, float.MinValue, float.MinValue };
      for (int i = 0; i < world.Length; i++)
        for (int ax = 0; ax < 3; ax++)
        {
          float c = Axis(world[i], ax);
          if (c < min[ax]) min[ax] = c;
          if (c > max[ax]) max[ax] = c;
        }
      int thin = 0;
      for (int ax = 0; ax < 3; ax++)
      {
        if (max[ax] - min[ax] <= 2f)
        {
          thin++;
          axis = ax;
          plane = (min[ax] + max[ax]) * 0.5f;
        }
      }
      if (thin != 1) return false;
      a = axis == 0 ? 1 : 0;
      b = axis == 2 ? 1 : 2;
      a0 = min[a]; a1 = max[a];
      b0 = min[b]; b1 = max[b];
      return a1 - a0 > 2f && b1 - b0 > 2f;
    }

    static float Axis(Vector3 v, int axis) => axis == 0 ? v.x : axis == 1 ? v.y : v.z;

    static Vector3 WithAxis(int axis, float plane, int a, float av, int b, float bv)
    {
      var v = Vector3.zero;
      if (axis == 0) v.x = plane; else if (axis == 1) v.y = plane; else v.z = plane;
      if (a == 0) v.x = av; else if (a == 1) v.y = av; else v.z = av;
      if (b == 0) v.x = bv; else if (b == 1) v.y = bv; else v.z = bv;
      return v;
    }
  }
}