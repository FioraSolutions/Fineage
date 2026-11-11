using System;
using Object = UnityEngine.Object;
using UnityEngine;
using System.Globalization;
using UnityEngine.Rendering;

public partial class L2CelestialDiscs
{
    void ApplyBody(
        string body,
        ref BodyTrack track,
        Transform tf,
        MeshRenderer mr,
        Material mat,
        Camera cam,
        Vector3 camPos,
        float hours,
        L2CelestialLut.Sample sample,
        Vector3 lutOffset,
        Vector3 lutDir,
        float lutElevDeg,
        float drawScale,
        float visScale,
        float refSideUu,
        float refDrawScale,
        float refDistanceUu,
        Color tint,
        bool mirrorX,
        bool periodic)
    {
        bool visible = visScale > VisibleScaleEpsilon;
        if (mr != null && mr.enabled != visible)
        {
            mr.enabled = visible;
        }

        bool locYNeg = lutOffset.y < 0f;
        bool locTiny = lutOffset.sqrMagnitude < 0.01f;

        // Sweep actor Location has frozen/outlier keys (e.g. LUT 27, 50). Lerp through
        // those keys bounces the disc. Direction is smooth; L2 loc ≈ -dir when valid.
        Vector3 placeDir = lutDir.sqrMagnitude > 1e-8f ? lutDir.normalized : Vector3.up;
        bool flipped = visible && placeDir.y < 0f;
        string source = "DIR";
        if (flipped)
        {
            placeDir = -placeDir;
            source = "DIR_FLIP";
        }

        float skyDistance = ResolveSkyDistance(cam);

        Vector3 pos = camPos + placeDir * skyDistance;
        if (!_followCamera)
        {
            pos = placeDir * skyDistance;
        }

        LogTrajectory(body, ref track, hours, sample, visible, flipped, locYNeg, locTiny, source,
            lutOffset, lutDir, lutElevDeg, drawScale, visScale, placeDir, pos, camPos, periodic);

        if (!visible)
        {
            return;
        }

        tf.position = pos;

        if (cam != null)
        {
            // Screen-aligned: a world LookRotation(+up) tilts the quad and stretches the disc.
            tf.rotation = cam.transform.rotation * Quaternion.Euler(0f, 180f, 0f);
        }

        float sideUu = refSideUu * (drawScale / Mathf.Max(0.01f, refDrawScale));
        float distanceM = Vector3.Distance(pos, camPos);
        float refDistanceM = VectorUtils.ConvertL2UuToMeters(refDistanceUu);
        if (distanceM > 0.05f && refDistanceM > 0.05f)
        {
            sideUu *= distanceM / refDistanceM;
        }

        float sideM = VectorUtils.ConvertL2UuToMeters(sideUu);
        tf.localScale = new Vector3(mirrorX ? -sideM : sideM, sideM, 1f);

        if (mat != null)
        {
            mat.SetColor("_Color", tint);
        }
    }

    void LogTrajectory(
        string body,
        ref BodyTrack track,
        float hours,
        L2CelestialLut.Sample sample,
        bool visible,
        bool fallback,
        bool locYNeg,
        bool locTiny,
        string source,
        Vector3 lutOffset,
        Vector3 lutDir,
        float lutElevDeg,
        float drawScale,
        float visScale,
        Vector3 placeDir,
        Vector3 pos,
        Vector3 camPos,
        bool periodic)
    {
        if (!Application.isPlaying || !_logTrajectory)
        {
            track.hasLast = visible;
            track.lastVisible = visible;
            track.lastFallback = fallback;
            track.lastPlaceDir = placeDir;
            track.lastPos = pos;
            track.lastHours = hours;
            return;
        }

        float locMag = lutOffset.magnitude;
        Vector3 locDir = locMag > 1e-6f ? lutOffset / locMag : Vector3.zero;
        float locElev = locMag > 1e-6f ? Mathf.Asin(Mathf.Clamp(locDir.y, -1f, 1f)) * Mathf.Rad2Deg : 0f;
        Vector3 dirN = lutDir.sqrMagnitude > 1e-8f ? lutDir.normalized : Vector3.zero;
        float dirElev = dirN.sqrMagnitude > 1e-8f ? Mathf.Asin(Mathf.Clamp(dirN.y, -1f, 1f)) * Mathf.Rad2Deg : 0f;
        float placeElev = Mathf.Asin(Mathf.Clamp(placeDir.y, -1f, 1f)) * Mathf.Rad2Deg;
        float placeAzim = Mathf.Atan2(placeDir.x, placeDir.z) * Mathf.Rad2Deg;
        float locVsDir = (locMag > 1e-6f && dirN.sqrMagnitude > 1e-8f) ? Vector3.Angle(locDir, dirN) : -1f;

        float angDelta = 0f;
        float hoursDelta = 0f;
        if (track.hasLast)
        {
            angDelta = Vector3.Angle(track.lastPlaceDir, placeDir);
            hoursDelta = hours - track.lastHours;
            if (hoursDelta < -12f)
            {
                hoursDelta += 24f;
            }
        }

        bool jump = track.hasLast && visible && track.lastVisible && angDelta >= _jumpAngleDeg;
        bool fallbackFlip = track.hasLast && visible && track.lastFallback != fallback;
        bool visFlip = track.hasLast && track.lastVisible != visible;
        if (periodic || jump || fallbackFlip || visFlip)
        {
            string tag = jump ? "JUMP " : (fallbackFlip ? "FLIP " : (visFlip ? "VIS  " : "     "));
            // Debug.Log(string.Format(CultureInfo.InvariantCulture,
                // "[L2Sky] {0}{1} {2} h={3:00.000} {4} vis={5} src={6} i={7}-{8} u={9:0.00} phase={10} " +
                // "angΔ={11:0.00}° dh={12:0.0000}h locVsDir={13:0.0}° " +
                // "lutEl={14:0.0} dirEl={15:0.0} locEl={16:0.0} placeEl={17:0.0} az={18:0.0} " +
                // "visS={19:0.00} draw={20:0.00} locY={21:0.00} locMag={22:0.00} tiny={23} yNeg={24} fb={25} " +
                // "loc=({26:0.00},{27:0.00},{28:0.00}) dir=({29:0.00},{30:0.00},{31:0.00}) " +
                // "place=({32:0.00},{33:0.00},{34:0.00}) pos=({35:0.00},{36:0.00},{37:0.00}) dist={38:0.00}",
                // tag, body, FormatClock(hours), hours, visible ? "ON" : "OFF", visible, source,
                // sample.sampleA, sample.sampleB, sample.lerpU, sample.phase,
                // angDelta, hoursDelta, locVsDir,
                // lutElevDeg, dirElev, locElev, placeElev, placeAzim,
                // visScale, drawScale, lutOffset.y, locMag, locTiny, locYNeg, fallback,
                // lutOffset.x, lutOffset.y, lutOffset.z,
                // lutDir.x, lutDir.y, lutDir.z,
                // placeDir.x, placeDir.y, placeDir.z,
                // pos.x, pos.y, pos.z,
                // Vector3.Distance(pos, camPos)));
        }

        track.hasLast = true;
        track.lastVisible = visible;
        track.lastFallback = fallback;
        track.lastPlaceDir = placeDir;
        track.lastPos = pos;
        track.lastHours = hours;
    }
}
