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
}
