using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Editor play against the public Fineage login. Clears a stale character hint so an empty
/// account opens CharSelect instead of the login error overlay.
/// A request file <c>/tmp/fineage-rp-editor-play.request</c> starts the same path.
/// First line <c>probe account password</c> logs in and writes <c>/tmp/fineage-rp-probe.result</c>.
/// </summary>
[InitializeOnLoad]
public static class FineageRpEditorPlay
{
    const string RequestPath = "/tmp/fineage-dev-rp-play.request";
    const string AutoLoginPath = "/tmp/fineage-rp-autologin.txt";
    const string ResultPath = "/tmp/fineage-rp-probe.result";
    const string MenuScenePath = "Assets/Resources/Scenes/Menu.unity";

    static bool _hooked;
    const string ExitPath = "/tmp/fineage-rp-probe.exit";

    static bool _loginSent;
    static bool _createSent;
    static bool _finished;
    static double _createDeadline;
    static bool _sawEmptyRoster;
    static double _loginNotBefore;
    static double _deadline;
    static string _probeUser;
    static string _probePass;
    static string _badLog;
    static string _pathLog;

    static FineageRpEditorPlay()
    {
        Hook();
    }

    static void Hook()
    {
        if (_hooked)
            return;
        _hooked = true;
        EditorApplication.delayCall += TryConsume;
        EditorApplication.update += Poll;
        EditorApplication.playModeStateChanged += OnPlayMode;
    }

    static double _next;

    static void Poll()
    {
        if (EditorApplication.timeSinceStartup < _next)
            return;
        _next = EditorApplication.timeSinceStartup + 0.5;
        TryConsume();
        TickProbe();
    }

