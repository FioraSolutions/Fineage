using System.Collections.Generic;
using UnityEngine;

public partial class TableColumn
{
    public void SetColor(string rowName, string hex_color)
    {
        if (_allColors == null) _allColors = new Dictionary<string, UnityEngine.Color>();

        UnityEngine.Color color;
        if (ColorUtility.TryParseHtmlString(hex_color, out color))
        {
            if (_allColors.ContainsKey(rowName))
            {
                _allColors[rowName] = color;  // Update existing color
            }
            else
            {
                _allColors.Add(rowName, color);  // Add new color
            }
        }
        else
        {
            // Debug.LogError($"Failed to parse color from hex string: {hex_color}");
        }
    }

    public UnityEngine.Color GetColor(string rowName)
    {
        if (_allColors != null && _allColors.ContainsKey(rowName))
        {
            return _allColors[rowName];
        }
        return UnityEngine.Color.white;
    }

    public void ResetColor(string rowName)
    {
        if (_allColors != null && _allColors.ContainsKey(rowName))
        {
            _allColors.Remove(rowName);
        }
    }

    public bool IsSetColor()
    {
        return _allColors != null && _allColors.Count > 0;
    }
}
