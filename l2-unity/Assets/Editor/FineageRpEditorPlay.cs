using System;
using System.Collections.Generic;
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
/// Request file <c>/tmp/fineage-dev-rp-play.request</c>:
/// <list type="bullet">
/// <item><c>probe user pass</c> — empty-roster create path</item>
/// <item><c>worldprobe user pass [char]</c> — login → start → assert lobby faces gone</item>
/// </list>
/// Result: <c>/tmp/fineage-rp-probe.result</c>
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
    static bool _startSent;
    static bool _worldMode;
    static bool _finished;
    static double _createDeadline;
    static double _worldAssertNotBefore;
    static bool _sawEmptyRoster;
    static double _loginNotBefore;
    static double _deadline;
    static int _loginAttempts;
    static double _loginRetryAt;
    static string _probeUser;
    static string _probePass;
    static string _probeChar;
    static string _badLog;
    static string _pathLog;
    static string _lobbyCleanLog;

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
            _startSent = false;
            _finished = false;
            _sawEmptyRoster = false;
            _badLog = null;
            _pathLog = null;
            _lobbyCleanLog = null;
            _worldAssertNotBefore = 0;
            _loginAttempts = 0;
            _loginRetryAt = 0;
            _loginNotBefore = EditorApplication.timeSinceStartup + 1.0;
            _deadline = EditorApplication.timeSinceStartup + (_worldMode ? 420.0 : 240.0);
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
        if (condition.IndexOf("[Fineage] login failed:", StringComparison.Ordinal) >= 0)
            _badLog = condition;
        if (condition.IndexOf("[Fineage] roster count=0", StringComparison.Ordinal) >= 0)
            _sawEmptyRoster = true;
        if (condition.IndexOf("[Fineage] lobby login", StringComparison.Ordinal) >= 0
            || condition.IndexOf("[Fineage] empty roster", StringComparison.Ordinal) >= 0
            || condition.IndexOf("[Fineage] lobby ready", StringComparison.Ordinal) >= 0
            || condition.IndexOf("[Fineage] auto-pick", StringComparison.Ordinal) >= 0)
            _pathLog = condition;
        if (condition.IndexOf("LobbyClean PASS", StringComparison.Ordinal) >= 0
            || condition.IndexOf("LobbyClean FAIL", StringComparison.Ordinal) >= 0)
            _lobbyCleanLog = condition;
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
        _worldMode = false;
        _probeChar = null;
        if (parts.Length >= 3 && (parts[0] == "probe" || parts[0] == "worldprobe"))
        {
            _worldMode = parts[0] == "worldprobe";
            _probeUser = parts[1];
            _probePass = parts[2];
            if (parts.Length >= 4)
                _probeChar = parts[3];
            // Persist mode+char so domain reload mid-Play still knows worldprobe.
            File.WriteAllText(
                AutoLoginPath,
                (_worldMode ? "worldprobe" : "probe") + "\n"
                + _probeUser + "\n"
                + _probePass + "\n"
                + (_probeChar ?? "") + "\n");
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

    /// <summary>Unity -executeMethod: world enter + lobby-clean assert (default qqqq/qqqq/lol1).</summary>
    public static void PlayWorldProbe()
    {
        string user = Environment.GetEnvironmentVariable("FINEAGE_RP_USER");
        string pass = Environment.GetEnvironmentVariable("FINEAGE_RP_PASS");
        string character = Environment.GetEnvironmentVariable("FINEAGE_RP_CHAR");
        if (string.IsNullOrEmpty(user))
            user = "qqqq";
        if (string.IsNullOrEmpty(pass))
            pass = "qqqq";
        if (string.IsNullOrEmpty(character))
            character = "lol1";

        string body = "worldprobe " + user + " " + pass + " " + character;
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
        PlayerPrefs.DeleteKey("Fineage.LocalBackend");
        PlayerPrefs.DeleteKey("Fineage.PublicPlaytest");
        PlayerPrefs.DeleteKey(FineageLaunchArgs.CharacterPrefKey);
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
            if (_worldMode && _startSent)
                FinishWorldAssert("timeout after Start");
            else
            {
                // Domain reload may have cleared _worldMode; recover from autologin file.
                string ignoredUser;
                string ignoredPass;
                TryReadAutoLogin(out ignoredUser, out ignoredPass);
                Finish(false, _worldMode ? "timeout before world enter" : "timeout before CharSelect");
            }
            return;
        }

        if (!string.IsNullOrEmpty(_badLog))
        {
            bool credFail = _badLog.IndexOf("invalid_credentials", StringComparison.Ordinal) >= 0;
            if (credFail && _loginAttempts < 5 && !_startSent && !_createSent)
            {
                Debug.LogWarning("[FineageRP] login failed, retry " + _loginAttempts + "/5: " + _badLog);
                _badLog = null;
                _loginSent = false;
                _loginRetryAt = EditorApplication.timeSinceStartup + 2.0;
                return;
            }

            Finish(false, "console: " + _badLog);
            return;
        }

        if (!_loginSent)
        {
            if (EditorApplication.timeSinceStartup < _loginRetryAt)
                return;
            if (!SceneManager.GetSceneByName("l2_lobby").isLoaded)
                return;
            if (LoginWindow.Instance == null || CharSelectWindow.Instance == null)
                return;
            if (!FineageWireConfig.UseLoginHandoff)
                return;

            string user;
            string pass;
            if (!TryReadAutoLogin(out user, out pass))
            {
                Finish(false, "autologin file was not readable");
                return;
            }

            _loginSent = true;
            _loginAttempts++;
            Debug.Log("[FineageRP] probe login account=" + user
                + (_worldMode ? " mode=worldprobe" : "")
                + " attempt=" + _loginAttempts);
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

        if (_worldMode)
        {
            TickWorldProbe(manager);
            return;
        }

        if (!SceneManager.GetSceneByName("l2_lobby").isLoaded)
            return;
        if (LoginWindow.Instance == null || CharSelectWindow.Instance == null)
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

    static void TickWorldProbe(GameManager manager)
    {
        if (!_startSent)
        {
            if (!SceneManager.GetSceneByName("l2_lobby").isLoaded)
                return;
            if (manager.GameState != GameState.CHAR_SELECT)
                return;

            CharSelectWindow select = CharSelectWindow.Instance;
            if (select == null)
                return;

            List<LobbyCharacter> roster = Field<List<LobbyCharacter>>(select, "_lobbyCharacters");
            if (roster == null || roster.Count == 0)
                return;

            int index = 0;
            if (!string.IsNullOrEmpty(_probeChar))
            {
                index = -1;
                for (int i = 0; i < roster.Count; i++)
                {
                    if (roster[i] != null
                        && string.Equals(roster[i].Name, _probeChar, StringComparison.OrdinalIgnoreCase))
                    {
                        index = i;
                        break;
                    }
                }

                if (index < 0)
                {
                    Finish(false, "char '" + _probeChar + "' not in roster count=" + roster.Count);
                    return;
                }
            }

            select.SelectLobbySlot(index);
            CharacterSelector characters = CharacterSelector.Instance;
            if (characters != null)
                characters.SelectLobbyCharacter(index);

            MethodInfo start = typeof(CharSelectWindow).GetMethod(
                "StartGamePressed",
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (start == null)
            {
                Finish(false, "StartGamePressed missing");
                return;
            }

            _startSent = true;
            _worldAssertNotBefore = EditorApplication.timeSinceStartup + 2.0;
            Debug.Log("[FineageRP] worldprobe StartGame index=" + index
                + " name=" + (roster[index] != null ? roster[index].Name : "?")
                + " state=" + manager.GameState);
            try
            {
                start.Invoke(select, null);
            }
            catch (Exception ex)
            {
                Finish(false, "StartGamePressed threw " + ex.GetBaseException().Message);
            }

            return;
        }

        if (!string.IsNullOrEmpty(_lobbyCleanLog))
        {
            bool pass = _lobbyCleanLog.IndexOf("LobbyClean PASS", StringComparison.Ordinal) >= 0;
            Finish(pass, _lobbyCleanLog);
            return;
        }

        if (EditorApplication.timeSinceStartup < _worldAssertNotBefore)
            return;

        // Fallback if SceneLoader diag log was missed: assert once player + Menu gone.
        bool menuGone = !SceneManager.GetSceneByName("Menu").isLoaded;
        bool lobbyGone = !SceneManager.GetSceneByName("l2_lobby").isLoaded;
        if (PlayerEntity.Instance == null || !menuGone || !lobbyGone)
            return;
        // Let WorldEnterDiagDelayed (+1s) emit LobbyClean PASS/FAIL when possible.
        if (EditorApplication.timeSinceStartup < _worldAssertNotBefore + 3.0)
            return;

        FinishWorldAssert("fallback after Menu unload");
    }

    static void FinishWorldAssert(string reason)
    {
        try
        {
            LobbyCleanGate.Report report = LobbyCleanGate.Evaluate();
            Finish(report.Ok, reason + " " + report.Detail
                + (string.IsNullOrEmpty(_lobbyCleanLog) ? "" : " log='" + _lobbyCleanLog + "'"));
        }
        catch (Exception ex)
        {
            Finish(false, reason + " LobbyCleanGate threw " + ex.Message);
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

            int userIdx = 0;
            if (lines[0] == "worldprobe" || lines[0] == "probe")
            {
                _worldMode = lines[0] == "worldprobe";
                userIdx = 1;
                if (lines.Length < 3)
                    return false;
                if (lines.Length >= 4 && !string.IsNullOrEmpty(lines[3]))
                    _probeChar = lines[3].Trim();
            }

            user = lines[userIdx].Trim();
            pass = lines[userIdx + 1].Trim();
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
