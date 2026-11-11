using System;

public partial class DragIcon : L2PopupWindow
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DragIconPacketServices.Bind(icon: this);
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/DragAndDrop/DragIcon");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);


        yield return new WaitForEndOfFrame();

        icon = GetElementByClass("Icon");
        var content = GetElementByClass("drag_content");
        L2SlotIconView.SetNamedIcon(icon, "noimage");
        RegisterClickWindow(content);
        HideWindow();
    }
}
