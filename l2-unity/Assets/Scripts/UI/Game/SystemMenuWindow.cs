using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
using VContainer;

public class SystemMenuWindow : L2PopupWindow
{
    [Inject] L2GameUI _gameUi;
    [Inject] AudioManager _audio;

    private float _windowHeight = 0f;
    private static SystemMenuWindow _instance;
    public static SystemMenuWindow Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            WireSystemUIPacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireSystemUIPacketServices();
    }

    private void WireSystemUIPacketServices()
    {
        SystemUIPacketServices.Bind(ui: _gameUi, audio: _audio);
    }

    private void Update()
    {
        if (_isWindowHidden)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!MouseOverThisWindow() && !MenuWindow.Instance.MouseOverThisWindow())
            {
                HideWindow();
            }
        }
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/SystemMenuWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        // root.Add(_windowEle);

        yield return new WaitForEndOfFrame();

        VisualElement exitButton = GetElementByClass("exit-btn");
        exitButton.AddManipulator(new ButtonClickSoundManipulator(exitButton));
        exitButton.RegisterCallback<ClickEvent>((evt) => HandleExitButtonClick());
        VisualElement restartButton = GetElementByClass("restart-btn");
        restartButton.AddManipulator(new ButtonClickSoundManipulator(restartButton));
        restartButton.RegisterCallback<ClickEvent>((evt) => HandleRestartButtonClick());

        _windowHeight = _windowEle.worldBound.height;
        RegisterClickWindowEvent(_windowEle, null);

        HideWindow();
    }

    private void HandleExitButtonClick()
    {
        HideWindow();
        ExitWindow.Instance.OpenWindow(true);
    }

    private void HandleRestartButtonClick()
    {
        HideWindow();
        ExitWindow.Instance.OpenWindow(false);
    }

    public override void ToggleHideWindow()
    {
        Vector2 basePosition = MenuWindow.Instance.GetWindowPosition();
        _windowEle.transform.position = new Vector2(basePosition.x, basePosition.y - _windowHeight);
        base.ToggleHideWindow();
    }

    public override void ShowWindow()
    {
        base.ShowWindow();
        SystemUIPacketServices.Audio.PlayUISound("window_open");
        SystemUIPacketServices.Ui.WindowOpened(this);
    }

    public override void HideWindow()
    {
        base.HideWindow();
        SystemUIPacketServices.Audio.PlayUISound("window_close");
        SystemUIPacketServices.Ui.WindowClosed(this);
    }
}