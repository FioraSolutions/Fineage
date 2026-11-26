using System;
using UnityEngine;

/// <summary>
/// Interlude <c>interface.xdat</c> MagicSkillWnd skill grid — fixed columns per row (not width-derived).
/// </summary>
public static class InterludeMagicSkillWndLayout
{
    public const string ResourcesKey = "Data/UI/Interlude/MagicSkillWndLayout";

    static bool _loadAttempted;
    static InterludeMagicSkillWndLayoutJson _data;

    public static int ColumnsPerRow
    {
        get
        {
            EnsureLoaded();
            return _data != null && _data.columnsPerRow > 0 ? _data.columnsPerRow : 0;
        }
    }

    public static string RowFrameResource
    {
        get
        {
            EnsureLoaded();
            return _data != null ? _data.rowFrameResource : null;
        }
    }

    public static void EnsureLoaded()
    {
        if (_loadAttempted)
            return;

        _loadAttempted = true;

        TextAsset asset = Resources.Load<TextAsset>(ResourcesKey);
        if (asset == null)
        {
            Debug.LogError(
                "[SkillsLayout] Missing layout config Resources key '" + ResourcesKey +
                "' (expected MagicSkillWnd ItemWindow.MaxItemNum from interface.xdat).");
            return;
        }

        try
        {
            _data = JsonUtility.FromJson<InterludeMagicSkillWndLayoutJson>(asset.text);
        }
        catch (Exception ex)
        {
            Debug.LogError(
                "[SkillsLayout] Failed to parse layout config '" + ResourcesKey + "': " + ex.Message);
            _data = null;
            return;
        }

        if (_data == null)
        {
            Debug.LogError("[SkillsLayout] Layout config '" + ResourcesKey + "' parsed to null.");
            return;
        }

        if (string.IsNullOrWhiteSpace(_data.xdatWindow))
        {
            Debug.LogError(
                "[SkillsLayout] Layout config '" + ResourcesKey + "' missing required field 'xdatWindow'.");
        }

        if (string.IsNullOrWhiteSpace(_data.xdatSkillRowItemWindow))
        {
            Debug.LogError(
                "[SkillsLayout] Layout config '" + ResourcesKey +
                "' missing required field 'xdatSkillRowItemWindow'.");
        }

        if (_data.columnsPerRow <= 0)
        {
            Debug.LogError(
                "[SkillsLayout] Layout config '" + ResourcesKey +
                "' missing or invalid field 'columnsPerRow' (xdat " + _data.xdatMaxItemNumField +
                " on " + _data.xdatSkillRowItemWindow + ").");
        }

        if (string.IsNullOrWhiteSpace(_data.parityTemplateResource))
        {
            Debug.LogError(
                "[SkillsLayout] Layout config '" + ResourcesKey + "' missing required field 'parityTemplateResource'.");
        }
        else if (Resources.Load(_data.parityTemplateResource) == null)
        {
            Debug.LogError(
                "[SkillsLayout] Missing parity template Resources key '" + _data.parityTemplateResource +
                "' referenced by layout config '" + ResourcesKey + "'.");
        }

        if (string.IsNullOrWhiteSpace(_data.rowFrameResource))
        {
            Debug.LogError(
                "[SkillsLayout] Layout config '" + ResourcesKey + "' missing required field 'rowFrameResource'.");
        }
        else if (Resources.Load(_data.rowFrameResource) == null)
        {
            Debug.LogError(
                "[SkillsLayout] Missing row frame Resources key '" + _data.rowFrameResource +
                "' referenced by layout config '" + ResourcesKey + "'.");
        }
    }

#if UNITY_EDITOR
    public static void ResetForEditModeTests()
    {
        _loadAttempted = false;
        _data = null;
    }
#endif

    [Serializable]
    sealed class InterludeMagicSkillWndLayoutJson
    {
        public string xdatSourcePath;
        public string xdatWindow;
        public string xdatSkillRowItemWindow;
        public string xdatMaxItemNumField;
        public int columnsPerRow;
        public float rowWidthPx;
        public string parityTemplateResource;
        public string rowFrameResource;
        public string notes;
    }
}