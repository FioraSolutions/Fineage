using System;

public partial class DragIcon : L2PopupWindow
{
    public VisualElement icon;


    private static DragIcon _instance;
    private bool _isShow = false;
    [System.Obsolete("Use [Inject] or DragIconPacketServices.Icon instead")]
    public static DragIcon Instance
    {
        get { return _instance; }
    }







}
