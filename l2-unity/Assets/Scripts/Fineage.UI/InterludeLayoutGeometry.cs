using System;
using System.Collections.Generic;

/// <summary>
/// Places one extracted window. Width or height &lt;= 0 means parentSize + value.
/// -9999 means the extract has no size on that axis.
/// </summary>
public static class InterludeLayoutGeometry
{
    public const float Unspecified = -9999f;

    public struct Size
    {
        public float Left;
        public float Top;
        public bool HasWidth;
        public bool HasHeight;
        public float Width;
        public float Height;
    }

    public static bool TryResolve(
        InterludeLayoutWindow window,
        float rootWidth,
        float rootHeight,
        out Dictionary<string, Size> rects,
        out string error)
    {
        rects = new Dictionary<string, Size>();
        error = null;
        if (window == null || window.controls == null || window.controls.Length == 0)
        {
            error = "window has no controls";
            return false;
        }

        InterludeLayoutControl root = window.controls[0];
        Size rootSize = new Size();
        rootSize.Left = 0f;
        rootSize.Top = 0f;
        rootSize.HasWidth = true;
        rootSize.HasHeight = true;
        rootSize.Width = rootWidth;
        rootSize.Height = rootHeight;
        rects[root.name] = rootSize;

        var parentOf = new Dictionary<string, string>();
        parentOf[root.name] = root.name;

        for (int i = 1; i < window.controls.Length; i++)
        {
            InterludeLayoutControl control = window.controls[i];
            Size parentRect;
            if (!rects.TryGetValue(control.parent, out parentRect))
            {
                error = "control '" + control.name + "' parent '" + control.parent + "' is not an earlier control";
                return false;
            }

            if (!parentRect.HasWidth || !parentRect.HasHeight || parentRect.Width <= 0f || parentRect.Height <= 0f)
            {
                error = "control '" + control.name + "' parent '" + control.parent + "' has no positive size";
                return false;
            }

            bool hasWidth;
            bool hasHeight;
            float width = Axis(control.width, parentRect.Width, out hasWidth);
            float height = Axis(control.height, parentRect.Height, out hasHeight);

            float refLeft = 0f;
            float refTop = 0f;
            float refWidth = parentRect.Width;
            float refHeight = parentRect.Height;
            if (!string.IsNullOrEmpty(control.relativeTo))
            {
                string refName = control.relativeTo;
                Size refRect;
                if (!rects.TryGetValue(refName, out refRect) && refName.IndexOf('.') >= 0)
                {
                    refName = refName.Substring(refName.LastIndexOf('.') + 1);
                    rects.TryGetValue(refName, out refRect);
                }

                if (!rects.ContainsKey(refName))
                {
                    error = "control '" + control.name + "' relativeTo '" + control.relativeTo + "' was not resolved";
                    return false;
                }

                string refParent;
                parentOf.TryGetValue(refName, out refParent);
                if (refParent != control.parent && refName != control.parent)
                {
                    error = "control '" + control.name + "' relativeTo '" + control.relativeTo + "' is not in the same parent";
                    return false;
                }

                if (refName != control.parent)
                {
                    refLeft = refRect.Left;
                    refTop = refRect.Top;
                    refWidth = refRect.Width;
                    refHeight = refRect.Height;
                }
            }

            float thisX;
            float thisY;
            float parentX;
            float parentY;
            if (!TryAnchor(control.anchorThis, out thisX, out thisY))
            {
                error = "control '" + control.name + "' anchor '" + control.anchorThis + "' is not an extracted alignment";
                return false;
            }

            if (!TryAnchor(control.anchorParent, out parentX, out parentY))
            {
                error = "control '" + control.name + "' anchor '" + control.anchorParent + "' is not an extracted alignment";
                return false;
            }

            float anchorX = refLeft + parentX * refWidth + control.x;
            float anchorY = refTop + parentY * refHeight + control.y;
            float usedW = hasWidth ? width : 0f;
            float usedH = hasHeight ? height : 0f;
            Size size = new Size();
            size.Left = anchorX - thisX * usedW;
            size.Top = anchorY - thisY * usedH;
            size.HasWidth = hasWidth;
            size.HasHeight = hasHeight;
            size.Width = width;
            size.Height = height;
            rects[control.name] = size;
            parentOf[control.name] = control.parent;
        }

        return true;
    }

    public static float Axis(float value, float parent, out bool hasSize)
    {
        if (value <= -9000f)
        {
            hasSize = false;
            return 0f;
        }

        hasSize = true;
        if (value <= 0f)
            return parent + value;
        return value;
    }

    public static bool TryAnchor(string name, out float x, out float y)
    {
        x = 0f;
        y = 0f;
        if (string.IsNullOrEmpty(name) || name == "TOP_LEFT" || name == "NONE")
            return true;
        if (name == "TOP_CENTER")
        {
            x = 0.5f;
            return true;
        }

        if (name == "TOP_RIGHT")
        {
            x = 1f;
            return true;
        }

        if (name == "CENTER_LEFT")
        {
            y = 0.5f;
            return true;
        }

        if (name == "CENTER")
        {
            x = 0.5f;
            y = 0.5f;
            return true;
        }

        if (name == "CENTER_RIGHT")
        {
            x = 1f;
            y = 0.5f;
            return true;
        }

        if (name == "BOTTOM_LEFT")
        {
            y = 1f;
            return true;
        }

        if (name == "BOTTOM_CENTER")
        {
            x = 0.5f;
            y = 1f;
            return true;
        }

        if (name == "BOTTOM_RIGHT")
        {
            x = 1f;
            y = 1f;
            return true;
        }

        return false;
    }
}

[Serializable]
public class InterludeLayoutFile
{
    public InterludeLayoutWindow[] windows;
}

[Serializable]
public class InterludeLayoutWindow
{
    public string name;
    public string alignment;
    public float placeX;
    public float placeY;
    public int placementKnown;
    public int stretchToParent;
    public InterludeLayoutControl[] controls;
}

[Serializable]
public class InterludeLayoutControl
{
    public string name;
    public string type;
    public string parent;
    public float x;
    public float y;
    public float width;
    public float height;
    public string anchorParent;
    public string anchorThis;
    public string relativeTo;
    public string texture;
    public string textureBack;
    public string texturePushed;
    public string textureWarn;
    public string textureRegen;
    public string text;
    public string textAlign;
    public string fontColor;
    public int fontType;
    public int maxLength;
}
