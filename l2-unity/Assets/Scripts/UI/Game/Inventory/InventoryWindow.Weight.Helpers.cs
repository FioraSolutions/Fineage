public partial class InventoryWindow
{
    public void UpdateStats(PlayerStats stats)
    {
        if (stats == null)
            return;

        _currentWeight = stats.CurrWeight;
        _maximumWeight = stats.MaxWeight;
        float weightRatio = stats.MaxWeight > 0
            ? (float)stats.CurrWeight / stats.MaxWeight
            : 0f;
        if (_weightLabel != null)
            _weightLabel.text = $"{weightRatio * 100f:0.00}%";

        // WeightBg = track, WeightGauge = Left/Center/Right fill clipped by width.
        VisualElement track = _weiBar;
        VisualElement fill = _weiBarBg;
        if (track == null || fill == null)
            return;

        PaintWeightFillParts();
        ApplyInventoryWeightFill(weightRatio);

        // First open often runs before UITK has resolved the track width — retry next frame.
        if (track.resolvedStyle.width <= 1f)
            StartCoroutine(ApplyInventoryWeightFillNextFrame(weightRatio));
    }

    IEnumerator ApplyInventoryWeightFillNextFrame(float weightRatio)
    {
        yield return null;
        ApplyInventoryWeightFill(weightRatio);
    }

    void ApplyInventoryWeightFill(float weightRatio)
    {
        VisualElement track = _weiBar;
        VisualElement fill = _weiBarBg;
        if (track == null || fill == null)
            return;

        float trackWidth = track.resolvedStyle.width;
        if (float.IsNaN(trackWidth) || trackWidth <= 1f)
            trackWidth = track.layout.width;
        if (float.IsNaN(trackWidth) || trackWidth <= 1f)
            trackWidth = 160f;

        fill.style.position = Position.Absolute;
        fill.style.left = 0;
        fill.style.top = 0;
        fill.style.height = 12f;
        fill.style.overflow = Overflow.Hidden;
        fill.style.flexDirection = FlexDirection.Row;
        fill.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
        L2ResourceBarView.SetFillFixed(fill, track, weightRatio, trackWidth);
    }
}
