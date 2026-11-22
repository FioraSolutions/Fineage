using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Draws one tile's DecoLayers from <c>XX_YY_Deco.bytes</c>. Instances are rebuilt per
/// sector from the official density map and seed. Each layer is culled at its own
/// <c>FadeoutRadius</c> (fully visible through Min, gone past Max).
/// </summary>
public sealed class L2DecoInstancedRenderer : MonoBehaviour
{
    const int DrawChunk = 1023;
    const int FadeBands = 8;

    [SerializeField] TextAsset _archive;

    L2DecoTileFile _tile;
    LayerDraw[] _layers;
    MeshCollider _ground;
    bool _groundMissing;
    bool _reportedArchive;
    int _sectorOriginX = int.MinValue;
    int _sectorOriginY = int.MinValue;
    readonly List<Sector> _sectors = new List<Sector>();
    readonly List<Matrix4x4>[] _bands = new List<Matrix4x4>[FadeBands];
    readonly Matrix4x4[] _chunk = new Matrix4x4[DrawChunk];
    readonly MaterialPropertyBlock[] _fadeBlocks = new MaterialPropertyBlock[FadeBands];

    struct Slot
    {
        public Mesh Mesh;
        public Material[] Materials;
        public Matrix4x4 Local;
    }

    struct LayerDraw
    {
        public bool Active;
        public float FadeMinM;
        public float FadeMaxM;
        public bool HardClip;
        public bool Align;
        public byte DrawOrder;
        public Slot[] Slots;
    }

    struct Sector
    {
        public int Layer;
        public int Slot;
        public Vector3[] Positions;
        public Matrix4x4[] Matrices;
    }

    public void SetArchive(TextAsset archive)
    {
        _archive = archive;
        _reportedArchive = false;
        Reload();
    }

    void Awake()
    {
        for (int i = 0; i < FadeBands; i++)
        {
            _bands[i] = new List<Matrix4x4>(256);
            _fadeBlocks[i] = new MaterialPropertyBlock();
        }
    }

    void OnEnable()
    {
        if (_layers == null)
            Reload();
    }

    void LateUpdate()
    {
        if (_tile == null || _layers == null)
        {
            if (!_reportedArchive)
            {
                _reportedArchive = true;
                FineageLog.Error(
                    FineageLogCategory.World,
                    "[Interlude deco] " + name + ": no " + name.Replace("_Deco", "") + "_Deco.bytes archive");
            }
            return;
        }

        Camera camera = Camera.main;
        if (camera == null)
            return;
        if (_ground == null && !_groundMissing)
            BindGround();
        if (_ground == null)
            return;

        Vector3 cam = camera.transform.position;
        int sx = SectorIndex(cam.x, true);
        int sy = SectorIndex(cam.z, false);
        if (sx != _sectorOriginX || sy != _sectorOriginY)
        {
            _sectorOriginX = sx;
            _sectorOriginY = sy;
            RebuildSectors(cam);
        }

        Draw(cam);
    }

    void Reload()
    {
        _layers = null;
        _tile = null;
        _sectors.Clear();
        _sectorOriginX = int.MinValue;
        _ground = null;
        _groundMissing = false;
        if (_archive == null || _archive.bytes == null || _archive.bytes.Length == 0)
            return;
        if (!L2DecoLayerArchive.TryRead(_archive.bytes, out _tile, out string error))
        {
            FineageLog.Error(FineageLogCategory.World, error);
            return;
        }

        _layers = new LayerDraw[_tile.Layers.Length];
        for (int i = 0; i < _tile.Layers.Length; i++)
            _layers[i] = BuildLayer(i, _tile.Layers[i]);
    }

    LayerDraw BuildLayer(int index, L2DecoLayerInput layer)
    {
        var draw = new LayerDraw();
        string who = "[Interlude deco] " + _tile.TileId + " layer " + index
            + " (" + layer.MeshRef + ")";
        if (!layer.ShowOnTerrain && !layer.ForceRender)
            return draw;
        if (layer.FadeMaxUu <= 0f)
        {
            FineageLog.Error(FineageLogCategory.World, who + ": FadeoutRadius.Max is " + layer.FadeMaxUu + ", layer is not drawn");
            return draw;
        }

        GameObject meshAsset = Resources.Load<GameObject>(layer.MeshResourcePath);
        if (meshAsset == null)
        {
            FineageLog.Error(FineageLogCategory.World, who + ": StaticMesh resource missing '" + layer.MeshResourcePath + "'");
            return draw;
        }

        MeshFilter[] filters = meshAsset.GetComponentsInChildren<MeshFilter>(true);
        var slots = new List<Slot>();
        for (int f = 0; f < filters.Length; f++)
        {
            MeshFilter filter = filters[f];
            if (filter.sharedMesh == null)
                continue;
            Renderer renderer = filter.GetComponent<Renderer>();
            Material[] materials = renderer != null ? renderer.sharedMaterials : null;
            if (materials == null || materials.Length == 0)
            {
                FineageLog.Error(FineageLogCategory.World, who + ": mesh '" + filter.sharedMesh.name + "' has no material");
                continue;
            }

            for (int m = 0; m < materials.Length; m++)
            {
                if (materials[m] != null && !materials[m].enableInstancing)
                    materials[m].enableInstancing = true;
            }

            slots.Add(new Slot
            {
                Mesh = filter.sharedMesh,
                Materials = materials,
                Local = LocalMatrix(filter.transform, meshAsset.transform)
            });
        }

        if (slots.Count == 0)
        {
            FineageLog.Error(FineageLogCategory.World, who + ": StaticMesh '" + layer.MeshResourcePath + "' has no MeshFilter");
            return draw;
        }

        if (!string.IsNullOrEmpty(layer.ColorRef))
        {
            FineageLog.Error(
                FineageLogCategory.World,
                who + ": ColorMap " + layer.ColorRef + " is stored but instanced draw has no per-instance tint");
        }

        draw.Active = true;
        draw.FadeMinM = layer.FadeMinUu / L2DecoGenerator.UnrealUnitsPerMeter;
        draw.FadeMaxM = layer.FadeMaxUu / L2DecoGenerator.UnrealUnitsPerMeter;
        draw.HardClip = layer.FadeMaxUu <= layer.FadeMinUu;
        draw.Align = layer.AlignToTerrain;
        draw.DrawOrder = layer.DrawOrder;
        draw.Slots = slots.ToArray();
        return draw;
    }

