#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// One-shot snapshots for world-enter regressions (scenes, cameras, nameplates).
/// Face leftovers are asserted by <see cref="LobbyCleanGate"/> only.
/// Writes via <see cref="FineageLog"/> → Console + <c>logs/diag/world.txt</c>.
/// </summary>
public static class WorldEnterDiag
{
    public static void Snapshot(string phase)
    {
        StringBuilder sb = new StringBuilder(512);
        sb.Append(phase);

        sb.Append(" scenes=");
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);
            if (i > 0)
                sb.Append(',');
            sb.Append(s.name).Append(s.isLoaded ? ":L" : ":U");
            if (s == SceneManager.GetActiveScene())
                sb.Append('*');
        }

        Camera main = Camera.main;
        sb.Append(" Camera.main=");
        AppendCamera(sb, main);

        CameraController cc = CameraController.Instance;
        sb.Append(" CameraController=");
        if (cc == null)
            sb.Append("null");
        else
        {
            sb.Append("ok behaviour=").Append(cc.enabled)
                .Append(" goActive=").Append(cc.gameObject.activeInHierarchy)
                .Append(" target=").Append(cc.Target != null ? cc.Target.name : "null");
            AppendCamera(sb, cc.GetComponent<Camera>());
        }

        InputManager input = InputManager.Instance;
        sb.Append(" Input=");
        if (input == null)
            sb.Append("null");
        else
            sb.Append("ok active=").Append(input.gameObject.activeInHierarchy)
                .Append(" move=").Append(input.Move)
                .Append(" turnCam=").Append(input.TurnCamera);

        ClickManager clicks = ClickManager.Instance;
        sb.Append(" ClickManager=").Append(clicks != null
            ? (clicks.isActiveAndEnabled ? "active" : "disabled")
            : "null");

        NameplatesManager plates = NameplatesManager.Instance;
        sb.Append(" Nameplates=");
        if (plates == null)
            sb.Append("null");
        else
        {
            sb.Append(plates.isActiveAndEnabled ? "active" : "disabled");
            AppendCamera(sb, plates.Camera != null ? plates.Camera : Camera.main);
        }

        PlayerEntity player = PlayerEntity.Instance;
        sb.Append(" PlayerEntity=").Append(player != null ? player.name + "@" + player.transform.position : "null");

        PlayerController pc = PlayerController.Instance;
        sb.Append(" PlayerController=").Append(pc != null
            ? (pc.isActiveAndEnabled ? "active" : "disabled")
            : "null");

        L2GameUI ui = L2GameUI.Instance;
        if (ui != null)
            sb.Append(" MouseOverUI=").Append(ui.MouseOverUI)
                .Append(" OverPanel=").Append(ui.IsPointerOverPanel());
        else
            sb.Append(" L2GameUI=null");

        int charSelectLayer = LobbyPresentation.Layer;
        sb.Append(" CharSelectLayer=").Append(charSelectLayer);

        int namedCharacters = 0;
        int namedLobbyRoot = 0;
        int onCharSelectLayer = 0;
        int enabledRenderersOnCharSelect = 0;
        Transform[] transforms = Object.FindObjectsByType<Transform>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);
        for (int i = 0; i < transforms.Length; i++)
        {
            Transform t = transforms[i];
            if (t == null)
                continue;
            string n = t.name;
            if (n == "Characters" || n.StartsWith("Characters"))
                namedCharacters++;
            if (n == LobbyPresentation.RootName)
                namedLobbyRoot++;
            if (charSelectLayer >= 0 && t.gameObject.layer == charSelectLayer)
            {
                onCharSelectLayer++;
                Renderer r = t.GetComponent<Renderer>();
                if (r != null && r.enabled && t.gameObject.activeInHierarchy)
                    enabledRenderersOnCharSelect++;
            }
        }

        sb.Append(" leftovers CharactersGO=").Append(namedCharacters)
            .Append(" LobbyRoot=").Append(namedLobbyRoot)
            .Append(" layerObjs=").Append(onCharSelectLayer)
            .Append(" visibleCharSelectRenderers=").Append(enabledRenderersOnCharSelect);

        sb.Append(" cams=[");
        Camera[] cams = Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < cams.Length; i++)
        {
            Camera c = cams[i];
            if (c == null)
                continue;
            if (i > 0)
                sb.Append("; ");
            sb.Append(c.name)
                .Append(" tag=").Append(c.tag)
                .Append(" en=").Append(c.enabled)
                .Append(" active=").Append(c.gameObject.activeInHierarchy)
                .Append(" scene=").Append(c.gameObject.scene.name);
        }
        sb.Append(']');

        // Face leftovers asserted once by LobbyCleanGate after +1s — avoid a second full renderer scan here.
        FineageLog.Info(FineageLog.Category.World, sb.ToString());
    }

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
