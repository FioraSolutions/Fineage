using System.Collections.Generic;

public sealed partial class HomeProjectileService : IHomeProjectileService
{
    public HomeProjectileService(
        HomeProjectileDualFlightRoots dualFlightRoots,
        HomeProjectileLauncher launcher)
    {
        _dualFlightRoots = dualFlightRoots;
        _launcher = launcher;
    }

    public void EnsureDualFlightRoots(BaseEffect instance, bool mirrorDualFlight)
    {
        _dualFlightRoots.Ensure(instance, mirrorDualFlight);
    }
}
