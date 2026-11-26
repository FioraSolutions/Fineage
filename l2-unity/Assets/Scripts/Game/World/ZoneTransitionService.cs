using System;
using UnityEngine;
/// <summary>
/// Detects Mobius map-region cell changes from the local hero position and shows the town/zone name banner.
/// Peace/combat zone system messages remain server-driven (SystemMessage 116 / 283).
/// </summary>
public sealed class ZoneTransitionService : MonoBehaviour
{
    const float MinResampleSeconds = 0.25f;

    string _lastRegionKey;
    float _nextSample;
    ILocalPlayerSlot _players;
    ZoneNameBannerOverlay _banner;

    public void Use(ILocalPlayerSlot players, ZoneNameBannerOverlay banner)
    {
        if (players == null)
        {
            throw new InvalidOperationException(
                "[ZoneTransitionService] ILocalPlayerSlot missing — World.EnsureZoneTransition did not pass the slot, so region changes were not tracked.");
        }

        if (banner == null)
        {
            throw new InvalidOperationException(
                "[ZoneTransitionService] ZoneNameBannerOverlay missing — World.EnsureZoneTransition did not pass the banner, so the zone name was not shown.");
        }

        _players = players;
        _banner = banner;
    }

    void Awake() => ZoneTransitionPacketServices.Bind(service: this);

    void OnDestroy()
    {
        if (ZoneTransitionPacketServices.Service == this)
            ZoneTransitionPacketServices.Bind(service: null);
    }

    void Update()
    {
        if (Time.unscaledTime < _nextSample)
            return;
        _nextSample = Time.unscaledTime + MinResampleSeconds;

        if (_players == null || _banner == null || !_players.TryGetPosition(out Vector3 position))
            return;

        Vector3 l2 = VectorUtils.ConvertPosUnityToL2j(position);
        VectorUtils.GetSwMapTile(l2, out int cellX, out int cellY);
        string regionKey = cellX + ":" + cellY;

        if (regionKey == _lastRegionKey)
            return;
        _lastRegionKey = regionKey;

        string town = ClientMapRegionCatalog.ResolveTownName(l2.x, l2.y);
        _banner.Show(town);
    }

    /// <summary>EditMode/test hook: evaluate without waiting for Update cadence.</summary>
    public bool TryNotifyRegionForTest(float l2X, float l2Y, out string townShown)
    {
        VectorUtils.GetSwMapTile(new Vector3(l2X, l2Y, 0f), out int cellX, out int cellY);
        string regionKey = cellX + ":" + cellY;
        if (regionKey == _lastRegionKey)
        {
            townShown = null;
            return false;
        }

        _lastRegionKey = regionKey;
        townShown = ClientMapRegionCatalog.ResolveTownName(l2X, l2Y);
        _banner?.Show(townShown);
        return true;
    }
}