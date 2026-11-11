using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class L2StaticMeshSunVisibility
{
    public const int SlotCount = 8;
    public const float HoursPerDay = 24f;
    public const float TraceLengthUu = 262144f * 1.375f;
    public const int MaxTraceHits = 64;

    public sealed class MaskSet
    {
        public readonly int VertexCount;
        public readonly byte[][] Slots;
        public readonly Vector3[] IncomingDirections;

        public MaskSet(int vertexCount)
        {
            VertexCount = vertexCount;
            Slots = new byte[SlotCount][];
            IncomingDirections = new Vector3[SlotCount];
            int byteCount = (vertexCount + 7) >> 3;
            for (int i = 0; i < SlotCount; i++)
                Slots[i] = new byte[byteCount];
        }

        public bool IsEnabled(int slot, int vertex)
        {
            return (Slots[slot][vertex >> 3] & (1 << (vertex & 7))) != 0;
        }

        public void SetEnabled(int slot, int vertex)
        {
            Slots[slot][vertex >> 3] |= (byte)(1 << (vertex & 7));
        }
    }

    /// <summary>
    /// Returns the center of an ActorStaticLight slot: 1.5, 4.5, ... 22.5
    /// for the original eight-slot configuration.
    /// </summary>

    /// <summary>
    /// GetSafeDirectionForIntMap: negate GetSunLightDirection, prevent an
    /// upward incoming ray, then normalize. Unity Y corresponds to UE Z.
    /// </summary>



    sealed class RaycastHitDistanceComparer : IComparer<RaycastHit>
    {
        public static readonly RaycastHitDistanceComparer Instance = new RaycastHitDistanceComparer();

        public int Compare(RaycastHit a, RaycastHit b)
        {
            return a.distance.CompareTo(b.distance);
        }
    }
}
