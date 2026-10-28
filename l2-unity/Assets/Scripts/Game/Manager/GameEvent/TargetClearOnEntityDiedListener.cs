using System;

/// <summary>
/// Subscribes to <see cref="EntityDiedEvent"/> and drops target/attack selection (#143).
/// </summary>
public sealed class TargetClearOnEntityDiedListener : IDisposable
{
    private readonly IDisposable _subscription;

    public TargetClearOnEntityDiedListener(IGameEventBus gameEvents)
    {
        if (gameEvents == null)
            throw new ArgumentNullException(nameof(gameEvents));

        _subscription = gameEvents.Subscribe<EntityDiedEvent>(OnEntityDied);
    }

    private void OnEntityDied(EntityDiedEvent e)
    {
        TargetManager targets = GameEventPacketServices.Targets;
        if (targets == null)
            return;

        if (targets.HasTarget())
        {
            TargetData target = targets.Target;
            int id = target != null && target.Identity != null ? target.Identity.Id : -1;
            if (id == e.ObjectId)
            {
                targets.ClearTarget();
                return;
            }
        }

        if (!targets.HasAttackTarget())
            return;

        TargetData attack = targets.AttackTarget;
        int attackId = attack != null && attack.Identity != null ? attack.Identity.Id : -1;
        if (attackId == e.ObjectId)
            targets.ClearAttackTarget();
    }

    public void Dispose()
    {
        _subscription?.Dispose();
    }
}
