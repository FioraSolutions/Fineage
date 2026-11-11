using System;
using VContainer;

public static partial class NetworkingPacketServices
{
    static World ResolveWorld()
    {
        if (_world != null)
            return _world;
        return ResolveFromGameContainer<World>();
    }

    static WorldPacketApply ResolveWorldApply()
    {
        if (_worldApply != null)
            return _worldApply;
        return ResolveFromGameContainer<WorldPacketApply>();
    }

    static IWorldDomainSink ResolveDomain()
    {
        if (_domain != null)
            return _domain;
        return ResolveFromGameContainer<IWorldDomainSink>();
    }

    static LoginWindow LegacyLoginWindowInstance() => LoginWindow.Instance;
}