    static Matrix4x4 LocalMatrix(Transform target, Transform root)
    {
        Matrix4x4 local = Matrix4x4.identity;
        Transform cursor = target;
        while (cursor != null && cursor != root)
        {
            local = Matrix4x4.TRS(cursor.localPosition, cursor.localRotation, cursor.localScale) * local;
            cursor = cursor.parent;
        }
        return local;
    }

    void BindGround()
    {
        Transform root = transform.parent != null ? transform.parent : transform;
        MeshCollider[] colliders = root.GetComponentsInChildren<MeshCollider>(true);
        MeshCollider best = null;
        float bestArea = -1f;
        for (int i = 0; i < colliders.Length; i++)
        {
            Vector3 size = colliders[i].bounds.size;
            float area = size.x * size.z;
            if (area > bestArea)
            {
                bestArea = area;
                best = colliders[i];
            }
        }

        _ground = best;
        if (_ground == null)
        {
            _groundMissing = true;
            FineageLog.Error(
                FineageLogCategory.World,
                "[Interlude deco] " + _tile.TileId + ": no terrain MeshCollider to sample height");
        }
    }

    int SectorIndex(float unity, bool alongX)
    {
        float scale = alongX ? _tile.ScaleY : _tile.ScaleX;
        float origin = alongX ? _tile.OriginY : _tile.OriginX;
        int quads = alongX ? _tile.QuadsY : _tile.QuadsX;
        float half = quads * scale * 0.5f;
        float minUe = origin - half - scale;
        float ue = unity * L2DecoGenerator.UnrealUnitsPerMeter;
        int quad = (int)((ue - minUe) / scale);
        int sector = _tile.SectorQuads > 0 ? _tile.SectorQuads : L2DecoTileFile.Ue2TerrainSectorSizeDefault;
        return FloorDiv(quad, sector);
    }

    static int FloorDiv(int value, int divisor)
    {
        int q = value / divisor;
        if (value < 0 && value % divisor != 0)
            q--;
        return q;
    }

    void RebuildSectors(Vector3 cameraPos)
    {
        _sectors.Clear();
        float maxFade = 0f;
        for (int i = 0; i < _layers.Length; i++)
        {
            if (_layers[i].Active && _layers[i].FadeMaxM > maxFade)
                maxFade = _layers[i].FadeMaxM;
        }

        float sectorMeters = _tile.SectorQuads * _tile.ScaleX / L2DecoGenerator.UnrealUnitsPerMeter;
        int reach = sectorMeters > 0f ? Mathf.CeilToInt(maxFade / sectorMeters) + 1 : 1;
        var spawned = new List<L2DecoInstance>(512);
        for (int layer = 0; layer < _layers.Length; layer++)
        {
            if (!_layers[layer].Active)
                continue;
            for (int sy = _sectorOriginY - reach; sy <= _sectorOriginY + reach; sy++)
            {
                for (int sx = _sectorOriginX - reach; sx <= _sectorOriginX + reach; sx++)
                {
                    spawned.Clear();
                    L2DecoGenerator.AppendSector(_tile, layer, sx, sy, spawned);
                    if (spawned.Count == 0)
                        continue;
                    Place(layer, spawned, cameraPos);
                }
            }
        }
    }

