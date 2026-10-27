using Fineage.ZoneServer.Interest;
using Fineage.ZoneServer.Session;
using Fineage.ZoneServer.Wire;

namespace Fineage.ZoneServer.World;

/// <summary>
/// Single-zone presence: AOI + thin CharInfo/NpcInfo/DeleteObject fan-out.
/// </summary>
public sealed class ZoneWorld
{
    readonly IAoiProvider _aoi;
    readonly Dictionary<int, ZoneEntity> _entities = new();
    readonly Dictionary<int, PlayerSession> _playersByObjectId = new();
    readonly object _lock = new();
    int _nextPlayerObjectId = 0x0010_0000;

    public ZoneWorld(IAoiProvider? aoi = null) => _aoi = aoi ?? new GridAoiProvider();

    public IAoiProvider Aoi => _aoi;

    public void SeedTalkingIslandNpcs()
    {
        // Static TI samples — object ids are server-local, templates are Interlude-ish.
        AddStaticNpc(20_001, 30_001, "Village Guard",
            PlayerSession.SpawnX + 800, PlayerSession.SpawnY + 400, PlayerSession.SpawnZ);
        AddStaticNpc(20_002, 30_006, "Gremlin",
            PlayerSession.SpawnX + 12_000, PlayerSession.SpawnY, PlayerSession.SpawnZ);
    }

    public void AddStaticNpc(int objectId, int templateId, string name, int x, int y, int z)
    {
        lock (_lock)
        {
            var entity = new ZoneEntity
            {
                ObjectId = objectId,
                Kind = ZoneEntityKind.Npc,
                Name = name,
                X = x,
                Y = y,
                Z = z,
                NpcTemplateId = templateId
            };
            _entities[objectId] = entity;
            _aoi.Enter(objectId, x, y);
        }
    }

    public int EnterPlayer(PlayerSession session, string name, int x, int y, int z)
    {
        lock (_lock)
        {
            int objectId = _nextPlayerObjectId++;
            var entity = new ZoneEntity
            {
                ObjectId = objectId,
                Kind = ZoneEntityKind.Player,
                Name = name,
                X = x,
                Y = y,
                Z = z,
                ClassId = 0
            };
            _entities[objectId] = entity;
            if (session != null)
                _playersByObjectId[objectId] = session;
            _aoi.Enter(objectId, x, y);
            return objectId;
        }
    }

    public void LeavePlayer(int objectId)
    {
        List<int> watchers;
        lock (_lock)
        {
            if (!_entities.ContainsKey(objectId))
                return;

            watchers = _aoi.GetVisible(objectId).ToList();
            _aoi.Leave(objectId);
            _entities.Remove(objectId);
            _playersByObjectId.Remove(objectId);
        }

        var delete = new DeleteObjectMsg { ObjectId = objectId };
        foreach (int watcherId in watchers)
            _ = NotifyPlayerAsync(watcherId, MsgType.DeleteObject, delete);
    }

    public async Task ApplyEnterPresenceAsync(PlayerSession joiner, int joinerObjectId, CancellationToken ct)
    {
        IReadOnlyCollection<int> visible;
        ZoneEntity joinerEntity;
        lock (_lock)
        {
            if (!_entities.TryGetValue(joinerObjectId, out joinerEntity!))
                return;
            visible = _aoi.GetVisible(joinerObjectId);
        }

        List<(int Id, MsgType Type, object Msg)> toJoiner = new();
        List<(int WatcherId, object Msg)> toWatchers = new();
        lock (_lock)
        {
            foreach (int id in visible)
            {
                if (!_entities.TryGetValue(id, out ZoneEntity? other))
                    continue;
                var (type, msg) = BuildPresenceMsg(other);
                toJoiner.Add((id, type, msg));
                toWatchers.Add((id, BuildCharInfo(joinerEntity)));
            }
        }

        foreach (var (_, type, msg) in toJoiner)
            await joiner.SendWireAsync(type, msg, ct).ConfigureAwait(false);
        foreach (var (watcherId, msg) in toWatchers)
            await NotifyPlayerAsync(watcherId, MsgType.CharInfo, msg, ct).ConfigureAwait(false);
    }

    public async Task ApplyMovePresenceAsync(
        PlayerSession mover,
        int objectId,
        int newX,
        int newY,
        int newZ,
        CancellationToken ct)
    {
        HashSet<int> before;
        ZoneEntity moverEntity;
        lock (_lock)
        {
            if (!_entities.TryGetValue(objectId, out moverEntity!))
                return;
            before = _aoi.GetVisible(objectId).ToHashSet();
            moverEntity.X = newX;
            moverEntity.Y = newY;
            moverEntity.Z = newZ;
            _aoi.Move(objectId, newX, newY);
        }

        HashSet<int> after;
        List<(MsgType Type, object Msg)> enteredForMover = new();
        List<(int WatcherId, object Msg)> enteredForWatchers = new();
        List<int> leftIds = new();
        lock (_lock)
        {
            after = _aoi.GetVisible(objectId).ToHashSet();
            foreach (int id in after)
            {
                if (before.Contains(id))
                    continue;
                if (!_entities.TryGetValue(id, out ZoneEntity? other))
                    continue;
                var (type, msg) = BuildPresenceMsg(other);
                enteredForMover.Add((type, msg));
                enteredForWatchers.Add((id, BuildCharInfo(moverEntity)));
            }

            foreach (int id in before)
            {
                if (after.Contains(id))
                    continue;
                leftIds.Add(id);
            }
        }

        foreach (var (type, msg) in enteredForMover)
            await mover.SendWireAsync(type, msg, ct).ConfigureAwait(false);
        foreach (var (watcherId, msg) in enteredForWatchers)
            await NotifyPlayerAsync(watcherId, MsgType.CharInfo, msg, ct).ConfigureAwait(false);

        foreach (int id in leftIds)
        {
            await mover.SendWireAsync(MsgType.DeleteObject, new DeleteObjectMsg { ObjectId = id }, ct)
                .ConfigureAwait(false);
            await NotifyPlayerAsync(id, MsgType.DeleteObject, new DeleteObjectMsg { ObjectId = objectId }, ct)
                .ConfigureAwait(false);
        }
    }

    async Task NotifyPlayerAsync(int playerObjectId, MsgType type, object msg, CancellationToken ct = default)
    {
        if (!_playersByObjectId.TryGetValue(playerObjectId, out PlayerSession? session))
            return;
        await session.SendWireAsync(type, msg, ct).ConfigureAwait(false);
    }

    public static (MsgType Type, object Msg) BuildPresenceMsg(ZoneEntity entity) =>
        entity.Kind switch
        {
            ZoneEntityKind.Npc => (MsgType.NpcInfo, BuildNpcInfo(entity)),
            _ => (MsgType.CharInfo, BuildCharInfo(entity))
        };

    public static CharInfoMsg BuildCharInfo(ZoneEntity entity) => new()
    {
        ObjectId = entity.ObjectId,
        Name = entity.Name,
        X = entity.X,
        Y = entity.Y,
        Z = entity.Z,
        ClassId = entity.ClassId
    };

    public static NpcInfoMsg BuildNpcInfo(ZoneEntity entity) => new()
    {
        ObjectId = entity.ObjectId,
        NpcTemplateId = entity.NpcTemplateId,
        X = entity.X,
        Y = entity.Y,
        Z = entity.Z
    };
}
