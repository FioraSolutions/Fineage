using System;
using UnityEngine;

namespace Fineage.TerrainBsp
{
    [Serializable]
    public sealed class L2BspBrush
    {
        public string name;
        public string csgOper;
        public string[] polyFlags;
        public Vector3 position;
        public Vector3 prePivot;
        public L2BspPoly[] polygons;
    }

    [Serializable]
    public sealed class L2BspPoly
    {
        public string texture;
        public string[] polyFlags;
        public Vector3 origin;
        public Vector3 textureU;
        public Vector3 textureV;
        public Vector3 normal;
        public Vector3[] vertices;
    }

    public struct L2BspAabb
    {
        public float X0, X1, Y0, Y1, Z0, Z1;

        public bool IntersectsPlane(int axis, float plane, float eps = 2f)
        {
            float mn = axis == 0 ? X0 : axis == 1 ? Y0 : Z0;
            float mx = axis == 0 ? X1 : axis == 1 ? Y1 : Z1;
            return mn <= plane + eps && mx >= plane - eps;
        }

        public float Min(int axis) => axis == 0 ? X0 : axis == 1 ? Y0 : Z0;
        public float Max(int axis) => axis == 0 ? X1 : axis == 1 ? Y1 : Z1;
    }

    public struct L2BspPortalRect
    {
        public string brushName;
        public int axis;
        public float plane;
        public int a;
        public int b;
        public float a0, a1, b0, b1;
    }
}