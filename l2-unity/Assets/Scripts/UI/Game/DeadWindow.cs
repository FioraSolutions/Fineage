using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public class DeadWindow : L2Window
{

    private static DeadWindow _instance;
    public static DeadWindow Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            WireGameWindowPacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireGameWindowPacketServices();
    }

    private void WireGameWindowPacketServices()
    {
        GameWindowPacketServices.Bind(dead: this);
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }


    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/DeadWindow");
    }


    protected override IEnumerator BuildWindow(VisualElement root)
    {

        InitWindow(root);

        yield return new WaitForEndOfFrame();

        Button toVillage = _windowEle.Q<Button>("VillageButton");
        
        //default window dead - 152/203
        //_windowEle.style.left = (root.resolvedStyle.width - 152) / 2;
        //float top = (root.resolvedStyle.height - 130) / 2;
        //_windowEle.style.top = top;
        OnCenterScreen(root);
        RegisterClick(toVillage);

    }

    private void RegisterClick(Button toVillage)
    {
        toVillage.RegisterCallback<MouseUpEvent>(evt => {

            ZoneCommands.Send(new RequestRestartPointCommand());

            HideWindow();
        }, TrickleDown.TrickleDown);
    }


}
