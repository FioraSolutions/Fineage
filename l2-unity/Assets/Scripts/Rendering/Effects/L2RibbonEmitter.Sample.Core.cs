using UnityEngine;
using System.Collections.Generic;

public partial class L2RibbonEmitter
{
    private bool TrySample(bool force)
    {
        _sampleAttempts++;
        ResolveBladeAnchors();

        if (!TryBuildEdges(out L2FxRibbonGetPoint.Edges edges))
        {
            if (_debugLog && (_sampleAttempts <= 3 || force))
            {
                Log($"TrySample FAIL source={_lastSampleSource} tipNull={_swordTip == null} baseNull={_swordBase == null}");
            }

            return false;
        }

        Transform space = ResolveSpace();
        Vector3 a2 = space != null ? space.InverseTransformPoint(edges.A2) : edges.A2;
        Vector3 a3 = space != null ? space.InverseTransformPoint(edges.A3) : edges.A3;

        if (_stabilizeEdgePolarity && _points.Count > 0 && ShouldStabilizeEdgePolarity())
        {
            L2FxRibbonGetPoint.Edges cur = new L2FxRibbonGetPoint.Edges
            {
                A2 = a2,
                A3 = a3,
                A4 = edges.A4,
            };
            L2FxRibbonGetPoint.Edges reference = new L2FxRibbonGetPoint.Edges
            {
                A2 = _points[0].A2,
                A3 = _points[0].A3,
                A4 = _points[0].A4,
            };
            L2FxRibbonGetPoint.Edges stable = L2FxRibbonGetPoint.StabilizeEdgeEndsByProximity(cur, reference);
            if (!Mathf.Approximately(stable.A2.x, a2.x) ||
                !Mathf.Approximately(stable.A2.y, a2.y) ||
                !Mathf.Approximately(stable.A2.z, a2.z))
            {
                _lastTipWorld = space != null ? space.TransformPoint(stable.A2) : stable.A2;
                _lastBaseWorld = space != null ? space.TransformPoint(stable.A3) : stable.A3;
            }

            a2 = stable.A2;
            a3 = stable.A3;
            edges.A2 = space != null ? space.TransformPoint(a2) : a2;
            edges.A3 = space != null ? space.TransformPoint(a3) : a3;
        }

        if (_points.Count > 0)
        {
            Vector3 mid = L2FxRibbonGetPoint.EdgeMid(a2, a3);
            Vector3 prevMid = L2FxRibbonGetPoint.EdgeMid(_points[0].A2, _points[0].A3);
            float midDist = Vector3.Distance(mid, prevMid);
            float tipDist = Vector3.Distance(a2, _points[0].A2);
            float baseDist = Vector3.Distance(a3, _points[0].A3);
            // Pure rotation around mid: tip/base move while mid stays — still need sheets.
            float travel = Mathf.Max(midDist, Mathf.Max(tipDist, baseDist));

            if (!force && travel < _minSampleDistance)
            {
                RibbonPoint head = _points[0];
                head.A2 = a2;
                head.A3 = a3;
                head.A4 = edges.A4;
                head.Time = Time.time;
                _points[0] = head;
                if (_debugLog && _debugTraceMotion && _traceFrame < 3)
                {
                    Log(
                        $"BUILD head-update travel={travel:F5} mid={midDist:F5} tip={tipDist:F5} " +
                        $"a2={Fmt(edges.A2)} a3={Fmt(edges.A3)} " +
                        $"tipW={Fmt(_lastTipWorld)} baseW={Fmt(_lastBaseWorld)}");
                }

                return true;
            }

            L2FxRibbonGetPoint.Edges newest = new L2FxRibbonGetPoint.Edges
            {
                A2 = a2,
                A3 = a3,
                A4 = edges.A4,
            };
            L2FxRibbonGetPoint.Edges previous = new L2FxRibbonGetPoint.Edges
            {
                A2 = _points[0].A2,
                A3 = _points[0].A3,
                A4 = _points[0].A4,
            };

            int sheets = L2FxRibbonGetPoint.CountInterpSheetsForEdges(
                previous,
                newest,
                _maxSegmentMeters,
                _maxSheetsPerSample);

            Vector3 pivot = ResolveSheetPivot(space);

            // Head = newest. Insert sheets between newest and previous head.
            RibbonPoint newestPt = new RibbonPoint
            {
                A2 = newest.A2,
                A3 = newest.A3,
                A4 = newest.A4,
                Time = Time.time,
            };
            _points.Insert(0, newestPt);

            for (int i = 1; i <= sheets; i++)
            {
                // t=0 at newest, t=1 at previous — same order as before.
                float t = (float)i / (sheets + 1);
                L2FxRibbonGetPoint.Edges sheet = _arcSheetInterpolation
                    ? L2FxRibbonGetPoint.LerpEdgesArc(newest, previous, t, pivot)
                    : L2FxRibbonGetPoint.LerpEdges(newest, previous, t);
                _points.Insert(i, new RibbonPoint
                {
                    A2 = sheet.A2,
                    A3 = sheet.A3,
                    A4 = sheet.A4,
                    Time = Time.time,
                });
            }

            while (_points.Count > _maxPoints)
            {
                _points.RemoveAt(_points.Count - 1);
            }

            _sampleAccepted++;
            if (_debugLog && (_debugTracePoints || _sampleAccepted <= 8 || sheets > 0))
            {
                float tipDelta = _havePrevBlade
                    ? Vector3.Distance(_lastTipWorld, _prevTipWorld)
                    : 0f;
                Log(
                    $"BUILD#{_sampleAccepted} mode=insert+sheets source={_lastSampleSource} " +
                    $"a2={Fmt(edges.A2)} a3={Fmt(edges.A3)} a4={edges.A4:F4} " +
                    $"|a2-a3|={Vector3.Distance(edges.A2, edges.A3):F4} " +
                    $"midDist={midDist:F4} sheets={sheets} points={_points.Count} " +
                    $"tipW={Fmt(_lastTipWorld)} baseW={Fmt(_lastBaseWorld)} tipStep={tipDelta:F4}");
            }

            return true;
        }

        RibbonPoint point = new RibbonPoint
        {
            A2 = a2,
            A3 = a3,
            A4 = edges.A4,
            Time = Time.time,
        };

        _points.Insert(0, point);
        while (_points.Count > _maxPoints)
        {
            _points.RemoveAt(_points.Count - 1);
        }

        _sampleAccepted++;
        if (_debugLog && (_debugTracePoints || _sampleAccepted <= 8 || force))
        {
            Log(
                $"BUILD#{_sampleAccepted} mode=first source={_lastSampleSource} " +
                $"a2={Fmt(edges.A2)} a3={Fmt(edges.A3)} a4={edges.A4:F4} " +
                $"|a2-a3|={Vector3.Distance(edges.A2, edges.A3):F4} points={_points.Count} " +
                $"tipW={Fmt(_lastTipWorld)} baseW={Fmt(_lastBaseWorld)}");
        }

        return true;
    }
}
