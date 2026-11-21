using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Shared Interlude detailed-map UI: '+' hotspots, auto-open in zone, back to world map, markers.
/// </summary>
public sealed class DetailedMapOverlay
{
    readonly VisualElement _map;
    readonly VisualElement _layer;
    readonly VisualElement _layerImage;
    readonly VisualElement _layerMarkersRoot;
    readonly VisualElement _playerMarker;
    readonly Label _layerTitle;
    readonly VisualElement _layerClose;
    readonly List<Button> _hotspotButtons = new();
    readonly List<VisualElement> _partyMarkers = new();

    DetailedMapZoneRecord _openZone;

    const string PlusTexture = "Data/UI/Map/Town/btn_town_plus";
    const string PlusOverTexture = "Data/UI/Map/Town/btn_town_plus_over";
    const string PartyPinTexture = "Data/UI/Map/MinimapPos";

    public bool IsDetailedOpen => _openZone != null;

    public DetailedMapOverlay(VisualElement map, VisualElement layer)
    {
        _map = map;
        _layer = layer;

        _layerImage = layer?.Q<VisualElement>("town-map-image");
        _layerMarkersRoot = layer?.Q<VisualElement>("town-map-markers");
        _playerMarker = layer?.Q<VisualElement>("town-map-marker");
        _layerTitle = layer?.Q<Label>("town-map-title");
        _layerClose = layer?.Q<VisualElement>("town-map-close");

        if (_layerClose != null)
            _layerClose.RegisterCallback<ClickEvent>(_ => Close());

        DetailedMapCatalog.EnsureLoaded();
        BuildHotspotButtons();
    }

