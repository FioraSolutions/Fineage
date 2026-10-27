using System;

/// <summary>
/// Clears local player auto-attack when the current attack target dies (#143).
/// </summary>
public sealed class PlayerAttackClearOnEntityDiedListener : IDisposable
{
    private readonly IDisposable _subscription;

    public PlayerAttackClearOnEntityDiedListener(IGameEventBus gameEvents)
    {
        if (gameEvents == null)
            throw new ArgumentNullException(nameof(gameEvents));

        _subscription = gameEvents.Subscribe<EntityDiedEvent>(OnEntityDied);
    }

    private void OnEntityDied(EntityDiedEvent e)
    {
        PlayerEntity player = GameEventPacketServices.LocalPlayer;
        if (player == null || player.TargetId != e.ObjectId)
            return;

        player.IsAttack = false;
    }

    public void Dispose()
    {
        _subscription?.Dispose();
    }
}
