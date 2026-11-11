using System.Collections.Generic;
using UnityEngine;

public sealed partial class L2RibbonEmitter : EffectPart
{
    private void SeedStarterStrip()
    {
        RibbonPoint head = _points[0];
        Vector3 blade = head.A2 - head.A3;
        Vector3 side = Vector3.Cross(blade.normalized, Vector3.up);
        if (side.sqrMagnitude < 1e-8f)
        {
            side = Vector3.Cross(blade.normalized, Vector3.right);
        }

        side = side.normalized * Mathf.Max(0.01f, _starterStripMeters);
        _points.Add(new RibbonPoint
        {
            A2 = head.A2 - side,
            A3 = head.A3 - side,
            A4 = head.A4,
            Time = head.Time - 0.001f,
        });
        Log($"SeedStarterStrip side={side.magnitude:F3} points={_points.Count} a4={head.A4:F3}");
    }

    public override void StopPart()
    {
        // Keep sampling until destroy — BeginFadeOut would otherwise freeze/clear the trail mid-swing.
    }

    private void Awake()
    {
        EnsureMesh();
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }
    }

    private void OnDisable()
    {
        _playing = false;
    }
}
