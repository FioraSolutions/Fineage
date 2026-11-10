using System;

public partial class SeedInfoWindow
{
    public void OnSwitchTab(int indexTab)
    {
      
        if (!ArrayUtils.IsValidIndexArray(_tabsName, indexTab))
        {
            return;
        }

        switch (indexTab)
        {
            case 0:
                // Debug.Log("Event index 0 ");
                GameWindowPacketServices.Html.UseActionCommand(commandTab0);
                _creatorTable.SetSelectRow(-1);
                _creatorWindow.RefreshDataColumns(_defaultColumns0);
                AddContent0Footer(_creatorWindow, _footerContainer);
                break;
            case 1:
                // Debug.Log("Event index 1 ");
                GameWindowPacketServices.Html.UseActionCommand(commandTab1);
                _creatorTable.SetSelectRow(-1);
                _creatorWindow.RefreshDataColumns(_defaultColumns1);
                AddContent1Footer(_creatorWindow, _footerContainer);
                break;
            case 2:
                // Debug.Log("Event index 2 ");
                GameWindowPacketServices.Html.UseActionCommand(commandTab2);
                _creatorTable.SetSelectRow(-1);
                _creatorWindow.RefreshDataColumns(_defaultColumns2);
                AddContent2Footer(_creatorWindow, _footerContainer);
                break;
        }
    }

    public void OnClickButtonSeed(ClickEvent evt)
    {
        // Debug.Log("OnClickButtonSeed 1");
    }

    public void OnClickButtonHarvest(ClickEvent evt)
    {
        // Debug.Log("OnClickButtonHarvest 1");
    }

    public void OnClickAllButton1(ClickEvent evt)
    {

        GameWindowPacketServices.Html.UseActionCommand(commandButton1);
    }

    public void OnClickAllButton2(ClickEvent evt)
    {
        GameWindowPacketServices.Html.UseActionCommand(commandButton0);
    }

    public void ShowWindowActiveTabSeed()
    {
        _creatorWindow.SwitchTab(0 , false , true);
        AddContent0Footer(_creatorWindow, _footerContainer);
        base.ShowWindow();
    }

    public void ShowWindowActiveTabAllDefault()
    {
        _creatorWindow.SwitchTab(2, false, true);
        AddContent2Footer(_creatorWindow, _footerContainer);
        base.ShowWindow();
    }

}
