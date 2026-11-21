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

        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player == null)
            return;

        Vector3 l2 = VectorUtils.ConvertPosUnityToL2j(player.transform.position);
        VectorUtils.GetSwMapTile(l2, out int cellX, out int cellY);
        string regionKey = cellX + ":" + cellY;

        if (regionKey == _lastRegionKey)
            return;
        _lastRegionKey = regionKey;

        string town = ClientMapRegionCatalog.ResolveTownName(l2.x, l2.y);
        ZoneNameBannerOverlay banner = ZoneTransitionPacketServices.Banner;
        if (banner == null)
        {
            Debug.LogError(
                "[ZoneTransition] banner overlay missing for region=" + regionKey +
                " town=\"" + town + "\"");
            return;
        }

        banner.Show(town);
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
        ZoneTransitionPacketServices.Banner?.Show(townShown);
        return true;
    }
}