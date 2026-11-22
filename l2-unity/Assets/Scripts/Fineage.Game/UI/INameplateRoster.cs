using UnityEngine;

/// <summary>
/// Nameplate updates <c>World</c> injects (#131 S38b, S38n).
/// <c>GameLifetimeScope</c> registers the scene <c>NameplatesManager</c> as this interface.
/// </summary>
public interface INameplateRoster
{
    void Remove(int id);
    void SetMask(LayerMask mask);
}
