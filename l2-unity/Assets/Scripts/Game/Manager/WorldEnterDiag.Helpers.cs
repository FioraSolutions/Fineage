#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public static partial class WorldEnterDiag
{
    static void AppendCamera(StringBuilder sb, Camera cam)
    {
        if (cam == null)
        {
            sb.Append("(cam=null)");
            return;
        }

        sb.Append("(cam=").Append(cam.name)
            .Append(" en=").Append(cam.enabled)
            .Append(" active=").Append(cam.gameObject.activeInHierarchy)
            .Append(" mask=").Append(cam.cullingMask)
            .Append(')');
    }
}
#endif