    void BuildHotspotButtons()
    {
        foreach (DetailedMapZoneRecord zone in DetailedMapCatalog.Zones)
        {
            if (zone == null || !zone.HasWorldMapHotspot)
                continue;

            var btn = new Button();
            btn.name = $"detailed-map-plus-{zone.id}";
            btn.style.position = Position.Absolute;
            btn.style.width = DetailedMapProjection.HotspotButtonSize;
            btn.style.height = DetailedMapProjection.HotspotButtonSize;
            btn.style.left = zone.hotspotMapX;
            btn.style.top = zone.hotspotMapY;
            btn.style.marginLeft = 0;
            btn.style.marginTop = 0;
            btn.style.paddingLeft = 0;
            btn.style.paddingRight = 0;
            btn.style.paddingTop = 0;
            btn.style.paddingBottom = 0;
            btn.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>(PlusTexture));
            btn.style.borderTopWidth = 0;
            btn.style.borderBottomWidth = 0;
            btn.style.borderLeftWidth = 0;
            btn.style.borderRightWidth = 0;
            btn.tooltip = zone.name;
            btn.RegisterCallback<ClickEvent>(evt =>
            {
                evt.StopPropagation();
                Open(zone);
            });
            btn.RegisterCallback<PointerEnterEvent>(_ =>
            {
                var over = Resources.Load<Texture2D>(PlusOverTexture);
                if (over != null)
                    btn.style.backgroundImage = new StyleBackground(over);
            });
            btn.RegisterCallback<PointerLeaveEvent>(_ =>
            {
                var normal = Resources.Load<Texture2D>(PlusTexture);
                if (normal != null)
                    btn.style.backgroundImage = new StyleBackground(normal);
            });
            btn.RegisterCallback<PointerDownEvent>(evt => evt.StopPropagation());

            _map.Add(btn);
            _hotspotButtons.Add(btn);
        }
    }

    public bool TryAutoOpenForPlayer()
    {
        PlayerController player = ControllerPacketServices.Player;
        if (player == null)
            return false;

        Vector2 l2 = PlayerL2(player);
        DetailedMapZoneRecord zone = DetailedMapCatalog.FindByWorldPosition(l2.x, l2.y);
        if (zone == null)
            return false;

        Open(zone);
        return true;
    }

    public void Open(DetailedMapZoneRecord zone)
    {
        if (zone == null || _layer == null)
            return;

        if (string.IsNullOrEmpty(zone.importedResource))
        {
            Debug.LogError(
                $"[DetailedMap] Missing texture for zone '{zone.name}' (id={zone.id}). " +
                $"Expected {zone.ExpectedTextureLabel}. " +
                "Export from the Interlude client and import to Resources — see docs/research/detailed-maps-assets.md.");
            return;
        }

        Texture2D tex = Resources.Load<Texture2D>(zone.importedResource);
        if (tex == null)
        {
            Debug.LogError(
                $"[DetailedMap] Resources.Load failed for zone '{zone.name}' (id={zone.id}), " +
                $"path='{zone.importedResource}', expected original {zone.ExpectedTextureLabel}.");
            return;
        }

        _openZone = zone;
        if (_layerTitle != null)
            _layerTitle.text = zone.name;

        if (_layerImage != null)
        {
            _layerImage.style.backgroundImage = new StyleBackground(tex);
            _layerImage.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
        }

        _layer.style.display = DisplayStyle.Flex;
        UpdateMarkers();
    }

    public void Close()
    {
        _openZone = null;
        if (_layer != null)
            _layer.style.display = DisplayStyle.None;
    }

    static Vector2 PlayerL2(PlayerController player)
    {
        if (player == null)
            return Vector2.zero;

        Entity entity = player.GetComponent<Entity>();
        if (entity != null && entity.Identity != null)
        {
            Vector3 l2j = entity.Identity.GetL2jPos(player.GetPlayerPosition());
            return L2MapCoordinates.MapPlaneFromL2jVector(l2j);
        }

        return L2MapCoordinates.MapPlaneFromUnity(player.GetPlayerPosition());
    }

    public void UpdateMarkers()
    {
        if (_openZone == null || _layerImage == null)
            return;

        float w = _layerImage.layout.width;
        float h = _layerImage.layout.height;
        if (w <= 0f || h <= 0f)
            return;

        PlacePlayerMarker(w, h);
        PlacePartyMarkers(w, h);
    }

    void PlacePlayerMarker(float uiW, float uiH)
    {
        if (_playerMarker == null)
            return;

        PlayerController player = ControllerPacketServices.Player;
        if (player == null)
            return;

        Vector2 l2 = PlayerL2(player);
        Vector2 ui = DetailedMapProjection.ProjectDetailedToUi(_openZone, l2.x, l2.y, uiW, uiH);
        float mw = _playerMarker.layout.width > 0 ? _playerMarker.layout.width : 32f;
        float mh = _playerMarker.layout.height > 0 ? _playerMarker.layout.height : 32f;
        _playerMarker.style.left = ui.x - mw * 0.5f;
        _playerMarker.style.top = ui.y - mh * 0.5f;
    }

    void PlacePartyMarkers(float uiW, float uiH)
    {
        if (_layerMarkersRoot == null)
            return;

        int localId = DomainPacketServices.LocalPlayer != null
            ? DomainPacketServices.LocalPlayer.Identity.Id
            : 0;

        var visible = new List<MapPartyPositions.MemberPosition>();
        foreach (KeyValuePair<int, MapPartyPositions.MemberPosition> kv in MapPartyPositions.Positions)
        {
            if (kv.Key == localId)
                continue;
            if (!DetailedMapProjection.ContainsWorldPosition(_openZone, kv.Value.L2X, kv.Value.L2Y))
                continue;
            visible.Add(kv.Value);
        }

        while (_partyMarkers.Count < visible.Count)
        {
            var marker = new VisualElement();
            marker.style.position = Position.Absolute;
            marker.style.width = 16;
            marker.style.height = 16;
            marker.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>(PartyPinTexture));
            _layerMarkersRoot.Add(marker);
            _partyMarkers.Add(marker);
        }

        for (int i = 0; i < _partyMarkers.Count; i++)
        {
            bool show = i < visible.Count;
            _partyMarkers[i].style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
            if (!show)
                continue;

            MapPartyPositions.MemberPosition pos = visible[i];
            Vector2 ui = DetailedMapProjection.ProjectDetailedToUi(
                _openZone, pos.L2X, pos.L2Y, uiW, uiH);
            _partyMarkers[i].style.left = ui.x - 8f;
            _partyMarkers[i].style.top = ui.y - 8f;
        }
    }
}