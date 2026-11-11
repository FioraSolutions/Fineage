public abstract partial class AbstractCreator 
{
    protected ITab[] _tabsTradeTabs;
    protected ITab[] _tabsContentTabs;
    protected string[] _nameTabs;
    protected ITab _activeTab;
    public event Action<int, ItemCategory, int> EventLeftClick;
    public event Action<ITab , bool> EventSwitchOut;

    
}