    static void OnPlayMode(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            _loginSent = false;
            _createSent = false;
            _finished = false;
            _sawEmptyRoster = false;
            _badLog = null;
            _pathLog = null;
            _loginNotBefore = EditorApplication.timeSinceStartup + 1.0;
            _deadline = EditorApplication.timeSinceStartup + 240.0;
            Application.logMessageReceived += OnLog;
        }
        else if (state == PlayModeStateChange.ExitingPlayMode)
        {
            Application.logMessageReceived -= OnLog;
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            TryExitAfterProbe();
        }
    }

    static void TryExitAfterProbe()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || !File.Exists(ExitPath))
            return;
        int code = 2;
        try
        {
            int parsed;
            if (int.TryParse(File.ReadAllText(ExitPath).Trim(), out parsed))
                code = parsed;
            File.Delete(ExitPath);
        }
        catch
        {
            /* still exit */
        }

        EditorApplication.delayCall += () => EditorApplication.Exit(code);
    }

    static void OnLog(string condition, string stackTrace, LogType type)
    {
        if (string.IsNullOrEmpty(condition))
            return;
        if (condition.IndexOf("[FineageLocal] login failed:", StringComparison.Ordinal) >= 0)
            _badLog = condition;
        if (condition.IndexOf("[FineageLocal] roster count=0", StringComparison.Ordinal) >= 0)
            _sawEmptyRoster = true;
        if (condition.IndexOf("[FineageLocal] lobby login", StringComparison.Ordinal) >= 0
            || condition.IndexOf("[FineageLocal] empty roster", StringComparison.Ordinal) >= 0
            || condition.IndexOf("[FineageLocal] lobby ready", StringComparison.Ordinal) >= 0
            || condition.IndexOf("[FineageLocal] auto-pick", StringComparison.Ordinal) >= 0)
            _pathLog = condition;
    }

    static void TryConsume()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.isPlayingOrWillChangePlaymode)
            return;
        if (!File.Exists(RequestPath))
            return;

        string body;
        try
        {
            body = File.ReadAllText(RequestPath);
            File.Delete(RequestPath);
        }
        catch (Exception ex)
        {
            Debug.LogError("[FineageRP] could not read play request: " + ex.Message);
            return;
        }

        ApplyRequest(body);
        PlayRp();
    }

    static void ApplyRequest(string body)
    {
        _probeUser = null;
        _probePass = null;
        try
        {
            if (File.Exists(AutoLoginPath))
                File.Delete(AutoLoginPath);
        }
        catch
        {
            /* ignore */
        }

        if (string.IsNullOrEmpty(body))
            return;
        string[] lines = body.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length == 0)
            return;
        string[] parts = lines[0].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 3 && parts[0] == "probe")
        {
            _probeUser = parts[1];
            _probePass = parts[2];
            File.WriteAllText(AutoLoginPath, _probeUser + "\n" + _probePass + "\n");
        }
    }

    /// <summary>Unity -executeMethod entry. Reads the dev request file, or probes qqqq/qqqq.</summary>
    public static void PlayProbe()
    {
        string body = "probe qqqq qqqq";
        if (File.Exists(RequestPath))
        {
            try
            {
                body = File.ReadAllText(RequestPath);
                File.Delete(RequestPath);
            }
            catch (Exception ex)
            {
                Debug.LogError("[FineageRP] could not read play request: " + ex.Message);
            }
        }

        ApplyRequest(body);
        PlayRp();
    }

    [MenuItem("Fineage/Play RP")]
    public static void PlayRp()
    {
        Environment.SetEnvironmentVariable("FINEAGE_LOCAL_BACKEND", "0");
        Environment.SetEnvironmentVariable("FINEAGE_LOCAL_CHAR", null);
        PlayerPrefs.SetInt(FineageLocalBackend.PrefKey, 0);
        PlayerPrefs.DeleteKey(FineageLocalBackend.CharacterPrefKey);
        PlayerPrefs.DeleteKey("Fineage.PublicPlaytest"); // retired flag; public RP is the default now
        PlayerPrefs.Save();

        Debug.Log("[FineageRP] Editor Play → public RP ("
            + PlaytestEndpoint.DefaultLoginHost + ":" + PlaytestEndpoint.DefaultLoginPort
            + "). Character hint cleared.");

        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
            EditorApplication.delayCall += OpenMenuThenPlay;
            return;
        }

        OpenMenuThenPlay();
    }

    static void OpenMenuThenPlay()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            EditorApplication.delayCall += OpenMenuThenPlay;
            return;
        }

        if (EditorApplication.isPlaying)
        {
            EditorApplication.delayCall += OpenMenuThenPlay;
            return;
        }

        var scene = EditorSceneManager.GetActiveScene();
        if (scene.path != MenuScenePath)
        {
            if (scene.isDirty)
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(MenuScenePath, OpenSceneMode.Single);
            Debug.Log("[FineageRP] Opened " + MenuScenePath);
        }

        EditorApplication.isPlaying = true;
    }

    static void TickProbe()
    {
        if (!EditorApplication.isPlaying || _finished)
            return;
        if (!File.Exists(AutoLoginPath))
            return;
        if (EditorApplication.timeSinceStartup < _loginNotBefore)
            return;

        if (EditorApplication.timeSinceStartup > _deadline)
        {
            Finish(false, "timeout before CharSelect");
            return;
        }

        if (!string.IsNullOrEmpty(_badLog))
        {
            Finish(false, "console: " + _badLog);
            return;
        }

        if (!SceneManager.GetSceneByName("l2_lobby").isLoaded)
            return;
        if (LoginWindow.Instance == null || CharSelectWindow.Instance == null)
            return;
        if (!FineageWireConfig.IsZoneV2)
            return;

        if (!_loginSent)
        {
            string user;
            string pass;
            if (!TryReadAutoLogin(out user, out pass))
            {
                Finish(false, "autologin file was not readable");
                return;
            }

            _loginSent = true;
            Debug.Log("[FineageRP] probe login account=" + user);
            try
            {
                IZoneTransport transport = null;
                if (App.HasContainer)
                    transport = App.Resolve<IZoneTransport>();
                if (transport != null)
                    transport.BeginSession(user, pass);
                else
                    ZoneV2Client.Ensure().BeginSession(user, pass);
            }
            catch (Exception ex)
            {
                Finish(false, "BeginSession: " + ex.Message);
            }

            return;
        }

        GameManager manager = UnityEngine.Object.FindFirstObjectByType<GameManager>();
        if (manager == null)
            return;

        bool loginHidden = WindowHidden(LoginWindow.Instance);
        string loginError = LabelText(LoginWindow.Instance, "_labelError");
        if (_createSent)
        {
            CharCreationWindow creation = CharCreationWindow.Instance;
            bool creationOpen = creation != null && creation.IsOpen;
            if (manager.GameState == GameState.CHAR_CREATION && creationOpen)
            {
                bool errorOverlay = loginError != null
                    && loginError.IndexOf("No characters on this account.", StringComparison.Ordinal) >= 0;
                bool ok = loginHidden && !errorOverlay && string.IsNullOrEmpty(_badLog) && _sawEmptyRoster;
                Finish(ok, "state=" + manager.GameState
                    + " loginHidden=" + loginHidden
                    + " lobbyStatus='No characters yet.'"
                    + " loginError='" + loginError + "'"
                    + " creationOpen=" + creationOpen
                    + " path='" + (_pathLog ?? "") + "'"
                    + " badLog='" + (_badLog ?? "") + "'");
                return;
            }

            if (EditorApplication.timeSinceStartup > _createDeadline)
            {
                Finish(false, "CreatePressed left state=" + manager.GameState
                    + " creationOpen=" + creationOpen
                    + " loginHidden=" + loginHidden
                    + " loginError='" + loginError + "'");
            }

            return;
        }

        if (manager.GameState != GameState.CHAR_SELECT)
            return;

        CharSelectWindow select = CharSelectWindow.Instance;
        Button create = Field<Button>(select, "_createButton");
        bool createEnabled = create != null && create.enabledSelf;
        string status = LabelText(select, "_lobbyStatus");
        if (!_sawEmptyRoster || !createEnabled || status != "No characters yet.")
            return;

        if (!_createSent)
        {
            _createSent = true;
            MethodInfo method = typeof(CharSelectWindow).GetMethod(
                "CreatePressed",
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (method == null)
            {
                Finish(false, "CreatePressed missing");
                return;
            }

            try
            {
                method.Invoke(select, null);
            }
            catch (Exception ex)
            {
                Finish(false, "CreatePressed threw " + ex.GetBaseException().Message);
                return;
            }

            _createDeadline = EditorApplication.timeSinceStartup + 8.0;
            Debug.Log("[FineageRP] probe CreatePressed status='" + status + "' state=" + manager.GameState);
            return;
        }
    }

    static bool TryReadAutoLogin(out string user, out string pass)
    {
        user = null;
        pass = null;
        try
        {
            string[] lines = File.ReadAllText(AutoLoginPath)
                .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2)
                return false;
            user = lines[0].Trim();
            pass = lines[1].Trim();
            return user.Length > 0 && pass.Length > 0;
        }
        catch
        {
            return false;
        }
    }

    static void Finish(bool ok, string detail)
    {
        if (_finished)
            return;
        _finished = true;
        string text = (ok ? "ok" : "fail") + " " + detail + "\n";
        try
        {
            File.WriteAllText(ResultPath, text);
        }
        catch (Exception ex)
        {
            Debug.LogError("[FineageRP] could not write probe result: " + ex.Message);
        }

        try
        {
            if (File.Exists(AutoLoginPath))
                File.Delete(AutoLoginPath);
        }
        catch
        {
            /* ignore */
        }

        try
        {
            File.WriteAllText(ExitPath, ok ? "0" : "2");
        }
        catch
        {
            /* EnteredEditMode still tries to exit */
        }

        Debug.Log("[FineageRP] probe " + text.Trim());
        EditorApplication.isPlaying = false;
    }

    static T Field<T>(object instance, string name) where T : class
    {
        if (instance == null)
            return null;
        FieldInfo field = instance.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == null)
            return null;
        return field.GetValue(instance) as T;
    }

    static string LabelText(object instance, string fieldName)
    {
        Label label = Field<Label>(instance, fieldName);
        return label != null ? label.text : "";
    }

    static bool WindowHidden(object instance)
    {
        if (instance == null)
            return false;
        FieldInfo field = instance.GetType().GetField("_isWindowHidden", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == null)
            field = typeof(L2Window).GetField("_isWindowHidden", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == null)
            return false;
        object value = field.GetValue(instance);
        return value is bool && (bool)value;
    }
}
