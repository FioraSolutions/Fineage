#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public static partial class WorldEnterDiag
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

        CameraController cc = Scene<CameraController>();
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

        InputManager input = Scene<InputManager>();
        sb.Append(" Input=");
        if (input == null)
            sb.Append("null");
        else
            sb.Append("ok active=").Append(input.gameObject.activeInHierarchy)
                .Append(" move=").Append(input.Move)
                .Append(" turnCam=").Append(input.TurnCamera);

        ClickManager clicks = Scene<ClickManager>();
        sb.Append(" ClickManager=").Append(clicks != null
            ? (clicks.isActiveAndEnabled ? "active" : "disabled")
            : "null");

        NameplatesManager plates = Scene<NameplatesManager>();
        sb.Append(" Nameplates=");
        if (plates == null)
            sb.Append("null");
        else
        {
            sb.Append(plates.isActiveAndEnabled ? "active" : "disabled");
            AppendCamera(sb, plates.Camera != null ? plates.Camera : Camera.main);
        }

        sb.Append(" PlayerEntity=").Append(LocalPlayerLabel());

        PlayerController pc = Scene<PlayerController>();
        sb.Append(" PlayerController=").Append(pc != null
            ? (pc.isActiveAndEnabled ? "active" : "disabled")
            : "null");

        L2GameUI ui = Scene<L2GameUI>();
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
        Transform[] transforms = UnityEngine.Object.FindObjectsByType<Transform>(
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
        Camera[] cams = UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
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
        FineageLog.Info(FineageLogCategory.World, sb.ToString());
    }

    static T Scene<T>() where T : UnityEngine.Object
    {
        return UnityEngine.Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
    }

    static string LocalPlayerLabel()
    {
        if (App.GameContainer == null)
            return "null";

        ILocalPlayerSlot slot;
        try
        {
            slot = App.GameContainer.Resolve<ILocalPlayerSlot>();
        }
        catch (VContainerException)
        {
            return "null";
        }

        if (slot == null || !slot.TryGetName(out string name) || !slot.TryGetPosition(out Vector3 position))
            return "null";

        return name + "@" + position;
    }
}
#endif
