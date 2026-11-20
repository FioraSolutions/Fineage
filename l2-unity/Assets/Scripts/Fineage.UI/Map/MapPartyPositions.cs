using System.Collections.Generic;
using UnityEngine;

/// <summary>Latest L2 world (X,Y) for party members on the map (from PartyMemberPosition).</summary>
public static class MapPartyPositions
{
    public readonly struct MemberPosition
    {
        public int ObjectId;
        public float L2X;
        public float L2Y;
        public float L2Z;
    }

    static readonly Dictionary<int, MemberPosition> _positions = new();

    public static IReadOnlyDictionary<int, MemberPosition> Positions => _positions;

    public static void Apply(int objectId, int l2X, int l2Y, int l2Z)
    {
        if (objectId <= 0)
            return;
        _positions[objectId] = new MemberPosition
        {
            ObjectId = objectId,
            L2X = l2X,
            L2Y = l2Y,
            L2Z = l2Z
        };
    }

    public static void Clear()
    {
        _positions.Clear();
    }
}