using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class L2InstancedPropRenderer : MonoBehaviour
{
    private const int DrawChunk = 1023;

    [SerializeField] private L2InstancedPropSet _propSet;
    [SerializeField] private int _layer = 15;
    [SerializeField] private float _cellSize = 80f;
    [SerializeField] private float _maxDrawDistance = 30f;
    [SerializeField] private bool _receiveShadows;

    private readonly Matrix4x4[] _drawBuffer = new Matrix4x4[DrawChunk];
    private List<Cell> _cells;
    private Transform _transform;

    private struct Cell
    {
        public Mesh mesh;
        public Material material;
        public ShadowCastingMode shadows;
        public Vector3 center;
        public float radius;
        public Matrix4x4[] matrices;
    }

    public L2InstancedPropSet PropSet
    {
        get { return _propSet; }
        set { _propSet = value; }
    }

    public float MaxDrawDistance
    {
        get { return _maxDrawDistance; }
        set { _maxDrawDistance = value; }
    }

    public int Layer
    {
        get { return _layer; }
        set { _layer = value; }
    }


    private struct CellKey : System.IEquatable<CellKey>
    {
        public readonly int batch;
        public readonly int x;
        public readonly int z;

        public CellKey(int batch, int x, int z)
        {
            this.batch = batch;
            this.x = x;
            this.z = z;
        }

        public bool Equals(CellKey other)
        {
            return batch == other.batch && x == other.x && z == other.z;
        }

        public override bool Equals(object obj)
        {
            return obj is CellKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (batch * 73856093) ^ (x * 19349663) ^ (z * 83492791);
        }
    }

    private struct BatchMeta
    {
        public Mesh mesh;
        public Material material;
        public ShadowCastingMode shadows;
        public float meshRadius;
    }
}
