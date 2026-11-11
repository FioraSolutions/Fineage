using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public sealed partial class L2BitmapFont
{
    public L2BitmapFont(Texture2D atlas, TextAsset csv)
    {
        _atlas = atlas;
        _invW = 1f / Mathf.Max(1, atlas != null ? atlas.width : 1024);
        _invH = 1f / Mathf.Max(1, atlas != null ? atlas.height : 128);

        if (csv == null || string.IsNullOrEmpty(csv.text))
        {
            return;
        }

        string[] lines = csv.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (i == 0 && line.StartsWith("char", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            // char,code,u,v,w,h,page — char may be comma → ",,44,..."
            string payload;
            if (line.Length >= 2 && line[0] == ',' && line[1] == ',')
            {
                payload = line.Substring(2);
            }
            else
            {
                int firstComma = line.IndexOf(',');
                if (firstComma < 0)
                {
                    continue;
                }

                payload = line.Substring(firstComma + 1);
            }

            string[] parts = payload.Split(',');
            if (parts.Length < 5)
            {
                continue;
            }

            if (!int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int code))
            {
                continue;
            }

            if (code < 0 || code >= _byCode.Length)
            {
                continue;
            }

            if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int u) ||
                !int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out int v) ||
                !int.TryParse(parts[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out int w) ||
                !int.TryParse(parts[4], NumberStyles.Integer, CultureInfo.InvariantCulture, out int h))
            {
                continue;
            }

            _byCode[code] = new Glyph { U = u, V = v, W = w, H = h, Valid = w > 0 && h > 0 };
            if (h > _lineHeight)
            {
                _lineHeight = h;
            }
        }
    }
}
