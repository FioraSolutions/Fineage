using System;

public partial class SkillLearnWindow : L2PopupWindow
{
    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        yield return new WaitForEndOfFrame();

        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);
        var content = (VisualElement)GetElementById("content");
        _spLabel = (Label)GetElementById("spCount");


        //_creatorWindow.InitTradeTabs(new string[] { "ALL", "Other" });
        _creatorWindow.CreateTabs(content, null, _rowsTemplate);
        _creatorWindow.EventLeftClick += ClickItem;

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);
        OnCenterScreen(_root);

    }

    public void AddData(List<OtherModel> list)
    {
        if(_list != null ) _list.Clear();
        _list = list;
        _creatorWindow.AddOtherData(list);
    }

    public void ClickItem(int skillId , ItemCategory category , int position)
    {

       AcquireData model = FilterBySKillId(_list, skillId);

       if(model != null)
       {
            //base.OnlyHideWindow();
            ZoneCommands.Send(new RequestAcquireSkillInfoCommand(model.GetId(), model.GetValue1(), model.GetAcqType()));
        }

    }

    public AcquireData FilterBySKillId(List<OtherModel> list , int skillId)
    {
        foreach (var item in list)
        {
            AcquireData data = (AcquireData)item.GetOtherModel();
            if (data.GetId() == skillId) return data;
        }
        return null;

    }

    public override void ShowWindow()
    {
        UserInfoDto user = StorageNpc.getInstance().GetFirstUser();
        _spLabel.text = user.PlayerInfoInterlude.Stats.Sp.ToString();
        base.ShowWindow();
    }
}