    void Place(int layer, List<L2DecoInstance> spawned, Vector3 cameraPos)
    {
        Slot[] slots = _layers[layer].Slots;
        float lift = _tile.DecoLayerOffsetUu / L2DecoGenerator.UnrealUnitsPerMeter;
        bool align = _layers[layer].Align;
        for (int s = 0; s < slots.Length; s++)
        {
            var positions = new Vector3[spawned.Count];
            var matrices = new Matrix4x4[spawned.Count];
            int kept = 0;
            for (int i = 0; i < spawned.Count; i++)
            {
                L2DecoInstance inst = spawned[i];
                if (!TryGround(inst.UnityX, inst.UnityZ, out Vector3 point, out Vector3 normal))
                    continue;
                point.y += lift;
                Quaternion yaw = Quaternion.Euler(0f, inst.YawDegrees, 0f);
                if (align)
                    yaw = Quaternion.FromToRotation(Vector3.up, normal) * yaw;
                var scale = new Vector3(inst.UnityScaleX, inst.UnityScaleY, inst.UnityScaleZ);
                Matrix4x4 world = Matrix4x4.TRS(point, yaw, scale) * slots[s].Local;
                positions[kept] = point;
                matrices[kept] = world;
                kept++;
            }

            if (kept == 0)
                continue;
            if (kept != positions.Length)
            {
                var trimP = new Vector3[kept];
                var trimM = new Matrix4x4[kept];
                for (int i = 0; i < kept; i++)
                {
                    trimP[i] = positions[i];
                    trimM[i] = matrices[i];
                }
                positions = trimP;
                matrices = trimM;
            }

            if (_layers[layer].DrawOrder == 1)
                SortFarToNear(positions, matrices, cameraPos);
            _sectors.Add(new Sector
            {
                Layer = layer,
                Slot = s,
                Positions = positions,
                Matrices = matrices
            });
        }
    }

    bool TryGround(float unityX, float unityZ, out Vector3 point, out Vector3 normal)
    {
        point = default;
        normal = Vector3.up;
        Bounds bounds = _ground.bounds;
        float top = bounds.max.y + 50f;
        var ray = new Ray(new Vector3(unityX, top, unityZ), Vector3.down);
        if (!_ground.Raycast(ray, out RaycastHit hit, top - bounds.min.y + 100f))
            return false;
        point = hit.point;
        normal = hit.normal;
        return true;
    }

    static void SortFarToNear(Vector3[] positions, Matrix4x4[] matrices, Vector3 cameraPos)
    {
        int n = positions.Length;
        for (int i = 1; i < n; i++)
        {
            Vector3 p = positions[i];
            Matrix4x4 m = matrices[i];
            float d = (p - cameraPos).sqrMagnitude;
            int j = i - 1;
            while (j >= 0 && (positions[j] - cameraPos).sqrMagnitude < d)
            {
                positions[j + 1] = positions[j];
                matrices[j + 1] = matrices[j];
                j--;
            }
            positions[j + 1] = p;
            matrices[j + 1] = m;
        }
    }

    void Draw(Vector3 cameraPos)
    {
        for (int s = 0; s < _sectors.Count; s++)
        {
            Sector sector = _sectors[s];
            LayerDraw layer = _layers[sector.Layer];
            Slot slot = layer.Slots[sector.Slot];
            for (int b = 0; b < FadeBands; b++)
                _bands[b].Clear();
            float min = layer.FadeMinM;
            float max = layer.FadeMaxM;
            float span = max - min;
            for (int i = 0; i < sector.Matrices.Length; i++)
            {
                float dist = Vector3.Distance(cameraPos, sector.Positions[i]);
                if (dist > max)
                    continue;
                float opacity = 1f;
                if (!layer.HardClip && dist > min && span > 0f)
                    opacity = 1f - (dist - min) / span;
                if (opacity <= 0f)
                    continue;
                int band = opacity >= 1f ? FadeBands - 1 : (int)(opacity * (FadeBands - 1));
                _bands[band].Add(sector.Matrices[i]);
            }

            for (int b = 0; b < FadeBands; b++)
            {
                if (_bands[b].Count == 0)
                    continue;
                float opacity = (b + 1f) / FadeBands;
                var color = new Color(1f, 1f, 1f, opacity);
                _fadeBlocks[b].SetColor("_BaseColor", color);
                _fadeBlocks[b].SetColor("_Color", color);
                Submit(slot, _bands[b], _fadeBlocks[b]);
            }
        }
    }

    void Submit(Slot slot, List<Matrix4x4> matrices, MaterialPropertyBlock block)
    {
        int submeshes = slot.Mesh.subMeshCount;
        for (int sub = 0; sub < submeshes; sub++)
        {
            Material material = sub < slot.Materials.Length ? slot.Materials[sub] : slot.Materials[0];
            if (material == null)
                continue;
            var rp = new RenderParams(material)
            {
                worldBounds = _ground.bounds,
                layer = gameObject.layer,
                shadowCastingMode = ShadowCastingMode.Off,
                receiveShadows = false,
                matProps = block
            };
            int offset = 0;
            while (offset < matrices.Count)
            {
                int count = matrices.Count - offset;
                if (count > DrawChunk)
                    count = DrawChunk;
                for (int i = 0; i < count; i++)
                    _chunk[i] = matrices[offset + i];
                Graphics.RenderMeshInstanced(rp, slot.Mesh, sub, _chunk, count);
                offset += count;
            }
        }
    }
}
