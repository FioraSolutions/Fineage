using System;
using UnityEngine;

/// <summary>
/// Console TX/RX log for Zone V2 frames. High-frequency move/status lines are sampled
/// so the console stays readable while debugging CharSelect/vitals/shortcuts.
/// Toggle with PlayerPrefs <c>Fineage.ZoneWireLog</c> (1 = on, default on in Editor).
/// Prefs are cached — FrameIO runs on a worker thread and must not call PlayerPrefs there.
/// </summary>
public static class ZoneV2WireLog
{
    public const string PrefKey = "Fineage.ZoneWireLog";

    static bool? _enabled;
    static int _moveTxSample;
    static int _moveRxSample;
    static int _statusRxSample;

    public static bool Enabled
    {
        get
        {
            if (_enabled.HasValue)
                return _enabled.Value;
            try
            {
#if UNITY_EDITOR
                _enabled = PlayerPrefs.GetInt(PrefKey, 1) != 0;
#else
                _enabled = PlayerPrefs.GetInt(PrefKey, 0) != 0;
#endif
            }
            catch (UnityException)
            {
                // Off main thread before cache was primed — Editor default on, player off.
#if UNITY_EDITOR
                return true;
#else
                return false;
#endif
            }

            return _enabled.Value;
        }
        set
        {
            _enabled = value;
            try
            {
                PlayerPrefs.SetInt(PrefKey, value ? 1 : 0);
                PlayerPrefs.Save();
            }
            catch (UnityException)
            {
                /* ignore off-thread toggle */
            }
        }
    }

    /// <summary>Call from main thread (boot / Awake) so worker FrameIO never hits PlayerPrefs.</summary>
    public static void Warmup()
    {
        _ = Enabled;
    }

    public static void Tx(ZoneV2MsgType type, byte[] body)
    {
        if (!Enabled)
            return;
        if (IsNoisyTx(type) && (++_moveTxSample % 20) != 1)
            return;
        int len = body != null ? body.Length : 0;
        Debug.Log("[ZoneV2][TX] " + Name(type) + " (" + (ushort)type + ") bytes=" + len);
    }

    public static void Rx(ZoneV2MsgType type, byte[] body)
    {
        if (!Enabled)
            return;
        if (type == ZoneV2MsgType.MoveToAck || type == ZoneV2MsgType.MoveTo)
        {
            if ((++_moveRxSample % 20) != 1)
                return;
        }
        else if (type == ZoneV2MsgType.StatusUpdate)
        {
            if ((++_statusRxSample % 10) != 1)
                return;
        }

        int len = body != null ? body.Length : 0;
        string detail = Detail(type, body);
        if (string.IsNullOrEmpty(detail))
            Debug.Log("[ZoneV2][RX] " + Name(type) + " (" + (ushort)type + ") bytes=" + len);
        else
            Debug.Log("[ZoneV2][RX] " + Name(type) + " (" + (ushort)type + ") bytes=" + len + " " + detail);
    }

    static bool IsNoisyTx(ZoneV2MsgType type)
    {
        return type == ZoneV2MsgType.MoveTo || type == ZoneV2MsgType.AttackRequest;
    }

    static string Name(ZoneV2MsgType type)
    {
        return Enum.IsDefined(typeof(ZoneV2MsgType), type) ? type.ToString() : ("Unknown_" + (ushort)type);
    }

    static string Detail(ZoneV2MsgType type, byte[] body)
    {
        if (body == null || body.Length == 0)
            return "";
        try
        {
            switch (type)
            {
                case ZoneV2MsgType.UserInfo:
                {
                    ZoneV2UserInfo user;
                    if (!ZoneV2Messages.TryDecodeUserInfo(body, out user))
                        return "decode_fail";
                    return "oid=" + user.ObjectId
                        + " class=" + user.ClassId
                        + " hp=" + user.CurHp + "/" + user.MaxHp
                        + " mp=" + user.CurMp + "/" + user.MaxMp
                        + " exp=" + user.Exp
                        + " vitals=" + user.HasVitals
                        + " stats=" + user.HasStats;
                }
                case ZoneV2MsgType.CharacterList:
                {
                    ZoneV2CharacterSummary[] rows;
                    if (!ZoneV2Messages.TryDecodeCharacterList(body, out rows))
                        return "decode_fail";
                    if (rows == null || rows.Length == 0)
                        return "count=0";
                    ZoneV2CharacterSummary r = rows[0];
                    return "count=" + rows.Length
                        + " [0]=" + r.Name
                        + " hp=" + r.CurHp + "/" + r.MaxHp
                        + " weapon=" + r.WeaponItemId
                        + " paperdoll=" + (r.Paperdoll != null ? r.Paperdoll.Length : 0);
                }
                case ZoneV2MsgType.StatusUpdate:
                {
                    ZoneV2StatusUpdate status;
                    if (!ZoneV2Messages.TryDecodeStatusUpdate(body, out status))
                        return "decode_fail";
                    return "oid=" + status.ObjectId + " attrs=" + (status.AttributeIds != null ? status.AttributeIds.Length : 0);
                }
                case ZoneV2MsgType.SystemMessage:
                {
                    ZoneV2SystemMessage msg;
                    if (!ZoneV2Messages.TryDecodeSystemMessage(body, out msg))
                        return "";
                    return "id=" + msg.Id + " text=" + Truncate(msg.Text, 60);
                }
                default:
                    return "";
            }
        }
        catch (Exception)
        {
            return "detail_err";
        }
    }

    static string Truncate(string value, int max)
    {
        if (string.IsNullOrEmpty(value))
            return "";
        return value.Length <= max ? value : value.Substring(0, max) + "…";
    }
}
