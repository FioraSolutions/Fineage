#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public partial class L2InterludeTileBatchWindow : EditorWindow
{
    string _allowlist = "16_24,16_25,17_24,17_25";
    string _denylist = string.Empty;
    string _clientRoot = string.Empty;
    string _umodelExport = string.Empty;
    string _umodelSoundExport = string.Empty;
    string _umodelSoundUnityExport = string.Empty;
    bool _includeGlobalMaterials = true;
    bool _refreshManifestAfterRun = true;
    L2InterludeBakeStep _steps = L2InterludeTileBakePipeline.DefaultSteps | L2InterludeBakeStep.GenerateMaterials;
    Vector2 _scroll;
    List<L2InterludeTileBakeResult> _lastResults = new List<L2InterludeTileBakeResult>();








}
#endif
