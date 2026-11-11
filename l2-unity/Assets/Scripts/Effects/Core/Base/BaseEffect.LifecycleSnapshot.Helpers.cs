using UnityEngine;
using System;
using System.Text;

public abstract partial class BaseEffect
{
    private string TryReadFloat(Material mat, string property, string label)
    {
        if (mat.HasProperty(property))
        {
            return $"{label}={mat.GetFloat(property):F3}";
        }

        return "n/a";
    }

    private string TryReadVector(Material mat, string property, string label)
    {
        if (mat.HasProperty(property))
        {
            Vector4 v = mat.GetVector(property);
            return $"{label}=({v.x:F3},{v.y:F3},{v.z:F3},{v.w:F3})";
        }

        return "n/a";
    }
}
