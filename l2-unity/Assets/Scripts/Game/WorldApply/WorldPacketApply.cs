using System;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class WorldPacketApply
{
    private readonly Dictionary<Type, EntityWorldApply> _byType = new Dictionary<Type, EntityWorldApply>();

    /// <summary>
    /// Per-object spawn attempt counter. A prefab that spawns without its NpcEntity, or a spawner
    /// that never gets wired, would otherwise be retried every frame forever.
    /// </summary>
    private readonly Dictionary<int, int> _spawnAttempts = new Dictionary<int, int>();
    private const int MaxSpawnAttempts = 3;
    private readonly HtmlWindow _html;
    private readonly PlayerPositionSender _positionSender;
    private readonly GameManager _gameManager;
    private readonly PacketApplyQueue _applyQueue;
    private readonly IGameEventBus _gameEvents;


    private World GameWorld => WorldApplyPacketServices.GameWorld;

    public void Register<TEntity>(EntityWorldApply apply) where TEntity : Entity
    {
        _byType[typeof(TEntity)] = apply;
    }



    /// <summary>
    /// Applies a chunk of knownlist rows, re-queueing itself while the spawn budget is exhausted.
    /// One continuation per frame instead of one queued action per NPC.
    /// </summary>

}
