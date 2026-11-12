using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed partial class AppLifetimeScope : LifetimeScope
{
    IZoneTransport _boundTransport;
    IGameManager _boundManager;
    Action _onDisconnected;

    protected override void OnDestroy()
    {
        if (_boundTransport != null && _onDisconnected != null)
            _boundTransport.OnDisconnected -= _onDisconnected;
        _onDisconnected = null;
        ZoneCommands.Unbind(_boundTransport);
        _boundTransport = null;
        NetGameHost.Unbind(_boundManager);
        _boundManager = null;
        UnbindDatTableSeams();
        if (ReferenceEquals(App.Container, Container))
            App.Container = null;
        base.OnDestroy();
    }

    /// <summary>
    /// Clear static Bind seams owned by this scope so a rebuild does not keep stale table/animation hosts.
    /// </summary>
    static void UnbindDatTableSeams()
    {
        AnimationManager.Unbind();
        LogongrpTable.Unbind();
        ItemNameTable.Unbind();
        SkillgrpTable.Unbind();
        ItemStatDataTable.Unbind();
        ArmorgrpTable.Unbind();
        EtcItemgrpTable.Unbind();
        WeapongrpTable.Unbind();
        NpcgrpTable.Unbind();
        NpcNameTable.Unbind();
        ActionNameTable.Unbind();
        SkillNameTable.Unbind();
    }
}
