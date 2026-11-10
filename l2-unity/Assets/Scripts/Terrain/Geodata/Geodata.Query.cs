using System;

public partial class Geodata : MonoBehaviour
{
    public Node GetClosestNodeAt(Vector3 nodePos) {
        if (TryGetClosestNodeAt(nodePos, out Node node))
            return node;
        throw new Exception("Node not found");
    }

    public Node GetClosestNodeAt(Vector3 nodePos, string mapId) {
        if (TryGetClosestNodeAt(nodePos, mapId, out Node node))
            return node;
        throw new Exception("Node not found");
    }

    /// <summary>Non-throwing closest layer at XZ (#930 WorldPlacement).</summary>
    public bool TryGetClosestNodeAt(Vector3 nodePos, out Node node)
    {
        node = null;
        if (!TryGetCurrentMap(nodePos, out string mapId))
            return false;
        return TryGetClosestNodeAt(nodePos, mapId, out node);
    }

    public bool TryGetClosestNodeAt(Vector3 nodePos, string mapId, out Node node)
    {
        node = null;
        if (!TryGetAllNodesAt(nodePos, mapId, out List<Node> layers) || layers.Count == 0)
            return false;

        Node closest = null;
        float lowestDiff = float.PositiveInfinity;
        for (int i = 0; i < layers.Count; i++)
        {
            Node n = layers[i];
            if (n == null)
                continue;
            float nodeHeightDiff = Math.Abs(n.center.y - nodePos.y);
            if (closest == null ||
                nodeHeightDiff < lowestDiff - 0.0001f ||
                (Math.Abs(nodeHeightDiff - lowestDiff) <= 0.0001f && n.center.y > closest.center.y))
            {
                closest = n;
                lowestDiff = nodeHeightDiff;
            }
        }

        if (closest == null)
            return false;

        node = closest;
        return true;
    }

    public List<Node> GetAllNodesAt(Vector3 nodePos) {
        if (TryGetAllNodesAt(nodePos, out List<Node> layers))
            return layers;
        throw new Exception("Nodes not found");
    }

    public List<Node> GetAllNodesAt(Vector3 nodePos, string mapId) {
        if (TryGetAllNodesAt(nodePos, mapId, out List<Node> layers))
            return layers;
        throw new Exception("Nodes not found");
    }

    public bool TryGetAllNodesAt(Vector3 nodePos, out List<Node> layers)
    {
        layers = null;
        if (!TryGetCurrentMap(nodePos, out string mapId))
            return false;
        return TryGetAllNodesAt(nodePos, mapId, out layers);
    }

    public bool TryGetAllNodesAt(Vector3 nodePos, string mapId, out List<Node> layers)
    {
        layers = null;
        if (_geodata == null || string.IsNullOrEmpty(mapId) || !_geodata.ContainsKey(mapId))
            return false;

        Vector3 nodeIndex;
        try
        {
            nodeIndex = FromWorldToNodePos(nodePos, mapId);
        }
        catch
        {
            return false;
        }

        int x = (int)nodeIndex.x;
        int z = (int)nodeIndex.z;
        Node[,,] map = _geodata[mapId];
        if (x < 0 || z < 0 || x >= map.GetLength(0) || z >= map.GetLength(2))
            return false;

        layers = new List<Node>();
        for (int y = 0; y < map.GetLength(1); y++)
        {
            Node layer = map[x, y, z];
            if (layer != null)
                layers.Add(layer);
        }

        return layers.Count > 0;
    }

    public Vector3 FromNodeToWorldPos(Vector3 nodePos, Vector3 origin) {
        Vector3 worldPos = nodePos * _nodeSize + origin;
        worldPos = new Vector3(
            NumberUtils.FloorToNearest(worldPos.x, _nodeSize),
            NumberUtils.FloorToNearest(worldPos.y, _nodeSize),
            NumberUtils.FloorToNearest(worldPos.z, _nodeSize));
        return worldPos;
    }

    private Vector3 FromWorldToNodePos(Vector3 worldPos, String mapId) {
        Vector3 terrainPos;
        if(!_mapsOrigin.TryGetValue(mapId, out terrainPos)) {
            throw new Exception("Terrain not found.");
        }

        Vector3 offsetPos = worldPos - terrainPos;
        Vector3 nodePos = new Vector3(
            Mathf.Floor(offsetPos.x / _nodeSize),
            Mathf.Floor(offsetPos.y / _nodeSize),
            Mathf.Floor(offsetPos.z / _nodeSize));

        return nodePos;
    }

    void OnDrawGizmos() {
        if(!_drawGizmos || !Application.isPlaying)
            return;

        Gizmos.color = Color.green;

        if(_drawMapGizmos) {
            DrawMapGizmos("17_25");
        }

        if(_drawGizmosAroundPlayer) {
            DrawGizmosAroundPlayer();
        }
    }

    private void DrawMapGizmos(string mapId) {
        int count = 0;
        Vector3 cubeSize = new Vector3(_nodeSize - _nodeSize / 10f, 0.1f, _nodeSize - _nodeSize / 10f);

        Node[,,] nodes;
        if(!_geodata.TryGetValue(mapId, out nodes)) {
            // Debug.LogWarning("Can't draw gizmos for map " + mapId);
            return;
        }

        for (int x = 0; x < nodes.GetLength(0); x++) {
            for (int z = 0; z < nodes.GetLength(2); z++) {
                for (int y = 0; y < nodes.GetLength(1); y++) {
                    if(nodes[x, y, z] != null) {
                        if(count++ > _drawMapGizmosLimit) {
                            return;
                        }

                        Gizmos.DrawCube(nodes[x, y, z].center, cubeSize);
                    }
                }
            }
        }
    }

    private void DrawGizmosAroundPlayer() {
        PlayerController player = ControllerPacketServices.Player ?? WorldApplyPacketServices.Player;
        if(player == null) {
            return;
        }

        Vector3 playerPos = player.transform.position;
        Vector3 cubeSize = new Vector3(_nodeSize - _nodeSize / 10f, 0.1f, _nodeSize - _nodeSize / 10f);
        Gizmos.color = Color.green;

        for(int x = -_drawGizmosAroundPlayerRadius; x <= _drawGizmosAroundPlayerRadius; x++) {
            for(int z = -_drawGizmosAroundPlayerRadius; z <= _drawGizmosAroundPlayerRadius; z++) {
                try {
                    List<Node> nodes = GetAllNodesAt(playerPos + new Vector3(x * _nodeSize, 0, z * _nodeSize));
                    nodes.ForEach(n => {
                        Gizmos.DrawCube(n.center, cubeSize);
                    });
                } catch(Exception) {
                }
            }
        }
    }
}
