using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Writes a proof PNG of the credentials window when
/// <c>/tmp/fineage-interlude-login.outpath</c> names a file.
/// Does nothing in normal play.
/// </summary>
public sealed class InterludeLoginProof : MonoBehaviour
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

    public static void Arm(VisualElement documentRoot)
    {
        if (!IsArmed || documentRoot == null)
            return;

        var host = new GameObject("InterludeLoginProof");
        var proof = host.AddComponent<InterludeLoginProof>();
        proof._root = documentRoot;
        proof.StartCoroutine(proof.Capture());
    }

    IEnumerator Capture()
    {
        string path;
        try
        {
            path = File.ReadAllText(ShotPathFile).Trim();
        }
        catch (IOException ex)
        {
            Debug.LogWarning("[InterludeLogin] shot path unreadable: " + ex.Message);
            yield break;
        }

        if (string.IsNullOrEmpty(path))
            yield break;

        bool ready = false;
        for (int i = 0; i < 900; i++)
        {
            VisualElement splash = _root.Q("InterludeSplash");
            VisualElement login = _root.Q("LoginWindow");
            bool splashGone = splash == null || splash.parent == null;
            float width = login != null ? login.resolvedStyle.width : 0f;
            bool loginShown = login != null
                && login.resolvedStyle.display != DisplayStyle.None
                && width > 200f;
            if (splashGone && loginShown)
            {
                ready = true;
                break;
            }

            yield return null;
        }

        if (!ready)
            Debug.LogWarning("[InterludeLogin] credentials window was not ready for the proof shot");

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();
        if (tex == null)
        {
            Debug.LogWarning("[InterludeLogin] screenshot capture returned null");
            yield break;
        }

        byte[] png = tex.EncodeToPNG();
        Destroy(tex);
        string dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllBytes(path, png);
        Debug.Log("[InterludeLogin] wrote " + path + " bytes=" + png.Length);
        Destroy(gameObject);
    }
}
