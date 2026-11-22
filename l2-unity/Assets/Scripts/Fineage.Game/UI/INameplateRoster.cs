using UnityEngine;

/// <summary>
/// Nameplate updates <c>World</c> applies: mask and removal (#131 S38b).
/// </summary>
public interface INameplateRoster
{
    void Remove(int id);
    void SetMask(LayerMask mask);
}
