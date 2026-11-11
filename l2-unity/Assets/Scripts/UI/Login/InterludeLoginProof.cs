using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class InterludeLoginProof : MonoBehaviour
{
    public const string ShotPathFile = "/tmp/fineage-interlude-login.outpath";

    VisualElement _root;

    public static bool IsArmed
    {
        get
        {
            try
            {
                return File.Exists(ShotPathFile);
            }
            catch (IOException)
            {
                return false;
            }
        }
    }


}
