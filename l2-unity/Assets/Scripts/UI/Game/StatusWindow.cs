using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public class StatusWindow : L2Window
{

    [Inject] GameClient _gameClient;

    private GameClient Game => GameWindowPacketServices.Game;
    private Label _nameLabel;
    private Label _levelLabel;
    private Label _HPTextLabel;
    private Label _MPTextLabel;
    private Label _CPTextLabel;
    private VisualElement _CPBar;
    private VisualElement _CPBarBG;
    private VisualElement _HPBar;
    private VisualElement _HPBarBG;
    private VisualElement _MPBar;
    private VisualElement _MPBarBG;

    [SerializeField] private float _statusWindowMinWidth = 175.0f;
    [SerializeField] private float _statusWindowMaxWidth = 400.0f;

    private static StatusWindow _instance;
    public static StatusWindow Instance { get { return _instance; } }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
            WireGameWindowPacketServices();
        } else {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireGameWindowPacketServices();
    }

    private void WireGameWindowPacketServices()
    {
        GameWindowPacketServices.Bind(game: _gameClient);
    }

    private void OnDestroy() {
        _instance = null;
    }

    protected override void LoadAssets() {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/StatusWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root) {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        var statusWindowDragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(statusWindowDragArea, _windowEle);
        statusWindowDragArea.AddManipulator(drag);

        var horizontalResizeHandle = GetElementByClass("hor-resize-handle");
        HorizontalResizeManipulator horizontalResize = new HorizontalResizeManipulator(
            horizontalResizeHandle, _windowEle, _statusWindowMinWidth, _statusWindowMaxWidth);
        horizontalResizeHandle.AddManipulator(horizontalResize);

        _windowEle.RegisterCallback<ClickEvent>(OnStatusWindowClicked);

        _nameLabel = (Label)GetElementById("PlayerNameText");
        if(_nameLabel == null) {
            // Debug.LogError("Status window PlayerNameText is null.");
        }

        _levelLabel = (Label)GetElementById("LevelText");
        if(_levelLabel == null) {
            // Debug.LogError("Status window LevelText is null.");
        }

        _CPTextLabel = (Label)GetElementById("CPText");
        if(_CPTextLabel == null) {
            // Debug.LogError("Status window CPText is null.");
        }

        _HPTextLabel = (Label)GetElementById("HPText");
        if(_HPTextLabel == null) {
            // Debug.LogError("Status window Hp text is null.");
        }

        _MPTextLabel = (Label)GetElementById("MPText");
        if(_MPTextLabel == null) {
            // Debug.LogError("Status window MPText is null.");
        }

        _CPBarBG = GetElementById("CPBarBG");
        if(_CPBarBG == null) {
            // Debug.LogError("Status window CPBarBG is null");
        }

        _CPBar = GetElementById("CPBar");
        if(_CPBar == null) {
            // Debug.LogError("Status window CPBar is null");
        }

        _HPBar = GetElementById("HPBar");
        if(_HPBar == null) {
            // Debug.LogError("Status window HPBar is null");
        }

        _HPBarBG = GetElementById("HPBarBG");
        if(_HPBarBG == null) {
            // Debug.LogError("Status window HPBarBG is null");
        }

        _MPBarBG = GetElementById("MPBarBG");
        if(_MPBarBG == null) {
            // Debug.LogError("Status window MPBarBG is null");
        }

        _MPBar = GetElementById("MPBar");
        if(_MPBar == null) {
            // Debug.LogError("Status windowar MPBar is null");
        }
    }

    private void OnStatusWindowClicked(ClickEvent evt)
    {
        if (evt.button != 0)
        {
            return;
        }

        SelectPlayerAsTarget();
        evt.StopPropagation();
    }

    private void SelectPlayerAsTarget()
    {
        PlayerEntity player = PlayerEntity.Instance;
        if (player == null || TargetManager.Instance == null)
        {
            return;
        }

        //TargetManager.Instance.SetTarget(new ObjectData(player.gameObject), "#ffffff");
        SendSelfTargetAction(player);
    }

    private void SendSelfTargetAction(PlayerEntity player)
    {
        if (player.Identity == null || Game == null)
        {
            return;
        }

        var position = player.transform.position;
        var l2jpos = VectorUtils.ConvertPosUnityToL2j(position);

        Game.Send(new ClickActionCommand(
            player.Identity.Id,
            (int)l2jpos.x,
            (int)l2jpos.y,
            (int)l2jpos.z,
            0));
    }

    void FixedUpdate()
    {
        if(PlayerEntity.Instance == null) { 
            return; 
        }

        if(!(PlayerEntity.Instance.Status is PlayerStatus)) {
            // Debug.LogWarning("Player status is not of type playerstatus");
            return;
        }

        PlayerStatus status = (PlayerStatus)PlayerEntity.Instance.Status;
        PlayerStats stats = (PlayerStats)PlayerEntity.Instance.Stats;

        if (_levelLabel != null) {
            _levelLabel.text = stats.Level.ToString();
        }

        if(_nameLabel != null) {
            _nameLabel.text = PlayerEntity.Instance.Identity.Name;
        }

        if(_CPTextLabel != null) {
            int cpMax = PlayerResourceCaps.EffectiveMax(stats.MaxCp, status.Cp);
            int cpCur = PlayerResourceCaps.DisplayCurrent(status.Cp, cpMax);
            _CPTextLabel.text = cpCur + "/" + cpMax;
        }

        if(_HPTextLabel != null) {
            int hpMax = PlayerResourceCaps.EffectiveMax(stats.MaxHp, status.GetHp());
            int hpCur = PlayerResourceCaps.DisplayCurrent(status.GetHp(), hpMax);
            _HPTextLabel.text = hpCur + "/" + hpMax;
        }

        if(_MPTextLabel != null) {
            int mpMax = PlayerResourceCaps.EffectiveMax(stats.MaxMp, status.GetMp());
            int mpCur = PlayerResourceCaps.DisplayCurrent(status.GetMp(), mpMax);
            _MPTextLabel.text = mpCur + "/" + mpMax;
        }

        if(_CPBarBG != null && _CPBar != null) {
            float cMaxCP = (float)PlayerResourceCaps.EffectiveMax(stats.MaxCp, status.Cp);
            if (cMaxCP > 0f) {
                float cpRatio = (float)status.Cp / cMaxCP;
                cpRatio = UnityEngine.Mathf.Min(1f, cpRatio);
                float bgWidth = _CPBarBG.resolvedStyle.width;
                if (bgWidth > 1f && float.IsFinite(bgWidth)) {
                    _CPBar.style.width = bgWidth * cpRatio;
                }
            }
        }

        if(_HPBarBG != null && _HPBar != null) {
            float hpMax = (float)PlayerResourceCaps.EffectiveMax(stats.MaxHp, status.GetHp());
            if (hpMax > 0f) {
                float hpRatio = (float)status.GetHp() / hpMax;
                hpRatio = UnityEngine.Mathf.Min(1f, hpRatio);
                float bgWidth = _HPBarBG.resolvedStyle.width;
                if (bgWidth > 1f && float.IsFinite(bgWidth)) {
                    _HPBar.style.width = bgWidth * hpRatio;
                }
            }
        }

        if(_MPBarBG != null && _MPBar != null) {
            float mpMax = (float)PlayerResourceCaps.EffectiveMax(stats.MaxMp, status.GetMp());
            if (mpMax > 0f) {
                float mpRatio = (float)status.GetMp() / mpMax;
                mpRatio = UnityEngine.Mathf.Min(1f, mpRatio);
                float bgWidth = _MPBarBG.resolvedStyle.width;
                if (bgWidth > 1f && float.IsFinite(bgWidth)) {
                    _MPBar.style.width = bgWidth * mpRatio;
                }
            }
        }
    }
}
