using System;
using UnityEngine;

public static partial class L2StaticMeshSunMaskDatabase
{
    [Serializable]
    public sealed class MapData
    {
        public bool ok;
        public string command;
        public int actorCount;
        public int rowCount;
        public int withMasks;
        public ActorData[] actors;
    }

    [Serializable]
    public sealed class ActorData
    {
        public string actor;
        public int actorExportIndex;
        public string mesh;
        public string instance;
        public string location;
        public string rotation;
        public string drawScale;
        public string drawScale3D;
        public string sunAffect;
        public string shadowCast;
        public int nativeOffset;
        public int maskBytes;
        public string[] masks;
        public string[] hashes;
        public int[] setBits;
        public string error;

        public bool HasMasks
        {
            get
            {
                return maskBytes > 0 &&
                    masks != null &&
                    masks.Length == L2StaticMeshSunVisibility.SlotCount;
            }
        }

        public byte[] DecodeMask(int slot)
        {
            if (!HasMasks || slot < 0 || slot >= masks.Length)
                return Array.Empty<byte>();
            byte[] bytes = Convert.FromBase64String(masks[slot]);
            if (bytes.Length != maskBytes)
                throw new InvalidOperationException(
                    actor + " slot " + slot + " has " + bytes.Length +
                    " bytes, expected " + maskBytes + ".");
            return bytes;
        }

        public Vector3 UnityPosition
        {
            get
            {
                Vector3 ue = ParseVector(location);
                return L2StaticMeshVertexLight.ToUnityPosition(ue);
            }
        }
    }



}
