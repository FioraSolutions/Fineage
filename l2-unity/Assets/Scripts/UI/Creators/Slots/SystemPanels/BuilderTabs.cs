using UnityEngine.UIElements;

public class BuilderTabs : AbstractCreator
{
    public VisualElement GetTabContent(string tabName)
    {
        int index = FindTabByName(tabName);
        if (index < 0 || _tabsContentTabs == null || index >= _tabsContentTabs.Length)
            return null;
        return _tabsContentTabs[index].GetContentElement();
    }
}
