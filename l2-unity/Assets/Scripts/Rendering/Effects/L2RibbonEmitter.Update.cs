using System.Collections.Generic;
using UnityEngine;

public sealed partial class L2RibbonEmitter : EffectPart
{
    private void OnDestroy()
    {
        _playing = false;
        _points.Clear();
        ClearMesh();
        if (_mesh != null)
        {
            if (Application.isPlaying)
            {
                Destroy(_mesh);
            }
            else
            {
                DestroyImmediate(_mesh);
            }

            _mesh = null;
        }
    }

    private void LateUpdate()
    {
        if (!_playing)
        {
            return;
        }

        if (!_loggedPlayOnce)
        {
            ResolveBladeAnchors();
            Log("LateUpdate playing but PlayPart flag missing — forcing first sample");
            TrySample(force: true);
            _loggedPlayOnce = true;
        }

        if (Time.deltaTime <= 0f)
        {
            return;
        }

        TrySample(force: false);

        if (_points.Count > 0)
        {
            RebuildMesh();
        }

        LogMotionAndPointsTrace();
    }

    private void LogMotionAndPointsTrace()
    {
        if (!_debugLog)
        {
            return;
        }

        _traceFrame++;
        bool burst = _traceFrame <= Mathf.Max(1, _debugTraceFirstFrames);
        bool intervalDue = Time.time >= _nextDebugLogTime;
        if (!burst && !intervalDue)
        {
            return;
        }

        if (intervalDue)
        {
            _nextDebugLogTime = Time.time + Mathf.Max(0.02f, _debugLogInterval);
        }

        float age = Time.time - _playStartedAt;
        Vector3 tip = _lastTipWorld;
        Vector3 hilt = _lastBaseWorld;
        Vector3 mid = (tip + hilt) * 0.5f;
        float bladeLen = Vector3.Distance(tip, hilt);

        float tipDelta = 0f;
        float baseDelta = 0f;
        float midDelta = 0f;
        Vector3 tipVel = Vector3.zero;
        Vector3 midVel = Vector3.zero;
        if (_havePrevBlade)
        {
            tipDelta = Vector3.Distance(tip, _prevTipWorld);
            baseDelta = Vector3.Distance(hilt, _prevBaseWorld);
            midDelta = Vector3.Distance(mid, _prevMidWorld);
            float dt = Mathf.Max(1e-5f, Time.deltaTime);
            tipVel = (tip - _prevTipWorld) / dt;
            midVel = (mid - _prevMidWorld) / dt;
        }

        if (_debugTraceMotion)
        {
            RibbonPoint head = _points.Count > 0 ? _points[0] : default;
            RibbonPoint tail = _points.Count > 0 ? _points[_points.Count - 1] : default;
            Vector3 headMid = _points.Count > 0
                ? L2FxRibbonGetPoint.EdgeMid(head.A2, head.A3)
                : Vector3.zero;
            float headWidth = _points.Count > 0 ? Vector3.Distance(head.A2, head.A3) : 0f;
            float trailLen = EstimateTrailMidLength();

            Log(
                $"MOTION f={_traceFrame} age={age:F3}s dt={Time.deltaTime:F4} source={_lastSampleSource}/{_lastBladeResolveSource} " +
                $"storeLocal={_storeInOwnerLocal} emitter={transform.position} " +
                $"tip={Fmt(tip)} base={Fmt(hilt)} mid={Fmt(mid)} bladeLen={bladeLen:F4} " +
                $"dTip={tipDelta:F4} dBase={baseDelta:F4} dMid={midDelta:F4} " +
                $"tipSpeed={tipVel.magnitude:F2} midSpeed={midVel.magnitude:F2} " +
                $"points={_points.Count} accepted={_sampleAccepted} " +
                $"headMid={Fmt(headMid)} headW={headWidth:F4} trailMidLen={trailLen:F3}");
        }

        if (_debugTracePoints && (_points.Count != _lastLoggedPointCount || burst || intervalDue))
        {
            _lastLoggedPointCount = _points.Count;
            LogPointsDump();
        }

        _prevTipWorld = tip;
        _prevBaseWorld = hilt;
        _prevMidWorld = mid;
        _havePrevBlade = bladeLen > 1e-6f || tip.sqrMagnitude > 0f;
    }

    private float EstimateTrailMidLength()
    {
        if (_points.Count < 2)
        {
            return 0f;
        }

        float len = 0f;
        for (int i = 0; i < _points.Count - 1; i++)
        {
            Vector3 a = L2FxRibbonGetPoint.EdgeMid(_points[i].A2, _points[i].A3);
            Vector3 b = L2FxRibbonGetPoint.EdgeMid(_points[i + 1].A2, _points[i + 1].A3);
            len += Vector3.Distance(a, b);
        }

        return len;
    }

    private void LogPointsDump()
    {
        int n = _points.Count;
        if (n == 0)
        {
            Log("POINTS empty");
            return;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder(512);
        sb.Append("POINTS n=").Append(n)
            .Append(" space=").Append(_storeInOwnerLocal ? "ownerLocal" : "world");

        int show = Mathf.Min(4, n);
        for (int i = 0; i < show; i++)
        {
            AppendPoint(sb, "H" + i, _points[i]);
        }

        if (n > show + 1)
        {
            sb.Append(" ...");
            AppendPoint(sb, "T0", _points[n - 1]);
            if (n > show + 2)
            {
                AppendPoint(sb, "T1", _points[n - 2]);
            }
        }
        else if (n > show)
        {
            AppendPoint(sb, "T0", _points[n - 1]);
        }

        // Segment lengths head→next few (detect jumps vs L2 continuous path).
        sb.Append(" seg=");
        int segShow = Mathf.Min(6, n - 1);
        for (int i = 0; i < segShow; i++)
        {
            float seg = Vector3.Distance(
                L2FxRibbonGetPoint.EdgeMid(_points[i].A2, _points[i].A3),
                L2FxRibbonGetPoint.EdgeMid(_points[i + 1].A2, _points[i + 1].A3));
            if (i > 0)
            {
                sb.Append(',');
            }

            sb.Append(seg.ToString("F3"));
        }

        Log(sb.ToString());
    }

    private static void AppendPoint(System.Text.StringBuilder sb, string label, RibbonPoint p)
    {
        Vector3 mid = L2FxRibbonGetPoint.EdgeMid(p.A2, p.A3);
        float w = Vector3.Distance(p.A2, p.A3);
        sb.Append(' ').Append(label)
            .Append(" a2=").Append(Fmt(p.A2))
            .Append(" a3=").Append(Fmt(p.A3))
            .Append(" mid=").Append(Fmt(mid))
            .Append(" a4=").Append(p.A4.ToString("F3"))
            .Append(" w=").Append(w.ToString("F3"));
    }

    private static string Fmt(Vector3 v)
    {
        return $"({v.x:F3},{v.y:F3},{v.z:F3})";
    }

    private void Log(string message)
    {
        if (!_debugLog)
        {
            return;
        }

        // Debug.Log($"[L2RibbonEmitter] {name} {message}", this);
    }
}
